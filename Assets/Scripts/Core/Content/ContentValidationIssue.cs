namespace ChemistryLab.Core.Content
{
    public sealed class ContentValidationIssue
    {
        public ContentValidationIssue(string code, string field)
        {
            Code = code;
            Field = field;
        }

        public string Code { get; }

        public string Field { get; }
    }
}

