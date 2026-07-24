using System;

namespace ChemistryLab.Core.Records
{
    public sealed class ExperimentRecord
    {
        public ExperimentRecord(
            Guid recordId,
            string experimentId,
            string contentVersion,
            string workflowStatus,
            string currentStepId,
            DateTime createdAtUtc)
        {
            if (string.IsNullOrWhiteSpace(experimentId))
            {
                throw new ArgumentException("Experiment ID is required.", nameof(experimentId));
            }

            if (string.IsNullOrWhiteSpace(contentVersion))
            {
                throw new ArgumentException("Content version is required.", nameof(contentVersion));
            }

            if (string.IsNullOrWhiteSpace(workflowStatus))
            {
                throw new ArgumentException("Workflow status is required.", nameof(workflowStatus));
            }

            if (createdAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Record timestamps must be UTC.", nameof(createdAtUtc));
            }

            RecordId = recordId;
            ExperimentId = experimentId;
            ContentVersion = contentVersion;
            WorkflowStatus = workflowStatus;
            CurrentStepId = currentStepId;
            CreatedAtUtc = createdAtUtc;
        }

        public Guid RecordId { get; }

        public string ExperimentId { get; }

        public string ContentVersion { get; }

        public string WorkflowStatus { get; }

        public string CurrentStepId { get; }

        public DateTime CreatedAtUtc { get; }
    }
}
