using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyToStart : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Submit") > 0) playerMovement.enabled = true;
    }
}
