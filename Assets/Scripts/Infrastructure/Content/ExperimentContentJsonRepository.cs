using System;
using System.Collections.Generic;
using ChemistryLab.Core.Content;
using UnityEngine;

namespace ChemistryLab.Infrastructure.Content
{
    public sealed class ExperimentContentJsonRepository
    {
        private readonly ExperimentContentValidator validator;

        public ExperimentContentJsonRepository(ExperimentContentValidator validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public ContentLoadResult Load(string json, bool requireApproved)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return ContentLoadResult.Failure(new[]
                {
                    new ContentValidationIssue("CONTENT_EMPTY_DOCUMENT", "root")
                });
            }

            ExperimentContentJsonDto dto;
            try
            {
                dto = JsonUtility.FromJson<ExperimentContentJsonDto>(json);
            }
            catch (Exception)
            {
                return ContentLoadResult.Failure(new[]
                {
                    new ContentValidationIssue("CONTENT_INVALID_JSON", "root")
                });
            }

            if (dto == null)
            {
                return ContentLoadResult.Failure(new[]
                {
                    new ContentValidationIssue("CONTENT_INVALID_JSON", "root")
                });
            }

            var issues = new List<ContentValidationIssue>();
            if (!Enum.TryParse(dto.reviewStatus, true, out ContentReviewStatus reviewStatus))
            {
                reviewStatus = ContentReviewStatus.Draft;
                issues.Add(new ContentValidationIssue("CONTENT_INVALID_REVIEW_STATUS", "reviewStatus"));
            }

            var definition = new ExperimentContentDefinition(
                dto.schemaVersion,
                dto.contentVersion,
                dto.experimentId,
                dto.displayName,
                reviewStatus,
                dto.sourceReference,
                ConvertSteps(dto.steps),
                ConvertParameters(dto.parameters),
                ConvertCalibrationPoints(dto.calibrationPoints),
                ConvertSampleMeasurement(dto.sampleMeasurement));

            var validation = validator.Validate(definition, requireApproved);
            issues.AddRange(validation.Issues);
            if (issues.Count > 0)
            {
                return ContentLoadResult.Failure(issues.AsReadOnly());
            }

            return ContentLoadResult.Success(definition);
        }

        private static IEnumerable<ContentStepDefinition> ConvertSteps(ExperimentContentStepDto[] steps)
        {
            if (steps == null)
            {
                return Array.Empty<ContentStepDefinition>();
            }

            var definitions = new List<ContentStepDefinition>(steps.Length);
            foreach (var step in steps)
            {
                definitions.Add(step == null
                    ? null
                    : new ContentStepDefinition(step.stepId, step.title));
            }

            return definitions;
        }

        private static IEnumerable<ExperimentParameterDefinition> ConvertParameters(ExperimentContentParameterDto[] parameters)
        {
            if (parameters == null) return Array.Empty<ExperimentParameterDefinition>();
            var definitions = new List<ExperimentParameterDefinition>(parameters.Length);
            foreach (var parameter in parameters)
            {
                if (parameter != null) definitions.Add(new ExperimentParameterDefinition(parameter.parameterId, parameter.displayName, parameter.unit, parameter.defaultValue, parameter.minimum, parameter.maximum));
            }
            return definitions;
        }

        private static IEnumerable<ContentCalibrationPoint> ConvertCalibrationPoints(ExperimentContentCalibrationPointDto[] points)
        {
            if (points == null) return Array.Empty<ContentCalibrationPoint>();
            var definitions = new List<ContentCalibrationPoint>(points.Length);
            foreach (var point in points)
            {
                if (point != null) definitions.Add(new ContentCalibrationPoint(point.concentration, point.response));
            }
            return definitions;
        }

        private static ContentSampleMeasurement ConvertSampleMeasurement(ExperimentContentSampleMeasurementDto measurement)
        {
            return measurement == null ? null : new ContentSampleMeasurement(measurement.response);
        }

        [Serializable]
        private sealed class ExperimentContentJsonDto
        {
            public string schemaVersion;
            public string contentVersion;
            public string experimentId;
            public string displayName;
            public string reviewStatus;
            public string sourceReference;
            public ExperimentContentStepDto[] steps;
            public ExperimentContentParameterDto[] parameters;
            public ExperimentContentCalibrationPointDto[] calibrationPoints;
            public ExperimentContentSampleMeasurementDto sampleMeasurement;
        }

        [Serializable]
        private sealed class ExperimentContentStepDto
        {
            public string stepId;
            public string title;
        }

        [Serializable]
        private sealed class ExperimentContentParameterDto
        {
            public string parameterId;
            public string displayName;
            public string unit;
            public double defaultValue;
            public double minimum;
            public double maximum;
        }

        [Serializable]
        private sealed class ExperimentContentCalibrationPointDto
        {
            public double concentration;
            public double response;
        }

        [Serializable]
        private sealed class ExperimentContentSampleMeasurementDto
        {
            public double response;
        }
    }
}
