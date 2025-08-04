namespace Gamegaard.RuntimeDebug
{
    public class TwoStateVisibilityObject : TwoStateObject
    {
        public override void Active()
        {
            base.Active();
            gameObject.SetActive(true);
        }

        public override void Desactive()
        {
            base.Desactive();
            gameObject.SetActive(false);
        }
    }
}