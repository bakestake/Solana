using System;
using UnityEngine;

public class LightBeam_Reflector : MonoBehaviour
{
    [SerializeField] protected Transform origin;
    [SerializeField] protected Direction8 currentDirection;

    [Header("Rotation")]
    [SerializeField] private SpriteRenderer mirrorSpriteRenderer;
    [SerializeField] private Sprite[] directionSprites = new Sprite[8];

    public event Action<Direction8> OnRotate;
    public event Action<bool> OnReflectionStateChanged;

    public Direction8 CurrentDirection => currentDirection;
    public Vector3 OriginPosition => origin.position;
    public bool IsReflecting => isReflecting;

    private bool isReflecting;

    private void OnValidate()
    {
        UpdateSprite();
    }

    public virtual Direction8? GetReflectedDirection(Direction8 incoming)
    {
        bool reflecting = false;

        if (incoming == currentDirection)
        {
            reflecting = false;
        }
        else if (IsOrthogonal(incoming, currentDirection))
        {
            reflecting = false;
            SetReflectionState(false);
            return incoming;
        }
        else
        {
            reflecting = true;
        }

        SetReflectionState(reflecting);
        return reflecting ? currentDirection : null;
    }

    public void RotateClockwise()
    {
        currentDirection = (Direction8)(((int)currentDirection + 1) % 8);
        UpdateSprite();
        OnRotate?.Invoke(currentDirection);
    }

    private void UpdateSprite()
    {
        if (mirrorSpriteRenderer != null && directionSprites != null && directionSprites.Length > (int)currentDirection)
        {
            mirrorSpriteRenderer.sprite = directionSprites[(int)currentDirection];
        }
    }

    private bool IsOrthogonal(Direction8 dirA, Direction8 dirB)
    {
        bool isAHorizontal = dirA == Direction8.Left || dirA == Direction8.Right;
        bool isBHorizontal = dirB == Direction8.Left || dirB == Direction8.Right;

        bool isAVertical = dirA == Direction8.Up || dirA == Direction8.Down;
        bool isBVertical = dirB == Direction8.Up || dirB == Direction8.Down;

        return (isAHorizontal && isBVertical) || (isAVertical && isBHorizontal);
    }

    private void SetReflectionState(bool reflecting)
    {
        if (isReflecting != reflecting)
        {
            isReflecting = reflecting;
            OnReflectionStateChanged?.Invoke(isReflecting);
        }
    }
}
