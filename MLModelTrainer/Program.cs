using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using Microsoft.ML.Data;
using MLModelTrainer.MLModels;

class Program
{
    static void Main(string[] args)
    {
        var mlContext = new MLContext();

        string dataPath = @"E:\ESOFT\All SE\RealSystemsEsoft\EventBooking\BookIt1_v17\BookIt1\wwwroot\data\trainingData.csv";
        string modelPath = @"E:\ESOFT\All SE\RealSystemsEsoft\EventBooking\BookIt1_v17\BookIt1\MLModels\EventRecommender.zip";


        var dataView = mlContext.Data.LoadFromTextFile<EventRating>(
            dataPath, hasHeader: true, separatorChar: ',');


        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = nameof(EventRating.UserId),
            MatrixRowIndexColumnName = nameof(EventRating.EventId),
            LabelColumnName = nameof(EventRating.Label),
            NumberOfIterations = 20,
            ApproximationRank = 100
        };

        var pipeline = mlContext.Recommendation().Trainers.MatrixFactorization(options);
        var model = pipeline.Fit(dataView);

        mlContext.Model.Save(model, dataView.Schema, modelPath);

        Console.WriteLine("Model trained and saved to: " + modelPath);
    }
}
