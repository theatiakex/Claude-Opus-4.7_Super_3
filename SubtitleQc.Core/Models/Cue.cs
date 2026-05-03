using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitleQc.Core.Models;

/// <summary>
/// Format-agnostic representation of a single subtitle cue. Acts as the boundary
/// between parsers (which know about SRT/WebVTT/TTML) and the QC rule engine
/// (which only operates on this internal model). Designed to be serializable to
/// JSON without external libraries — all members expose plain CLR types.
/// </summary>
public sealed class Cue
{
    public string Id { get; }

    public TimeSpan Start { get; }

    public TimeSpan End { get; }

    public IReadOnlyList<string> Lines { get; }

    /// <summary>Optional frame number for the cue's start (for frame-accurate rules).</summary>
    public int? StartFrame { get; }

    /// <summary>Optional frame number for the cue's end (for frame-accurate rules).</summary>
    public int? EndFrame { get; }

    /// <summary>
    /// Free-form attributes carried over from source formats (e.g. WebVTT settings,
    /// TTML region IDs). Kept as strings so the model stays serializable.
    /// </summary>
    public IReadOnlyDictionary<string, string> Attributes { get; }

    public TimeSpan Duration => End - Start;

    public string Text => string.Join("\n", Lines);

    public Cue(
        string id,
        TimeSpan start,
        TimeSpan end,
        IReadOnlyList<string> lines,
        int? startFrame = null,
        int? endFrame = null,
        IReadOnlyDictionary<string, string>? attributes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(lines);
        Id = id;
        Start = start;
        End = end;
        Lines = lines.ToArray();
        StartFrame = startFrame;
        EndFrame = endFrame;
        Attributes = attributes ?? new Dictionary<string, string>();
    }
}
