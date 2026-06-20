namespace Lib.Modules.Storages.Entities.Schema;

public class TextSchemaField : SchemaField
{
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public string? Placeholder { get; init; }
    public string? Pattern { get; init; }

    public TextSchemaField()
    {
        Type = SchemaFieldType.Text;
    }
}