using ChemistryLab.Core.Instrument;
using NUnit.Framework;

namespace ChemistryLab.Tests.EditMode.Instrument
{
    public sealed class InstrumentControllerTests
    {
        [Test]
        public void ValidStartupSequenceIgnitesPlasma()
        {
            var controller = new InstrumentController();

            controller.Execute(InstrumentAction.PowerOn);
            controller.Execute(InstrumentAction.StartPump);
            var result = controller.Execute(InstrumentAction.IgnitePlasma);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.State.IsPoweredOn, Is.True);
            Assert.That(result.State.IsPumpRunning, Is.True);
            Assert.That(result.State.IsPlasmaIgnited, Is.True);
        }

        [Test]
        public void StartingPumpBeforePowerOnDoesNotChangeState()
        {
            var controller = new InstrumentController();

            var result = controller.Execute(InstrumentAction.StartPump);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(InstrumentErrorCode.PowerMustBeOn));
            Assert.That(result.State.IsPoweredOn, Is.False);
            Assert.That(result.State.IsPumpRunning, Is.False);
        }

        [Test]
        public void IgnitingPlasmaBeforePumpReturnsStructuredError()
        {
            var controller = new InstrumentController();
            controller.Execute(InstrumentAction.PowerOn);

            var result = controller.Execute(InstrumentAction.IgnitePlasma);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(InstrumentErrorCode.PumpMustBeRunning));
            Assert.That(result.State.IsPlasmaIgnited, Is.False);
        }

        [Test]
        public void StoppingPumpWhilePlasmaIsIgnitedReturnsStructuredError()
        {
            var controller = CreateRunningController();

            var result = controller.Execute(InstrumentAction.StopPump);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(InstrumentErrorCode.PlasmaMustBeOff));
            Assert.That(result.State.IsPumpRunning, Is.True);
            Assert.That(result.State.IsPlasmaIgnited, Is.True);
        }

        [Test]
        public void ValidShutdownSequencePowersInstrumentOff()
        {
            var controller = CreateRunningController();

            controller.Execute(InstrumentAction.ExtinguishPlasma);
            controller.Execute(InstrumentAction.StopPump);
            var result = controller.Execute(InstrumentAction.PowerOff);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.State.IsPoweredOn, Is.False);
            Assert.That(result.State.IsPumpRunning, Is.False);
            Assert.That(result.State.IsPlasmaIgnited, Is.False);
        }

        private static InstrumentController CreateRunningController()
        {
            var controller = new InstrumentController();
            controller.Execute(InstrumentAction.PowerOn);
            controller.Execute(InstrumentAction.StartPump);
            controller.Execute(InstrumentAction.IgnitePlasma);
            return controller;
        }
    }
}
