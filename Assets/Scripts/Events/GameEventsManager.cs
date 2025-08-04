using Gamegaard.Singleton;

public class GameEventsManager : MonoBehaviourSingleton<GameEventsManager>
{
    public GoldEvents goldEvents;
    public MiscEvents miscEvents;
    public QuestEvents questEvents;
    public InventoryEvents inventoryEvents;
    public CombatEvents combatEvents;

    protected override void Awake()
    {
        base.Awake();

        goldEvents = new GoldEvents();
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        inventoryEvents = new InventoryEvents();
        combatEvents = new CombatEvents();
    }
}
