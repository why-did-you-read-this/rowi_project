namespace rowi_project.Exceptions;

public class DuplicateEntryException : Exception
{
    public string? FieldName { get; }

    public DuplicateEntryException(string message, string? fieldName = null) : base(message)
    {
        FieldName = fieldName;
    }
}
