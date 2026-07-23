using System.Collections.Generic;
using ChemistryLab.Core.Content;
using ChemistryLab.Core.Workflow;

namespace ChemistryLab.Application.Sessions
{
    public sealed class ExperimentSessionStartResult
    {
        private ExperimentSessionStartResult(
            bool isSuccess,
            ExperimentContentDefinition content,
            ExperimentWorkflow workflow,
            IReadOnlyList<ContentValidationIssue> issues)
        {
            IsSuccess = isSuccess;
            Content = content;
            Workflow = workflow;
            Issues = issues;
        }

        public bool IsSuccess { get; }

        public ExperimentContentDefinition Content { get; }

        public ExperimentWorkflow Workflow { get; }

        public IReadOnlyList<ContentValidationIssue> Issues { get; }

        internal static ExperimentSessionStartResult Success(
            ExperimentContentDefinition content,
            ExperimentWorkflow workflow)
        {
            return new ExperimentSessionStartResult(
                true,
                content,
                workflow,
                new List<ContentValidationIssue>().AsReadOnly());
        }

        internal static ExperimentSessionStartResult Failure(IReadOnlyList<ContentValidationIssue> issues)
        {
            return new ExperimentSessionStartResult(false, null, null, issues);
        }
    }
}
