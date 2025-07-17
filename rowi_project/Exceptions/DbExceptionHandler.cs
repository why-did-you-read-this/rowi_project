using Npgsql;

namespace rowi_project.Exceptions;

public class DbExceptionHandler
{
    public static int HandleUniqueViolation(PostgresException pgEx)
    {
        var message = pgEx.ConstraintName switch
        {
            "IX_Companies_ShortName" => "Компания с таким кратким наименованием уже существует",
            "IX_Companies_FullName" => "Компания с таким полным наименованием уже существует",
            "IX_Companies_Inn" => "Компания с таким ИНН уже существует",
            "IX_Companies_Ogrn" => "Компания с таким ОГРН уже существует",
            "IX_Companies_RepEmail" => "Представитель с таким email уже существует",
            "IX_Companies_RepPhoneNumber" => "Представитель с таким телефоном уже существует",
            _ => "Нарушено уникальное ограничение",
        };

        throw new DuplicateEntryException(message);
    }
}
