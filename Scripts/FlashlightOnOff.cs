using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FlashlightOnOff : MonoBehaviour
{
    [SerializeField] private GameObject spotLight;

    // Start is called before the first frame update
    void Awake()
    {
        spotLight.GetComponent<Light>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) spotLight.GetComponent<Light>().enabled = !spotLight.GetComponent<Light>().enabled;
    }
}
