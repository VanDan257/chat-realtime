using System.Diagnostics.CodeAnalysis;

namespace notip_server.Models
{
    public class TrafficStatisticsResult
    {
        public int? Year { get; set; }
        public int Month { get; set; }
        public int? Day { get; set; }
        public int MessageCount { get; set; }
        public int LoginCount { get; set; }
    }
}
