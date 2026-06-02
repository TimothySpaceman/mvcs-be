namespace Lib.Modules.Storages.Entities.Schema;

public record SelectSchemaFieldOption(string Value, string Label);

public class SelectSchemaField : SchemaField
{
    public string? Placeholder { get; init; }
    public IReadOnlyList<SelectSchemaFieldOption> Options { get; init; } = [];

    public SelectSchemaField()
    {
        Type = SchemaFieldType.Select;
    }
}