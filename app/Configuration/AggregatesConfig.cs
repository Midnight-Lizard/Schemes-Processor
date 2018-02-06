using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Configuration
{
    public class AggregatesConfig
    {
        public bool AGGREGATES_CACHE_ENABLED { get; set; }
        public int AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS { get; set; }
        public int AGGREGATES_CACHE_ABSOLUTE_EXPIRATION_SECONDS { get; set; }
        public int AGGREGATES_MAX_EVENTS_COUNT { get; set; }
    }
}
