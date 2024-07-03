using EchoLex;

static class SimilarityCalculator
{
    public static int Calculate(SimilarityAlgorithm algorithm, List<int> promptWords, List<int> dataWords)
    {
        return algorithm switch
        {
            SimilarityAlgorithm.Presence => CalculatePresenceSimilarity(promptWords, dataWords),
            SimilarityAlgorithm.Position => CalculateWeightedSimilarity(promptWords, dataWords),
            _ => 0
        };
    }

    private static int CalculatePresenceSimilarity(List<int> promptWords, List<int> dataWords)
    {
        int promptLength = promptWords.Count;
        int dataLength = dataWords.Count;

        if (promptWords[promptLength - 1] != dataWords[dataLength - 1])
        {
            return 0;
        }

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

    private static int CalculateWeightedSimilarity(List<int> promptWords, List<int> dataWords)
    {
        int maxLength = Math.Min(promptWords.Count, dataWords.Count);

        if (promptWords[maxLength - 1] != dataWords[maxLength - 1])
        {
            return 0;
        }

        int similarity = 0;
        double weight = 1.0;

        for (int i = maxLength - 1; i >= 0; i--)
        {
            similarity += Convert.ToInt32(promptWords[i] == dataWords[i] ? weight : 0);
            weight *= 0.9;
        }

        return similarity;
    }
}