using System;

namespace ChemistryLab.Core.Calculation
{
    /// <summary>
    /// Converts one sample response to concentration using a successful linear calibration.
    /// </summary>
    public sealed class SampleConcentrationCalculator
    {
        /// <summary>
        /// Calculates concentration = (sample response - intercept) / slope.
        /// </summary>
        public SampleConcentrationCalculationResult Calculate(
            double sampleResponse,
            LinearCalibrationResult calibration)
        {
            if (calibration == null)
            {
                throw new ArgumentNullException(nameof(calibration));
            }

            if (!IsFinite(sampleResponse))
            {
                return SampleConcentrationCalculationResult.Failure(
                    SampleConcentrationErrorCode.InvalidSampleResponse);
            }

            if (!calibration.IsSuccess)
            {
                return SampleConcentrationCalculationResult.Failure(
                    SampleConcentrationErrorCode.CalibrationUnavailable);
            }

            if (!IsFinite(calibration.Slope) || !IsFinite(calibration.Intercept))
            {
                return SampleConcentrationCalculationResult.Failure(
                    SampleConcentrationErrorCode.NumericalFailure);
            }

            if (calibration.Slope == 0d)
            {
                return SampleConcentrationCalculationResult.Failure(
                    SampleConcentrationErrorCode.ZeroSensitivity);
            }

            var concentration = (sampleResponse - calibration.Intercept) / calibration.Slope;
            if (!IsFinite(concentration))
            {
                return SampleConcentrationCalculationResult.Failure(
                    SampleConcentrationErrorCode.NumericalFailure);
            }

            return SampleConcentrationCalculationResult.Success(concentration);
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
