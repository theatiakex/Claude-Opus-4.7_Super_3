# Test Performance Report

## Summary

| Attempt | Build | Total | Passed | Failed | Pass Rate |
| ------- | ----- | ----- | ------ | ------ | --------- |
| 1       | OK    | 19    | 19     | 0      | 100%      |

The acceptance suite (`SubtitleQc.Tests`) reached a 100% pass rate on the first
build attempt. The architecture was designed against `superspec.md` end-to-end
(SRT, WebVTT, TTML and shot-change data) before any test was executed, so no
remedial iterations were required.

## Pass Rate Over Attempts

```mermaid
xychart-beta
    title "Pass Rate (%) per dotnet test attempt"
    x-axis "Attempt" [1]
    y-axis "Pass Rate %" 0 --> 100
    line [100]
```
