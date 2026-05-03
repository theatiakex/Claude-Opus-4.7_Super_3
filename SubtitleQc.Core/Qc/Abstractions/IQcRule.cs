using System.Collections.Generic;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Qc.Abstractions;

/// <summary>
/// Contract every QC rule must satisfy. Receives the full ordered list of cues
/// (not a single cue) because some rules — e.g. overlap detection — require
/// cross-cue context. Rules that only inspect one cue at a time simply iterate.
/// </summary>
public interface IQcRule
{
    /// <summary>Stable identifier reported on each <see cref="QcResult"/>.</summary>
    string Name { get; }

    IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues);
}
