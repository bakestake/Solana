using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gamegaard.SavingSystem.Types
{
    [Serializable]
    public struct SerializableBounds : ITypeConverter<Bounds>
    {
        public float centerX;
        public float centerY;
        public float centerZ;
        public float sizeX;
        public float sizeY;
        public float sizeZ;

        public SerializableBounds(Bounds bounds)
        {
            centerX = bounds.center.x;
            centerY = bounds.center.y;
            centerZ = bounds.center.z;
            sizeX = bounds.size.x;
            sizeY = bounds.size.y;
            sizeZ = bounds.size.z;
        }

        public Bounds ToOriginalType()
        {
            Vector3 center = new Vector3(centerX, centerY, centerZ);
            Vector3 size = new Vector3(sizeX, sizeY, sizeZ);
            return new Bounds(center, size);
        }

        public static implicit operator SerializableBounds(Bounds bounds)
        {
            return new SerializableBounds(bounds);
        }

        public static implicit operator Bounds(SerializableBounds serializableBounds)
        {
            return serializableBounds.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableAudioClip : ITypeConverter<AudioClip>
    {
        public string name;
        public float length;
        public int channels;
        public int frequency;
        public int samples;
        public float[] data;

        public SerializableAudioClip(AudioClip audioClip)
        {
            name = audioClip.name;
            length = audioClip.length;
            channels = audioClip.channels;
            frequency = audioClip.frequency;
            samples = audioClip.samples;

            float[] audioData = new float[samples * channels];
            audioClip.GetData(audioData, 0);

            data = new float[audioData.Length];
            Array.Copy(audioData, data, audioData.Length);
        }

        public AudioClip ToOriginalType()
        {
            AudioClip audioClip = AudioClip.Create(name, samples, channels, frequency, false);
            audioClip.SetData(data, 0);
            return audioClip;
        }

        public static implicit operator SerializableAudioClip(AudioClip audioClip)
        {
            return new SerializableAudioClip(audioClip);
        }

        public static implicit operator AudioClip(SerializableAudioClip serializableAudioClip)
        {
            return serializableAudioClip.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableBoneWeight : ITypeConverter<BoneWeight>
    {
        public int boneIndex0;
        public int boneIndex1;
        public int boneIndex2;
        public int boneIndex3;
        public float weight0;
        public float weight1;
        public float weight2;
        public float weight3;

        public SerializableBoneWeight(BoneWeight boneWeight)
        {
            boneIndex0 = boneWeight.boneIndex0;
            boneIndex1 = boneWeight.boneIndex1;
            boneIndex2 = boneWeight.boneIndex2;
            boneIndex3 = boneWeight.boneIndex3;
            weight0 = boneWeight.weight0;
            weight1 = boneWeight.weight1;
            weight2 = boneWeight.weight2;
            weight3 = boneWeight.weight3;
        }

        public BoneWeight ToOriginalType()
        {
            BoneWeight boneWeight = new BoneWeight();
            boneWeight.boneIndex0 = boneIndex0;
            boneWeight.boneIndex1 = boneIndex1;
            boneWeight.boneIndex2 = boneIndex2;
            boneWeight.boneIndex3 = boneIndex3;
            boneWeight.weight0 = weight0;
            boneWeight.weight1 = weight1;
            boneWeight.weight2 = weight2;
            boneWeight.weight3 = weight3;
            return boneWeight;
        }

        public static implicit operator SerializableBoneWeight(BoneWeight boneWeight)
        {
            return new SerializableBoneWeight(boneWeight);
        }

        public static implicit operator BoneWeight(SerializableBoneWeight serializableBoneWeight)
        {
            return serializableBoneWeight.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableRect : ITypeConverter<Rect>
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public SerializableRect(Rect rect)
        {
            x = rect.x;
            y = rect.y;
            width = rect.width;
            height = rect.height;
        }

        public Rect ToOriginalType()
        {
            return new Rect(x, y, width, height);
        }

        public static implicit operator SerializableRect(Rect color)
        {
            return new SerializableRect(color);
        }

        public static implicit operator Rect(SerializableRect serializableRect)
        {
            return serializableRect.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableGuid : ITypeConverter<Guid>
    {
        public string guidValue;

        public SerializableGuid(Guid guid)
        {
            this.guidValue = guid.ToString();
        }

        public Guid ToOriginalType()
        {
            return new Guid(guidValue);
        }

        public static implicit operator SerializableGuid(Guid guid)
        {
            return new SerializableGuid(guid);
        }

        public static implicit operator Guid(SerializableGuid serializableGuid)
        {
            return serializableGuid.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableEnum<T> where T : Enum
    {
        public string value;

        public SerializableEnum(T enumValue)
        {
            value = enumValue.ToString();
        }

        public T ToOriginalType()
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }

    [Serializable]
    public struct SerializableLayerMask
    {
        public string value;

        public SerializableLayerMask(LayerMask enumValue)
        {
            value = enumValue.ToString();
        }

        public LayerMask ToOriginalType()
        {
            return (LayerMask)Enum.Parse(typeof(LayerMask), value);
        }

        public static implicit operator SerializableLayerMask(LayerMask layerMask)
        {
            return new SerializableLayerMask(layerMask);
        }

        public static implicit operator LayerMask(SerializableLayerMask serializableLayerMask)
        {
            return serializableLayerMask.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableTexture2D : ITypeConverter<Texture2D>
    {
        public string encodedTexture;

        public SerializableTexture2D(Texture2D texture)
        {
            if (texture != null)
            {
                byte[] textureData = texture.EncodeToPNG();
                encodedTexture = Convert.ToBase64String(textureData);
            }
            else
            {
                encodedTexture = null;
            }
        }

        public Texture2D ToOriginalType()
        {
            if (!string.IsNullOrEmpty(encodedTexture))
            {
                byte[] textureData = Convert.FromBase64String(encodedTexture);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(textureData);
                return texture;
            }
            else
            {
                return null;
            }
        }

        public static implicit operator SerializableTexture2D(Texture2D texture)
        {
            return new SerializableTexture2D(texture);
        }

        public static implicit operator Texture2D(SerializableTexture2D serializableTexture)
        {
            return serializableTexture.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableRenderTexture : ITypeConverter<RenderTexture>
    {
        public string encodedTexture;

        public SerializableRenderTexture(RenderTexture texture)
        {
            if (texture != null)
            {
                RenderTexture.active = texture;
                Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
                texture2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                texture2D.Apply();
                byte[] textureData = texture2D.EncodeToPNG();
                encodedTexture = Convert.ToBase64String(textureData);
            }
            else
            {
                encodedTexture = null;
            }
        }

        public RenderTexture ToOriginalType()
        {
            if (!string.IsNullOrEmpty(encodedTexture))
            {
                byte[] textureData = Convert.FromBase64String(encodedTexture);
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(textureData);
                RenderTexture renderTexture = new RenderTexture(texture2D.width, texture2D.height, 0);
                Graphics.Blit(texture2D, renderTexture);
                return renderTexture;
            }
            else
            {
                return null;
            }
        }

        public static implicit operator SerializableRenderTexture(RenderTexture texture)
        {
            return new SerializableRenderTexture(texture);
        }

        public static implicit operator RenderTexture(SerializableRenderTexture serializableTexture)
        {
            return serializableTexture.ToOriginalType();
        }
    }

    #region Vectors
    [Serializable]
    public struct SerializableVector3 : ITypeConverter<Vector3>
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToOriginalType()
        {
            return new Vector3(x, y, z);
        }

        public static implicit operator SerializableVector3(Vector3 vector)
        {
            return new SerializableVector3(vector);
        }

        public static implicit operator Vector3(SerializableVector3 serializableVector)
        {
            return serializableVector.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableVector3Int : ITypeConverter<Vector3Int>
    {
        public int x;
        public int y;
        public int z;

        public SerializableVector3Int(Vector3Int vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3Int ToOriginalType()
        {
            return new Vector3Int(x, y, z);
        }

        public static implicit operator SerializableVector3Int(Vector3Int vector)
        {
            return new SerializableVector3Int(vector);
        }

        public static implicit operator Vector3Int(SerializableVector3Int serializableVector)
        {
            return serializableVector.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableVector2 : ITypeConverter<Vector2>
    {
        public float x;
        public float y;

        public SerializableVector2(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public Vector2 ToOriginalType()
        {
            return new Vector2(x, y);
        }

        public static implicit operator SerializableVector2(Vector2 vector)
        {
            return new SerializableVector2(vector);
        }

        public static implicit operator Vector2(SerializableVector2 serializableVector)
        {
            return serializableVector.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableVector2Int : ITypeConverter<Vector2Int>
    {
        public int x;
        public int y;

        public SerializableVector2Int(Vector2Int vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public Vector2Int ToOriginalType()
        {
            return new Vector2Int(x, y);
        }

        public static implicit operator SerializableVector2Int(Vector2Int vector)
        {
            return new SerializableVector2Int(vector);
        }

        public static implicit operator Vector2Int(SerializableVector2Int serializableVector)
        {
            return serializableVector.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableVector4 : ITypeConverter<Vector4>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableVector4(Vector4 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = vector.w;
        }

        public Vector4 ToOriginalType()
        {
            return new Vector4(x, y, z, w);
        }

        public static implicit operator SerializableVector4(Vector4 vector)
        {
            return new SerializableVector4(vector);
        }

        public static implicit operator Vector4(SerializableVector4 serializableVector)
        {
            return serializableVector.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableQuaternion : ITypeConverter<Quaternion>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }

        public Quaternion ToOriginalType()
        {
            return new Quaternion(x, y, z, w);
        }

        public static implicit operator SerializableQuaternion(Quaternion quaternion)
        {
            return new SerializableQuaternion(quaternion);
        }

        public static implicit operator Quaternion(SerializableQuaternion serializableQuaternion)
        {
            return serializableQuaternion.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableMatrix4x4 : ITypeConverter<Matrix4x4>
    {
        public SerializableVector4 row0;
        public SerializableVector4 row1;
        public SerializableVector4 row2;
        public SerializableVector4 row3;

        public SerializableMatrix4x4(Matrix4x4 matrix)
        {
            row0 = matrix.GetRow(0);
            row1 = matrix.GetRow(1);
            row2 = matrix.GetRow(2);
            row3 = matrix.GetRow(3);
        }

        public Matrix4x4 ToOriginalType()
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetRow(0, row0.ToOriginalType());
            matrix.SetRow(1, row1.ToOriginalType());
            matrix.SetRow(2, row2.ToOriginalType());
            matrix.SetRow(3, row3.ToOriginalType());
            return matrix;
        }

        public static implicit operator SerializableMatrix4x4(Matrix4x4 matrix)
        {
            return new SerializableMatrix4x4(matrix);
        }

        public static implicit operator Matrix4x4(SerializableMatrix4x4 serializableMatrix)
        {
            return serializableMatrix.ToOriginalType();
        }
    }

    #endregion

    #region Color
    [Serializable]
    public struct SerializableColor : ITypeConverter<Color>
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public SerializableColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public Color ToOriginalType()
        {
            return new Color(r, g, b, a);
        }

        public static implicit operator SerializableColor(Color color)
        {
            return new SerializableColor(color);
        }

        public static implicit operator Color(SerializableColor serializableColor)
        {
            return serializableColor.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableColor32 : ITypeConverter<Color32>
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public SerializableColor32(Color32 color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public Color32 ToOriginalType()
        {
            return new Color32(r, g, b, a);
        }

        public static implicit operator SerializableColor32(Color32 color)
        {
            return new SerializableColor32(color);
        }

        public static implicit operator Color32(SerializableColor32 serializableColor)
        {
            return serializableColor.ToOriginalType();
        }
    }
    #endregion

    #region Gradient
    [Serializable]
    public struct SerializableGradientColorKey : ITypeConverter<GradientColorKey>
    {
        public float time;
        public SerializableColor color;

        public SerializableGradientColorKey(GradientColorKey colorKey)
        {
            time = colorKey.time;
            color = colorKey.color;
        }

        public GradientColorKey ToOriginalType()
        {
            return new GradientColorKey(color.ToOriginalType(), time);
        }

        public static implicit operator SerializableGradientColorKey(GradientColorKey colorKey)
        {
            return new SerializableGradientColorKey(colorKey);
        }

        public static implicit operator GradientColorKey(SerializableGradientColorKey serializableColorKey)
        {
            return serializableColorKey.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableGradientAlphaKey : ITypeConverter<GradientAlphaKey>
    {
        public float time;
        public float alpha;

        public SerializableGradientAlphaKey(GradientAlphaKey alphaKey)
        {
            time = alphaKey.time;
            alpha = alphaKey.alpha;
        }

        public GradientAlphaKey ToOriginalType()
        {
            return new GradientAlphaKey(alpha, time);
        }

        public static implicit operator SerializableGradientAlphaKey(GradientAlphaKey alphaKey)
        {
            return new SerializableGradientAlphaKey(alphaKey);
        }

        public static implicit operator GradientAlphaKey(SerializableGradientAlphaKey serializableAlphaKey)
        {
            return serializableAlphaKey.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableGradient : ITypeConverter<Gradient>
    {
        public SerializableGradientColorKey[] colorKeys;
        public SerializableGradientAlphaKey[] alphaKeys;

        public SerializableGradient(Gradient gradient)
        {
            colorKeys = new SerializableGradientColorKey[gradient.colorKeys.Length];
            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                colorKeys[i] = gradient.colorKeys[i];
            }

            alphaKeys = new SerializableGradientAlphaKey[gradient.alphaKeys.Length];
            for (int i = 0; i < gradient.alphaKeys.Length; i++)
            {
                alphaKeys[i] = gradient.alphaKeys[i];
            }
        }

        public Gradient ToOriginalType()
        {
            Gradient newGradient = new Gradient();

            newGradient.colorKeys = new GradientColorKey[colorKeys.Length];
            for (int i = 0; i < colorKeys.Length; i++)
            {
                newGradient.colorKeys[i] = colorKeys[i];
            }

            newGradient.alphaKeys = new GradientAlphaKey[alphaKeys.Length];
            for (int i = 0; i < alphaKeys.Length; i++)
            {
                newGradient.alphaKeys[i] = alphaKeys[i];
            }

            return newGradient;
        }

        public static implicit operator SerializableGradient(Gradient gradient)
        {
            return new SerializableGradient(gradient);
        }

        public static implicit operator Gradient(SerializableGradient serializableGradient)
        {
            return serializableGradient.ToOriginalType();
        }
    }
    #endregion

    #region AnimationCurve
    [Serializable]
    public struct SerializableAnimationCurve : ITypeConverter<AnimationCurve>
    {
        public SerializableKeyFrame[] keys;

        public SerializableAnimationCurve(AnimationCurve animationCurve)
        {
            keys = new SerializableKeyFrame[animationCurve.keys.Length];
            for (int i = 0; i < animationCurve.keys.Length; i++)
            {
                keys[i] = animationCurve.keys[i];
            }
        }

        public AnimationCurve ToOriginalType()
        {
            AnimationCurve animationCurve = new AnimationCurve();

            animationCurve.keys = new Keyframe[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                animationCurve.keys[i] = keys[i];
            }

            return animationCurve;
        }

        public static implicit operator SerializableAnimationCurve(AnimationCurve animationCurve)
        {
            return new SerializableAnimationCurve(animationCurve);
        }

        public static implicit operator AnimationCurve(SerializableAnimationCurve serializableCurve)
        {
            return serializableCurve.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableKeyFrame
    {
        public float time;
        public float value;
        public float inTangent;
        public float outTangent;
        public float inWeight;
        public float outWeight;

        public SerializableKeyFrame(Keyframe keyframe)
        {
            time = keyframe.time;
            value = keyframe.value;
            inTangent = keyframe.inTangent;
            outTangent = keyframe.outTangent;
            inWeight = keyframe.inWeight;
            outWeight = keyframe.outWeight;
        }

        public Keyframe ToOriginalType()
        {
            return new Keyframe(time, value, inTangent, outTangent, inWeight, outWeight);
        }

        public static implicit operator SerializableKeyFrame(Keyframe keyframe)
        {
            return new SerializableKeyFrame(keyframe);
        }

        public static implicit operator Keyframe(SerializableKeyFrame serializableKeyFrame)
        {
            return serializableKeyFrame.ToOriginalType();
        }
    }

    #endregion

    #region ParticleSystem
    [Serializable]
    public struct SerializableEmissionModule : ITypeConverter<ParticleSystem.EmissionModule>
    {
        public bool enabled;
        public SerializableMinMaxCurve rateOverTime;
        public float rateOverTimeMultiplier;
        public SerializableMinMaxCurve rateOverDistance;
        public float rateOverDistanceMultiplier;
        public int burstCount;

        public SerializableEmissionModule(ParticleSystem.EmissionModule emissionModule)
        {
            enabled = emissionModule.enabled;
            rateOverTime = emissionModule.rateOverTime;
            rateOverTimeMultiplier = emissionModule.rateOverTimeMultiplier;
            rateOverDistance = emissionModule.rateOverDistance;
            rateOverDistanceMultiplier = emissionModule.rateOverDistanceMultiplier;
            burstCount = emissionModule.burstCount;
        }

        public ParticleSystem.EmissionModule ToOriginalType()
        {
            ParticleSystem.EmissionModule emissionModule = new ParticleSystem.EmissionModule();

            emissionModule.enabled = enabled;
            emissionModule.rateOverTime = rateOverTime;
            emissionModule.rateOverTimeMultiplier = rateOverDistanceMultiplier;
            emissionModule.rateOverDistance = rateOverDistance;
            emissionModule.rateOverDistanceMultiplier = rateOverDistanceMultiplier;
            emissionModule.burstCount = burstCount;

            return emissionModule;
        }

        public static implicit operator SerializableEmissionModule(ParticleSystem.EmissionModule emissionModule)
        {
            return new SerializableEmissionModule(emissionModule);
        }

        public static implicit operator ParticleSystem.EmissionModule(SerializableEmissionModule serializableEmissionModule)
        {
            return serializableEmissionModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableShapeModule : ITypeConverter<ParticleSystem.ShapeModule>
    {
        public SerializableEnum<ParticleSystemShapeType> shapeType;
        public bool alignToDirection;
        public float angle;
        public float arc;
        public SerializableEnum<ParticleSystemShapeMultiModeValue> arcMode;
        public float arcSpread;
        public SerializableMinMaxCurve arcSpeed;
        public float arcSpeedMultiplier;
        public SerializableVector3 boxThickness;
        public float length;
        public Mesh mesh;
        public SerializableEnum<ParticleSystemMeshShapeType> meshShapeType;
        public float meshSpawnSpread;
        public SerializableMinMaxCurve meshSpawnSpeed;
        public float meshSpawnSpeedMultiplier;
        public int meshMaterialIndex;
        public float randomDirectionAmount;
        public float randomPositionAmount;
        public float radius;
        public SerializableEnum<ParticleSystemShapeMultiModeValue> radiusMode;
        public float radiusSpread;
        public SerializableMinMaxCurve radiusSpeed;
        public float radiusSpeedMultiplier;
        public float radiusThickness;
        public SerializableVector3 scale;
        public Sprite sprite;
        public SerializableEnum<ParticleSystemShapeTextureChannel> textureClipChannel;
        public float textureClipThreshold;
        public bool textureColorAffectsParticles;
        public bool textureAlphaAffectsParticles;
        public bool textureBilinearFiltering;
        public int textureUVChannel;
        public float donutRadius;
        public Vector3 position;
        public Vector3 rotation;
        public bool useMeshMaterialIndex;
        public bool useMeshColors;
        public float normalOffset;

        public SerializableShapeModule(ParticleSystem.ShapeModule shapeModule)
        {
            shapeType = new SerializableEnum<ParticleSystemShapeType>(shapeModule.shapeType);
            alignToDirection = shapeModule.alignToDirection;
            angle = shapeModule.angle;
            arc = shapeModule.arc;
            arcMode = new SerializableEnum<ParticleSystemShapeMultiModeValue>(shapeModule.arcMode);
            arcSpread = shapeModule.arcSpread;
            arcSpeed = shapeModule.arcSpeed;
            arcSpeedMultiplier = shapeModule.arcSpeedMultiplier;
            boxThickness = shapeModule.boxThickness;
            length = shapeModule.length;
            mesh = shapeModule.mesh;
            meshShapeType = new SerializableEnum<ParticleSystemMeshShapeType>(shapeModule.meshShapeType);
            meshSpawnSpread = shapeModule.meshSpawnSpread;
            meshSpawnSpeed = shapeModule.meshSpawnSpeed;
            meshSpawnSpeedMultiplier = shapeModule.meshSpawnSpeedMultiplier;
            meshMaterialIndex = shapeModule.meshMaterialIndex;
            randomDirectionAmount = shapeModule.randomDirectionAmount;
            randomPositionAmount = shapeModule.randomPositionAmount;
            radius = shapeModule.radius;
            radiusMode = new SerializableEnum<ParticleSystemShapeMultiModeValue>(shapeModule.radiusMode);
            radiusSpread = shapeModule.radiusSpread;
            radiusSpeed = shapeModule.radiusSpeed;
            radiusSpeedMultiplier = shapeModule.radiusSpeedMultiplier;
            radiusThickness = shapeModule.radiusThickness;
            scale = shapeModule.scale;
            sprite = shapeModule.sprite;
            textureClipChannel = new SerializableEnum<ParticleSystemShapeTextureChannel>(shapeModule.textureClipChannel);
            textureClipThreshold = shapeModule.textureClipThreshold;
            textureColorAffectsParticles = shapeModule.textureColorAffectsParticles;
            textureAlphaAffectsParticles = shapeModule.textureAlphaAffectsParticles;
            textureBilinearFiltering = shapeModule.textureBilinearFiltering;
            textureUVChannel = shapeModule.textureUVChannel;
            donutRadius = shapeModule.donutRadius;
            position = shapeModule.position;
            rotation = shapeModule.rotation;
            useMeshMaterialIndex = shapeModule.useMeshMaterialIndex;
            useMeshColors = shapeModule.useMeshColors;
            normalOffset = shapeModule.normalOffset;
        }

        public ParticleSystem.ShapeModule ToOriginalType()
        {
            ParticleSystem.ShapeModule shapeModule = new ParticleSystem.ShapeModule();
            shapeModule.shapeType = shapeType.ToOriginalType();
            shapeModule.alignToDirection = alignToDirection;
            shapeModule.angle = angle;
            shapeModule.arc = arc;
            shapeModule.arcMode = arcMode.ToOriginalType();
            shapeModule.arcSpread = arcSpread;
            shapeModule.arcSpeed = arcSpeed;
            shapeModule.arcSpeedMultiplier = arcSpeedMultiplier;
            shapeModule.boxThickness = boxThickness;
            shapeModule.length = length;
            shapeModule.mesh = mesh;
            shapeModule.meshShapeType = meshShapeType.ToOriginalType();
            shapeModule.meshSpawnSpread = meshSpawnSpread;
            shapeModule.meshSpawnSpeed = meshSpawnSpeed;
            shapeModule.meshSpawnSpeedMultiplier = meshSpawnSpeedMultiplier;
            shapeModule.meshMaterialIndex = meshMaterialIndex;
            shapeModule.randomDirectionAmount = randomDirectionAmount;
            shapeModule.randomPositionAmount = randomPositionAmount;
            shapeModule.radius = radius;
            shapeModule.radiusMode = radiusMode.ToOriginalType();
            shapeModule.radiusSpread = radiusSpread;
            shapeModule.radiusSpeed = radiusSpeed;
            shapeModule.radiusSpeedMultiplier = radiusSpeedMultiplier;
            shapeModule.radiusThickness = radiusThickness;
            shapeModule.scale = scale;
            shapeModule.sprite = sprite;
            shapeModule.textureClipChannel = textureClipChannel.ToOriginalType();
            shapeModule.textureClipThreshold = textureClipThreshold;
            shapeModule.textureColorAffectsParticles = textureColorAffectsParticles;
            shapeModule.textureAlphaAffectsParticles = textureAlphaAffectsParticles;
            shapeModule.textureBilinearFiltering = textureBilinearFiltering;
            shapeModule.textureUVChannel = textureUVChannel;
            shapeModule.donutRadius = donutRadius;
            shapeModule.position = position;
            shapeModule.rotation = rotation;
            shapeModule.useMeshMaterialIndex = useMeshMaterialIndex;
            shapeModule.useMeshColors = useMeshColors;
            shapeModule.normalOffset = normalOffset;

            return shapeModule;
        }

        public static implicit operator SerializableShapeModule(ParticleSystem.ShapeModule shapeModule)
        {
            return new SerializableShapeModule(shapeModule);
        }

        public static implicit operator ParticleSystem.ShapeModule(SerializableShapeModule serializableShapeModule)
        {
            return serializableShapeModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableMinMaxCurve : ITypeConverter<ParticleSystem.MinMaxCurve>
    {
        public SerializableEnum<ParticleSystemCurveMode> mode;
        public SerializableAnimationCurve curve;
        public float constant;
        public SerializableAnimationCurve curveMin;
        public SerializableAnimationCurve curveMax;
        public float constantMin;
        public float constantMax;

        public SerializableMinMaxCurve(ParticleSystem.MinMaxCurve minMaxCurve)
        {
            ParticleSystemCurveMode particleMode = minMaxCurve.mode;
            mode = new SerializableEnum<ParticleSystemCurveMode>(particleMode);
            constant = minMaxCurve.constant;
            curve = minMaxCurve.curve;
            constantMin = minMaxCurve.constantMin;
            constantMax = minMaxCurve.constantMax;
            curveMin = minMaxCurve.curveMin;
            curveMax = minMaxCurve.curveMax;
        }

        public ParticleSystem.MinMaxCurve ToOriginalType()
        {
            ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve();

            minMaxCurve.mode = mode.ToOriginalType();
            minMaxCurve.constant = constant;
            minMaxCurve.curve = curve;
            minMaxCurve.constantMin = constantMin;
            minMaxCurve.constantMax = constantMax;
            minMaxCurve.curveMin = curveMin;
            minMaxCurve.curveMax = curveMax;
            return minMaxCurve;
        }

        public static implicit operator SerializableMinMaxCurve(ParticleSystem.MinMaxCurve minMaxCurve)
        {
            return new SerializableMinMaxCurve(minMaxCurve);
        }

        public static implicit operator ParticleSystem.MinMaxCurve(SerializableMinMaxCurve serializableMinMaxCurve)
        {
            return serializableMinMaxCurve.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableMinMaxGradient : ITypeConverter<ParticleSystem.MinMaxGradient>
    {
        public SerializableColor color;
        public SerializableGradient gradient;
        public ParticleSystemGradientMode mode;

        public SerializableMinMaxGradient(ParticleSystem.MinMaxGradient minMaxGradient)
        {
            mode = minMaxGradient.mode;
            color = minMaxGradient.color;
            gradient = new SerializableGradient(minMaxGradient.gradient);
        }

        public ParticleSystem.MinMaxGradient ToOriginalType()
        {
            ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient();

            minMaxGradient.mode = mode;
            minMaxGradient.color = color;
            minMaxGradient.gradient = gradient;

            return minMaxGradient;
        }

        public static implicit operator SerializableMinMaxGradient(ParticleSystem.MinMaxGradient minMaxGradient)
        {
            return new SerializableMinMaxGradient(minMaxGradient);
        }

        public static implicit operator ParticleSystem.MinMaxGradient(SerializableMinMaxGradient serializableMinMaxGradient)
        {
            return serializableMinMaxGradient.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableColorBySpeedModule : ITypeConverter<ParticleSystem.ColorBySpeedModule>
    {
        public bool enabled;
        public SerializableMinMaxGradient color;
        public SerializableVector2 range;

        public SerializableColorBySpeedModule(ParticleSystem.ColorBySpeedModule colorBySpeedModule)
        {
            enabled = colorBySpeedModule.enabled;
            color = colorBySpeedModule.color;
            range = colorBySpeedModule.range;
        }

        public ParticleSystem.ColorBySpeedModule ToOriginalType()
        {
            ParticleSystem.ColorBySpeedModule colorBySpeedModule = new ParticleSystem.ColorBySpeedModule();

            colorBySpeedModule.enabled = enabled;
            colorBySpeedModule.color = color;
            colorBySpeedModule.range = range;

            return colorBySpeedModule;
        }

        public static implicit operator SerializableColorBySpeedModule(ParticleSystem.ColorBySpeedModule colorBySpeedModule)
        {
            return new SerializableColorBySpeedModule(colorBySpeedModule);
        }

        public static implicit operator ParticleSystem.ColorBySpeedModule(SerializableColorBySpeedModule serializableColorBySpeedModule)
        {
            return serializableColorBySpeedModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableLimitVelocityOverLifetimeModule : ITypeConverter<ParticleSystem.LimitVelocityOverLifetimeModule>
    {
        public bool enabled;
        public SerializableMinMaxCurve limitX;
        public float limitXMultiplier;
        public SerializableMinMaxCurve limitY;
        public float limitYMultiplier;
        public SerializableMinMaxCurve limitZ;
        public float limitZMultiplier;
        public SerializableMinMaxCurve limit;
        public float limitMultiplier;

        public SerializableLimitVelocityOverLifetimeModule(ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetimeModule)
        {
            enabled = limitVelocityOverLifetimeModule.enabled;
            limitX = limitVelocityOverLifetimeModule.limitX;
            limitXMultiplier = limitVelocityOverLifetimeModule.limitXMultiplier;
            limitY = limitVelocityOverLifetimeModule.limitY;
            limitYMultiplier = limitVelocityOverLifetimeModule.limitYMultiplier;
            limitZ = limitVelocityOverLifetimeModule.limitZ;
            limitZMultiplier = limitVelocityOverLifetimeModule.limitZMultiplier;
            limit = limitVelocityOverLifetimeModule.limit;
            limitMultiplier = limitVelocityOverLifetimeModule.limitMultiplier;
        }

        public ParticleSystem.LimitVelocityOverLifetimeModule ToOriginalType()
        {
            ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetimeModule = new ParticleSystem.LimitVelocityOverLifetimeModule();

            limitVelocityOverLifetimeModule.enabled = enabled;
            limitVelocityOverLifetimeModule.limitX = limitX;
            limitVelocityOverLifetimeModule.limitXMultiplier = limitXMultiplier;
            limitVelocityOverLifetimeModule.limitY = limitY;
            limitVelocityOverLifetimeModule.limitYMultiplier = limitYMultiplier;
            limitVelocityOverLifetimeModule.limitZ = limitZ;
            limitVelocityOverLifetimeModule.limitZMultiplier = limitZMultiplier;
            limitVelocityOverLifetimeModule.limit = limit;
            limitVelocityOverLifetimeModule.limitMultiplier = limitMultiplier;

            return limitVelocityOverLifetimeModule;
        }

        public static implicit operator SerializableLimitVelocityOverLifetimeModule(ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetimeModule)
        {
            return new SerializableLimitVelocityOverLifetimeModule(limitVelocityOverLifetimeModule);
        }

        public static implicit operator ParticleSystem.LimitVelocityOverLifetimeModule(SerializableLimitVelocityOverLifetimeModule serializableLimitVelocityOverLifetimeModule)
        {
            return serializableLimitVelocityOverLifetimeModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableInheritVelocityModule : ITypeConverter<ParticleSystem.InheritVelocityModule>
    {
        public bool enabled;
        public SerializableEnum<ParticleSystemInheritVelocityMode> mode;
        public SerializableMinMaxCurve curve;
        public float curveMultiplier;

        public SerializableInheritVelocityModule(ParticleSystem.InheritVelocityModule inheritVelocityModule)
        {
            enabled = inheritVelocityModule.enabled;
            mode = new SerializableEnum<ParticleSystemInheritVelocityMode>(inheritVelocityModule.mode);
            curve = inheritVelocityModule.curve;
            curveMultiplier = inheritVelocityModule.curveMultiplier;
        }

        public ParticleSystem.InheritVelocityModule ToOriginalType()
        {
            ParticleSystem.InheritVelocityModule inheritVelocityModule = new ParticleSystem.InheritVelocityModule();

            inheritVelocityModule.enabled = enabled;
            inheritVelocityModule.mode = mode.ToOriginalType();
            inheritVelocityModule.curve = curve;
            inheritVelocityModule.curveMultiplier = curveMultiplier;

            return inheritVelocityModule;
        }

        public static implicit operator SerializableInheritVelocityModule(ParticleSystem.InheritVelocityModule inheritVelocityModule)
        {
            return new SerializableInheritVelocityModule(inheritVelocityModule);
        }

        public static implicit operator ParticleSystem.InheritVelocityModule(SerializableInheritVelocityModule serializableInheritVelocityModule)
        {
            return serializableInheritVelocityModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableVelocityOverLifetimeModule : ITypeConverter<ParticleSystem.VelocityOverLifetimeModule>
    {
        public bool enabled;
        public SerializableMinMaxCurve x;
        public float xMultiplier;
        public SerializableMinMaxCurve y;
        public float yMultiplier;
        public SerializableMinMaxCurve z;
        public float zMultiplier;
        public SerializableMinMaxCurve orbitalX;
        public float orbitalXMultiplier;
        public SerializableMinMaxCurve orbitalY;
        public float orbitalYMultiplier;
        public SerializableMinMaxCurve orbitalZ;
        public float orbitalZMultiplier;
        public SerializableMinMaxCurve orbitalOffsetX;
        public float orbitalOffsetXMultiplier;
        public SerializableMinMaxCurve orbitalOffsetY;
        public float orbitalOffsetYMultiplier;
        public SerializableMinMaxCurve orbitalOffsetZ;
        public float orbitalOffsetZMultiplier;
        public SerializableMinMaxCurve radial;
        public float radialMultiplier;
        public SerializableMinMaxCurve speedModifier;
        public float speedModifierMultiplier;
        public SerializableEnum<ParticleSystemSimulationSpace> space;

        public SerializableVelocityOverLifetimeModule(ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule)
        {
            enabled = velocityOverLifetimeModule.enabled;
            x = velocityOverLifetimeModule.x;
            xMultiplier = velocityOverLifetimeModule.xMultiplier;
            y = velocityOverLifetimeModule.y;
            yMultiplier = velocityOverLifetimeModule.yMultiplier;
            z = velocityOverLifetimeModule.z;
            zMultiplier = velocityOverLifetimeModule.zMultiplier;
            orbitalX = velocityOverLifetimeModule.orbitalX;
            orbitalXMultiplier = velocityOverLifetimeModule.orbitalXMultiplier;
            orbitalY = velocityOverLifetimeModule.orbitalY;
            orbitalYMultiplier = velocityOverLifetimeModule.orbitalYMultiplier;
            orbitalZ = velocityOverLifetimeModule.orbitalZ;
            orbitalZMultiplier = velocityOverLifetimeModule.orbitalZMultiplier;
            orbitalOffsetX = velocityOverLifetimeModule.orbitalOffsetX;
            orbitalOffsetXMultiplier = velocityOverLifetimeModule.orbitalOffsetXMultiplier;
            orbitalOffsetY = velocityOverLifetimeModule.orbitalOffsetY;
            orbitalOffsetYMultiplier = velocityOverLifetimeModule.orbitalOffsetYMultiplier;
            orbitalOffsetZ = velocityOverLifetimeModule.orbitalOffsetZ;
            orbitalOffsetZMultiplier = velocityOverLifetimeModule.orbitalOffsetZMultiplier;
            radial = velocityOverLifetimeModule.radial;
            radialMultiplier = velocityOverLifetimeModule.radialMultiplier;
            speedModifier = velocityOverLifetimeModule.speedModifier;
            speedModifierMultiplier = velocityOverLifetimeModule.speedModifierMultiplier;
            space = new SerializableEnum<ParticleSystemSimulationSpace>(velocityOverLifetimeModule.space);
        }

        public ParticleSystem.VelocityOverLifetimeModule ToOriginalType()
        {
            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = new ParticleSystem.VelocityOverLifetimeModule();

            velocityOverLifetimeModule.enabled = enabled;
            velocityOverLifetimeModule.x = x;
            velocityOverLifetimeModule.xMultiplier = xMultiplier;
            velocityOverLifetimeModule.y = y;
            velocityOverLifetimeModule.yMultiplier = yMultiplier;
            velocityOverLifetimeModule.z = z;
            velocityOverLifetimeModule.zMultiplier = zMultiplier;
            velocityOverLifetimeModule.orbitalX = orbitalX;
            velocityOverLifetimeModule.orbitalXMultiplier = orbitalXMultiplier;
            velocityOverLifetimeModule.orbitalY = orbitalY;
            velocityOverLifetimeModule.orbitalYMultiplier = orbitalYMultiplier;
            velocityOverLifetimeModule.orbitalZ = orbitalZ;
            velocityOverLifetimeModule.orbitalZMultiplier = orbitalZMultiplier;
            velocityOverLifetimeModule.orbitalOffsetX = orbitalOffsetX;
            velocityOverLifetimeModule.orbitalOffsetXMultiplier = orbitalOffsetXMultiplier;
            velocityOverLifetimeModule.orbitalOffsetY = orbitalOffsetY;
            velocityOverLifetimeModule.orbitalOffsetYMultiplier = orbitalOffsetYMultiplier;
            velocityOverLifetimeModule.orbitalOffsetZ = orbitalOffsetZ;
            velocityOverLifetimeModule.orbitalOffsetZMultiplier = orbitalOffsetZMultiplier;
            velocityOverLifetimeModule.radial = radial;
            velocityOverLifetimeModule.radialMultiplier = radialMultiplier;
            velocityOverLifetimeModule.speedModifier = speedModifier;
            velocityOverLifetimeModule.speedModifierMultiplier = speedModifierMultiplier;
            velocityOverLifetimeModule.space = space.ToOriginalType();

            return velocityOverLifetimeModule;
        }

        public static implicit operator SerializableVelocityOverLifetimeModule(ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule)
        {
            return new SerializableVelocityOverLifetimeModule(velocityOverLifetimeModule);
        }

        public static implicit operator ParticleSystem.VelocityOverLifetimeModule(SerializableVelocityOverLifetimeModule serializableVelocityOverLifetimeModule)
        {
            return serializableVelocityOverLifetimeModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableColorOverLifetimeModule : ITypeConverter<ParticleSystem.ColorOverLifetimeModule>
    {
        public bool enabled;
        public SerializableMinMaxGradient color;

        public SerializableColorOverLifetimeModule(ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule)
        {
            color = colorOverLifetimeModule.color;
            enabled = colorOverLifetimeModule.enabled;
        }

        public ParticleSystem.ColorOverLifetimeModule ToOriginalType()
        {
            ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = new ParticleSystem.ColorOverLifetimeModule();

            colorOverLifetimeModule.color = color;
            colorOverLifetimeModule.enabled = enabled;

            return colorOverLifetimeModule;
        }

        public static implicit operator SerializableColorOverLifetimeModule(ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule)
        {
            return new SerializableColorOverLifetimeModule(colorOverLifetimeModule);
        }

        public static implicit operator ParticleSystem.ColorOverLifetimeModule(SerializableColorOverLifetimeModule serializableColorOverLifetimeModule)
        {
            return serializableColorOverLifetimeModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableForceOverLifetimeModule : ITypeConverter<ParticleSystem.ForceOverLifetimeModule>
    {
        public bool enabled;
        public SerializableMinMaxCurve x;
        public float xMultiplier;
        public SerializableMinMaxCurve y;
        public float yMultiplier;
        public SerializableMinMaxCurve z;
        public float zMultiplier;
        public SerializableEnum<ParticleSystemSimulationSpace> space;
        public bool randomized;

        public SerializableForceOverLifetimeModule(ParticleSystem.ForceOverLifetimeModule forceOverLifetimeModule)
        {
            enabled = forceOverLifetimeModule.enabled;
            x = forceOverLifetimeModule.x;
            xMultiplier = forceOverLifetimeModule.xMultiplier;
            y = forceOverLifetimeModule.y;
            yMultiplier = forceOverLifetimeModule.yMultiplier;
            z = forceOverLifetimeModule.z;
            zMultiplier = forceOverLifetimeModule.zMultiplier;
            space = new SerializableEnum<ParticleSystemSimulationSpace>(forceOverLifetimeModule.space);
            randomized = forceOverLifetimeModule.randomized;
        }

        public ParticleSystem.ForceOverLifetimeModule ToOriginalType()
        {
            ParticleSystem.ForceOverLifetimeModule forceOverLifetimeModule = new ParticleSystem.ForceOverLifetimeModule();

            forceOverLifetimeModule.enabled = enabled;
            forceOverLifetimeModule.x = x;
            forceOverLifetimeModule.xMultiplier = xMultiplier;
            forceOverLifetimeModule.y = y;
            forceOverLifetimeModule.yMultiplier = yMultiplier;
            forceOverLifetimeModule.z = z;
            forceOverLifetimeModule.zMultiplier = zMultiplier;
            forceOverLifetimeModule.space = space.ToOriginalType();
            forceOverLifetimeModule.randomized = randomized;

            return forceOverLifetimeModule;
        }

        public static implicit operator SerializableForceOverLifetimeModule(ParticleSystem.ForceOverLifetimeModule forceOverLifetimeModule)
        {
            return new SerializableForceOverLifetimeModule(forceOverLifetimeModule);
        }

        public static implicit operator ParticleSystem.ForceOverLifetimeModule(SerializableForceOverLifetimeModule serializableForceOverLifetimeModule)
        {
            return serializableForceOverLifetimeModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableLightsModule : ITypeConverter<ParticleSystem.LightsModule>
    {
        public bool enabled;
        public float ratio;
        public bool useRandomDistribution;
        public SerializableLight light;
        public bool useParticleColor;
        public bool sizeAffectsRange;
        public bool alphaAffectsIntensity;
        public SerializableMinMaxCurve range;
        public float rangeMultiplier;
        public SerializableMinMaxCurve intensity;
        public float intensityMultiplier;
        public int maxLights;

        public SerializableLightsModule(ParticleSystem.LightsModule lightsModule)
        {
            enabled = lightsModule.enabled;
            ratio = lightsModule.ratio;
            useRandomDistribution = lightsModule.useRandomDistribution;
            light = lightsModule.light;
            useParticleColor = lightsModule.useParticleColor;
            sizeAffectsRange = lightsModule.sizeAffectsRange;
            alphaAffectsIntensity = lightsModule.alphaAffectsIntensity;
            range = lightsModule.range;
            rangeMultiplier = lightsModule.rangeMultiplier;
            intensity = lightsModule.intensity;
            intensityMultiplier = lightsModule.intensityMultiplier;
            maxLights = lightsModule.maxLights;
        }

        public ParticleSystem.LightsModule ToOriginalType()
        {
            ParticleSystem.LightsModule lightsModule = new ParticleSystem.LightsModule();

            lightsModule.enabled = enabled;
            lightsModule.ratio = ratio;
            lightsModule.useRandomDistribution = useRandomDistribution;
            lightsModule.light = light;
            lightsModule.useParticleColor = useParticleColor;
            lightsModule.sizeAffectsRange = sizeAffectsRange;
            lightsModule.alphaAffectsIntensity = alphaAffectsIntensity;
            lightsModule.range = range;
            lightsModule.rangeMultiplier = rangeMultiplier;
            lightsModule.intensity = intensity;
            lightsModule.intensityMultiplier = intensityMultiplier;
            lightsModule.maxLights = maxLights;

            return lightsModule;
        }

        public static implicit operator SerializableLightsModule(ParticleSystem.LightsModule lightsModule)
        {
            return new SerializableLightsModule(lightsModule);
        }

        public static implicit operator ParticleSystem.LightsModule(SerializableLightsModule serializableLightsModule)
        {
            return serializableLightsModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableLight : ITypeConverter<Light>
    {
        public SerializableEnum<LightType> type;
        public SerializableEnum<LightShape> shape;
        public float spotAngle;
        public float innerSpotAngle;
        public SerializableColor color;
        public float intensity;
        public bool useColorTemperature;
        public float colorTemperature;
        public float bounceIntensity;
        public bool useBoundingSphereOverride;
        public SerializableVector4 boundingSphereOverride;
        public bool useViewFrustumForShadowCasterCull;
        public int shadowCustomResolution;
        public float shadowBias;
        public float shadowNormalBias;
        public float shadowNearPlane;
        public bool useShadowMatrixOverride;

        public SerializableLight(Light light)
        {
            type = new SerializableEnum<LightType>(light.type);
            shape = new SerializableEnum<LightShape>(light.shape);
            spotAngle = light.spotAngle;
            innerSpotAngle = light.innerSpotAngle;
            color = light.color;
            intensity = light.intensity;
            useColorTemperature = light.useColorTemperature;
            colorTemperature = light.colorTemperature;
            bounceIntensity = light.bounceIntensity;
            useBoundingSphereOverride = light.useBoundingSphereOverride;
            boundingSphereOverride = light.boundingSphereOverride;
            useViewFrustumForShadowCasterCull = light.useViewFrustumForShadowCasterCull;
            shadowCustomResolution = light.shadowCustomResolution;
            shadowBias = light.shadowBias;
            shadowNormalBias = light.shadowNormalBias;
            shadowNearPlane = light.shadowNearPlane;
            useShadowMatrixOverride = light.useShadowMatrixOverride;
        }

        public Light ToOriginalType()
        {
            Light light = new Light();

            light.type = type.ToOriginalType();
            light.shape = shape.ToOriginalType();
            light.spotAngle = spotAngle;
            light.innerSpotAngle = innerSpotAngle;
            light.color = color;
            light.intensity = intensity;
            light.useColorTemperature = useColorTemperature;
            light.colorTemperature = colorTemperature;
            light.bounceIntensity = bounceIntensity;
            light.useBoundingSphereOverride = useBoundingSphereOverride;
            light.boundingSphereOverride = boundingSphereOverride;
            light.useViewFrustumForShadowCasterCull = useViewFrustumForShadowCasterCull;
            light.shadowCustomResolution = shadowCustomResolution;
            light.shadowBias = shadowBias;
            light.shadowNormalBias = shadowNormalBias;
            light.shadowNearPlane = shadowNearPlane;
            light.useShadowMatrixOverride = useShadowMatrixOverride;

            return light;
        }

        public static implicit operator SerializableLight(Light light)
        {
            return new SerializableLight(light);
        }

        public static implicit operator Light(SerializableLight serializableLight)
        {
            return serializableLight.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableSizeBySpeedModule : ITypeConverter<ParticleSystem.SizeBySpeedModule>
    {
        public bool enabled;
        public SerializableMinMaxCurve size;
        public float sizeMultiplier;
        public SerializableMinMaxCurve x;
        public float xMultiplier;
        public SerializableMinMaxCurve y;
        public float yMultiplier;
        public SerializableMinMaxCurve z;
        public float zMultiplier;
        public bool separateAxes;
        public SerializableVector2 range;

        public SerializableSizeBySpeedModule(ParticleSystem.SizeBySpeedModule sizeBySpeedModule)
        {
            enabled = sizeBySpeedModule.enabled;
            size = sizeBySpeedModule.size;
            sizeMultiplier = sizeBySpeedModule.sizeMultiplier;
            x = sizeBySpeedModule.x;
            xMultiplier = sizeBySpeedModule.xMultiplier;
            y = sizeBySpeedModule.y;
            yMultiplier = sizeBySpeedModule.yMultiplier;
            z = sizeBySpeedModule.z;
            zMultiplier = sizeBySpeedModule.zMultiplier;
            separateAxes = sizeBySpeedModule.separateAxes;
            range = sizeBySpeedModule.range;
        }

        public ParticleSystem.SizeBySpeedModule ToOriginalType()
        {
            ParticleSystem.SizeBySpeedModule sizeBySpeedModule = new ParticleSystem.SizeBySpeedModule();

            sizeBySpeedModule.enabled = enabled;
            sizeBySpeedModule.size = size;
            sizeBySpeedModule.sizeMultiplier = sizeMultiplier;
            sizeBySpeedModule.x = x;
            sizeBySpeedModule.xMultiplier = xMultiplier;
            sizeBySpeedModule.y = y;
            sizeBySpeedModule.yMultiplier = yMultiplier;
            sizeBySpeedModule.z = z;
            sizeBySpeedModule.zMultiplier = zMultiplier;
            sizeBySpeedModule.separateAxes = separateAxes;
            sizeBySpeedModule.range = range;

            return sizeBySpeedModule;
        }

        public static implicit operator SerializableSizeBySpeedModule(ParticleSystem.SizeBySpeedModule sizeBySpeedModule)
        {
            return new SerializableSizeBySpeedModule(sizeBySpeedModule);
        }

        public static implicit operator ParticleSystem.SizeBySpeedModule(SerializableSizeBySpeedModule serializableSizeBySpeedModule)
        {
            return serializableSizeBySpeedModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableRotationBySpeedModule : ITypeConverter<ParticleSystem.RotationBySpeedModule>
    {
        public bool enabled;
        public SerializableMinMaxCurve x;
        public float xMultiplier;
        public SerializableMinMaxCurve y;
        public float yMultiplier;
        public SerializableMinMaxCurve z;
        public float zMultiplier;
        public bool separateAxes;
        public SerializableVector2 range;

        public SerializableRotationBySpeedModule(ParticleSystem.RotationBySpeedModule rotationBySpeedModule)
        {
            enabled = rotationBySpeedModule.enabled;
            x = rotationBySpeedModule.x;
            xMultiplier = rotationBySpeedModule.xMultiplier;
            y = rotationBySpeedModule.y;
            yMultiplier = rotationBySpeedModule.yMultiplier;
            z = rotationBySpeedModule.z;
            zMultiplier = rotationBySpeedModule.zMultiplier;
            separateAxes = rotationBySpeedModule.separateAxes;
            range = rotationBySpeedModule.range;
        }

        public ParticleSystem.RotationBySpeedModule ToOriginalType()
        {
            ParticleSystem.RotationBySpeedModule rotationBySpeedModule = new ParticleSystem.RotationBySpeedModule();

            rotationBySpeedModule.enabled = enabled;
            rotationBySpeedModule.x = x;
            rotationBySpeedModule.xMultiplier = xMultiplier;
            rotationBySpeedModule.y = y;
            rotationBySpeedModule.yMultiplier = yMultiplier;
            rotationBySpeedModule.z = z;
            rotationBySpeedModule.zMultiplier = zMultiplier;
            rotationBySpeedModule.separateAxes = separateAxes;
            rotationBySpeedModule.range = range;

            return rotationBySpeedModule;
        }

        public static implicit operator SerializableRotationBySpeedModule(ParticleSystem.RotationBySpeedModule rotationBySpeedModule)
        {
            return new SerializableRotationBySpeedModule(rotationBySpeedModule);
        }

        public static implicit operator ParticleSystem.RotationBySpeedModule(SerializableRotationBySpeedModule serializableRotationBySpeedModule)
        {
            return serializableRotationBySpeedModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableRotationOverLifetimeModule : ITypeConverter<ParticleSystem.RotationOverLifetimeModule>
    {
        public bool enabled;
        public SerializableMinMaxCurve x;
        public float xMultiplier;
        public SerializableMinMaxCurve y;
        public float yMultiplier;
        public SerializableMinMaxCurve z;
        public float zMultiplier;
        public bool separateAxes;

        public SerializableRotationOverLifetimeModule(ParticleSystem.RotationOverLifetimeModule rotationOverLifetimeModule)
        {
            enabled = rotationOverLifetimeModule.enabled;
            x = rotationOverLifetimeModule.x;
            xMultiplier = rotationOverLifetimeModule.xMultiplier;
            y = rotationOverLifetimeModule.y;
            yMultiplier = rotationOverLifetimeModule.yMultiplier;
            z = rotationOverLifetimeModule.z;
            zMultiplier = rotationOverLifetimeModule.zMultiplier;
            separateAxes = rotationOverLifetimeModule.separateAxes;
        }

        public ParticleSystem.RotationOverLifetimeModule ToOriginalType()
        {
            ParticleSystem.RotationOverLifetimeModule rotationOverLifetimeModule = new ParticleSystem.RotationOverLifetimeModule();

            rotationOverLifetimeModule.enabled = enabled;
            rotationOverLifetimeModule.x = x;
            rotationOverLifetimeModule.xMultiplier = xMultiplier;
            rotationOverLifetimeModule.y = y;
            rotationOverLifetimeModule.yMultiplier = yMultiplier;
            rotationOverLifetimeModule.z = z;
            rotationOverLifetimeModule.zMultiplier = zMultiplier;
            rotationOverLifetimeModule.separateAxes = separateAxes;

            return rotationOverLifetimeModule;
        }

        public static implicit operator SerializableRotationOverLifetimeModule(ParticleSystem.RotationOverLifetimeModule rotationOverLifetimeModule)
        {
            return new SerializableRotationOverLifetimeModule(rotationOverLifetimeModule);
        }

        public static implicit operator ParticleSystem.RotationOverLifetimeModule(SerializableRotationOverLifetimeModule serializableRotationOverLifetimeModule)
        {
            return serializableRotationOverLifetimeModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableCollisionModule : ITypeConverter<ParticleSystem.CollisionModule>
    {
        public bool enabled;
        public SerializableEnum<ParticleSystemCollisionType> type;
        public SerializableEnum<ParticleSystemCollisionMode> mode;
        public SerializableMinMaxCurve dampen;
        public float dampenMultiplier;
        public SerializableMinMaxCurve bounce;
        public float bounceMultiplier;
        public SerializableMinMaxCurve lifetimeLoss;
        public float lifetimeLossMultiplier;
        public float minKillSpeed;
        public float maxKillSpeed;
        public SerializableLayerMask collidesWith;
        public bool enableDynamicColliders;
        public int maxCollisionShapes;
        public SerializableEnum<ParticleSystemCollisionQuality> quality;
        public float voxelSize;
        public float radiusScale;
        public bool sendCollisionMessages;
        public float colliderForce;
        public bool multiplyColliderForceByCollisionAngle;
        public bool multiplyColliderForceByParticleSpeed;
        public bool multiplyColliderForceByParticleSize;

        public SerializableCollisionModule(ParticleSystem.CollisionModule collisionModule)
        {
            enabled = collisionModule.enabled;
            type = new SerializableEnum<ParticleSystemCollisionType>(collisionModule.type);
            mode = new SerializableEnum<ParticleSystemCollisionMode>(collisionModule.mode);
            dampen = collisionModule.dampen;
            dampenMultiplier = collisionModule.dampenMultiplier;
            bounce = collisionModule.bounce;
            bounceMultiplier = collisionModule.bounceMultiplier;
            lifetimeLoss = collisionModule.lifetimeLoss;
            lifetimeLossMultiplier = collisionModule.lifetimeLossMultiplier;
            minKillSpeed = collisionModule.minKillSpeed;
            maxKillSpeed = collisionModule.maxKillSpeed;
            collidesWith = collisionModule.collidesWith;
            enableDynamicColliders = collisionModule.enableDynamicColliders;
            maxCollisionShapes = collisionModule.maxCollisionShapes;
            quality = new SerializableEnum<ParticleSystemCollisionQuality>(collisionModule.quality);
            voxelSize = collisionModule.voxelSize;
            radiusScale = collisionModule.radiusScale;
            sendCollisionMessages = collisionModule.sendCollisionMessages;
            colliderForce = collisionModule.colliderForce;
            multiplyColliderForceByCollisionAngle = collisionModule.multiplyColliderForceByCollisionAngle;
            multiplyColliderForceByParticleSpeed = collisionModule.multiplyColliderForceByParticleSpeed;
            multiplyColliderForceByParticleSize = collisionModule.multiplyColliderForceByParticleSize;
        }

        public ParticleSystem.CollisionModule ToOriginalType()
        {
            ParticleSystem.CollisionModule collisionModule = new ParticleSystem.CollisionModule();

            collisionModule.enabled = enabled;
            collisionModule.type = type.ToOriginalType();
            collisionModule.mode = mode.ToOriginalType();
            collisionModule.dampen = dampen;
            collisionModule.dampenMultiplier = dampenMultiplier;
            collisionModule.bounce = bounce;
            collisionModule.bounceMultiplier = bounceMultiplier;
            collisionModule.lifetimeLoss = lifetimeLoss;
            collisionModule.lifetimeLossMultiplier = lifetimeLossMultiplier;
            collisionModule.minKillSpeed = minKillSpeed;
            collisionModule.maxKillSpeed = maxKillSpeed;
            collisionModule.collidesWith = collidesWith;
            collisionModule.enableDynamicColliders = enableDynamicColliders;
            collisionModule.maxCollisionShapes = maxCollisionShapes;
            collisionModule.quality = quality.ToOriginalType();
            collisionModule.voxelSize = voxelSize;
            collisionModule.radiusScale = radiusScale;
            collisionModule.sendCollisionMessages = sendCollisionMessages;
            collisionModule.colliderForce = colliderForce;
            collisionModule.multiplyColliderForceByCollisionAngle = multiplyColliderForceByCollisionAngle;
            collisionModule.multiplyColliderForceByParticleSpeed = multiplyColliderForceByParticleSpeed;
            collisionModule.multiplyColliderForceByParticleSize = multiplyColliderForceByParticleSize;

            return collisionModule;
        }

        public static implicit operator SerializableCollisionModule(ParticleSystem.CollisionModule collisionModule)
        {
            return new SerializableCollisionModule(collisionModule);
        }

        public static implicit operator ParticleSystem.CollisionModule(SerializableCollisionModule serializableCollisionModule)
        {
            return serializableCollisionModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableSizeOverLifetimeModule : ITypeConverter<ParticleSystem.SizeOverLifetimeModule>
    {
        public bool enabled;
        public SerializableMinMaxCurve size;
        public SerializableMinMaxCurve x;
        public SerializableMinMaxCurve y;
        public SerializableMinMaxCurve z;
        public bool separateAxes;

        public SerializableSizeOverLifetimeModule(ParticleSystem.SizeOverLifetimeModule sizeOverLifetimeModule)
        {
            enabled = sizeOverLifetimeModule.enabled;
            size = sizeOverLifetimeModule.size;
            x = sizeOverLifetimeModule.x;
            y = sizeOverLifetimeModule.y;
            z = sizeOverLifetimeModule.z;
            separateAxes = sizeOverLifetimeModule.separateAxes;
        }

        public ParticleSystem.SizeOverLifetimeModule ToOriginalType()
        {
            ParticleSystem.SizeOverLifetimeModule sizeOverLifetimeModule = new ParticleSystem.SizeOverLifetimeModule();

            sizeOverLifetimeModule.enabled = enabled;
            sizeOverLifetimeModule.size = size;
            sizeOverLifetimeModule.x = x;
            sizeOverLifetimeModule.y = y;
            sizeOverLifetimeModule.z = z;
            sizeOverLifetimeModule.separateAxes = separateAxes;

            return sizeOverLifetimeModule;
        }

        public static implicit operator SerializableSizeOverLifetimeModule(ParticleSystem.SizeOverLifetimeModule sizeOverLifetimeModule)
        {
            return new SerializableSizeOverLifetimeModule(sizeOverLifetimeModule);
        }

        public static implicit operator ParticleSystem.SizeOverLifetimeModule(SerializableSizeOverLifetimeModule serializableSizeOverLifetimeModule)
        {
            return serializableSizeOverLifetimeModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableExternalForcesModule : ITypeConverter<ParticleSystem.ExternalForcesModule>
    {
        public bool enabled;
        public float multiplier;
        public SerializableMinMaxCurve multiplierCurve;
        public SerializableEnum<ParticleSystemGameObjectFilter> influenceFilter;
        public SerializableLayerMask influenceMask;
        public int influenceCount;

        public SerializableExternalForcesModule(ParticleSystem.ExternalForcesModule externalForcesModule)
        {
            enabled = externalForcesModule.enabled;
            multiplier = externalForcesModule.multiplier;
            multiplierCurve = externalForcesModule.multiplierCurve;
            influenceFilter = new SerializableEnum<ParticleSystemGameObjectFilter>(externalForcesModule.influenceFilter);
            influenceMask = externalForcesModule.influenceMask;
            influenceCount = externalForcesModule.influenceCount;
        }

        public ParticleSystem.ExternalForcesModule ToOriginalType()
        {
            ParticleSystem.ExternalForcesModule externalForcesModule = new ParticleSystem.ExternalForcesModule();

            externalForcesModule.enabled = enabled;
            externalForcesModule.multiplier = multiplier;
            externalForcesModule.multiplierCurve = multiplierCurve;
            externalForcesModule.influenceFilter = influenceFilter.ToOriginalType();
            externalForcesModule.influenceMask = influenceMask;

            return externalForcesModule;
        }

        public static implicit operator SerializableExternalForcesModule(ParticleSystem.ExternalForcesModule externalForcesModule)
        {
            return new SerializableExternalForcesModule(externalForcesModule);
        }

        public static implicit operator ParticleSystem.ExternalForcesModule(SerializableExternalForcesModule serializableExternalForcesModule)
        {
            return serializableExternalForcesModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableTextureSheetAnimationModule : ITypeConverter<ParticleSystem.TextureSheetAnimationModule>
    {
        public bool enabled;
        public SerializableEnum<ParticleSystemAnimationMode> mode;
        public SerializableEnum<ParticleSystemAnimationTimeMode> timeMode;
        public float fps;
        public int numTilesX;
        public int numTilesY;
        public SerializableEnum<ParticleSystemAnimationType> animation;
        public SerializableEnum<ParticleSystemAnimationRowMode> rowMode;
        public SerializableMinMaxCurve frameOverTime;
        public float frameOverTimeMultiplier;
        public SerializableMinMaxCurve startFrame;
        public float startFrameMultiplier;
        public int cycleCount;
        public int rowIndex;
        public SerializableEnum<UVChannelFlags> uvChannelMask;

        public SerializableTextureSheetAnimationModule(ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule)
        {
            enabled = textureSheetAnimationModule.enabled;
            mode = new SerializableEnum<ParticleSystemAnimationMode>(textureSheetAnimationModule.mode);
            timeMode = new SerializableEnum<ParticleSystemAnimationTimeMode>(textureSheetAnimationModule.timeMode);
            fps = textureSheetAnimationModule.fps;
            numTilesX = textureSheetAnimationModule.numTilesX;
            numTilesY = textureSheetAnimationModule.numTilesY;
            animation = new SerializableEnum<ParticleSystemAnimationType>(textureSheetAnimationModule.animation);
            rowMode = new SerializableEnum<ParticleSystemAnimationRowMode>(textureSheetAnimationModule.rowMode);
            frameOverTime = new SerializableMinMaxCurve(textureSheetAnimationModule.frameOverTime);
            frameOverTimeMultiplier = textureSheetAnimationModule.frameOverTimeMultiplier;
            startFrame = new SerializableMinMaxCurve(textureSheetAnimationModule.startFrame);
            startFrameMultiplier = textureSheetAnimationModule.startFrameMultiplier;
            cycleCount = textureSheetAnimationModule.cycleCount;
            rowIndex = textureSheetAnimationModule.rowIndex;
            uvChannelMask = new SerializableEnum<UVChannelFlags>(textureSheetAnimationModule.uvChannelMask);
        }

        public ParticleSystem.TextureSheetAnimationModule ToOriginalType()
        {
            ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = new ParticleSystem.TextureSheetAnimationModule();

            textureSheetAnimationModule.enabled = enabled;
            textureSheetAnimationModule.mode = mode.ToOriginalType();
            textureSheetAnimationModule.timeMode = timeMode.ToOriginalType();
            textureSheetAnimationModule.fps = fps;
            textureSheetAnimationModule.numTilesX = numTilesX;
            textureSheetAnimationModule.numTilesY = numTilesY;
            textureSheetAnimationModule.animation = animation.ToOriginalType();
            textureSheetAnimationModule.rowMode = rowMode.ToOriginalType();
            textureSheetAnimationModule.frameOverTime = frameOverTime;
            textureSheetAnimationModule.frameOverTimeMultiplier = frameOverTimeMultiplier;
            textureSheetAnimationModule.startFrame = startFrame;
            textureSheetAnimationModule.startFrameMultiplier = startFrameMultiplier;
            textureSheetAnimationModule.cycleCount = cycleCount;
            textureSheetAnimationModule.rowIndex = rowIndex;
            textureSheetAnimationModule.uvChannelMask = uvChannelMask.ToOriginalType();

            return textureSheetAnimationModule;
        }

        public static implicit operator SerializableTextureSheetAnimationModule(ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule)
        {
            return new SerializableTextureSheetAnimationModule(textureSheetAnimationModule);
        }

        public static implicit operator ParticleSystem.TextureSheetAnimationModule(SerializableTextureSheetAnimationModule serializableTextureSheetAnimationModule)
        {
            return serializableTextureSheetAnimationModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableSubEmittersModule : ITypeConverter<ParticleSystem.SubEmittersModule>
    {
        public bool enabled;
        public SerializableSubEmitter[] subEmitters;

        public SerializableSubEmittersModule(ParticleSystem.SubEmittersModule subEmittersModule)
        {
            enabled = subEmittersModule.enabled;

            subEmitters = new SerializableSubEmitter[subEmittersModule.subEmittersCount];
            for (int i = 0; i < subEmittersModule.subEmittersCount; i++)
            {
                var subEmitter = new SerializableSubEmitter();
                subEmitter.type = subEmittersModule.GetSubEmitterType(i);
                subEmitter.system = subEmittersModule.GetSubEmitterSystem(i);
                subEmitter.properties = subEmittersModule.GetSubEmitterProperties(i);
                subEmitter.emitProbability = subEmittersModule.GetSubEmitterEmitProbability(i);
                subEmitters[i] = subEmitter;
            }
        }

        public ParticleSystem.SubEmittersModule ToOriginalType()
        {
            ParticleSystem.SubEmittersModule subEmittersModule = new ParticleSystem.SubEmittersModule();
            subEmittersModule.enabled = enabled;

            foreach (var subEmitter in subEmitters)
            {
                subEmittersModule.AddSubEmitter(subEmitter.system, subEmitter.type, subEmitter.properties, subEmitter.emitProbability);
            }

            return subEmittersModule;
        }

        public static implicit operator SerializableSubEmittersModule(ParticleSystem.SubEmittersModule subEmittersModule)
        {
            return new SerializableSubEmittersModule(subEmittersModule);
        }

        public static implicit operator ParticleSystem.SubEmittersModule(SerializableSubEmittersModule serializableSubEmittersModule)
        {
            return serializableSubEmittersModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableSubEmitter
    {
        public ParticleSystem system;
        public ParticleSystemSubEmitterType type;
        public ParticleSystemSubEmitterProperties properties;
        public float emitProbability;

        public SerializableSubEmitter(ParticleSystem system, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties, float emitProbability)
        {
            this.system = system;
            this.type = type;
            this.properties = properties;
            this.emitProbability = emitProbability;
        }
    }

    [Serializable]
    public struct SerializableNoiseModule : ITypeConverter<ParticleSystem.NoiseModule>
    {
        public bool enabled;
        public bool separateAxes;
        public SerializableMinMaxCurve strength;
        public SerializableMinMaxCurve strengthX;
        public SerializableMinMaxCurve strengthY;
        public SerializableMinMaxCurve strengthZ;
        public float frequency;
        public bool damping;
        public int octaveCount;
        public float octaveMultiplier;
        public float octaveScale;
        public SerializableEnum<ParticleSystemNoiseQuality> quality;
        public SerializableMinMaxCurve scrollSpeed;
        public bool remapEnabled;
        public SerializableMinMaxCurve remap;
        public SerializableMinMaxCurve remapX;
        public SerializableMinMaxCurve remapY;
        public SerializableMinMaxCurve remapZ;
        public SerializableMinMaxCurve positionAmount;
        public SerializableMinMaxCurve rotationAmount;
        public SerializableMinMaxCurve sizeAmount;

        public SerializableNoiseModule(ParticleSystem.NoiseModule noiseModule)
        {
            enabled = noiseModule.enabled;
            separateAxes = noiseModule.separateAxes;
            strength = noiseModule.strength;
            strengthX = noiseModule.strengthX;
            strengthY = noiseModule.strengthY;
            strengthZ = noiseModule.strengthZ;
            frequency = noiseModule.frequency;
            damping = noiseModule.damping;
            octaveCount = noiseModule.octaveCount;
            octaveMultiplier = noiseModule.octaveMultiplier;
            octaveScale = noiseModule.octaveScale;
            quality = new SerializableEnum<ParticleSystemNoiseQuality>(noiseModule.quality);
            scrollSpeed = noiseModule.scrollSpeed;
            remapEnabled = noiseModule.remapEnabled;
            remap = noiseModule.remap;
            remapX = noiseModule.remapX;
            remapY = noiseModule.remapY;
            remapZ = noiseModule.remapZ;
            positionAmount = noiseModule.positionAmount;
            rotationAmount = noiseModule.rotationAmount;
            sizeAmount = noiseModule.sizeAmount;
        }

        public ParticleSystem.NoiseModule ToOriginalType()
        {
            ParticleSystem.NoiseModule noiseModule = new ParticleSystem.NoiseModule();
            noiseModule.enabled = enabled;
            noiseModule.separateAxes = separateAxes;
            noiseModule.strength = strength;
            noiseModule.strengthX = strengthX;
            noiseModule.strengthY = strengthY;
            noiseModule.strengthZ = strengthZ;
            noiseModule.frequency = frequency;
            noiseModule.damping = damping;
            noiseModule.octaveCount = octaveCount;
            noiseModule.octaveMultiplier = octaveMultiplier;
            noiseModule.octaveScale = octaveScale;
            noiseModule.quality = quality.ToOriginalType();
            noiseModule.scrollSpeed = scrollSpeed;
            noiseModule.remapEnabled = remapEnabled;
            noiseModule.remap = remap;
            noiseModule.remapX = remapX;
            noiseModule.remapY = remapY;
            noiseModule.remapZ = remapZ;
            noiseModule.positionAmount = positionAmount;
            noiseModule.rotationAmount = rotationAmount;
            noiseModule.sizeAmount = sizeAmount;
            return noiseModule;
        }

        public static implicit operator SerializableNoiseModule(ParticleSystem.NoiseModule noiseModule)
        {
            return new SerializableNoiseModule(noiseModule);
        }

        public static implicit operator ParticleSystem.NoiseModule(SerializableNoiseModule serializableNoiseModule)
        {
            return serializableNoiseModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableMainModule : ITypeConverter<ParticleSystem.MainModule>
    {
        public Vector3 emitterVelocity;
        public float duration;
        public bool loop;
        public bool prewarm;
        public SerializableMinMaxCurve startDelay;
        public float startDelayMultiplier;
        public SerializableMinMaxCurve startLifetime;
        public float startLifetimeMultiplier;
        public SerializableMinMaxCurve startSpeed;
        public float startSpeedMultiplier;
        public bool startSize3D;
        public SerializableMinMaxCurve startSize;
        public float startSizeMultiplier;
        public SerializableMinMaxCurve startSizeX;
        public float startSizeXMultiplier;
        public SerializableMinMaxCurve startSizeY;
        public float startSizeYMultiplier;
        public SerializableMinMaxCurve startSizeZ;
        public float startSizeZMultiplier;
        public bool startRotation3D;
        public SerializableMinMaxCurve startRotation;
        public float startRotationMultiplier;
        public SerializableMinMaxCurve startRotationX;
        public float startRotationXMultiplier;
        public SerializableMinMaxCurve startRotationY;
        public float startRotationYMultiplier;
        public SerializableMinMaxCurve startRotationZ;
        public float startRotationZMultiplier;
        public float flipRotation;
        public SerializableMinMaxGradient startColor;
        public SerializableEnum<ParticleSystemGravitySource> gravitySource;
        public SerializableMinMaxCurve gravityModifier;
        public float gravityModifierMultiplier;
        public SerializableEnum<ParticleSystemSimulationSpace> simulationSpace;
        public Transform customSimulationSpace;
        public float simulationSpeed;
        public bool useUnscaledTime;
        public SerializableEnum<ParticleSystemScalingMode> scalingMode;
        public bool playOnAwake;
        public int maxParticles;
        public SerializableEnum<ParticleSystemEmitterVelocityMode> emitterVelocityMode;
        public SerializableEnum<ParticleSystemStopAction> stopAction;
        public SerializableEnum<ParticleSystemRingBufferMode> ringBufferMode;
        public Vector2 ringBufferLoopRange;
        public SerializableEnum<ParticleSystemCullingMode> cullingMode;

        public SerializableMainModule(ParticleSystem.MainModule mainModule)
        {
            emitterVelocity = mainModule.emitterVelocity;
            duration = mainModule.duration;
            loop = mainModule.loop;
            prewarm = mainModule.prewarm;
            startDelay = mainModule.startDelay;
            startDelayMultiplier = mainModule.startDelayMultiplier;
            startLifetime = mainModule.startLifetime;
            startLifetimeMultiplier = mainModule.startLifetimeMultiplier;
            startSpeed = mainModule.startSpeed;
            startSpeedMultiplier = mainModule.startSpeedMultiplier;
            startSize3D = mainModule.startSize3D;
            startSize = mainModule.startSize;
            startSizeMultiplier = mainModule.startSizeMultiplier;
            startSizeX = mainModule.startSizeX;
            startSizeXMultiplier = mainModule.startSizeXMultiplier;
            startSizeY = mainModule.startSizeY;
            startSizeYMultiplier = mainModule.startSizeYMultiplier;
            startSizeZ = mainModule.startSizeZ;
            startSizeZMultiplier = mainModule.startSizeZMultiplier;
            startRotation3D = mainModule.startRotation3D;
            startRotation = mainModule.startRotation;
            startRotationMultiplier = mainModule.startRotationMultiplier;
            startRotationX = mainModule.startRotationX;
            startRotationXMultiplier = mainModule.startRotationXMultiplier;
            startRotationY = mainModule.startRotationY;
            startRotationYMultiplier = mainModule.startRotationYMultiplier;
            startRotationZ = mainModule.startRotationZ;
            startRotationZMultiplier = mainModule.startRotationZMultiplier;
            flipRotation = mainModule.flipRotation;
            startColor = mainModule.startColor;
            gravitySource = new SerializableEnum<ParticleSystemGravitySource>(mainModule.gravitySource);
            gravityModifier = mainModule.gravityModifier;
            gravityModifierMultiplier = mainModule.gravityModifierMultiplier;
            simulationSpace = new SerializableEnum<ParticleSystemSimulationSpace>(mainModule.simulationSpace);
            customSimulationSpace = mainModule.customSimulationSpace;
            simulationSpeed = mainModule.simulationSpeed;
            useUnscaledTime = mainModule.useUnscaledTime;
            scalingMode = new SerializableEnum<ParticleSystemScalingMode>(mainModule.scalingMode);
            playOnAwake = mainModule.playOnAwake;
            maxParticles = mainModule.maxParticles;
            emitterVelocityMode = new SerializableEnum<ParticleSystemEmitterVelocityMode>(mainModule.emitterVelocityMode);
            stopAction = new SerializableEnum<ParticleSystemStopAction>(mainModule.stopAction);
            ringBufferMode = new SerializableEnum<ParticleSystemRingBufferMode>(mainModule.ringBufferMode);
            ringBufferLoopRange = mainModule.ringBufferLoopRange;
            cullingMode = new SerializableEnum<ParticleSystemCullingMode>(mainModule.cullingMode);
        }

        public ParticleSystem.MainModule ToOriginalType()
        {
            ParticleSystem.MainModule mainModule = new ParticleSystem.MainModule();
            mainModule.emitterVelocity = emitterVelocity;
            mainModule.duration = duration;
            mainModule.loop = loop;
            mainModule.prewarm = prewarm;
            mainModule.startDelay = startDelay;
            mainModule.startDelayMultiplier = startDelayMultiplier;
            mainModule.startLifetime = startLifetime;
            mainModule.startLifetimeMultiplier = startLifetimeMultiplier;
            mainModule.startSpeed = startSpeed;
            mainModule.startSpeedMultiplier = startSpeedMultiplier;
            mainModule.startSize3D = startSize3D;
            mainModule.startSize = startSize;
            mainModule.startSizeMultiplier = startSizeMultiplier;
            mainModule.startSizeX = startSizeX;
            mainModule.startSizeXMultiplier = startSizeXMultiplier;
            mainModule.startSizeY = startSizeY;
            mainModule.startSizeYMultiplier = startSizeYMultiplier;
            mainModule.startSizeZ = startSizeZ;
            mainModule.startSizeZMultiplier = startSizeZMultiplier;
            mainModule.startRotation3D = startRotation3D;
            mainModule.startRotation = startRotation;
            mainModule.startRotationMultiplier = startRotationMultiplier;
            mainModule.startRotationX = startRotationX;
            mainModule.startRotationXMultiplier = startRotationXMultiplier;
            mainModule.startRotationY = startRotationY;
            mainModule.startRotationYMultiplier = startRotationYMultiplier;
            mainModule.startRotationZ = startRotationZ;
            mainModule.startRotationZMultiplier = startRotationZMultiplier;
            mainModule.flipRotation = flipRotation;
            mainModule.startColor = startColor;
            mainModule.gravitySource = gravitySource.ToOriginalType();
            mainModule.gravityModifier = gravityModifier;
            mainModule.gravityModifierMultiplier = gravityModifierMultiplier;
            mainModule.simulationSpace = simulationSpace.ToOriginalType();
            mainModule.customSimulationSpace = customSimulationSpace;
            mainModule.simulationSpeed = simulationSpeed;
            mainModule.useUnscaledTime = useUnscaledTime;
            mainModule.scalingMode = scalingMode.ToOriginalType();
            mainModule.playOnAwake = playOnAwake;
            mainModule.maxParticles = maxParticles;
            mainModule.emitterVelocityMode = emitterVelocityMode.ToOriginalType();
            mainModule.stopAction = stopAction.ToOriginalType();
            mainModule.ringBufferMode = ringBufferMode.ToOriginalType();
            mainModule.ringBufferLoopRange = ringBufferLoopRange;
            mainModule.cullingMode = cullingMode.ToOriginalType();
            return mainModule;
        }

        public static implicit operator SerializableMainModule(ParticleSystem.MainModule mainModule)
        {
            return new SerializableMainModule(mainModule);
        }

        public static implicit operator ParticleSystem.MainModule(SerializableMainModule serializableMainModule)
        {
            return serializableMainModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableTrailModule : ITypeConverter<ParticleSystem.TrailModule>
    {
        public bool enabled;
        public SerializableEnum<ParticleSystemTrailMode> mode;
        public float ratio;
        public SerializableMinMaxCurve lifetime;
        public float lifetimeMultiplier;
        public float minVertexDistance;
        public SerializableEnum<ParticleSystemTrailTextureMode> textureMode;
        public SerializableVector2 textureScale;
        public bool worldSpace;
        public bool dieWithParticles;
        public bool sizeAffectsWidth;
        public bool sizeAffectsLifetime;
        public bool inheritParticleColor;
        public SerializableMinMaxGradient colorOverLifetime;
        public SerializableMinMaxCurve widthOverTrail;
        public float widthOverTrailMultiplier;
        public SerializableMinMaxGradient colorOverTrail;
        public bool generateLightingData;
        public int ribbonCount;
        public float shadowBias;
        public bool splitSubEmitterRibbons;
        public bool attachRibbonsToTransform;

        public SerializableTrailModule(ParticleSystem.TrailModule trailModule)
        {
            enabled = trailModule.enabled;
            mode = new SerializableEnum<ParticleSystemTrailMode>(trailModule.mode);
            ratio = trailModule.ratio;
            lifetime = trailModule.lifetime;
            lifetimeMultiplier = trailModule.lifetimeMultiplier;
            minVertexDistance = trailModule.minVertexDistance;
            textureMode = new SerializableEnum<ParticleSystemTrailTextureMode>(trailModule.textureMode);
            textureScale = trailModule.textureScale;
            worldSpace = trailModule.worldSpace;
            dieWithParticles = trailModule.dieWithParticles;
            sizeAffectsWidth = trailModule.sizeAffectsWidth;
            sizeAffectsLifetime = trailModule.sizeAffectsLifetime;
            inheritParticleColor = trailModule.inheritParticleColor;
            colorOverLifetime = trailModule.colorOverLifetime;
            widthOverTrail = trailModule.widthOverTrail;
            widthOverTrailMultiplier = trailModule.widthOverTrailMultiplier;
            colorOverTrail = trailModule.colorOverTrail;
            generateLightingData = trailModule.generateLightingData;
            ribbonCount = trailModule.ribbonCount;
            shadowBias = trailModule.shadowBias;
            splitSubEmitterRibbons = trailModule.splitSubEmitterRibbons;
            attachRibbonsToTransform = trailModule.attachRibbonsToTransform;
        }

        public ParticleSystem.TrailModule ToOriginalType()
        {
            ParticleSystem.TrailModule trailModule = new ParticleSystem.TrailModule();
            trailModule.enabled = enabled;
            trailModule.mode = mode.ToOriginalType();
            trailModule.ratio = ratio;
            trailModule.lifetime = lifetime;
            trailModule.lifetimeMultiplier = lifetimeMultiplier;
            trailModule.minVertexDistance = minVertexDistance;
            trailModule.textureMode = textureMode.ToOriginalType();
            trailModule.textureScale = textureScale;
            trailModule.worldSpace = worldSpace;
            trailModule.dieWithParticles = dieWithParticles;
            trailModule.sizeAffectsWidth = sizeAffectsWidth;
            trailModule.sizeAffectsLifetime = sizeAffectsLifetime;
            trailModule.inheritParticleColor = inheritParticleColor;
            trailModule.colorOverLifetime = colorOverLifetime;
            trailModule.widthOverTrail = widthOverTrail;
            trailModule.widthOverTrailMultiplier = widthOverTrailMultiplier;
            trailModule.colorOverTrail = colorOverTrail;
            trailModule.generateLightingData = generateLightingData;
            trailModule.ribbonCount = ribbonCount;
            trailModule.shadowBias = shadowBias;
            trailModule.splitSubEmitterRibbons = splitSubEmitterRibbons;
            trailModule.attachRibbonsToTransform = attachRibbonsToTransform;
            return trailModule;
        }

        public static implicit operator SerializableTrailModule(ParticleSystem.TrailModule trailModule)
        {
            return new SerializableTrailModule(trailModule);
        }

        public static implicit operator ParticleSystem.TrailModule(SerializableTrailModule serializableTrailModule)
        {
            return serializableTrailModule.ToOriginalType();
        }
    }

    [Serializable]
    public struct SerializableTriggerModule : ITypeConverter<ParticleSystem.TriggerModule>
    {
        public bool enabled;
        public SerializableEnum<ParticleSystemOverlapAction> inside;
        public SerializableEnum<ParticleSystemOverlapAction> outside;
        public SerializableEnum<ParticleSystemOverlapAction> enter;
        public SerializableEnum<ParticleSystemOverlapAction> exit;
        public SerializableEnum<ParticleSystemColliderQueryMode> colliderQueryMode;
        public float radiusScale;
        public int colliderCount;

        public SerializableTriggerModule(ParticleSystem.TriggerModule triggerModule)
        {
            enabled = triggerModule.enabled;
            inside = new SerializableEnum<ParticleSystemOverlapAction>(triggerModule.inside);
            outside = new SerializableEnum<ParticleSystemOverlapAction>(triggerModule.outside);
            enter = new SerializableEnum<ParticleSystemOverlapAction>(triggerModule.enter);
            exit = new SerializableEnum<ParticleSystemOverlapAction>(triggerModule.exit);
            colliderQueryMode = new SerializableEnum<ParticleSystemColliderQueryMode>(triggerModule.colliderQueryMode);
            radiusScale = triggerModule.radiusScale;
            colliderCount = triggerModule.colliderCount;
        }

        public ParticleSystem.TriggerModule ToOriginalType()
        {
            ParticleSystem.TriggerModule triggerModule = new ParticleSystem.TriggerModule();
            triggerModule.enabled = enabled;
            triggerModule.inside = inside.ToOriginalType();
            triggerModule.outside = outside.ToOriginalType();
            triggerModule.enter = enter.ToOriginalType();
            triggerModule.exit = exit.ToOriginalType();
            triggerModule.colliderQueryMode = colliderQueryMode.ToOriginalType();
            triggerModule.radiusScale = radiusScale;
            return triggerModule;
        }

        public static implicit operator SerializableTriggerModule(ParticleSystem.TriggerModule triggerModule)
        {
            return new SerializableTriggerModule(triggerModule);
        }

        public static implicit operator ParticleSystem.TriggerModule(SerializableTriggerModule serializableTriggerModule)
        {
            return serializableTriggerModule.ToOriginalType();
        }
    }
}
#endregion