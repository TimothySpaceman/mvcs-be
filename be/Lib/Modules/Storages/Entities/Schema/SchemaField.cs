namespace Lib.Modules.Storages.Entities.Schema;

public enum SchemaFieldType
{
    Text = 0,
    Number = 1,
    Boolean = 2,
    Select = 3,
    Password = 4
}

public abstract class SchemaField
{
    public string Key { get; init; } = null!;
    public string Label { get; init; } = null!;
    public string? Description { get; init; }
    public bool Required { get; init; }
    public SchemaFieldType Type { get; init; }
}