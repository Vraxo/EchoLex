using System.Text.Json;

namespace EchoLex;

public class Model
{
    private Dictionary<string, int> wordEncodings = new();
    private bool useWeightedSimilarity = false; // Flag to use weighted similarity

    public void Load()
    {
        Console.WriteLine("Loading...");

        string jsonString = File.ReadAllText("Resources/WordEncodings.json");
        wordEncodings = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonString);
    }

    public void SetUseWeightedSimilarity(bool useWeighted)
    {
        useWeightedSimilarity = useWeighted;
    }

    public string Predict(string prompt)
    {
        string[] words = prompt.Split(' ');
        int order = words.Length;

        List<int> encodedWords = GetEncodedWords(words);

        string bestMatch = null;
        int bestSimilarity = int.MinValue;
        List<int> bestDataWords = null;

        string[] files = Directory.GetFiles("Resources/Dataset");

        // Use tasks to process each file concurrently
        List<Task> tasks = new List<Task>();
        foreach (string file in files)
        {
            tasks.Add(Task.Run(() =>
            {
                string[] fileContent = File.ReadAllText(file).Split(' ');

                for (int i = 0; i <= fileContent.Length - order; i++)
                {
                    List<int> dataWords = fileContent.Skip(i).Take(order).Select(int.Parse).ToList();

                    int similarity = CalculateSimilarity(encodedWords, dataWords);

                    lock (this) // Use instance-level locking
                    {
                        if (similarity > bestSimilarity)
                        {
                            bestSimilarity = similarity;
                            bestMatch = fileContent[i + order]; // Get the next word as prediction
                            bestDataWords = dataWords;
                        }
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray()); // Wait for all tasks to complete

        if (bestMatch != null)
        {
            //Console.WriteLine($"\nBest matching sequence: {GetWordsFromEncoded(bestDataWords)}");
            int chosenPrediction = int.Parse(bestMatch);
            return GetStringFromInt(chosenPrediction);
        }
        else
        {
            return string.Empty;
        }
    }

    private List<int> GetEncodedWords(string[] words)
    {
        return words.Select(word => wordEncodings.ContainsKey(word) ? wordEncodings[word] : 0).ToList();
    }

    private string GetWordsFromEncoded(List<int> encodedWords)
    {
        return string.Join(" ", encodedWords.Select(encoded => wordEncodings.FirstOrDefault(x => x.Value == encoded).Key));
    }

    private int CalculateSimilarity(List<int> promptWords, List<int> dataWords)
    {
        if (promptWords.Last() != dataWords.Last())
        {
            return 0; // Return bad score if last words are not the same
        }

        if (useWeightedSimilarity)
        {
            return CalculateWeightedSimilarity(promptWords, dataWords);
        }
        else
        {
            return CalculatePresenceSimilarity(promptWords, dataWords);
        }
    }

    private int CalculatePresenceSimilarity(List<int> promptWords, List<int> dataWords)
    {
        int similarity = 0;

        foreach (int promptWord in promptWords)
        {
            if (dataWords.Contains(promptWord))
            {
                similarity++;
            }
        }

        return similarity;
    }

    private int CalculateWeightedSimilarity(List<int> promptWords, List<int> dataWords)
    {
        int maxLength = Math.Min(promptWords.Count, dataWords.Count);
        int similarity = 0;
        double weight = 1.0;

        for (int i = 0; i < maxLength; i++)
        {
            similarity += Convert.ToInt32(promptWords[i] == dataWords[i] ? weight : 0);
            weight *= 0.9;
        }

        return similarity;
    }

    public string GetStringFromInt(int number)
    {
        return wordEncodings.FirstOrDefault(x => x.Value == number).Key;
    }
}
