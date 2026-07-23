using System.Collections.Generic;

namespace ChemistryLab.Core.Content
{
    public sealed class ContentLoadResult
    {
        private ContentLoadResult(
            bool isSuccess,
            ExperimentContentDefinition definition,
            IReadOnlyList<ContentValidationIssue> issues)
        {
            IsSuccess = isSuccess;
            Definition = definition;
            Issues = issues;
        }

        public bool IsSuccess { get; }

        public ExperimentContentDefinition Definition { get; }

        public IReadOnlyList<ContentValidationIssue> Issues { get; }

        public static ContentLoadResult Success(ExperimentContentDefinition definition)
        {
            return new ContentLoadResult(true, definition, new List<ContentValidationIssue>().AsReadOnly());
        }

        public static ContentLoadResult Failure(IReadOnlyList<ContentValidationIssue> issues)
        {
            return new ContentLoadResult(false, null, issues);
        }
    }
}

