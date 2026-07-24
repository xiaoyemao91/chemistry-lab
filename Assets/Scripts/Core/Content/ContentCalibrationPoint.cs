using System;

namespace ChemistryLab.Core.Content
{
    public sealed class ContentCalibrationPoint
    {
        public ContentCalibrationPoint(double concentration, double response)
        {
            Concentration = concentration;
            Response = response;
        }

        public double Concentration { get; }
        public double Response { get; }

        internal bool IsValid()
        {
            return !double.IsNaN(Concentration) && !double.IsInfinity(Concentration)
                && !double.IsNaN(Response) && !double.IsInfinity(Response);
        }
    }
}
