using System;
using System.IO;
using ChemistryLab.Core.Records;
using ChemistryLab.Infrastructure.Records;
using NUnit.Framework;

namespace ChemistryLab.Tests.EditMode.Records
{
    public sealed class ExperimentRecordJsonStoreTests
    {
        private string temporaryDirectory;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectory = Path.Combine(Path.GetTempPath(), "ChemistryLabTests", Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(temporaryDirectory, true);
            }
        }

        [Test]
        public void SaveThenLoadPreservesRecordFields()
        {
            var record = CreateRecord();
            var store = new ExperimentRecordJsonStore(temporaryDirectory);

            var saveResult = store.Save(record);
            var loadResult = store.Load(record.RecordId.ToString("D"));

            Assert.That(saveResult.IsSuccess, Is.True);
            Assert.That(loadResult.IsSuccess, Is.True);
            Assert.That(loadResult.Record.ExperimentId, Is.EqualTo(record.ExperimentId));
            Assert.That(loadResult.Record.ContentVersion, Is.EqualTo(record.ContentVersion));
            Assert.That(loadResult.Record.WorkflowStatus, Is.EqualTo(record.WorkflowStatus));
            Assert.That(loadResult.Record.CurrentStepId, Is.EqualTo(record.CurrentStepId));
            Assert.That(loadResult.Record.CreatedAtUtc, Is.EqualTo(record.CreatedAtUtc));
        }

        [Test]
        public void LoadWithPathLikeIdReturnsStructuredError()
        {
            var result = new ExperimentRecordJsonStore(temporaryDirectory).Load("../untrusted-file");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(RecordErrorCode.InvalidRecordId));
        }

        [Test]
        public void LoadMissingRecordReturnsStructuredError()
        {
            var result = new ExperimentRecordJsonStore(temporaryDirectory).Load(Guid.NewGuid().ToString("D"));

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(RecordErrorCode.RecordNotFound));
        }

        private static ExperimentRecord CreateRecord()
        {
            return new ExperimentRecord(
                Guid.NewGuid(),
                "synthetic-fe-measurement",
                "synthetic-1.0",
                "Completed",
                "record-result",
                new DateTime(2026, 7, 24, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
