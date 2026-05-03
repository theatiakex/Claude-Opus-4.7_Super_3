using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Fails any cue whose time range strictly contains a shot-change cut. A cut
/// landing exactly on the cue's start or end is treated as touching, not
/// crossing, and therefore Passes — common practice in broadcast QC.
/// </summary>
public sealed class CrossShotBoundaryCheckRule : IQcRule
{
    private readonly IShotChangeProvider _shotChanges;

    public string Name => nameof(CrossShotBoundaryCheckRule);

    public CrossShotBoundaryCheckRule(IShotChangeProvider shotChanges)
    {
        ArgumentNullException.ThrowIfNull(shotChanges);
        _shotChanges = shotChanges;
    }

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        IReadOnlyList<TimeSpan> cuts = _shotChanges.GetShotChangeTimestamps();
        foreach (Cue cue in cues)
        {
            yield return EvaluateCue(cue, cuts);
        }
    }

    private QcResult EvaluateCue(Cue cue, IReadOnlyList<TimeSpan> cuts)
    {
        foreach (TimeSpan cut in cuts)
        {
            if (cut > cue.Start && cut < cue.End)
            {
                return new QcResult(cue.Id, Name, QcStatus.Failed,
                    $"Cue spans shot-change cut at {cut}.");
            }
        }
        return new QcResult(cue.Id, Name, QcStatus.Passed);
    }
}
