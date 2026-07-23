using System;

namespace ChemistryLab.Core.Calculation
{
    public sealed class CalibrationPoint
    {
        public CalibrationPoint(double concentration, double response)
        {
            if (!IsFinite(concentration))
            {
                throw new ArgumentOutOfRangeException(nameof(concentration));
            }

            if (!IsFinite(response))
            {
                throw new ArgumentOutOfRangeException(nameof(response));
            }

            Concentration = concentration;
            Response = response;
        }

        public double Concentration { get; }

        public double Response { get; }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
