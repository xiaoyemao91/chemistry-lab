namespace ChemistryLab.Core.Workflow
{
    public sealed class WorkflowState
    {
        internal WorkflowState(ExperimentStatus status, int currentStepIndex, string currentStepId)
        {
            Status = status;
            CurrentStepIndex = currentStepIndex;
            CurrentStepId = currentStepId;
        }

        public ExperimentStatus Status { get; }

        public int CurrentStepIndex { get; }

        public string CurrentStepId { get; }
    }
}

