using System;
using System.Collections.Generic;

namespace SubtitleQc.Core.Qc;

/// <summary>
/// Abstraction over external shot-change data. Decoupling allows the QC engine
/// to remain agnostic to the source of cuts (file, EDL, AI detector, …) while
/// rules consume cuts in either time or frame space depending on their need.
/// </summary>
public interface IShotChangeProvider
{
    IReadOnlyList<TimeSpan> GetShotChangeTimestamps();

    IReadOnlyList<int> GetShotChangeFrames();
}
