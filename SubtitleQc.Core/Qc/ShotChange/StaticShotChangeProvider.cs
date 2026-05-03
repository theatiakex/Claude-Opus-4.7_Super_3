using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitleQc.Core.Qc.ShotChange;

/// <summary>
/// Default <see cref="IShotChangeProvider"/> backed by in-memory lists. Useful
/// for tests, batch processing pipelines and any caller that has already
/// materialised the cuts (e.g. parsed from an EDL or detector JSON).
/// </summary>
public sealed class StaticShotChangeProvider : IShotChangeProvider
{
    private readonly IReadOnlyList<TimeSpan> _timestamps;
    private readonly IReadOnlyList<int> _frames;

    public StaticShotChangeProvider(IEnumerable<TimeSpan> timestamps, IEnumerable<int> frames)
    {
        ArgumentNullException.ThrowIfNull(timestamps);
        ArgumentNullException.ThrowIfNull(frames);
        _timestamps = timestamps.ToArray();
        _frames = frames.ToArray();
    }

    public IReadOnlyList<TimeSpan> GetShotChangeTimestamps() => _timestamps;

    public IReadOnlyList<int> GetShotChangeFrames() => _frames;
}
