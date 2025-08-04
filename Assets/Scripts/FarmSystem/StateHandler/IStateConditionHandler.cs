namespace Gamegaard.FarmSystem
{
    public interface IStateConditionHandler
    {
        bool ContainsCondition(string name);
        bool AddCondition(string name);
        bool RemoveCondition(string name);
        void ClearConditions();
    }
}