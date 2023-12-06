using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Transform t;

    public static bool inWater;
    public static bool isSwimming;

    public LayerMask waterMask;

    [Header("Player Rotation")]
    public float sensitivity = 1;

    float rotationX;
    float rotationY;

    public float rotationMin;
    public float rotationMax;

    [Header("Player Movement")]
    public float speedLand = 1;
    public float speedFloating = 1;
    public float speedSwim = 1;
    float moveX;
    float moveY;
    float moveZ;

    Rigidbody rb;

    InventorySystem iSystem;

    // Start is called before the first frame update
    void Start()
    {
        t = this.transform;

        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;

        inWater = true;

        iSystem = InventorySystem.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!iSystem.inventoryOpen)
        {
            LookAround();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void FixedUpdate()
    {
        if (!iSystem.inventoryOpen)
        {
            SwimmingOrFloating();
            Move();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SwitchMovement();
    }

    private void OnTriggerExit(Collider other)
    {
        SwitchMovement();
    }

    void SwitchMovement()
    {
        inWater = !inWater;

        rb.useGravity = !rb.useGravity;
    }

    void SwimmingOrFloating()
    {
        bool swimCheck = false;

        if(inWater)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(t.position.x, t.position.y + 0.5f, t.position.z), Vector3.down, out hit, Mathf.Infinity, waterMask))
            {
                if(hit.distance < 0.1f)
                {
                    swimCheck = true;
                }
            }
            else
            {
                swimCheck = true;
            }
        }

        isSwimming = swimCheck;
        Debug.Log("isSwiming = " + isSwimming);  
    }

    void LookAround()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;

        rotationY = Mathf.Clamp(rotationY, rotationMin, rotationMax);

        t.localRotation = Quaternion.Euler(-rotationY, rotationX, 0);
    }

    void Move()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Forward");

        if(!inWater)
        {
            //move (lang ver)
            t.Translate(new Quaternion(0, t.rotation.y, 0, t.rotation.w) * new Vector3(moveX, 0, moveZ) * Time.deltaTime * speedLand, Space.World);
        }
        else
        {
            if(!isSwimming)
            {
                //move (floating ver)
                moveY = Mathf.Min(moveY, 0);

                Vector3 clampedDirection = t.TransformDirection(new Vector3(moveX, moveY, moveZ));

                clampedDirection = new Vector3(clampedDirection.x, Mathf.Min(clampedDirection.y, 0), clampedDirection.z);
                t.Translate(clampedDirection * Time.deltaTime * speedFloating, Space.World);
            }
            else
            {
                //move (swimming ver)
                t.Translate(new Vector3(moveX, 0, moveZ) * Time.deltaTime * speedSwim);
                t.Translate(new Vector3(0, moveY, 0) * Time.deltaTime * speedSwim, Space.World);
            }
        } 
    }
}
