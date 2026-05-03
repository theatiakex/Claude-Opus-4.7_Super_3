using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Fails any cue whose display duration is strictly less than the configured
/// minimum. Equality with the threshold is tolerated; the threshold represents
/// the shortest acceptable duration, not the longest forbidden one.
/// </summary>
public sealed class MinDurationRule : IQcRule
{
    private readonly TimeSpan _threshold;

    public string Name => nameof(MinDurationRule);

    public MinDurationRule(TimeSpan threshold)
    {
        _threshold = threshold;
    }

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        foreach (Cue cue in cues)
        {
            yield return EvaluateCue(cue);
        }
    }

    private QcResult EvaluateCue(Cue cue)
    {
        if (cue.Duration < _threshold)
        {
            return new QcResult(cue.Id, Name, QcStatus.Failed,
                $"Duration {cue.Duration.TotalSeconds:F3}s is below minimum {_threshold.TotalSeconds:F3}s.");
        }
        return new QcResult(cue.Id, Name, QcStatus.Passed);
    }
}
