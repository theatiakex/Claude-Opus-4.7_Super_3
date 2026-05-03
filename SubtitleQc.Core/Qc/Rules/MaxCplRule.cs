using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Maximum Characters-Per-Line. Fails the cue if ANY of its lines strictly
/// exceeds the configured length. The first violating line determines the
/// failure message — additional violations are intentionally not enumerated to
/// keep the report flat (one entry per (cue, rule) pair).
/// </summary>
public sealed class MaxCplRule : IQcRule
{
    private readonly int _threshold;

    public string Name => nameof(MaxCplRule);

    public MaxCplRule(int threshold)
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
        for (int i = 0; i < cue.Lines.Count; i++)
        {
            int length = cue.Lines[i].Length;
            if (length > _threshold)
            {
                return new QcResult(cue.Id, Name, QcStatus.Failed,
                    $"Line {i + 1} has {length} characters (max {_threshold}).");
            }
        }
        return new QcResult(cue.Id, Name, QcStatus.Passed);
    }
}
