public class BasicToolbarTrigger : ToolbarTrigger
{
    public override void TriggerToolbar(bool selectFirstElement)
    {
        toolbar.OpenToolbar(options, selectFirstElement);
    }
}