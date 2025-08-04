using Gamegaard.FarmSystem;

public abstract class PlantReward
{
    public abstract void ReceiveReward(Plant plant, object receiver);
    public abstract string GetRewardDescription(Plant plant, object receiver);
}
