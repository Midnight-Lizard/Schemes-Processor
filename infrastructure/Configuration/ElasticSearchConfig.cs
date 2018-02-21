namespace MidnightLizard.Schemes.Infrastructure.Configuration
{
    public class ElasticSearchConfig
    {
        public string ELASTIC_SEARCH_CLIENT_URL { get; set; }
        public string ELASTIC_SEARCH_SCHEMES_EVENT_STORE_INDEX_NAME { get; set; }
        public int ELASTIC_SEARCH_SHARDS { get; set; } = 2;
        public int ELASTIC_SEARCH_REPLICAS { get; set; } = 1;

    }
}