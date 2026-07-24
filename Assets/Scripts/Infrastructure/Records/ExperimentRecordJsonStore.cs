using System;
using System.Globalization;
using System.IO;
using System.Text;
using ChemistryLab.Core.Records;
using UnityEngine;

namespace ChemistryLab.Infrastructure.Records
{
    public sealed class ExperimentRecordJsonStore
    {
        private readonly string recordsDirectory;

        public ExperimentRecordJsonStore(string recordsDirectory)
        {
            if (string.IsNullOrWhiteSpace(recordsDirectory))
            {
                throw new ArgumentException("Records directory is required.", nameof(recordsDirectory));
            }

            this.recordsDirectory = Path.GetFullPath(recordsDirectory);
        }

        public RecordSaveResult Save(ExperimentRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var destinationPath = GetRecordPath(record.RecordId);
            var temporaryPath = destinationPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                Directory.CreateDirectory(recordsDirectory);
                File.WriteAllText(temporaryPath, Serialize(record), new UTF8Encoding(false));
                if (File.Exists(destinationPath))
                {
                    File.Replace(temporaryPath, destinationPath, null);
                }
                else
                {
                    File.Move(temporaryPath, destinationPath);
                }

                return RecordSaveResult.Success();
            }
            catch (IOException)
            {
                return RecordSaveResult.Failure(RecordErrorCode.StorageFailure);
            }
            catch (UnauthorizedAccessException)
            {
                return RecordSaveResult.Failure(RecordErrorCode.StorageFailure);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        public RecordLoadResult Load(string recordId)
        {
            if (!Guid.TryParse(recordId, out var parsedRecordId))
            {
                return RecordLoadResult.Failure(RecordErrorCode.InvalidRecordId);
            }

            var path = GetRecordPath(parsedRecordId);
            if (!File.Exists(path))
            {
                return RecordLoadResult.Failure(RecordErrorCode.RecordNotFound);
            }

            try
            {
                var dto = JsonUtility.FromJson<ExperimentRecordDto>(File.ReadAllText(path, Encoding.UTF8));
                if (dto == null)
                {
                    return RecordLoadResult.Failure(RecordErrorCode.InvalidDocument);
                }

                var record = new ExperimentRecord(
                    Guid.Parse(dto.recordId),
                    dto.experimentId,
                    dto.contentVersion,
                    dto.workflowStatus,
                    dto.currentStepId,
                    DateTime.Parse(dto.createdAtUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
                return RecordLoadResult.Success(record);
            }
            catch (Exception exception) when (
                exception is ArgumentException ||
                exception is FormatException ||
                exception is IOException ||
                exception is UnauthorizedAccessException)
            {
                return RecordLoadResult.Failure(RecordErrorCode.InvalidDocument);
            }
        }

        private string GetRecordPath(Guid recordId)
        {
            return Path.Combine(recordsDirectory, recordId.ToString("D") + ".json");
        }

        private static string Serialize(ExperimentRecord record)
        {
            var dto = new ExperimentRecordDto
            {
                recordId = record.RecordId.ToString("D"),
                experimentId = record.ExperimentId,
                contentVersion = record.ContentVersion,
                workflowStatus = record.WorkflowStatus,
                currentStepId = record.CurrentStepId,
                createdAtUtc = record.CreatedAtUtc.ToString("O", CultureInfo.InvariantCulture)
            };
            return JsonUtility.ToJson(dto, true);
        }

        [Serializable]
        private sealed class ExperimentRecordDto
        {
            public string recordId;
            public string experimentId;
            public string contentVersion;
            public string workflowStatus;
            public string currentStepId;
            public string createdAtUtc;
        }
    }
}
