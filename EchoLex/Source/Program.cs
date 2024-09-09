namespace EchoLex;

public class Program
{
    public static void Main(string[] args)
    {
        //Trainer trainer = new();
        //trainer.Train();

        Model model = new();
        model.Load();

        Console.WriteLine("Type something...");

        string input = Console.ReadLine();
        string text = input;

        string lastWords = GetLastWords(text);
        string nextWord = model.Predict(lastWords);

        Console.Clear();
        Console.Write(text);

        //text += Console.ReadLine() + " " + nextWord;

        while (true)
        {
            lastWords = GetLastWords(text, 100);
            nextWord = model.Predict(lastWords);

            text += Console.ReadLine() + " " + nextWord;

            Console.Clear();
            Console.Write(text);
        }
    }

    private static string GetLastWords(string text, int contextLength = 15)
    {
        string[] words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        int takeCount = Math.Min(words.Length, contextLength);

        string[] lastWordsArray = words.Skip(words.Length - takeCount).ToArray();

        string lastWords = string.Join(" ", lastWordsArray);

        return lastWords;
    }
}