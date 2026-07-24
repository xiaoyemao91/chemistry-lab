using System;
using System.Collections.Generic;

namespace ChemistryLab.Core.Content
{
    public sealed class ExperimentContentDefinition
    {
        public ExperimentContentDefinition(
            string schemaVersion,
            string contentVersion,
            string experimentId,
            string displayName,
            ContentReviewStatus reviewStatus,
            string sourceReference,
            IEnumerable<ContentStepDefinition> steps)
            : this(schemaVersion, contentVersion, experimentId, displayName, reviewStatus, sourceReference, steps, Array.Empty<ExperimentParameterDefinition>())
        {
        }

        public ExperimentContentDefinition(
            string schemaVersion,
            string contentVersion,
            string experimentId,
            string displayName,
            ContentReviewStatus reviewStatus,
            string sourceReference,
            IEnumerable<ContentStepDefinition> steps,
            IEnumerable<ExperimentParameterDefinition> parameters)
        {
            SchemaVersion = schemaVersion;
            ContentVersion = contentVersion;
            ExperimentId = experimentId;
            DisplayName = displayName;
            ReviewStatus = reviewStatus;
            SourceReference = sourceReference;
            Steps = new List<ContentStepDefinition>(steps ?? Array.Empty<ContentStepDefinition>()).AsReadOnly();
            Parameters = new List<ExperimentParameterDefinition>(parameters ?? Array.Empty<ExperimentParameterDefinition>()).AsReadOnly();
        }

        public string SchemaVersion { get; }

        public string ContentVersion { get; }

        public string ExperimentId { get; }

        public string DisplayName { get; }

        public ContentReviewStatus ReviewStatus { get; }

        public string SourceReference { get; }

        public IReadOnlyList<ContentStepDefinition> Steps { get; }

        public IReadOnlyList<ExperimentParameterDefinition> Parameters { get; }
    }
}
