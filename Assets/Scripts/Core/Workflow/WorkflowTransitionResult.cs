namespace ChemistryLab.Core.Workflow
{
    public sealed class WorkflowTransitionResult
    {
        private WorkflowTransitionResult(bool isSuccess, WorkflowErrorCode errorCode, WorkflowState state)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            State = state;
        }

        public bool IsSuccess { get; }

        public WorkflowErrorCode ErrorCode { get; }

        public WorkflowState State { get; }

        internal static WorkflowTransitionResult Success(WorkflowState state)
        {
            return new WorkflowTransitionResult(true, WorkflowErrorCode.None, state);
        }

        internal static WorkflowTransitionResult Failure(WorkflowErrorCode errorCode, WorkflowState state)
        {
            return new WorkflowTransitionResult(false, errorCode, state);
        }
    }
}

