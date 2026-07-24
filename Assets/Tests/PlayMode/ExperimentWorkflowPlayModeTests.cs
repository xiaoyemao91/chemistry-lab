using System.Collections;
using ChemistryLab.Core.Workflow;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace ChemistryLab.Tests
{
    public sealed class ExperimentWorkflowPlayModeTests
    {
        [UnityTest]
        public IEnumerator WorkflowCanPauseResumeAndResetAcrossFrames()
        {
            var workflow = new ExperimentWorkflow(new ExperimentDefinition(
                new[]
                {
                    new ExperimentStepDefinition("step-a", "Step A"),
                    new ExperimentStepDefinition("step-b", "Step B")
                }));

            Assert.That(workflow.Start().IsSuccess, Is.True);
            yield return null;
            Assert.That(workflow.Pause().IsSuccess, Is.True);
            Assert.That(workflow.State.Status, Is.EqualTo(ExperimentStatus.Paused));
            yield return null;
            Assert.That(workflow.Resume().IsSuccess, Is.True);
            Assert.That(workflow.State.Status, Is.EqualTo(ExperimentStatus.Running));
            Assert.That(workflow.CompleteCurrentStep().IsSuccess, Is.True);
            Assert.That(workflow.State.CurrentStepId, Is.EqualTo("step-b"));
            workflow.Reset();
            Assert.That(workflow.State.Status, Is.EqualTo(ExperimentStatus.NotStarted));
        }
    }
}
