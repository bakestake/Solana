using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] Vector2 min;
    [SerializeField] Vector2 max;
    Vector3 currentTarget;

    void Start()
    {
        currentTarget = GetRandomPoint();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget) < 0.5f)
        {
            currentTarget = GetRandomPoint();
        }
    }

    Vector3 GetRandomPoint()
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), transform.position.z);
    }
}
