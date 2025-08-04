using System;
using UnityEngine;

namespace Gamegaard.SerializableAttributes
{
    public interface IAttributeValue
    {
        Type ValueType { get; }
        object Get();
        void Set(object value);
    }

    public abstract class AttributeValue : IAttributeValue
    {
        public abstract Type ValueType { get; }
        public abstract object Get();
        public abstract void Set(object value);
    }

    [Serializable]
    public abstract class AttributeValue<T> : AttributeValue
    {
        [SerializeReference] protected T value;

        public AttributeValue()
        {

        }

        public AttributeValue(T value)
        {
            this.value = value;
        }

        public override Type ValueType => typeof(T);

        public virtual T Value
        {
            get => value;
            set => this.value = value;
        }

        public override object Get()
        {
            return value;
        }
    }

    [Serializable]
    public class IntAttributeValue : AttributeValue<int>
    {
        public IntAttributeValue()
        {
        }

        public IntAttributeValue(int value) : base(value)
        {
        }

        public override void Set(object value) => this.value = (int)value;
    }

    [Serializable]
    public class StringAttributeValue : AttributeValue<string>
    {
        public StringAttributeValue()
        {
        }

        public StringAttributeValue(string value) : base(value)
        {
        }

        public override void Set(object value) => this.value = (string)value;
    }

    [Serializable]
    public class FloatAttributeValue : AttributeValue<float>
    {
        public FloatAttributeValue()
        {
        }

        public FloatAttributeValue(float value) : base(value)
        {
        }

        public override void Set(object value) => this.value = (float)value;
    }

    [Serializable]
    public class BoolAttributeValue : AttributeValue<bool>
    {
        public BoolAttributeValue()
        {
        }

        public BoolAttributeValue(bool value) : base(value)
        {
        }

        public override void Set(object value) => this.value = (bool)value;
    }

    [Serializable]
    public class CharAttributeValue : AttributeValue<char>
    {
        public CharAttributeValue()
        {
        }

        public CharAttributeValue(char value) : base(value)
        {
        }

        public override void Set(object value) => this.value = (char)value;
    }

    [Serializable]
    public class GameObjectAttributeValue : AttributeValue<GameObject>
    {
        public GameObjectAttributeValue()
        {
        }

        public GameObjectAttributeValue(GameObject value) : base(value)
        {
        }

        public override void Set(object value) => this.value = (GameObject)value;
    }

    [Serializable]
    public class UnityObjectAttributeValue : AttributeValue<UnityEngine.Object>
    {
        public UnityObjectAttributeValue()
        {
        }

        public UnityObjectAttributeValue(UnityEngine.Object value) : base(value)
        {
        }

        public override void Set(object value) => this.value = (GameObject)value;
    }

    [Serializable]
    public class ScriptableObjectAttributeValue : AttributeValue<ScriptableObject>
    {
        public ScriptableObjectAttributeValue()
        {
        }

        public ScriptableObjectAttributeValue(ScriptableObject value) : base(value)
        {
        }

        public override void Set(object value) => this.value = (ScriptableObject)value;
    }

    public abstract class ComplexAttributeValue<T> : AttributeValue<T>
    {
        [SerializeField] protected string typeName;
        [SerializeField] protected bool isInitialized;
        public bool IsInitialized => isInitialized;

        public virtual void Initialize(Type enumType)
        {
            isInitialized = true;
        }
    }

    [Serializable]
    public class EnumAttributeValue : ComplexAttributeValue<Enum>
    {
        public override Enum Value
        {
            get
            {
                if (value == null && !string.IsNullOrEmpty(typeName))
                {
                    Type enumType = Type.GetType(typeName);
                    if (enumType != null && enumType.IsEnum)
                    {
                        value = (Enum)Enum.GetValues(enumType).GetValue(0);
                    }
                    else
                    {
                        Debug.LogError($"Failed to resolve enum type: {typeName}");
                    }
                }

                return value;
            }
            set => this.value = value;
        }

        public EnumAttributeValue() { }

        public EnumAttributeValue(Type enumType)
        {
            Initialize(enumType);
        }

        public override void Initialize(Type enumType)
        {
            if (enumType == null || !enumType.IsEnum)
            {
                Debug.LogError($"Type '{enumType?.FullName}' is not an enum.");
                return;
            }

            typeName = enumType.AssemblyQualifiedName;
            value = (Enum)Enum.GetValues(enumType).GetValue(0);
            base.Initialize(enumType);
        }

        public override void Set(object value)
        {
            if (value is Enum enumValue)
            {
                this.value = enumValue;
            }
        }
    }

    [Serializable]
    public class CustomAttributeValue : ComplexAttributeValue<object>
    {
        public CustomAttributeValue() { }

        public CustomAttributeValue(Type customType)
        {
            Initialize(customType);
        }

        public override void Initialize(Type customType)
        {
            if (customType == null || customType.IsAbstract || customType.IsInterface)
            {
                Debug.LogError($"Type '{customType?.FullName}' is not a valid custom type.");
                return;
            }

            typeName = customType.AssemblyQualifiedName;
            value = Activator.CreateInstance(customType);
            base.Initialize(customType);
        }

        public override void Set(object value)
        {
            if (value != null && value.GetType() == ValueType)
            {
                this.value = value;
            }
        }

        public override Type ValueType
        {
            get
            {
                if (string.IsNullOrEmpty(typeName))
                    return null;

                Type type = Type.GetType(typeName);
                if (type == null)
                {
                    Debug.LogError($"Failed to resolve type: {typeName}");
                }

                return type;
            }
        }
    }
}
