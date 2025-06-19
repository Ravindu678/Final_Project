using Microsoft.ML.Data;


namespace BookIt1.MLModels
{
    public class EventRating
    {
        [LoadColumn(0)]
        public float UserId;

        [LoadColumn(1)]
        public float EventId;

        [LoadColumn(2)]
        public float Label; // 1 = booked, 0 = not booked
    }
}
