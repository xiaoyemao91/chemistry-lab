namespace ChemistryLab.Core.Instrument
{
    /// <summary>
    /// Models the coarse teaching states of the virtual instrument without encoding a real instrument SOP.
    /// </summary>
    public sealed class InstrumentController
    {
        private InstrumentState state = new InstrumentState(false, false, false);

        public InstrumentState State => state;

        public InstrumentTransitionResult Execute(InstrumentAction action)
        {
            switch (action)
            {
                case InstrumentAction.PowerOn:
                    return PowerOn();
                case InstrumentAction.StartPump:
                    return StartPump();
                case InstrumentAction.IgnitePlasma:
                    return IgnitePlasma();
                case InstrumentAction.ExtinguishPlasma:
                    return ExtinguishPlasma();
                case InstrumentAction.StopPump:
                    return StopPump();
                case InstrumentAction.PowerOff:
                    return PowerOff();
                default:
                    return InstrumentTransitionResult.Failure(InstrumentErrorCode.InstrumentMustBeIdle, state);
            }
        }

        private InstrumentTransitionResult PowerOn()
        {
            if (state.IsPoweredOn)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.AlreadyPoweredOn, state);
            }

            state = new InstrumentState(true, false, false);
            return InstrumentTransitionResult.Success(state);
        }

        private InstrumentTransitionResult StartPump()
        {
            if (!state.IsPoweredOn)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PowerMustBeOn, state);
            }

            if (state.IsPumpRunning)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PumpAlreadyRunning, state);
            }

            state = new InstrumentState(true, true, false);
            return InstrumentTransitionResult.Success(state);
        }

        private InstrumentTransitionResult IgnitePlasma()
        {
            if (!state.IsPoweredOn)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PowerMustBeOn, state);
            }

            if (!state.IsPumpRunning)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PumpMustBeRunning, state);
            }

            if (state.IsPlasmaIgnited)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PlasmaAlreadyIgnited, state);
            }

            state = new InstrumentState(true, true, true);
            return InstrumentTransitionResult.Success(state);
        }

        private InstrumentTransitionResult ExtinguishPlasma()
        {
            if (!state.IsPlasmaIgnited)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PlasmaMustBeIgnited, state);
            }

            state = new InstrumentState(true, true, false);
            return InstrumentTransitionResult.Success(state);
        }

        private InstrumentTransitionResult StopPump()
        {
            if (!state.IsPumpRunning)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PumpMustBeRunning, state);
            }

            if (state.IsPlasmaIgnited)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PlasmaMustBeOff, state);
            }

            state = new InstrumentState(true, false, false);
            return InstrumentTransitionResult.Success(state);
        }

        private InstrumentTransitionResult PowerOff()
        {
            if (!state.IsPoweredOn)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.PowerMustBeOn, state);
            }

            if (state.IsPumpRunning || state.IsPlasmaIgnited)
            {
                return InstrumentTransitionResult.Failure(InstrumentErrorCode.InstrumentMustBeIdle, state);
            }

            state = new InstrumentState(false, false, false);
            return InstrumentTransitionResult.Success(state);
        }
    }
}
