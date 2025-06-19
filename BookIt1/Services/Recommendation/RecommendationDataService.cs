using CsvHelper;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BookIt1.Data; // For ApplicationDbContext
using BookIt1.Models; // For Event, etc.
using CsvHelper.Configuration;
using BookIt1.MLModels;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using BookIt1.Areas.Admin.Models;

namespace BookIt1.Services.Recommendation
{
    public class RecommendationDataService
    {
        private readonly BookIt1DbContext _context;
        private readonly IWebHostEnvironment _env;

        public RecommendationDataService(BookIt1DbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public void GenerateTrainingDataCsv()
        {
            var bookingsPath = Path.Combine(_env.WebRootPath, "data", "bookings.csv");
            var trainingDataPath = Path.Combine(_env.WebRootPath, "data", "trainingdata.csv");

            if (!File.Exists(bookingsPath)) return;

            var trainingData = new List<TrainingDataRow>();

            using (var reader = new StreamReader(bookingsPath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    string userId = record.UserId;
                    string eventId = record.EventId;

                    var evt = _context.Events.FirstOrDefault(e => e.Id.ToString() == eventId);
                    if (evt != null)
                    {
                        trainingData.Add(new TrainingDataRow
                        {
                            UserId = userId,
                            EventId = eventId,
                            EventCategory = evt.Category
                        });
                    }
                }
            }

            using (var writer = new StreamWriter(trainingDataPath))
            using (var csv = new CsvHelper.CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteHeader<TrainingDataRow>();
                csv.NextRecord();
                foreach (var row in trainingData)
                {
                    csv.WriteRecord(row);
                    csv.NextRecord();
                }
            }
        }
        public Dictionary<string, List<string>> GetUserCategoryRankingFromCsv()
        {
            var trainingDataPath = Path.Combine(_env.WebRootPath, "data", "trainingdata.csv");
            var categoryMap = new Dictionary<string, List<string>>();

            if (!File.Exists(trainingDataPath)) return categoryMap;

            using (var reader = new StreamReader(trainingDataPath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<TrainingDataRow>().ToList();

                categoryMap = records
                    .GroupBy(r => r.UserId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.GroupBy(r => r.EventCategory)
                              .OrderByDescending(x => x.Count())
                              .Select(x => x.Key)
                              .ToList()
                    );
            }

            return categoryMap;
        }
        public List<Event> GetRecommendedEventsFromCsv(string userId)
        {
            var categoryMap = GetUserCategoryRankingFromCsv();
            if (!categoryMap.ContainsKey(userId)) return new List<Event>();

            var preferredCategories = categoryMap[userId];

            var upcomingEvents = _context.Events
                .Where(e => e.EventDate > DateTime.Now)
                .ToList();

            var orderedEvents = upcomingEvents
                .OrderBy(e =>
                    preferredCategories.IndexOf(e.Category) == -1
                        ? int.MaxValue
                        : preferredCategories.IndexOf(e.Category))
                .ThenBy(e => e.EventDate)
                .ToList();

            return orderedEvents;
        }

    }
}
