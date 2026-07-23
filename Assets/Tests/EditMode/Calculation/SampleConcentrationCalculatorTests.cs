using ChemistryLab.Core.Calculation;
using NUnit.Framework;

namespace ChemistryLab.Tests.EditMode.Calculation
{
    public sealed class SampleConcentrationCalculatorTests
    {
        [Test]
        public void CalculateWithSyntheticCalibrationReturnsExpectedConcentration()
        {
            var calibration = FitSyntheticLine(
                new CalibrationPoint(0d, 1d),
                new CalibrationPoint(1d, 3d),
                new CalibrationPoint(2d, 5d));

            var result = new SampleConcentrationCalculator().Calculate(7d, calibration);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ErrorCode, Is.EqualTo(SampleConcentrationErrorCode.None));
            Assert.That(result.Concentration, Is.EqualTo(3d).Within(0.0000001d));
        }

        [Test]
        public void CalculateWithFailedCalibrationReturnsStructuredError()
        {
            var calibration = FitSyntheticLine(
                new CalibrationPoint(1d, 2d),
                new CalibrationPoint(1d, 4d));

            var result = new SampleConcentrationCalculator().Calculate(3d, calibration);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(SampleConcentrationErrorCode.CalibrationUnavailable));
        }

        [Test]
        public void CalculateWithZeroSensitivityReturnsStructuredError()
        {
            var calibration = FitSyntheticLine(
                new CalibrationPoint(0d, 1d),
                new CalibrationPoint(1d, 2d),
                new CalibrationPoint(2d, 1d));

            var result = new SampleConcentrationCalculator().Calculate(1d, calibration);

            Assert.That(calibration.IsSuccess, Is.True);
            Assert.That(calibration.Slope, Is.EqualTo(0d).Within(0.0000001d));
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(SampleConcentrationErrorCode.ZeroSensitivity));
        }

        [Test]
        public void CalculateWithNonFiniteSampleResponseReturnsStructuredError()
        {
            var calibration = FitSyntheticLine(
                new CalibrationPoint(0d, 1d),
                new CalibrationPoint(1d, 3d));

            var result = new SampleConcentrationCalculator().Calculate(double.NaN, calibration);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(SampleConcentrationErrorCode.InvalidSampleResponse));
        }

        private static LinearCalibrationResult FitSyntheticLine(params CalibrationPoint[] points)
        {
            return new LinearCalibrationService().Fit(points);
        }
    }
}
