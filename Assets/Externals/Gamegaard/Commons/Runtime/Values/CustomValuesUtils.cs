using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamegaard.CustomValues
{
    public static class CustomValuesUtils
    {
        public static T GetRandomWeightedElement<T>(this IEnumerable<T> elements) where T : IWeightValue
        {
            float totalWeight = 0f;
            foreach (T element in elements)
            {
                totalWeight += element.Weight;
            }

            float randomWeight = Random.Range(0f, totalWeight);
            foreach (T element in elements)
            {
                if (randomWeight < element.Weight)
                {
                    return element;
                }
                randomWeight -= element.Weight;
            }

            Debug.LogWarning("No Element Found!");
            return default;
        }

        public static G GetRandomWeightedResult<G>(this IEnumerable<IWeightValueResult<G>> elements)
        {
            float totalWeight = 0f;

            foreach (IWeightValueResult<G> trunkWeight in elements)
            {
                totalWeight += trunkWeight.Weight;
            }

            float randomWeight = Random.Range(0f, totalWeight);

            foreach (IWeightValueResult<G> trunkWeight in elements)
            {
                float weightValue = trunkWeight.Weight;
                if (randomWeight < weightValue)
                {
                    return trunkWeight.GetValue();
                }
                randomWeight -= weightValue;
            }

            return elements.GetEnumerator().Current.GetValue();
        }

        public static T[] GetRandomWeightedElements<T>(this IEnumerable<T> elements, int amount, bool allowDuplicates = true) where T : IWeightValue
        {
            List<T> selectedElements = new List<T>();
            List<T> availableElements = elements.ToList();

            for (int i = 0; i < amount; i++)
            {
                if (!allowDuplicates && availableElements.Count == 0)
                {
                    Debug.LogWarning("Not enough unique elements to satisfy the request.");
                    break;
                }

                T selected = availableElements.GetRandomWeightedElement();
                selectedElements.Add(selected);

                if (!allowDuplicates)
                {
                    availableElements.Remove(selected);
                }
            }

            return selectedElements.ToArray();
        }

        public static G[] GetRandomWeightedResults<G>(this IEnumerable<IWeightValueResult<G>> elements, int amount, bool allowDuplicates = true)
        {
            List<G> selectedElements = new List<G>();
            List<IWeightValueResult<G>> availableElements = elements.ToList();

            for (int i = 0; i < amount; i++)
            {
                if (!allowDuplicates && availableElements.Count == 0)
                {
                    Debug.LogWarning("Not enough unique results to satisfy the request.");
                    break;
                }

                IWeightValueResult<G> selected = availableElements.GetRandomWeightedElement();
                selectedElements.Add(selected.GetValue());

                if (!allowDuplicates)
                {
                    availableElements.Remove(selected);
                }
            }

            return selectedElements.ToArray();
        }

    }
}