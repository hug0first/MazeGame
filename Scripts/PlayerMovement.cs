using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform transform; //added this line
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float lookSensitivity = 5.0f;
    [SerializeField] private Camera camera;
    private float diagonalSpeed;

    void Awake()
    {
        speed = speed / 100;//makes it an easier number to work with   
        diagonalSpeed = speed/Mathf.Sqrt(2); //makes it so that diagonal movement is the same speed as straight movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camera.transform.Rotate(new Vector3(0, 0, 0));//resets camera rotation on awake
    }

    void FixedUpdate()
    {
        float inputLR = Input.GetAxis("Horizontal");
        float inputUD = Input.GetAxis("Vertical");
        if (inputLR != 0 && inputUD != 0)
        {
            rb.MovePosition(transform.position + rb.transform.forward * inputUD * /* Time.deltaTime * */ diagonalSpeed + rb.transform.right * inputLR/*  * Time.deltaTime */ * diagonalSpeed);
        }
        else
        {
            rb.MovePosition(transform.position + rb.transform.forward * inputUD * /* Time.deltaTime * */ speed + rb.transform.right * inputLR/*  * Time.deltaTime */ * speed);
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * lookSensitivity * Time.deltaTime);
        //need to implement way to lock camera rotation to 90 degrees up and down
        /* if (camera.transform.rotation.x < 90 || camera.transform.eulerAngles.x > 270)  */camera.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0) * lookSensitivity * Time.deltaTime);
        Debug.Log(camera.transform.rotation.x);
    }
}
