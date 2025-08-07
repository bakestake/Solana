using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float laserDistance;
    public LineRenderer lineRenderer;

    void Update()
    {
        Vector3 endPos = transform.position + (transform.right * -laserDistance);
        lineRenderer.SetPositions(new Vector3[] { transform.position, endPos });
    }
}
