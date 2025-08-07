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

        // Canvas'�n ba�lang�� rotasyonunu kaydedin
        initialRotation = canvas.transform.rotation;
    }

    void LateUpdate()
    {
        // Canvas'�n rotasyonunu ba�lang�� rotasyonuna ayarlay�n
        canvas.transform.rotation = initialRotation;

        // Canvas i�indeki t�m UI ��elerinin rotasyonunu sabitleyin
        foreach (Transform child in canvas.transform)
        {
            child.rotation = Quaternion.identity;
        }
    }
}
