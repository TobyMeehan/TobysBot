namespace TobysBot.Mongo.Configuration;

public class MongoOptions
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? HangfireDatabaseName { get; set; }
}