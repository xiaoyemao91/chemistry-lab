using System.Collections.Generic;

namespace ChemistryLab.Core.Content
{
    public sealed class ContentValidationResult
    {
        internal ContentValidationResult(IReadOnlyList<ContentValidationIssue> issues)
        {
            Issues = issues;
        }

        public bool IsValid => Issues.Count == 0;

        public IReadOnlyList<ContentValidationIssue> Issues { get; }
    }
}

