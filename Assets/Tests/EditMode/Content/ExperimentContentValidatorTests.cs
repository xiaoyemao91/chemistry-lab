using ChemistryLab.Core.Content;
using NUnit.Framework;

namespace ChemistryLab.Tests.EditMode.Content
{
    public sealed class ExperimentContentValidatorTests
    {
        [Test]
        public void ApprovedContentWithValidStructureIsAccepted()
        {
            var result = Validate(CreateDefinition(ContentReviewStatus.Approved));

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Issues, Is.Empty);
        }

        [Test]
        public void DraftContentIsRejectedWhenApprovalIsRequired()
        {
            var result = Validate(CreateDefinition(ContentReviewStatus.Draft), requireApproved: true);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Issues.Count, Is.EqualTo(1));
            Assert.That(result.Issues[0].Code, Is.EqualTo("CONTENT_DRAFT_NOT_ALLOWED"));
        }

        [Test]
        public void UnsupportedSchemaIsRejected()
        {
            var definition = new ExperimentContentDefinition(
                "2.0",
                "0.1.0",
                "fe-measurement",
                "Fe 测定",
                ContentReviewStatus.Approved,
                "TEACHER_DOCUMENT_V1",
                new[] { new ContentStepDefinition("power-on-check", "开机与状态检查") });

            var result = Validate(definition);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Issues.Count, Is.EqualTo(1));
            Assert.That(result.Issues[0].Code, Is.EqualTo("CONTENT_UNSUPPORTED_SCHEMA"));
        }

        [Test]
        public void DuplicateStepIdsAreRejected()
        {
            var definition = new ExperimentContentDefinition(
                "1.0",
                "0.1.0",
                "fe-measurement",
                "Fe 测定",
                ContentReviewStatus.Approved,
                "TEACHER_DOCUMENT_V1",
                new[]
                {
                    new ContentStepDefinition("duplicate", "第一步"),
                    new ContentStepDefinition("duplicate", "第二步")
                });

            var result = Validate(definition);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Issues.Count, Is.EqualTo(1));
            Assert.That(result.Issues[0].Code, Is.EqualTo("CONTENT_DUPLICATE_STEP_ID"));
        }

        private static ContentValidationResult Validate(
            ExperimentContentDefinition definition,
            bool requireApproved = false)
        {
            return new ExperimentContentValidator().Validate(definition, requireApproved);
        }

        private static ExperimentContentDefinition CreateDefinition(ContentReviewStatus reviewStatus)
        {
            return new ExperimentContentDefinition(
                "1.0",
                "0.1.0",
                "fe-measurement",
                "Fe 测定",
                reviewStatus,
                "TEACHER_DOCUMENT_V1",
                new[] { new ContentStepDefinition("power-on-check", "开机与状态检查") });
        }
    }
}
