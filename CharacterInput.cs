using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterInput : MonoBehaviour
{

    private float mouseX;
    private float vertical;
    private float horizontal;
    private float mouseY;
    private bool jump;
    private bool mouse0;
    private bool mouse1;
    private bool reloadKey;
    private bool grenadeKey;
    private bool weapon1Key;
    private bool weapon2Key;
    private bool isMouseLocked;
    private bool showScore;
    [HideInInspector]
    public float leftRightVelocity;
    [HideInInspector]
    public float upDownVelocity;
    private bool lockMouse = false;

    public float sensitivityUpDown = 1f;
    public float sensitivityLeftRight = 1f;
    public bool invertMouse = false;

    private bool paused = false;
    private bool lockedMovement = false;

    public CursorLockMode cursorWantedLock;



    // Use this for initialization
    void Start()
    {
        //Cursor.lockState = cursorWantedLock;

    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }


    public void OnApplicationFocus(bool focus)
    {
        if (focus == true && paused != true)
        {

            LockMouse();
        }

    }


    public void Pause()
    {
        paused = true;
    }

    public void Unpause()
    {
        paused = false;
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F12))
        {
            GetComponent<Health>().CmdRespawn();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
        }


        if (paused == false)
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                showScore = true;
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                showScore = false;
            }
            if (!lockMouse)
            {
                
                leftRightVelocity = Input.GetAxisRaw("Mouse X") * sensitivityLeftRight / Time.deltaTime;
                mouseX += Input.GetAxisRaw("Mouse X") * sensitivityLeftRight;
                if (!invertMouse)
                {
                    upDownVelocity = Input.GetAxisRaw("Mouse Y") * sensitivityUpDown / Time.deltaTime;
                    mouseY += Input.GetAxisRaw("Mouse Y") * sensitivityUpDown;
                }else
                {
                    upDownVelocity = Input.GetAxisRaw("Mouse Y") * sensitivityUpDown / Time.deltaTime*-1f;
                    mouseY += Input.GetAxisRaw("Mouse Y") * sensitivityUpDown*-1f;
                }
                
            }

            //Debug.Log(lockedMovement);
            if (lockedMovement == false)
            {

                isMouseLocked = true;
                LockMouse();
               
                vertical = Input.GetAxisRaw("Vertical");
                horizontal = Input.GetAxisRaw("Horizontal");
                
                //Debug.Log("mousex " + mouseX + " mouseY " + mouseY);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jump = true;
                }
                else
                {
                    jump = false;
                }

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    mouse0 = true;
                    mouse1 = false;
                }
                else if (Input.GetKey(KeyCode.Mouse1))
                {
                    mouse1 = true;
                    mouse0 = false;
                }
                else
                {
                    mouse1 = false;
                    mouse0 = false;
                }
                if (Input.GetKey(KeyCode.R))
                {
                    reloadKey = true;
                }
                else
                {
                    reloadKey = false;
                }

                if (Input.GetKey(KeyCode.G)||Input.GetKey(KeyCode.Q))
                {
                    grenadeKey = true;
                }
                else
                {
                    grenadeKey = false;
                }

                if (Input.GetKey(KeyCode.Alpha1))
                {
                    weapon1Key = true;
                }
                else
                {
                    weapon1Key = false;
                }

                if (Input.GetKey(KeyCode.Alpha2))
                {
                    weapon2Key = true;
                }
                else
                {
                    weapon2Key = false;
                }
            }

        }
        else
        {
            UnlockMouse();
            isMouseLocked = false;



            horizontal = 0f;
            vertical = 0f;
            jump = false;
            mouse0 = false;
            mouse1 = false;
            reloadKey = false;
            grenadeKey = false;
            weapon1Key = false;
            weapon2Key = false;


        }



    }

    public bool GetWeapon1Key()
    {
        return weapon1Key;
    }
    public bool GetWeapon2Key()
    {
        return weapon2Key;
    }

    public bool GetGrenadeKey()
    {
        return grenadeKey;
    }

    public bool GetReloadKey()
    {
        return reloadKey;
    }

    public bool GetMouse0()
    {
        return mouse0;
    }

    public bool GetMouse1()
    {
        return mouse1;
    }

    public void setMouseY(float mouseY)
    {
        this.mouseY = mouseY;
    }

    public bool getJumped()
    {
        return jump;
    }

    public float getMouseY()
    {
        return mouseY;
    }
    public void ResetVertical()
    {
        vertical = 0f;
    
    } 

   public void ResetHorizontal()
    {
        horizontal = 0f;
    }

    public void resetInput()
    {
        horizontal = 0f;
        vertical = 0f;
        grenadeKey = false;
        weapon1Key = false;
        mouse0 = false;
        jump = false;
    }
    public float getVertical()
    {
        return vertical;
    }
    public float getHorizontal()
    {
        return horizontal;
    }

    public float getMouseX()
    {
        return mouseX;
    }


    public bool Paused
    {
        get
        {
            return paused;
        }

        set
        {
            paused = value;
        }
    }

    public float MouseX
    {
        get
        {
            return mouseX;
        }

        set
        {
            mouseX = value;
        }
    }

    public float MouseY
    {
        get
        {
            return mouseY;
        }

        set
        {
            mouseY = value;
        }
    }

    public bool ShowScore
    {
        get
        {
            return showScore;
        }

        set
        {
            showScore = value;
        }
    }

    public bool LockedRotation
    {
        get
        {
            return lockMouse;
        }

        set
        {
            lockMouse = value;
        }
    }

    public bool LockedMovement
    {
        get
        {
            return lockedMovement;
        }

        set
        {
            lockedMovement = value;
        }
    }
}
