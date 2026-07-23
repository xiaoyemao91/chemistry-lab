using System;
using ChemistryLab.Core.Workflow;
using NUnit.Framework;

namespace ChemistryLab.Tests.EditMode.Workflow
{
    public sealed class ExperimentWorkflowTests
    {
        [Test]
        public void StartActivatesFirstStep()
        {
            var workflow = CreateWorkflow("power-on-check", "parameter-setup");

            var result = workflow.Start();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.ErrorCode, Is.EqualTo(WorkflowErrorCode.None));
            Assert.That(result.State.Status, Is.EqualTo(ExperimentStatus.Running));
            Assert.That(result.State.CurrentStepIndex, Is.EqualTo(0));
            Assert.That(result.State.CurrentStepId, Is.EqualTo("power-on-check"));
        }

        [Test]
        public void CompleteCurrentStepMovesToNextStep()
        {
            var workflow = CreateWorkflow("power-on-check", "parameter-setup");
            workflow.Start();

            var result = workflow.CompleteCurrentStep();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.State.Status, Is.EqualTo(ExperimentStatus.Running));
            Assert.That(result.State.CurrentStepIndex, Is.EqualTo(1));
            Assert.That(result.State.CurrentStepId, Is.EqualTo("parameter-setup"));
        }

        [Test]
        public void CompletingLastStepMarksWorkflowCompleted()
        {
            var workflow = CreateWorkflow("only-step");
            workflow.Start();

            var result = workflow.CompleteCurrentStep();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.State.Status, Is.EqualTo(ExperimentStatus.Completed));
            Assert.That(result.State.CurrentStepId, Is.EqualTo("only-step"));
        }

        [Test]
        public void PauseAndResumePreserveCurrentStep()
        {
            var workflow = CreateWorkflow("power-on-check", "parameter-setup");
            workflow.Start();

            var pauseResult = workflow.Pause();
            var resumeResult = workflow.Resume();

            Assert.That(pauseResult.State.Status, Is.EqualTo(ExperimentStatus.Paused));
            Assert.That(resumeResult.State.Status, Is.EqualTo(ExperimentStatus.Running));
            Assert.That(resumeResult.State.CurrentStepId, Is.EqualTo("power-on-check"));
        }

        [Test]
        public void InvalidCommandDoesNotChangeState()
        {
            var workflow = CreateWorkflow("power-on-check");

            var result = workflow.Pause();

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(WorkflowErrorCode.NotRunning));
            Assert.That(result.State.Status, Is.EqualTo(ExperimentStatus.NotStarted));
            Assert.That(result.State.CurrentStepIndex, Is.EqualTo(-1));
        }

        [Test]
        public void ResetReturnsWorkflowToNotStarted()
        {
            var workflow = CreateWorkflow("power-on-check");
            workflow.Start();

            var result = workflow.Reset();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.State.Status, Is.EqualTo(ExperimentStatus.NotStarted));
            Assert.That(result.State.CurrentStepId, Is.Null);
        }

        [Test]
        public void DefinitionRejectsDuplicateStepIds()
        {
            Assert.Throws<ArgumentException>(() => new ExperimentDefinition(new[]
            {
                new ExperimentStepDefinition("duplicate", "First"),
                new ExperimentStepDefinition("duplicate", "Second")
            }));
        }

        private static ExperimentWorkflow CreateWorkflow(params string[] stepIds)
        {
            var steps = new ExperimentStepDefinition[stepIds.Length];
            for (var index = 0; index < stepIds.Length; index++)
            {
                steps[index] = new ExperimentStepDefinition(stepIds[index], "Step " + index);
            }

            return new ExperimentWorkflow(new ExperimentDefinition(steps));
        }
    }
}

