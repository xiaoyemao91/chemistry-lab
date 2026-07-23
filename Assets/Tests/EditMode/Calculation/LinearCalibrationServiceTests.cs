using System;
using ChemistryLab.Core.Calculation;
using NUnit.Framework;

namespace ChemistryLab.Tests.EditMode.Calculation
{
    public sealed class LinearCalibrationServiceTests
    {
        [Test]
        public void FitExactSyntheticLineReturnsExpectedCoefficients()
        {
            var result = CreateService().Fit(new[]
            {
                new CalibrationPoint(0d, 1d),
                new CalibrationPoint(1d, 3d),
                new CalibrationPoint(2d, 5d),
                new CalibrationPoint(3d, 7d)
            });

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ErrorCode, Is.EqualTo(CalibrationErrorCode.None));
            Assert.That(result.PointCount, Is.EqualTo(4));
            Assert.That(result.Slope, Is.EqualTo(2d).Within(0.0000001d));
            Assert.That(result.Intercept, Is.EqualTo(1d).Within(0.0000001d));
            Assert.That(result.DeterminationCoefficient, Is.EqualTo(1d).Within(0.0000001d));
        }

        [Test]
        public void FitWithOnePointReturnsStructuredError()
        {
            var result = CreateService().Fit(new[] { new CalibrationPoint(0d, 1d) });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(CalibrationErrorCode.InsufficientPoints));
            Assert.That(result.PointCount, Is.EqualTo(1));
        }

        [Test]
        public void FitWithRepeatedConcentrationsReturnsStructuredError()
        {
            var result = CreateService().Fit(new[]
            {
                new CalibrationPoint(1d, 2d),
                new CalibrationPoint(1d, 4d)
            });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(
                result.ErrorCode,
                Is.EqualTo(CalibrationErrorCode.InsufficientConcentrationVariation));
        }

        [Test]
        public void FitWithConstantResponsesReturnsStructuredError()
        {
            var result = CreateService().Fit(new[]
            {
                new CalibrationPoint(0d, 2d),
                new CalibrationPoint(1d, 2d)
            });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(CalibrationErrorCode.InsufficientResponseVariation));
        }

        [Test]
        public void CalibrationPointRejectsNonFiniteValues()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CalibrationPoint(double.NaN, 1d));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CalibrationPoint(1d, double.PositiveInfinity));
        }

        private static LinearCalibrationService CreateService()
        {
            return new LinearCalibrationService();
        }
    }
}
