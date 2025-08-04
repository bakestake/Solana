namespace Gamegaard.RuntimeDebug
{
    public interface ISimilarityCalculator
    {
        int Calculate(string command, string query);
    }
}