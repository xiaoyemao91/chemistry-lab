namespace ChemistryLab.Core.Records
{
    public sealed class RecordSaveResult
    {
        private RecordSaveResult(bool isSuccess, RecordErrorCode errorCode)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
        }

        public bool IsSuccess { get; }

        public RecordErrorCode ErrorCode { get; }

        public static RecordSaveResult Success()
        {
            return new RecordSaveResult(true, RecordErrorCode.None);
        }

        public static RecordSaveResult Failure(RecordErrorCode errorCode)
        {
            return new RecordSaveResult(false, errorCode);
        }
    }
}
