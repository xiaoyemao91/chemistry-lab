using System;

namespace ChemistryLab.Core.Workflow
{
    public sealed class ExperimentWorkflow
    {
        private readonly ExperimentDefinition definition;
        private WorkflowState state;

        public ExperimentWorkflow(ExperimentDefinition definition)
        {
            this.definition = definition ?? throw new ArgumentNullException(nameof(definition));
            state = CreateNotStartedState();
        }

        public WorkflowState State => state;

        public WorkflowTransitionResult Start()
        {
            if (state.Status != ExperimentStatus.NotStarted)
            {
                return WorkflowTransitionResult.Failure(WorkflowErrorCode.AlreadyStarted, state);
            }

            state = new WorkflowState(
                ExperimentStatus.Running,
                0,
                definition.Steps[0].StepId);
            return WorkflowTransitionResult.Success(state);
        }

        public WorkflowTransitionResult Pause()
        {
            if (state.Status == ExperimentStatus.Paused)
            {
                return WorkflowTransitionResult.Failure(WorkflowErrorCode.AlreadyPaused, state);
            }

            if (state.Status != ExperimentStatus.Running)
            {
                return WorkflowTransitionResult.Failure(WorkflowErrorCode.NotRunning, state);
            }

            state = new WorkflowState(
                ExperimentStatus.Paused,
                state.CurrentStepIndex,
                state.CurrentStepId);
            return WorkflowTransitionResult.Success(state);
        }

        public WorkflowTransitionResult Resume()
        {
            if (state.Status != ExperimentStatus.Paused)
            {
                return WorkflowTransitionResult.Failure(WorkflowErrorCode.NotPaused, state);
            }

            state = new WorkflowState(
                ExperimentStatus.Running,
                state.CurrentStepIndex,
                state.CurrentStepId);
            return WorkflowTransitionResult.Success(state);
        }

        public WorkflowTransitionResult CompleteCurrentStep()
        {
            if (state.Status == ExperimentStatus.Completed)
            {
                return WorkflowTransitionResult.Failure(WorkflowErrorCode.AlreadyCompleted, state);
            }

            if (state.Status != ExperimentStatus.Running)
            {
                return WorkflowTransitionResult.Failure(WorkflowErrorCode.NotRunning, state);
            }

            var isLastStep = state.CurrentStepIndex == definition.Steps.Count - 1;
            if (isLastStep)
            {
                state = new WorkflowState(
                    ExperimentStatus.Completed,
                    state.CurrentStepIndex,
                    state.CurrentStepId);
                return WorkflowTransitionResult.Success(state);
            }

            var nextStepIndex = state.CurrentStepIndex + 1;
            state = new WorkflowState(
                ExperimentStatus.Running,
                nextStepIndex,
                definition.Steps[nextStepIndex].StepId);
            return WorkflowTransitionResult.Success(state);
        }

        public WorkflowTransitionResult Reset()
        {
            state = CreateNotStartedState();
            return WorkflowTransitionResult.Success(state);
        }

        private WorkflowState CreateNotStartedState()
        {
            return new WorkflowState(ExperimentStatus.NotStarted, -1, null);
        }
    }
}

