using System.Text.Json;

namespace EchoLex;

public class Trainer
{
    private List<string> uniqueWords = new();
    private Dictionary<string, int> wordEncodings = new();

    public void Train(int maxOrder = 10)
    {
        Directory.CreateDirectory("Resources/PredictionPairs");
        Directory.CreateDirectory("Resources/Dataset");

        string[] files = Directory.GetFiles("Training");

        foreach (string file in files)
        {
            string[] words = File.ReadAllText(file).Split(' ');

            UpdateWordEncodings(words);
        }

        SaveWordEncodings();
        CreateEncodedFiles(files);
    }

    private void UpdateWordEncodings(string[] words)
    {
        foreach (string word in words)
        {
            if (!wordEncodings.ContainsKey(word))
            {
                wordEncodings[word] = wordEncodings.Count + 1; // Incremental encoding
                uniqueWords.Add(word);
            }
        }
    }

    private void SaveWordEncodings()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(wordEncodings, options);
        File.WriteAllText("Resources/WordEncodings.json", json);
    }

    private void CreateEncodedFiles(string[] files)
    {
        foreach (string file in files)
        {
            string[] words = File.ReadAllText(file).Split(' ');
            string[] encodedWords = words.Select(word => wordEncodings[word].ToString()).ToArray();

            string fileName = Path.GetFileName(file);
            string encodedFilePath = Path.Combine("Resources/Dataset", fileName);

            File.WriteAllText(encodedFilePath, string.Join(" ", encodedWords));
        }
    }
}
