namespace Lib.Modules.Storages.Entities.Schema;

public class BooleanSchemaField : SchemaField
{
    public bool DefaultValue { get; init; }

    public BooleanSchemaField()
    {
        Type = SchemaFieldType.Boolean;
    }
}