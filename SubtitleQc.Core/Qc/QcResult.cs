namespace SubtitleQc.Core.Qc;

/// <summary>
/// Outcome of a single (rule, cue) evaluation. Designed to be traceable: each
/// result carries the cue id and the rule name that produced it, so an external
/// reporter can correlate findings to source positions without re-running QC.
/// </summary>
public sealed class QcResult
{
    public string CueId { get; }

    public string RuleName { get; }

    public QcStatus Status { get; }

    public string? Message { get; }

    public QcResult(string cueId, string ruleName, QcStatus status, string? message = null)
    {
        CueId = cueId;
        RuleName = ruleName;
        Status = status;
        Message = message;
    }
}
