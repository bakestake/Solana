namespace Gamegaard
{
    public class EntityLifeBarUi : TimedAttributeBarUi
    {
        /// <summary>
        /// Atualiza o valor da barra de vida.
        /// </summary>
        public void UpdateLife(IBarUser life)
        {
            UpdateAttrBar(life);
        }
    }
}
