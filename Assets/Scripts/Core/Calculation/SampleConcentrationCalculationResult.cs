namespace ChemistryLab.Core.Calculation
{
    public sealed class SampleConcentrationCalculationResult
    {
        private SampleConcentrationCalculationResult(
            bool isSuccess,
            SampleConcentrationErrorCode errorCode,
            double concentration)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Concentration = concentration;
        }

        public bool IsSuccess { get; }

        public SampleConcentrationErrorCode ErrorCode { get; }

        public double Concentration { get; }

        internal static SampleConcentrationCalculationResult Success(double concentration)
        {
            return new SampleConcentrationCalculationResult(
                true,
                SampleConcentrationErrorCode.None,
                concentration);
        }

        internal static SampleConcentrationCalculationResult Failure(SampleConcentrationErrorCode errorCode)
        {
            return new SampleConcentrationCalculationResult(false, errorCode, 0d);
        }
    }
}
