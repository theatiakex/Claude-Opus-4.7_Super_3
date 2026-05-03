using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Fails any cue whose visible content is empty or whitespace-only after
/// concatenating all lines. Whitespace-only lines are treated as no content,
/// matching the spec's intent that a cue must convey actual text to viewers.
/// </summary>
public sealed class EmptyCueCheckRule : IQcRule
{
    public string Name => nameof(EmptyCueCheckRule);

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        foreach (Cue cue in cues)
        {
            yield return EvaluateCue(cue);
        }
    }

    private QcResult EvaluateCue(Cue cue)
    {
        foreach (string line in cue.Lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                return new QcResult(cue.Id, Name, QcStatus.Passed);
            }
        }
        return new QcResult(cue.Id, Name, QcStatus.Failed, "Cue contains no visible text.");
    }
}
