using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLModelTrainer.MLModels
{
    public class EventRating
    {
        [LoadColumn(0)]
        [KeyType(count: 1000)]  // Estimate max number of users
        public string UserId;

        [LoadColumn(1)]
        [KeyType(count: 1000)]  // Estimate max number of events
        public int EventId;

        public string EventCategory;

        [LoadColumn(2)]
        public float Label;
        
    }
}


