using System.Text.Json;

namespace EchoLex;

static class Model
{
    public static SimilarityAlgorithm Algorithm = SimilarityAlgorithm.Presence;

    private static Dictionary<int, List<PredictionPair>> predictionPairsByOrder = new();
    private static Dictionary<string, int> wordEncodings = new();
    private static Random random = new();

    public static void Load(int maxOrder = 10)
    {
        for (int order = 1; order <= maxOrder; order++)
        {
            string jsonString = File.ReadAllText($"Resources/PredictionPairs/PredictionPairs_Order{order}.json");
            var predictionPairs = JsonSerializer.Deserialize<List<PredictionPair>>(jsonString);
            predictionPairsByOrder[order] = predictionPairs;
        }

        string jsonString2 = File.ReadAllText("Resources/WordEncodings.json");
        wordEncodings = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonString2);
    }

    public static string Predict(string prompt)
    {
        List<int> encodedWords = GetEncodedWords(prompt.Split(' '));

        int order = Math.Min(encodedWords.Count, predictionPairsByOrder.Keys.Max());
        List<PredictionPair> predictionPairs = predictionPairsByOrder[order];

        var topMatch = predictionPairs
            .Select(predictionPair => (predictionPair, SimilarityCalculator.Calculate(Algorithm, encodedWords, predictionPair.Words)))
            .Aggregate((max, next) => next.Item2 > max.Item2 ? next : max);

        string decodedPrediction = GetStringFromInt(topMatch.predictionPair.Prediction);
        return decodedPrediction;
    }

    private static List<int> GetEncodedWords(string[] words)
    {
        return words.Select(word => wordEncodings.TryGetValue(word, out int encoding) ? encoding : 0).ToList();
    }

    private static string GetStringFromInt(int number)
    {
        return wordEncodings.FirstOrDefault(x => x.Value == number).Key;
    }
}