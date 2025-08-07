using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRot : MonoBehaviour
{
    public Canvas canvas;  // Canvas objesi
    private Quaternion initialRotation;

    void Start()
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas is not assigned.");
            return;
        }

        // Canvas'ýn baþlangýç rotasyonunu kaydedin
        initialRotation = canvas.transform.rotation;
    }

    void LateUpdate()
    {
        // Canvas'ýn rotasyonunu baþlangýç rotasyonuna ayarlayýn
        canvas.transform.rotation = initialRotation;

        // Canvas içindeki tüm UI öðelerinin rotasyonunu sabitleyin
        foreach (Transform child in canvas.transform)
        {
            child.rotation = Quaternion.identity;
        }
    }
}
