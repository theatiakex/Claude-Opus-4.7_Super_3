namespace SubtitleQc.Core.Qc;

/// <summary>
/// Result categories emitted by a QC rule for a single cue. Kept as a simple
/// enum (rather than rich subtypes) so the model stays JSON-serializable.
/// </summary>
public enum QcStatus
{
    Passed = 0,
    Failed = 1,
    Skipped = 2
}
