namespace Lib.Modules.Storages.Entities.Schema;

public class PasswordSchemaField : SchemaField
{
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public string? Placeholder { get; init; }

    public PasswordSchemaField()
    {
        Type = SchemaFieldType.Password;
    }
}