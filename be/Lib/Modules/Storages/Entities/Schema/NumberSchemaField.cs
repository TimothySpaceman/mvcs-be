namespace Lib.Modules.Storages.Entities.Schema;

public class NumberSchemaField : SchemaField
{
    public double? Min { get; init; }
    public double? Max { get; init; }
    public double? Step { get; init; }
    public string? Placeholder { get; init; }

    public NumberSchemaField()
    {
        Type = SchemaFieldType.Number;
    }
}