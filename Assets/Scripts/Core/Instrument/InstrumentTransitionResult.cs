namespace ChemistryLab.Core.Instrument
{
    public sealed class InstrumentTransitionResult
    {
        private InstrumentTransitionResult(bool isSuccess, InstrumentErrorCode errorCode, InstrumentState state)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            State = state;
        }

        public bool IsSuccess { get; }

        public InstrumentErrorCode ErrorCode { get; }

        public InstrumentState State { get; }

        internal static InstrumentTransitionResult Success(InstrumentState state)
        {
            return new InstrumentTransitionResult(true, InstrumentErrorCode.None, state);
        }

        internal static InstrumentTransitionResult Failure(InstrumentErrorCode errorCode, InstrumentState state)
        {
            return new InstrumentTransitionResult(false, errorCode, state);
        }
    }
}
