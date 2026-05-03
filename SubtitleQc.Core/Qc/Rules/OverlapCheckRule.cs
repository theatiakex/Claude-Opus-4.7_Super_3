using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Detects time-range overlaps between cues. A cue Fails when its start time is
/// strictly earlier than the end time of any preceding cue (touching boundaries
/// are allowed: cue B may start exactly when cue A ends). The first cue in the
/// stream always Passes — there is no "previous" to overlap with.
/// </summary>
public sealed class OverlapCheckRule : IQcRule
{
    public string Name => nameof(OverlapCheckRule);

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        for (int i = 0; i < cues.Count; i++)
        {
            yield return EvaluateAt(cues, i);
        }
    }

    private QcResult EvaluateAt(IReadOnlyList<Cue> cues, int index)
    {
        Cue current = cues[index];
        for (int j = 0; j < index; j++)
        {
            Cue earlier = cues[j];
            if (current.Start < earlier.End && current.End > earlier.Start)
            {
                return new QcResult(current.Id, Name, QcStatus.Failed,
                    $"Cue overlaps with earlier cue '{earlier.Id}'.");
            }
        }
        return new QcResult(current.Id, Name, QcStatus.Passed);
    }
}
