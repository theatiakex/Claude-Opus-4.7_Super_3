using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// Parser for the WebVTT format. Differs from SRT in three ways the parser
/// must handle: a mandatory "WEBVTT" header, an optional cue identifier line,
/// and trailing cue settings on the timing line (parked into Attributes so the
/// QC engine can reason about them later without re-parsing).
/// </summary>
public sealed class WebVttParser : ISubtitleParser
{
    private const string TimingArrow = "-->";
    private const string Header = "WEBVTT";

    public string Format => "VTT";

    public IReadOnlyList<Cue> Parse(string content)
    {
        ArgumentNullException.ThrowIfNull(content);
        IEnumerable<string> blocks = SplitIntoBlocks(content);
        List<Cue> cues = new();
        foreach (string block in blocks)
        {
            if (IsHeaderBlock(block))
            {
                continue;
            }
            Cue? cue = TryParseBlock(block);
            if (cue is not null)
            {
                cues.Add(cue);
            }
        }
        return cues;
    }

    private static IEnumerable<string> SplitIntoBlocks(string content)
    {
        string normalised = content.Replace("\r\n", "\n").Replace('\r', '\n');
        return normalised.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static bool IsHeaderBlock(string block)
    {
        return block.StartsWith(Header, StringComparison.Ordinal);
    }

    private static Cue? TryParseBlock(string block)
    {
        List<string> lines = ReadLines(block);
        int timingIndex = FindTimingLine(lines);
        if (timingIndex < 0)
        {
            return null;
        }
        (TimeSpan start, TimeSpan end, IReadOnlyDictionary<string, string> settings) = ParseTiming(lines[timingIndex]);
        string id = timingIndex > 0 ? lines[0].Trim() : Guid.NewGuid().ToString("N");
        List<string> textLines = lines.GetRange(timingIndex + 1, lines.Count - timingIndex - 1);
        return new Cue(id, start, end, textLines, attributes: settings);
    }

    private static List<string> ReadLines(string block)
    {
        using StringReader reader = new(block);
        List<string> lines = new();
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            lines.Add(line);
        }
        return lines;
    }

    private static int FindTimingLine(List<string> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains(TimingArrow, StringComparison.Ordinal))
            {
                return i;
            }
        }
        return -1;
    }

    private static (TimeSpan Start, TimeSpan End, IReadOnlyDictionary<string, string> Settings) ParseTiming(string line)
    {
        int arrowIndex = line.IndexOf(TimingArrow, StringComparison.Ordinal);
        string left = line[..arrowIndex].Trim();
        string right = line[(arrowIndex + TimingArrow.Length)..].Trim();
        string[] rightParts = right.Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        TimeSpan start = ParseTimestamp(left);
        TimeSpan end = ParseTimestamp(rightParts[0]);
        IReadOnlyDictionary<string, string> settings = rightParts.Length > 1
            ? ParseSettings(rightParts[1])
            : new Dictionary<string, string>();
        return (start, end, settings);
    }

    private static IReadOnlyDictionary<string, string> ParseSettings(string raw)
    {
        Dictionary<string, string> settings = new(StringComparer.Ordinal);
        foreach (string token in raw.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] kv = token.Split(':', 2);
            settings[kv[0]] = kv.Length > 1 ? kv[1] : string.Empty;
        }
        return settings;
    }

    private static TimeSpan ParseTimestamp(string raw)
    {
        string[] formats = { @"hh\:mm\:ss\.fff", @"mm\:ss\.fff" };
        return TimeSpan.ParseExact(raw, formats, CultureInfo.InvariantCulture);
    }
}
