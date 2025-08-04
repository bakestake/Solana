using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard.BasicBehaviours
{
    public class GraphicColorCyclicBehaviour : GraphicCyclicBehaviour
    {
        private Graphic graphic;

        protected override void Awake()
        {
            base.Awake();
            graphic = GetComponent<Graphic>();
        }

        protected override Color CurrentValue 
        { 
            get => graphic.color; 
            set => graphic.color = value; 
        }
    }
}