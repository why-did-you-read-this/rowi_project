namespace rowi_project.Exceptions;

public class DuplicateEntryException(string message, string? fieldName = null) : Exception(message)
{
    public string? FieldName { get; } = fieldName;
}
