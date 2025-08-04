namespace Gamegaard
{
    public interface IFertilizable
    {
        FertilizerData CurrentFertilizer { get; }
        public void SetFertilized(FertilizerData fertilizerData);
    }
}