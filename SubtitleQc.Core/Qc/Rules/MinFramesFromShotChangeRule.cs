using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Frame-accurate "safety zone" rule: a cue must start at least N frames away
/// from any shot-change cut. We measure absolute frame distance so cues placed
/// either just before OR just after a cut are caught. Cues without a known
/// start frame are Skipped — the rule has no basis to evaluate them.
/// </summary>
public sealed class MinFramesFromShotChangeRule : IQcRule
{
    private readonly IShotChangeProvider _shotChanges;
    private readonly int _thresholdFrames;

    public string Name => nameof(MinFramesFromShotChangeRule);

    public MinFramesFromShotChangeRule(IShotChangeProvider shotChanges, int thresholdFrames)
    {
        ArgumentNullException.ThrowIfNull(shotChanges);
        _shotChanges = shotChanges;
        _thresholdFrames = thresholdFrames;
    }

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        IReadOnlyList<int> cuts = _shotChanges.GetShotChangeFrames();
        foreach (Cue cue in cues)
        {
            yield return EvaluateCue(cue, cuts);
        }
    }

    private QcResult EvaluateCue(Cue cue, IReadOnlyList<int> cuts)
    {
        if (cue.StartFrame is not int startFrame)
        {
            return new QcResult(cue.Id, Name, QcStatus.Skipped, "Cue has no start frame.");
        }
        foreach (int cut in cuts)
        {
            int distance = Math.Abs(startFrame - cut);
            if (distance < _thresholdFrames)
            {
                return new QcResult(cue.Id, Name, QcStatus.Failed,
                    $"Cue starts {distance} frame(s) from cut at {cut} (min {_thresholdFrames}).");
            }
        }
        return new QcResult(cue.Id, Name, QcStatus.Passed);
    }
}
