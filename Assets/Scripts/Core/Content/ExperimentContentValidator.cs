using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChemistryLab.Core.Content
{
    public sealed class ExperimentContentValidator
    {
        public const string SupportedSchemaVersion = "1.0";

        private static readonly Regex ExperimentIdPattern = new Regex(
            "^[a-z0-9]+(?:-[a-z0-9]+)*$",
            RegexOptions.CultureInvariant);

        public ContentValidationResult Validate(ExperimentContentDefinition definition, bool requireApproved)
        {
            var issues = new List<ContentValidationIssue>();
            if (definition == null)
            {
                issues.Add(new ContentValidationIssue("CONTENT_NULL_DEFINITION", "root"));
                return new ContentValidationResult(issues.AsReadOnly());
            }

            if (!string.Equals(definition.SchemaVersion, SupportedSchemaVersion, StringComparison.Ordinal))
            {
                issues.Add(new ContentValidationIssue("CONTENT_UNSUPPORTED_SCHEMA", "schemaVersion"));
            }

            if (string.IsNullOrWhiteSpace(definition.ContentVersion))
            {
                issues.Add(new ContentValidationIssue("CONTENT_REQUIRED_FIELD", "contentVersion"));
            }

            if (string.IsNullOrWhiteSpace(definition.ExperimentId) || !ExperimentIdPattern.IsMatch(definition.ExperimentId))
            {
                issues.Add(new ContentValidationIssue("CONTENT_INVALID_ID", "experimentId"));
            }

            if (string.IsNullOrWhiteSpace(definition.DisplayName))
            {
                issues.Add(new ContentValidationIssue("CONTENT_REQUIRED_FIELD", "displayName"));
            }

            if (string.IsNullOrWhiteSpace(definition.SourceReference))
            {
                issues.Add(new ContentValidationIssue("CONTENT_REQUIRED_FIELD", "sourceReference"));
            }

            if (requireApproved && definition.ReviewStatus != ContentReviewStatus.Approved)
            {
                issues.Add(new ContentValidationIssue("CONTENT_DRAFT_NOT_ALLOWED", "reviewStatus"));
            }

            ValidateSteps(definition.Steps, issues);
            ValidateParameters(definition.Parameters, issues);
            ValidateCalibrationPoints(definition.CalibrationPoints, issues);
            return new ContentValidationResult(issues.AsReadOnly());
        }

        private static void ValidateCalibrationPoints(IReadOnlyList<ContentCalibrationPoint> points, ICollection<ContentValidationIssue> issues)
        {
            for (var index = 0; points != null && index < points.Count; index++)
            {
                if (points[index] == null || !points[index].IsValid())
                {
                    issues.Add(new ContentValidationIssue("CONTENT_INVALID_CALIBRATION_POINT", "calibrationPoints[" + index + "]"));
                }
            }
        }

        private static void ValidateParameters(IReadOnlyList<ExperimentParameterDefinition> parameters, ICollection<ContentValidationIssue> issues)
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; parameters != null && index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                var field = "parameters[" + index + "]";
                if (parameter == null || !parameter.IsValid()) issues.Add(new ContentValidationIssue("CONTENT_INVALID_PARAMETER", field));
                else if (!ids.Add(parameter.ParameterId)) issues.Add(new ContentValidationIssue("CONTENT_DUPLICATE_PARAMETER_ID", field + ".parameterId"));
            }
        }

        private static void ValidateSteps(
            IReadOnlyList<ContentStepDefinition> steps,
            ICollection<ContentValidationIssue> issues)
        {
            if (steps == null || steps.Count == 0)
            {
                issues.Add(new ContentValidationIssue("CONTENT_REQUIRED_FIELD", "steps"));
                return;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < steps.Count; index++)
            {
                var step = steps[index];
                var field = "steps[" + index + "]";
                if (step == null || !step.HasRequiredFields())
                {
                    issues.Add(new ContentValidationIssue("CONTENT_INVALID_STEP", field));
                    continue;
                }

                if (!ids.Add(step.StepId))
                {
                    issues.Add(new ContentValidationIssue("CONTENT_DUPLICATE_STEP_ID", field + ".stepId"));
                }
            }
        }
    }
}
