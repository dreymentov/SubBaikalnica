using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    //reference the transform
    Transform t;
    public static PlayerController Instance { get; private set; }

    InventorySystem iSystem;
    //StorageManager sm;
    CraftingManager cm;

    public static bool inWater;
    public static bool isSwimming;
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
    public float baseSpeed = 1;
    float speed;
    public float swimSpeedBonus = 0;
    public float walkSpeedBonus = 0;
    public List<float> SwimBonuses = new List<float>();
    public List<float> WalkBonuses = new List<float>();
    float moveX;
    float moveY;
    float moveZ;

    Rigidbody rb;

    [Header("Player Animation")]
    public Animator playerAnim;

    [Header("Player Interaction")]
    public GameObject cam;
    public Transform dropItemPoint;
    public float playerReach;
    public InteractableObject currentHoverObject;

    //hotbar
    public List<KeyCode> hotbarKeys;
    int itemHeld = -1; // -1 means nothing held. the other positions correspond to hotbar slots.
    List<HotbarSlot> hotbar;
    public Transform hand;

    //crosshair
    public GameObject crosshair;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody>();
        t = this.transform;

        Cursor.lockState = CursorLockMode.Locked;

        inWater = false;

        iSystem = InventorySystem.Instance;
        //sm = StorageManager.Instance;
        cm = CraftingManager.Instance;

        iSystem.dropItemPoint = dropItemPoint;
        InteractableObject.pc = Instance;
        ToolBaseClass.pc = Instance;
        ToolBaseClass.iSystem = iSystem;
        GearUniversalFunctions.pc = Instance;

        hotbarKeys = iSystem.hotbarKeys;
        hotbar = iSystem.hotbar;
        HotbarSlot.hand = hand;

        //playerAnim = GetComponent<Animator>();
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
        SwitchMovement();
    }

    private void OnTriggerExit(Collider other)
    {
        SwitchMovement();
    }

    // Update is called once per frame
    void Update()
    {
        //debug function to unlock cursor
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //stop all functions if a menu is open
        if (UIBaseClass.menuOpen)
        {
            return;
        }

        LookAround();

        UpdateAnim();

        //interacting with items
        currentHoverObject = HoverObject();

        if(currentHoverObject != null)
        {
            //display name

            //outline
            currentHoverObject.Highlight();

            //check if the player left clicks
            if(currentHoverObject.interactable && Input.GetMouseButtonDown(0))
            {
                InteractWithObject();
            }
        }

        DetectHotbarKeypress();
    }

    #region Update Functions

    void InteractWithObject()
    {
        //open storage
        /*if (currentHoverObject.tag == "Storage")
        {
            sm.SetStorage(currentHoverObject);
            sm.ToggleMenu();
            return;
        }
        else*/ if (currentHoverObject.tag == "Crafting")
        {
            cm.SetTable(currentHoverObject);
            cm.ToggleMenu();
            return;
        }
        else if (currentHoverObject.tag == "PickUp")
        {
            iSystem.PickUpItem(currentHoverObject);
        }
    }

    void DetectHotbarKeypress()
    {
        //hotbar keypressing detection
        for (int key = 0; key < 5; key++)
        {
            if (Input.GetKeyDown(hotbarKeys[key]))
            {
                Debug.Log("hotbar " + key);

                //assign item to hotbar
                HoldItem(key);
                break;
            }
        }
    }

    void UpdateAnim()
    {
        playerAnim.SetBool("inWater", inWater);
        playerAnim.SetFloat("moveX", moveX);
        playerAnim.SetFloat("moveZ", moveZ);
    }

    #endregion

    #region Movement Functions

    void LookAround()
    {
        //get the mous input
        rotationX += Input.GetAxis("Mouse X")*sensitivity;
        rotationY += Input.GetAxis("Mouse Y")*sensitivity;

        //clamp the y rotation
        rotationY = Mathf.Clamp(rotationY, rotationMin, rotationMax);

        if (inWater)
        {
            cam.transform.localRotation = Quaternion.identity;

            //setting the rotation value every update
            t.localRotation = Quaternion.Euler(-rotationY, rotationX, 0);
        }
        else
        {
            cam.transform.localRotation = Quaternion.Euler(-rotationY, 0, 0);
            t.localRotation = Quaternion.Euler(0, rotationX, 0);
        }
    }

    void Move()
    {
        CheckSpeedBoost();

        //get the movement input
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Forward");

        //check if the player is in water
        if (inWater)
        {
            rb.velocity = new Vector2(0,0);
            speed = baseSpeed + swimSpeedBonus;
        }
        else
        {
            speed = baseSpeed + walkSpeedBonus;

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
            if (Physics.Raycast(new Vector3(t.position.x, t.position.y + 0.5f, t.position.z), Vector3.down, out hit, Mathf.Infinity, waterMask))
            {
                if (hit.distance < 0.1f)
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

    public void CheckSpeedBoost()
    {
        

        walkSpeedBonus = 0;
        swimSpeedBonus = 0;

        //list of conditions of what gives speed boosts. add conditions here as needed.

        //swimming conditions
        if(itemHeld != -1 && hotbar[itemHeld].tool != null && hotbar[itemHeld].tool.GetType() == typeof(Seaglide))
        {
            Debug.Log("Checking speed");
            swimSpeedBonus += Seaglide.speedBonus;
        }
        foreach(int i in SwimBonuses)
        {
            swimSpeedBonus += i;
        }

        //walking conditions
        foreach (int i in WalkBonuses)
        {
            walkSpeedBonus += i;
        }
    }
    #endregion

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

    void HoldItem(int slotNum)
    {
        //if already holding this item, toggle equip
        if (itemHeld == slotNum)
        {
            itemHeld = -1;
            hotbar[slotNum].HideItem();
            return;
        }

        //disable other held items
        if (itemHeld != -1)
        {
            hotbar[itemHeld].HideItem();
        }

        //display item in hand
        hotbar[slotNum].DisplayItem();
        itemHeld = slotNum;

        Debug.Log("currently held item is a " + hotbar[itemHeld].tool + " tool.");
    }

    public void HideHotbaritem()
    {
        if (itemHeld != -1)
        {
            hotbar[itemHeld].HideItem();
            itemHeld = -1;
        }
    }

    public HotbarSlot CurrentHotbarItem()
    {
        if(itemHeld == -1)
        {
            return null;
        }
        return hotbar[itemHeld];
    }
    #endregion
}
