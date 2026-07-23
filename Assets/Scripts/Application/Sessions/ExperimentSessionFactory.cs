using System;
using ChemistryLab.Core.Content;
using ChemistryLab.Core.Workflow;
using ChemistryLab.Infrastructure.Content;

namespace ChemistryLab.Application.Sessions
{
    public sealed class ExperimentSessionFactory
    {
        private readonly ExperimentContentJsonRepository contentRepository;

        public ExperimentSessionFactory(ExperimentContentJsonRepository contentRepository)
        {
            this.contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        }

        public ExperimentSessionStartResult StartFromJson(string json, bool requireApproved)
        {
            var contentResult = contentRepository.Load(json, requireApproved);
            if (!contentResult.IsSuccess)
            {
                return ExperimentSessionStartResult.Failure(contentResult.Issues);
            }

            var content = contentResult.Definition;
            var steps = new ExperimentStepDefinition[content.Steps.Count];
            for (var index = 0; index < content.Steps.Count; index++)
            {
                var step = content.Steps[index];
                steps[index] = new ExperimentStepDefinition(step.StepId, step.Title);
            }

            var workflow = new ExperimentWorkflow(new ExperimentDefinition(steps));
            var transition = workflow.Start();
            if (!transition.IsSuccess)
            {
                throw new InvalidOperationException("A validated experiment workflow could not be started.");
            }

            return ExperimentSessionStartResult.Success(content, workflow);
        }
    }
}
