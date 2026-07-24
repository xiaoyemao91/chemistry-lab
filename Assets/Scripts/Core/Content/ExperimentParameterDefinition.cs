using System;

namespace ChemistryLab.Core.Content
{
    public sealed class ExperimentParameterDefinition
    {
        public ExperimentParameterDefinition(string parameterId, string displayName, string unit, double defaultValue, double minimum, double maximum)
        {
            ParameterId = parameterId;
            DisplayName = displayName;
            Unit = unit;
            DefaultValue = defaultValue;
            Minimum = minimum;
            Maximum = maximum;
        }

        public string ParameterId { get; }
        public string DisplayName { get; }
        public string Unit { get; }
        public double DefaultValue { get; }
        public double Minimum { get; }
        public double Maximum { get; }

        internal bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ParameterId) && !string.IsNullOrWhiteSpace(DisplayName) && !string.IsNullOrWhiteSpace(Unit)
                && !double.IsNaN(DefaultValue) && !double.IsInfinity(DefaultValue)
                && !double.IsNaN(Minimum) && !double.IsInfinity(Minimum)
                && !double.IsNaN(Maximum) && !double.IsInfinity(Maximum)
                && Minimum < Maximum && DefaultValue >= Minimum && DefaultValue <= Maximum;
        }
    }
}
