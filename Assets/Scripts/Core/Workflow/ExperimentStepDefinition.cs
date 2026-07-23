using System;

namespace ChemistryLab.Core.Workflow
{
    public sealed class ExperimentStepDefinition
    {
        public ExperimentStepDefinition(string stepId, string title)
        {
            if (string.IsNullOrWhiteSpace(stepId))
            {
                throw new ArgumentException("Step ID cannot be empty.", nameof(stepId));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Step title cannot be empty.", nameof(title));
            }

            StepId = stepId;
            Title = title;
        }

        public string StepId { get; }

        public string Title { get; }
    }
}

