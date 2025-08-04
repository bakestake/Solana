using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    //TODO: Não faz sentido. Possivelmente não é o que quero fazer, talvez não faça sentido ter parser para isso, pois ao invés de "value" ser o nome do componente, é na verdade o nome do game object.
    public class ComponentParser : ITypeParser<Component>
    {
        public Component Parse(string value)
        {
            GameObject obj = GameObject.Find(value);
            if (obj != null)
            {
                if (obj.TryGetComponent<Component>(out var component))
                {
                    return component;
                }
            }
            return null;
        }
    }
}