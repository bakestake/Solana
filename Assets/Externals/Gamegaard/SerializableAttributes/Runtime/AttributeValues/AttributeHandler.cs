using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gamegaard.SerializableAttributes
{

    [Serializable]
    public class AttributeHandler
    {
        [SerializeField]
        private AttributesDictionary attributes = new AttributesDictionary();

        public AttributesDictionary Attributes => attributes;

        /// <summary>
        /// Define o valor de um atributo existente
        /// </summary>
        public void SetAttributeValue(string key, object value)
        {
            if (ContainsAttribute(key))
            {
                attributes[key].Set(value);
            }
            else
            {
                throw new KeyNotFoundException($"Atributo '{key}' não encontrado para definir valor.");
            }
        }

        /// <summary>
        /// Adiciona um novo atributo com IAttributeValue. Não adiciona se o atributo já existe.
        /// </summary>
        public void AddAttribute(string key, IAttributeValue value)
        {
            if (!ContainsAttribute(key))
            {
                attributes[key] = value;
            }
        }

        /// <summary>
        /// Define o valor de um atributo existente ou adiciona um novo atributo.
        /// </summary>
        public void SetAttribute(string key, IAttributeValue value)
        {
            attributes[key] = value;
        }

        /// <summary>
        /// Remove um atributo pelo nome.
        /// </summary>
        public void RemoveAttribute(string key)
        {
            attributes.Remove(key);
        }

        /// <summary>
        /// Retorna a quantidade total de atributos.
        /// </summary>
        public int GetAttributeCount()
        {
            return attributes.Count;
        }

        /// <summary>
        /// Retorna a quantidade de atributos de um determinado tipo.
        /// </summary>
        public int GetAttributeCount(Type type)
        {
            return attributes.Values.Count(x => x.ValueType == type);
        }

        /// <summary>
        /// Verifica se o atributo existe.
        /// </summary>
        public bool ContainsAttribute(string name)
        {
            return attributes.ContainsKey(name);
        }

        /// <summary>
        /// Verifica se o atributo existe e se possui o mesmo tipo solicitado
        /// </summary>
        public bool ContainsAttribute<T>(string name)
        {
            return attributes.ContainsKey(name) && attributes[name].ValueType == typeof(T);
        }

        /// <summary>
        /// Obtém o valor de um atributo de um determinado tipo.
        /// </summary>
        public T GetAttribute<T>(string name)
        {
            if (attributes.TryGetValue(name, out var value))
            {
                if (value is AttributeValue<T> typedValue)
                {
                    return typedValue.Value;
                }
                else if (value is IAttributeValue genericAttribute)
                {
                    object rawValue = genericAttribute.Get();
                    if (rawValue is T castedValue)
                    {
                        return castedValue;
                    }

                    throw new InvalidCastException($"Atributo '{name}' não pode ser convertido para o tipo {typeof(T)}. O tipo real é {rawValue?.GetType().Name ?? "null"}.");
                }
                throw new InvalidCastException($"Atributo '{name}' não é do tipo {typeof(T)}");
            }
            throw new KeyNotFoundException($"Atributo '{name}' não encontrado");
        }

        /// <summary>
        /// Tenta obter o valor de um atributo pelo nome, verificando se ele é compatível com o tipo esperado.
        /// </summary>
        /// <typeparam name="T">O tipo de valor esperado.</typeparam>
        /// <param name="name">O nome do atributo.</param>
        /// <param name="value">O valor do atributo, se encontrado e compatível.</param>
        /// <returns>True se o atributo for encontrado e compatível; caso contrário, false.</returns>
        public bool TryGetAttribute<T>(string name, out T value)
        {
            if (attributes.TryGetValue(name, out var currentValue))
            {
                if (currentValue is AttributeValue<T> typedValue)
                {
                    value = typedValue.Value;
                    return true;
                }
                else if (currentValue is IAttributeValue genericAttribute)
                {
                    object rawValue = genericAttribute.Get();
                    if (rawValue is T castedValue)
                    {
                        value = castedValue;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gera uma mensagem com todos os atributos, tipos e valores armazenados no item.
        /// </summary>
        public void DebugAttributes()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Total de Atributos: {attributes.Count}");

            int index = 1;
            foreach (KeyValuePair<string, IAttributeValue> item in attributes)
            {
                if (item.Value == null)
                {
                    sb.AppendLine($"{index} - {item.Key}: null");
                }
                else
                {
                    string type = item.Value.ValueType.ToString();
                    sb.AppendLine($"{index} - [{type}] {item.Key}: {item.Value.Get()}");
                }
                index++;
            }

            Debug.Log(sb.ToString());
        }

        public void Merge(AttributeHandler otherAttributes)
        {
            foreach (KeyValuePair<string, IAttributeValue> attribute in otherAttributes.Attributes)
            {
                if (!attributes.ContainsKey(attribute.Key))
                {
                    attributes[attribute.Key] = attribute.Value;
                }
            }
        }

        public void Clear()
        {
            attributes.Clear();
        }
    }
}