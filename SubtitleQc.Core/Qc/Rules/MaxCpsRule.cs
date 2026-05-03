using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Maximum Characters-Per-Second (reading speed). Counts the total visible
/// character payload across all lines (newlines excluded) and divides by the
/// cue's display duration in seconds. A non-positive duration yields a Skipped
/// result rather than a divide-by-zero exception or a misleading Pass.
/// </summary>
public sealed class MaxCpsRule : IQcRule
{
    private readonly double _threshold;

    public string Name => nameof(MaxCpsRule);

    public MaxCpsRule(double threshold)
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
        double seconds = cue.Duration.TotalSeconds;
        if (seconds <= 0)
        {
            return new QcResult(cue.Id, Name, QcStatus.Skipped, "Non-positive duration.");
        }
        int charCount = CountVisibleCharacters(cue);
        double cps = charCount / seconds;
        if (cps > _threshold)
        {
            return new QcResult(cue.Id, Name, QcStatus.Failed,
                $"Reading speed {cps:F2} cps exceeds threshold {_threshold:F2}.");
        }
        return new QcResult(cue.Id, Name, QcStatus.Passed);
    }

    private static int CountVisibleCharacters(Cue cue)
    {
        int total = 0;
        foreach (string line in cue.Lines)
        {
            total += line.Length;
        }
        return total;
    }
}
