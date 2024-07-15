namespace notip_server.Service
{
    public class DailyResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string api_created { get; set; }
        public string privacy { get; set; }
        public string url { get; set; }
        public DateTime created_at { get; set; }
        public IDictionary<string, object> config { get; set; }

    }
}
