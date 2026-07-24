namespace ChemistryLab.Core.Records
{
    public sealed class RecordLoadResult
    {
        private RecordLoadResult(bool isSuccess, RecordErrorCode errorCode, ExperimentRecord record)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Record = record;
        }

        public bool IsSuccess { get; }

        public RecordErrorCode ErrorCode { get; }

        public ExperimentRecord Record { get; }

        public static RecordLoadResult Success(ExperimentRecord record)
        {
            return new RecordLoadResult(true, RecordErrorCode.None, record);
        }

        public static RecordLoadResult Failure(RecordErrorCode errorCode)
        {
            return new RecordLoadResult(false, errorCode, null);
        }
    }
}
