namespace Shared.Configuration
{
    public class CosmosDbConfiguration
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public bool CreateIfNotExists { get; set; }
    }
}