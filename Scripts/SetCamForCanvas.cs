using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamForCanvas : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private Camera cam = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null) cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        canvas.worldCamera = cam;
        canvas.planeDistance = 1;
    }
}
