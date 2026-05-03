### Superspec

Prototype complete. Build succeeded with 0 warnings, all 19 acceptance tests pass on the first attempt.

What I built
A SubtitleQc.Core class library targeting .NET 10, structured to satisfy superspec.md end-to-end (SRT + WebVTT + TTML + shot-change data) while honouring every constraint in agents_super.md (one class per file, methods ≤ 20 lines, no external packages, OCP/SRP/DIP, parsing decoupled from QC).

SubtitleQc.Core/
├── Models/
│   └── Cue.cs                              # format-agnostic, JSON-serialisable
├── Parsers/
│   ├── Abstractions/ISubtitleParser.cs
│   ├── SrtParser.cs
│   ├── WebVttParser.cs                     # parks cue settings into Attributes
│   └── TtmlParser.cs                       # XLinq-based, namespace-tolerant
└── Qc/
    ├── Abstractions/IQcRule.cs             # rules depend only on this
    ├── IShotChangeProvider.cs              # external-data abstraction
    ├── RuleEngine.cs                       # orchestrator, zero domain logic
    ├── QcReport.cs / QcResult.cs / QcStatus.cs
    ├── ShotChange/StaticShotChangeProvider.cs
    └── Rules/
        ├── MaxLinesRule.cs                 ├── MinDurationRule.cs
        ├── MaxCplRule.cs                   ├── OverlapCheckRule.cs
        ├── MaxCpsRule.cs                   ├── EmptyCueCheckRule.cs
        ├── CrossShotBoundaryCheckRule.cs
        └── MinFramesFromShotChangeRule.cs
Key architectural decisions
RuleEngine knows nothing about specific rules. It only iterates IQcRules and aggregates QcResults — adding TTML-specific rules tomorrow requires zero engine changes (OCP).
Cue.Attributes is the spec's "external attributes" hook. WebVTT settings (line:0, position:50%) and TTML region IDs flow in here without polluting timing/text fields.
StartFrame/EndFrame are nullable. Frame-aware rules like MinFramesFromShotChange return Skipped (not Failed) when a cue has no known frame, preserving the report's signal-to-noise ratio.
Boundary semantics matched to tests: > for line counts/CPL/CPS, < for duration, half-open [start, end) for overlap, strict start < cut < end for shot-boundary crossing.
Central Package Management (Directory.Packages.props) was added at the root because the existing SubtitleQc.Tests.csproj declares PackageReferences without versions — that's the CPM convention and lets me satisfy the test project without modifying the prohibited test files.
Build/test result
Passed!  - Failed: 0, Passed: 19, Skipped: 0, Total: 19, Duration: 87 ms
Logged to test_metrics.jsonl and summarised in test_performance_report.md with a Mermaid pass-rate chart, per the telemetry section of the agent profile.