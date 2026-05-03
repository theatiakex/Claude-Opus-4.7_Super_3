using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc;

/// <summary>
/// Orchestrates evaluation of a set of <see cref="IQcRule"/>s against a list of
/// cues. The engine itself contains zero QC logic — extending the system with
/// new rules requires no engine modification (Open/Closed Principle).
/// </summary>
public sealed class RuleEngine
{
    private readonly IReadOnlyList<IQcRule> _rules;

    public RuleEngine(IEnumerable<IQcRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        _rules = rules.ToArray();
    }

    public QcReport Evaluate(IEnumerable<Cue> cues)
    {
        ArgumentNullException.ThrowIfNull(cues);
        IReadOnlyList<Cue> snapshot = cues.ToArray();
        List<QcResult> aggregated = new();
        foreach (IQcRule rule in _rules)
        {
            aggregated.AddRange(rule.Evaluate(snapshot));
        }
        return new QcReport(aggregated);
    }
}
