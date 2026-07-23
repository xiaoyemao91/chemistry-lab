namespace ChemistryLab.Core.Instrument
{
    public enum InstrumentErrorCode
    {
        None,
        AlreadyPoweredOn,
        PowerMustBeOn,
        PumpAlreadyRunning,
        PumpMustBeRunning,
        PlasmaAlreadyIgnited,
        PlasmaMustBeIgnited,
        PlasmaMustBeOff,
        InstrumentMustBeIdle
    }
}
