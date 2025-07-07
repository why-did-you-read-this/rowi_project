using System.ComponentModel.DataAnnotations;

namespace rowi_project.Validation;

public class DateNotInFutureAttribute : ValidationAttribute
{
    public DateNotInFutureAttribute()
    {
        ErrorMessage = "Дата не может быть в будущем";
    }

    public override bool IsValid(object? value)
    {
        if (value == null) return true;
        if (value is DateOnly date)
        {
            return date <= DateOnly.FromDateTime(DateTime.Today);
        }
        return false;
    }
}
