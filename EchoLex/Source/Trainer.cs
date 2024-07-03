using System.Text.Json;

namespace EchoLex;

static class Trainer
{
    private static List<string> uniqueWords = new();
    private static Dictionary<string, int> wordEncodings = new();

    public static void Train(int order = 10)
    {
        Directory.CreateDirectory("Resources/PredictionPairs");

        string[] files = Directory.GetFiles("Training");

        foreach (string file in files)
        {
            TrainFile(file, order);
        }

        SaveWordEncodings();
    }

    private static void TrainFile(string file, int order)
    {
        string[] words = File.ReadAllText(file).Split(' ');

        foreach (string word in words)
        {
            if (!uniqueWords.Contains(word))
            {
                uniqueWords.Add(word);
                wordEncodings[word] = uniqueWords.Count;
            }
        }

        for (int currentOrder = 1; currentOrder <= order; currentOrder++)
        {
            List<PredictionPair> predictionPairs = new();

            for (int i = 0; i < words.Length - currentOrder; i++)
            {
                List<int> sequence = new();

                for (int j = 0; j < currentOrder; j++)
                {
                    sequence.Add(wordEncodings[words[i + j]]);
                }

                predictionPairs.Add(new PredictionPair(sequence, wordEncodings[words[i + currentOrder]]));
            }

            SavePredictions(predictionPairs, currentOrder);
        }
    }

    private static void SavePredictions(List<PredictionPair> predictionPairs, int order)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(predictionPairs, options);
        File.WriteAllText($"Resources/PredictionPairs/PredictionPairs_Order{order}.json", json);
    }

    private static void SaveWordEncodings()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(wordEncodings, options);
        File.WriteAllText("Resources/WordEncodings.json", json);
    }
}
