namespace backend.settings;


public class DatabaseSettings<T>
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
    public required string CollectionName { get; set; }
}

