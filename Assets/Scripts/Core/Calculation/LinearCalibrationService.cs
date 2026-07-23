using System;
using System.Collections.Generic;

namespace ChemistryLab.Core.Calculation
{
    /// <summary>
    /// Fits a least-squares line to validated calibration points.
    /// The caller must ensure that the points originate from reviewed teaching content.
    /// </summary>
    public sealed class LinearCalibrationService
    {
        /// <summary>
        /// Calculates response = slope * concentration + intercept and its coefficient of determination.
        /// </summary>
        public LinearCalibrationResult Fit(IReadOnlyList<CalibrationPoint> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            if (points.Count < 2)
            {
                return LinearCalibrationResult.Failure(CalibrationErrorCode.InsufficientPoints, points.Count);
            }

            var totalConcentration = 0d;
            var totalResponse = 0d;
            for (var index = 0; index < points.Count; index++)
            {
                var point = points[index];
                if (point == null)
                {
                    return LinearCalibrationResult.Failure(CalibrationErrorCode.InvalidPoint, points.Count);
                }

                totalConcentration += point.Concentration;
                totalResponse += point.Response;
            }

            var meanConcentration = totalConcentration / points.Count;
            var meanResponse = totalResponse / points.Count;
            if (!IsFinite(meanConcentration) || !IsFinite(meanResponse))
            {
                return LinearCalibrationResult.Failure(CalibrationErrorCode.NumericalFailure, points.Count);
            }

            var concentrationVariation = 0d;
            var responseVariation = 0d;
            var covariance = 0d;
            for (var index = 0; index < points.Count; index++)
            {
                var concentrationDelta = points[index].Concentration - meanConcentration;
                var responseDelta = points[index].Response - meanResponse;
                concentrationVariation += concentrationDelta * concentrationDelta;
                responseVariation += responseDelta * responseDelta;
                covariance += concentrationDelta * responseDelta;
            }

            if (!IsFinite(concentrationVariation) || !IsFinite(responseVariation) || !IsFinite(covariance))
            {
                return LinearCalibrationResult.Failure(CalibrationErrorCode.NumericalFailure, points.Count);
            }

            if (concentrationVariation == 0d)
            {
                return LinearCalibrationResult.Failure(
                    CalibrationErrorCode.InsufficientConcentrationVariation,
                    points.Count);
            }

            if (responseVariation == 0d)
            {
                return LinearCalibrationResult.Failure(
                    CalibrationErrorCode.InsufficientResponseVariation,
                    points.Count);
            }

            var slope = covariance / concentrationVariation;
            var intercept = meanResponse - (slope * meanConcentration);
            var determinationCoefficient = (covariance * covariance) / (concentrationVariation * responseVariation);
            if (!IsFinite(slope) || !IsFinite(intercept) || !IsFinite(determinationCoefficient))
            {
                return LinearCalibrationResult.Failure(CalibrationErrorCode.NumericalFailure, points.Count);
            }

            return LinearCalibrationResult.Success(
                points.Count,
                slope,
                intercept,
                determinationCoefficient);
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
