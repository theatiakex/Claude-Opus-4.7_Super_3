using System.Collections.Generic;
using System.Linq;

namespace SubtitleQc.Core.Qc;

/// <summary>
/// Aggregated output of a full QC run. The collection is kept flat (one entry
/// per (cue, rule) pair) so consumers can group/pivot freely without the engine
/// imposing a presentation hierarchy.
/// </summary>
public sealed class QcReport
{
    public IReadOnlyList<QcResult> Results { get; }

    public bool IsSuccessful => Results.All(r => r.Status != QcStatus.Failed);

    public QcReport(IReadOnlyList<QcResult> results)
    {
        Results = results;
    }
}
