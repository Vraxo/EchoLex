namespace EchoLex;

public record PredictionPair(List<int> Words, int Prediction)
{
    public string GetSentence(Model model)
    {
        string sentence = "";

        foreach (int word in Words)
        {
            sentence += model.GetStringFromInt(word) + " ";
        }

        return sentence;
    }
}