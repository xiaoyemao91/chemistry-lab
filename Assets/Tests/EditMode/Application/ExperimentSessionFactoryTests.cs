using ChemistryLab.Application.Sessions;
using ChemistryLab.Core.Content;
using ChemistryLab.Core.Workflow;
using ChemistryLab.Infrastructure.Content;
using NUnit.Framework;

namespace ChemistryLab.Tests.EditMode.Application
{
    public sealed class ExperimentSessionFactoryTests
    {
        [Test]
        public void ApprovedSyntheticContentStartsWorkflowAtFirstStep()
        {
            var result = CreateFactory().StartFromJson(ApprovedContentJson, requireApproved: true);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Content.ExperimentId, Is.EqualTo("synthetic-fe-measurement"));
            Assert.That(result.Workflow.State.Status, Is.EqualTo(ExperimentStatus.Running));
            Assert.That(result.Workflow.State.CurrentStepId, Is.EqualTo("power-on-check"));
        }

        [Test]
        public void DraftContentIsRejectedBeforeWorkflowStarts()
        {
            var result = CreateFactory().StartFromJson(DraftContentJson, requireApproved: true);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Workflow, Is.Null);
            Assert.That(result.Issues.Count, Is.EqualTo(1));
            Assert.That(result.Issues[0].Code, Is.EqualTo("CONTENT_DRAFT_NOT_ALLOWED"));
        }

        [Test]
        public void InvalidJsonReturnsContentIssue()
        {
            var result = CreateFactory().StartFromJson("{ not-valid-json", requireApproved: false);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Issues.Count, Is.EqualTo(1));
            Assert.That(result.Issues[0].Code, Is.EqualTo("CONTENT_INVALID_JSON"));
        }

        private static ExperimentSessionFactory CreateFactory()
        {
            return new ExperimentSessionFactory(
                new ExperimentContentJsonRepository(new ExperimentContentValidator()));
        }

        private const string ApprovedContentJson =
            "{\n" +
            "  \"schemaVersion\": \"1.0\",\n" +
            "  \"contentVersion\": \"synthetic-1.0\",\n" +
            "  \"experimentId\": \"synthetic-fe-measurement\",\n" +
            "  \"displayName\": \"Synthetic Fe workflow\",\n" +
            "  \"reviewStatus\": \"approved\",\n" +
            "  \"sourceReference\": \"SYNTHETIC_TEST_DATA\",\n" +
            "  \"steps\": [\n" +
            "    { \"stepId\": \"power-on-check\", \"title\": \"Power on check\" },\n" +
            "    { \"stepId\": \"parameter-setup\", \"title\": \"Parameter setup\" }\n" +
            "  ]\n" +
            "}";

        private const string DraftContentJson =
            "{\n" +
            "  \"schemaVersion\": \"1.0\",\n" +
            "  \"contentVersion\": \"synthetic-1.0\",\n" +
            "  \"experimentId\": \"synthetic-fe-measurement\",\n" +
            "  \"displayName\": \"Synthetic Fe workflow\",\n" +
            "  \"reviewStatus\": \"draft\",\n" +
            "  \"sourceReference\": \"SYNTHETIC_TEST_DATA\",\n" +
            "  \"steps\": [\n" +
            "    { \"stepId\": \"power-on-check\", \"title\": \"Power on check\" }\n" +
            "  ]\n" +
            "}";
    }
}
