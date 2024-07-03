namespace EchoLex;

class Program
{
    public static void Main(string[] args)
    {
        Trainer.Train(10);
        Model.Load();

        Console.WriteLine("Type something...");

        string input = Console.ReadLine();
        string text = input;

        string lastWords = GetLastWords(text);
        string nextWord = Model.Predict(lastWords);

        text += Console.ReadLine() + " " + nextWord;

        Console.Clear();
        Console.Write(text);

        while (true)
        {
            lastWords = GetLastWords(text);
            nextWord = Model.Predict(lastWords);

            text += Console.ReadLine() + " " + nextWord;

            Console.Clear();
            Console.Write(text);
        }
    }

    private static string GetLastWords(string text)
    {
        string[] words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        int takeCount = Math.Min(words.Length, 10);

        string[] lastWordsArray = words.Skip(words.Length - takeCount).ToArray();

        string lastWords = string.Join(" ", lastWordsArray);

        return lastWords;
    }
}