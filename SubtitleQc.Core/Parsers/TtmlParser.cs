using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// Parser for the TTML (Timed Text Markup Language) format. Uses XLinq because
/// it is part of the standard library and handles namespaces cleanly. The
/// parser is intentionally permissive about the exact namespace ("tt" or full
/// W3C URI) to accommodate authoring tools that strip the prefix.
/// </summary>
public sealed class TtmlParser : ISubtitleParser
{
    private const string TtmlNamespace = "http://www.w3.org/ns/ttml";

    public string Format => "TTML";

    public IReadOnlyList<Cue> Parse(string content)
    {
        ArgumentNullException.ThrowIfNull(content);
        XDocument document = XDocument.Parse(content);
        IEnumerable<XElement> paragraphs = FindParagraphs(document);
        return paragraphs.Select(ToCue).ToArray();
    }

    private static IEnumerable<XElement> FindParagraphs(XDocument document)
    {
        XName withNs = XName.Get("p", TtmlNamespace);
        IEnumerable<XElement> namespaced = document.Descendants(withNs);
        if (namespaced.Any())
        {
            return namespaced;
        }
        return document.Descendants().Where(e => e.Name.LocalName == "p");
    }

    private static Cue ToCue(XElement paragraph, int index)
    {
        TimeSpan start = ParseTimestamp(GetAttribute(paragraph, "begin"));
        TimeSpan end = ParseTimestamp(GetAttribute(paragraph, "end"));
        IReadOnlyList<string> lines = ExtractLines(paragraph);
        string id = paragraph.Attribute("id")?.Value ?? $"ttml-{index}";
        return new Cue(id, start, end, lines);
    }

    private static string GetAttribute(XElement element, string name)
    {
        XAttribute? attr = element.Attribute(name)
            ?? element.Attribute(XName.Get(name, TtmlNamespace));
        return attr?.Value
            ?? throw new FormatException($"<p> element is missing required attribute '{name}'.");
    }

    private static IReadOnlyList<string> ExtractLines(XElement paragraph)
    {
        List<string> lines = new();
        System.Text.StringBuilder current = new();
        foreach (XNode node in paragraph.Nodes())
        {
            if (IsLineBreak(node))
            {
                lines.Add(current.ToString());
                current.Clear();
                continue;
            }
            current.Append(NodeText(node));
        }
        lines.Add(current.ToString());
        return lines;
    }

    private static bool IsLineBreak(XNode node)
    {
        return node is XElement element && element.Name.LocalName == "br";
    }

    private static string NodeText(XNode node)
    {
        return node switch
        {
            XText text => text.Value,
            XElement element => element.Value,
            _ => string.Empty
        };
    }

    private static TimeSpan ParseTimestamp(string raw)
    {
        string[] formats = { @"hh\:mm\:ss\.fff", @"hh\:mm\:ss", @"mm\:ss\.fff" };
        return TimeSpan.ParseExact(raw, formats, CultureInfo.InvariantCulture);
    }
}
