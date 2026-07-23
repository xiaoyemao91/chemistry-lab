namespace ChemistryLab.Core.Content
{
    public sealed class ContentStepDefinition
    {
        public ContentStepDefinition(string stepId, string title)
        {
            StepId = stepId;
            Title = title;
        }

        public string StepId { get; }

        public string Title { get; }

        internal bool HasRequiredFields()
        {
            return !string.IsNullOrWhiteSpace(StepId) && !string.IsNullOrWhiteSpace(Title);
        }
    }
}

