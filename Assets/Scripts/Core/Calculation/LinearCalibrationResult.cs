namespace ChemistryLab.Core.Calculation
{
    public sealed class LinearCalibrationResult
    {
        private LinearCalibrationResult(
            bool isSuccess,
            CalibrationErrorCode errorCode,
            int pointCount,
            double slope,
            double intercept,
            double determinationCoefficient)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            PointCount = pointCount;
            Slope = slope;
            Intercept = intercept;
            DeterminationCoefficient = determinationCoefficient;
        }

        public bool IsSuccess { get; }

        public CalibrationErrorCode ErrorCode { get; }

        public int PointCount { get; }

        public double Slope { get; }

        public double Intercept { get; }

        public double DeterminationCoefficient { get; }

        internal static LinearCalibrationResult Success(
            int pointCount,
            double slope,
            double intercept,
            double determinationCoefficient)
        {
            return new LinearCalibrationResult(
                true,
                CalibrationErrorCode.None,
                pointCount,
                slope,
                intercept,
                determinationCoefficient);
        }

        internal static LinearCalibrationResult Failure(CalibrationErrorCode errorCode, int pointCount)
        {
            return new LinearCalibrationResult(false, errorCode, pointCount, 0d, 0d, 0d);
        }
    }
}
