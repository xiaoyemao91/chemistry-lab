namespace ChemistryLab.Core.Instrument
{
    public sealed class InstrumentState
    {
        internal InstrumentState(bool isPoweredOn, bool isPumpRunning, bool isPlasmaIgnited)
        {
            IsPoweredOn = isPoweredOn;
            IsPumpRunning = isPumpRunning;
            IsPlasmaIgnited = isPlasmaIgnited;
        }

        public bool IsPoweredOn { get; }

        public bool IsPumpRunning { get; }

        public bool IsPlasmaIgnited { get; }
    }
}
