using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Fails any cue whose number of text lines strictly exceeds the configured
/// threshold. Equality with the threshold is tolerated — the threshold is the
/// maximum allowed value, not the first forbidden value.
/// </summary>
public sealed class MaxLinesRule : IQcRule
{
    private readonly int _threshold;

    public string Name => nameof(MaxLinesRule);

    public MaxLinesRule(int threshold)
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
        int lineCount = cue.Lines.Count;
        if (lineCount > _threshold)
        {
            return new QcResult(cue.Id, Name, QcStatus.Failed,
                $"Cue has {lineCount} lines (max {_threshold}).");
        }
        return new QcResult(cue.Id, Name, QcStatus.Passed);
    }
}
