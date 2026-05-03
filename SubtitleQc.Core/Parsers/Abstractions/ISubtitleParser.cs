using System.Collections.Generic;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers.Abstractions;

/// <summary>
/// Common contract for every format-specific subtitle parser. Parsers are pure
/// converters from raw text into the internal model — they MUST NOT contain any
/// QC logic, preserving the parser/engine separation mandated by the spec.
/// </summary>
public interface ISubtitleParser
{
    /// <summary>Short identifier of the format the parser handles ("SRT", "VTT", "TTML").</summary>
    string Format { get; }

    IReadOnlyList<Cue> Parse(string content);
}
