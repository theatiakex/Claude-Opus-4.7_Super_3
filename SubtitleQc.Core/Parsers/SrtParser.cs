using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// Parser for the SubRip (SRT) format. Splits the input into blank-line-
/// separated blocks and converts each into a <see cref="Cue"/>. Designed to be
/// permissive about counter lines (some SRT files omit them).
/// </summary>
public sealed class SrtParser : ISubtitleParser
{
    private const string TimingArrow = "-->";

    public string Format => "SRT";

    public IReadOnlyList<Cue> Parse(string content)
    {
        ArgumentNullException.ThrowIfNull(content);
        List<Cue> cues = new();
        foreach (string block in SplitIntoBlocks(content))
        {
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

    private static Cue? TryParseBlock(string block)
    {
        using StringReader reader = new(block);
        List<string> lines = ReadLines(reader);
        int timingIndex = FindTimingLine(lines);
        if (timingIndex < 0)
        {
            return null;
        }
        (TimeSpan start, TimeSpan end) = ParseTiming(lines[timingIndex]);
        string id = timingIndex > 0 ? lines[0].Trim() : Guid.NewGuid().ToString("N");
        List<string> textLines = lines.GetRange(timingIndex + 1, lines.Count - timingIndex - 1);
        return new Cue(id, start, end, textLines);
    }

    private static List<string> ReadLines(StringReader reader)
    {
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

    private static (TimeSpan Start, TimeSpan End) ParseTiming(string line)
    {
        string[] parts = line.Split(TimingArrow, StringSplitOptions.TrimEntries);
        return (ParseTimestamp(parts[0]), ParseTimestamp(parts[1]));
    }

    private static TimeSpan ParseTimestamp(string raw)
    {
        string normalised = raw.Replace(',', '.');
        return TimeSpan.ParseExact(normalised, @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
    }
}
