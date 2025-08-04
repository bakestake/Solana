using UnityEngine;
using System.Collections.Generic;

public class LightBeam_Emitter : MonoBehaviour
{
    [Header("Beam Settings")]
    [SerializeField] protected bool toggled;
    [SerializeField] protected float maxDistance = 20f;
    [SerializeField] protected int maxBounces = 5;
    [SerializeField] protected Direction8 direction;
    [SerializeField] protected Transform origin;

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer.positionCount = 0;
    }

    private void FixedUpdate()
    {
        if (toggled)
        {
            DrawRay();
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    public virtual void Toggle()
    {
        SetRayState(!toggled);
    }

    public virtual void SetRayState(bool toggle)
    {
        toggled = toggle;
        if (!toggle)
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void DrawRay()
    {
        List<Vector3> points = new List<Vector3>();

        Vector2 currentPosition = origin.position;
        Direction8 currentDirection = direction;
        Vector2 directionVector = GetDirectionVector(currentDirection).normalized;

        points.Add(currentPosition);

        float remainingDistance = maxDistance;
        Transform currentEmitter = transform;

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, directionVector, remainingDistance, ~0);

            RaycastHit2D? validHit = null;
            float shortestDistance = float.MaxValue;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform == currentEmitter) continue;

                float distance = Vector2.Distance(currentPosition, hit.point);
                if (distance < shortestDistance)
                {
                    validHit = hit;
                    shortestDistance = distance;
                }
            }

            if (validHit.HasValue)
            {
                Vector2 hitPoint = validHit.Value.point;
                points.Add(hitPoint);

                LightBeam_Reflector reflector = validHit.Value.collider.GetComponent<LightBeam_Reflector>();
                if (reflector != null)
                {
                    Direction8? newDir = reflector.GetReflectedDirection(currentDirection);
                    if (newDir.HasValue)
                    {
                        currentEmitter = reflector.transform;
                        currentDirection = newDir.Value;
                        directionVector = GetDirectionVector(currentDirection).normalized;

                        currentPosition = reflector.OriginPosition;
                        points.Add(currentPosition);

                        remainingDistance = maxDistance;
                        continue;
                    }
                }

                break;
            }
            else
            {
                Vector2 endPoint = currentPosition + directionVector * remainingDistance;
                points.Add(endPoint);
                break;
            }
        }

        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    protected Vector2 GetDirectionVector(Direction8 dir)
    {
        switch (dir)
        {
            case Direction8.Up: return Vector2.up;
            case Direction8.UpRight: return new Vector2(1, 1);
            case Direction8.Right: return Vector2.right;
            case Direction8.DownRight: return new Vector2(1, -1);
            case Direction8.Down: return Vector2.down;
            case Direction8.DownLeft: return new Vector2(-1, -1);
            case Direction8.Left: return Vector2.left;
            case Direction8.UpLeft: return new Vector2(-1, 1);
            default: return Vector2.right;
        }
    }
}