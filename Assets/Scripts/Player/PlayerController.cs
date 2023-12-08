using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    //reference the transform
    Transform t;

    public static PlayerController Instance { get; private set; }

    InventorySystem iSystem;
    CraftingManager cm;

    public static bool inWater;
    public static bool isSwimming;
    public bool isCrafting;
    //if not in water, walk
    //if in water and not swimming, float
    //if in water and swimming, swim

    public LayerMask waterMask;

    [Header("Player Rotation")]
    public float sensitivity = 1;

    //clamp variables
    public float rotationMin;
    public float rotationMax;

    //mouse input variables
    float rotationX;
    float rotationY;

    [Header("Player Movement")]
    public float speed = 1;
    float moveX;
    float moveY;
    float moveZ;

    Rigidbody rb;

    [Header("Player Interactiion")]
    public GameObject cam;
    public Transform dropItemPoint;
    public float playerReach;
    public InteractableObject currentHoverObject;

    //hotbar
    public List<KeyCode> hotbarKeys;
    int itemHeld = -1;
    List<HotbarSlot> hotbar;
    public Transform hand;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        rb = GetComponent<Rigidbody>();
        t = this.transform;

        Cursor.lockState = CursorLockMode.Locked;

        inWater = false;
        isCrafting = false;

        iSystem = InventorySystem.Instance;
        cm = CraftingManager.Instance;

        iSystem.dropItemPoint = dropItemPoint;
        InteractableObject.pc = Instance;

        hotbarKeys = iSystem.hotbarKeys;
        hotbar = iSystem.hotbar;
        HotbarSlot.hand = hand;
    }

    private void FixedUpdate()
    {
        if (!UIBaseClass.menuOpen)
        {
            SwimmingOrFloating();
            Move();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Crafting")
        {
            cm.SetTable(currentHoverObject);
            cm.ToggleMenu();

            isCrafting = true;
        }

        SwitchMovement();
    }

    private void OnTriggerExit(Collider other)
    {
        SwitchMovement();
    }

    void SwitchMovement()
    {
        //toggle inWater
        inWater = !inWater;

        //change the rigidbody accordingly.
        rb.useGravity = !rb.useGravity;
    }

    void SwimmingOrFloating()
    {
        bool swimCheck = false;

        if (inWater)
        {
            RaycastHit hit;
            if(Physics.Raycast(new Vector3(t.position.x,t.position.y + 0.5f,t.position.z),Vector3.down,out hit, Mathf.Infinity, waterMask))
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
        //Debug.Log("isSwiming = " + isSwimming);
    }

    // Update is called once per frame
    void Update()
    {
        //debug function to unlock cursor
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if(UIBaseClass.menuOpen)
        {
            return;
        }

        LookAround();

        currentHoverObject = HoverObject();

        if (currentHoverObject != null)
        {
            //currentHoverObject.Highlight();

            if (/*currentHoverObject.interactable && */ Input.GetMouseButtonDown(0))
            {
                if(currentHoverObject.tag == "Crafting")
                {
                    cm.SetTable(currentHoverObject);
                    cm.ToggleMenu();
                    return;
                }
                else
                {
                    iSystem.PickUpItem(currentHoverObject);
                }
            }
        }

        for (int key = 0; key < 5; key++)
        {
            PointerEventData hoveredObj = ExtendedStandaloneInputModule.GetPointerEventData();
            
            if (Input.GetKeyDown(hotbarKeys[key]))
            {
                HoldItem(key);
                break;
            }
        }
    }

    void LookAround()
    {
        //get the mous input
        rotationX += Input.GetAxis("Mouse X")*sensitivity;
        rotationY += Input.GetAxis("Mouse Y")*sensitivity;

        //clamp the y rotation
        rotationY = Mathf.Clamp(rotationY, rotationMin, rotationMax);

        //setting the rotation value every update
        t.localRotation = Quaternion.Euler(-rotationY, rotationX, 0);
    }

    void Move()
    {
        //get the movement input
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Forward");

        //check if the player is in water
        if (inWater)
        {
            rb.velocity = new Vector2(0,0);
        }
        else
        {
            //check if the player is standing still
            if(moveX == 0 && moveZ == 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        if (!inWater)
        {
            //move the character (land ver)
            t.Translate(new Quaternion(0, t.rotation.y, 0, t.rotation.w) * new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed, Space.World);
        }
        else
        {
            //check if the player is swimming under water or floating along the top
            if (!isSwimming)
            {
                //move the player (floating ver)
                //clamp the moveY value, so they cannot use space or shift to move up
                moveY = Mathf.Min(moveY, 0);

                //conver the local direction vector into a worldspace vector/ 
                Vector3 clampedDirection = t.TransformDirection(new Vector3(moveX, moveY, moveZ));

                //clamp the values of this worldspace vector
                clampedDirection = new Vector3(clampedDirection.x, Mathf.Min(clampedDirection.y, 0), clampedDirection.z);

                t.Translate(clampedDirection * Time.deltaTime * speed, Space.World);
            }
            else
            {
                //move the character (swimming ver)
                t.Translate(new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed);
                t.Translate(new Vector3(0, moveY, 0) * Time.deltaTime * speed, Space.World);
            }
        }

    }

    #region Interaction Functions

    InteractableObject HoverObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, playerReach))
        {
            return hit.collider.gameObject.GetComponent<InteractableObject>();
        }

        return null;
    }

    public void ExitFromCraft()
    {
        cm.ToggleMenu();
    }

    void HoldItem(int slotNum)
    {
        if(itemHeld == slotNum)
        {
            Debug.Log("Hide");
            itemHeld = -1;
            hotbar[slotNum].HideItem();
            return;
        }

        if(itemHeld != -1)
        {
            hotbar[itemHeld].HideItem();
        }

        hotbar[slotNum].DisplayItem();
        itemHeld = slotNum;
    }

    public void HideHotbaritem()
    {
        if(itemHeld != -1)
        {
            hotbar[itemHeld].HideItem();
            itemHeld = -1;
        }
    }
    #endregion
}
