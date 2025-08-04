using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    //TODO: Não faz sentido. Possivelmente não é o que quero fazer, talvez não faça sentido ter parser para isso, pois ao invés de "value" ser o nome do monobehaviour, é na verdade o nome do game object.
    public class MonoBehaviourParser : ITypeParser<MonoBehaviour>
    {
        public MonoBehaviour Parse(string value)
        {
            GameObject obj = GameObject.Find(value);
            if (obj != null)
            {
                var component = obj.GetComponent<MonoBehaviour>();
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }
    }
}