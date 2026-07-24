using System;

namespace ChemistryLab.Core.Content
{
    public sealed class ContentSampleMeasurement
    {
        public ContentSampleMeasurement(double response)
        {
            Response = response;
        }

        public double Response { get; }

        public bool IsValid()
        {
            return !double.IsNaN(Response) && !double.IsInfinity(Response);
        }
    }
}
