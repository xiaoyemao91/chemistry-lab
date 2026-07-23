using System;
using System.Collections.Generic;

namespace ChemistryLab.Core.Workflow
{
    public sealed class ExperimentDefinition
    {
        public ExperimentDefinition(IEnumerable<ExperimentStepDefinition> steps)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            var stepList = new List<ExperimentStepDefinition>(steps);
            if (stepList.Count == 0)
            {
                throw new ArgumentException("An experiment must contain at least one step.", nameof(steps));
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var step in stepList)
            {
                if (step == null)
                {
                    throw new ArgumentException("Experiment steps cannot contain null values.", nameof(steps));
                }

                if (!ids.Add(step.StepId))
                {
                    throw new ArgumentException("Experiment step IDs must be unique.", nameof(steps));
                }
            }

            Steps = stepList.AsReadOnly();
        }

        public IReadOnlyList<ExperimentStepDefinition> Steps { get; }
    }
}

