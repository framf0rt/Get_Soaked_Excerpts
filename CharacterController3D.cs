using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
//[NetworkSettings(channel = 0, sendInterval = 0.01f)]
public class CharacterController3D : MonoBehaviour
{
    private CharacterInput input;
    // Use this for initialization
    private Rigidbody pRBody;
    private float moveFB, moveLR;
    public float moveUD;
    public float moveSpeed = 8f;
    public float jumpSpeed = 25f;
    public bool grounded;
    public LayerMask collisionLayer;
    public LayerMask collisionTeam1;
    public LayerMask collisionTeam2;
    public float gravity = 10f;
    float disableTimer;
    private RaycastHit hitInfoGround;
    private RaycastHit hitInfoHorizontal;
    public float clampValue = 1.05f;
    private CapsuleCollider capCol;
    private float height;
    private float raySpacing = 0.1f;
    public float rotationSpeed = 4f;
    private delegate void StatePointer(float vert, float hori);
    private PlayerState state;
    private StatePointer sp;
    private float V0Air;
    private float timeInAir;
    private float pickUpCd;
    public byte animStateChanged;
    public byte animGrounded;
    public byte animJump;
    private bool doNextFrame;
    private float padJumpHeight;
    private bool respawning = false;
    public Vector3 movement;
    private float vertical, horizontal;
    public LayerMask stopMovement;


    private Vector3 updateVector;
    public float syncInterval = 0.5f;
    private float syncTimer;

    private Footsteps footsteps;

    public NetworkClient nClient;

    public Text latencyText;


    private float _timertest;

    private enum PlayerState
    {
        GroundState, AirState
    }
    public void SetupLayer(byte layer) {
        if(layer == 0)
        {
            collisionLayer = collisionTeam1;
        }else
        {
            collisionLayer = collisionTeam2;
        }

    }

   



    void Start()
    {

        input = GetComponent<CharacterInput>();
        pRBody = GetComponent<Rigidbody>();
       
        capCol = GetComponent<CapsuleCollider>();
        height = capCol.height - 2 * capCol.radius;
        footsteps = GetComponent<Footsteps>();
        //latencyText.gameObject.SetActive(true);
        //nClient = GameObject.Find("Network Manager").GetComponent<NetworkManager>().client;
        //Debug.Log("Changing to ground state");
        sp = new StatePointer(MovePlayerPositionGround);
        animStateChanged = 1;
        animGrounded = 1;
        animJump = 0;
        //Debug.Log("Blir grounded");

    }

    // Update is called once per frame
    void Update()
    {
        if (doNextFrame)
        {
            moveUD = padJumpHeight;
            state = PlayerState.AirState;
            V0Air = padJumpHeight;
            timeInAir = 0f;
            sp = new StatePointer(MovePlayerPositionAir);
            animStateChanged = 1;
            animGrounded = 0;
            animJump = 1;
            disableTimer = 0f;
            doNextFrame = false;
        }
        
        pickUpCd += Time.deltaTime;


        //if (!isLocalPlayer)
        //{

        //    return;
        //}
        float mouseX = input.getMouseX();

        RotatePlayer(mouseX);

        if (state==PlayerState.GroundState && input.getJumped())
        {
            Jump();
        }
        //DisableTimer gör så att man inte automatiskt blir grounded direkt efter ett hopp


    }

    public void OnTriggerStay(Collider other)
    {

        if (Input.GetAxisRaw("Pickup Weapon") != 0&& pickUpCd>1f&&!respawning)
        {
            //Debug.Log("Pressing F");
            pickUpCd = 0f;
        
            if (other.CompareTag("Dropped Weapon"))
            {
                other.GetComponent<DroppedWeapon>().PickUpWeapon(GetComponent<NetworkIdentity>().netId);
            }
            
        }
    }
    private void Jump()
    {
        if (Time.timeScale != 0)
        {

           
            moveUD = jumpSpeed;
            //grounded = false;
            state = PlayerState.AirState;
            V0Air = jumpSpeed;
            timeInAir = 0f;
            //Debug.Log("Changing to air state");
            sp = new StatePointer(MovePlayerPositionAir);
            animStateChanged = 1;
            animGrounded = 0;
            animJump = 1;
            //Debug.Log("Blir air state");
            //Debug.Log("Hoppar");
            disableTimer = 0f;

        }
    }
    public void PadJump(float h)
    {
        doNextFrame = true;
        padJumpHeight = h;
       
    }

    void FixedUpdate()
    {
        //Debug.Log(transform.InverseTransformDirection(pRBody.velocity));
        disableTimer += Time.deltaTime;
        if (disableTimer > 0.2f)
        {
           
            Grounded();
        }

        vertical = input.getVertical();
        horizontal = input.getHorizontal();


       
        sp(vertical, horizontal);
        
        
       


    }

    public void SetRespawning(bool r)
    {
        respawning = r;
    }
    private int checkHorizontalMovement(Vector3 movement)
    {
     
        float horizontalVel = new Vector2(movement.x, movement.z).magnitude;

        for (float j = 0f; j < height + 0.1f; j += raySpacing)
        {
            for (int z = -50; z <= 50; z += 10)
            {
                //Vector3 horizontalDirection = new Vector3(pRBody.velocity.x, 0f, pRBody.velocity.z);
                Vector3 horizontalDirection = new Vector3(movement.x, 0f, movement.z);
                Vector3 currentRayPos = transform.position + Vector3.up * 0.375f - Vector3.up * j;
                //Debug.DrawLine(currentRayPos, Quaternion.Euler(0, y, 0) * horizontalDirection + currentRayPos, Color.red, Time.fixedDeltaTime, false);
                //Vector3 currentRayPos = transform.position + Vector3.up * 0.5f - Vector3.up * i;
                if (Physics.Raycast(currentRayPos, Quaternion.Euler(0, z, 0) * horizontalDirection, out hitInfoHorizontal, 0.375f * 1.2f + horizontalDirection.magnitude * Time.fixedDeltaTime, stopMovement))
                {

                    return 0;
                }
            }
        }
        for (float i = 0f; i < height + 0.1f; i += raySpacing)
        {
            for (int y = -50; y <= 50; y += 10)
            {
                //Vector3 horizontalDirection = new Vector3(pRBody.velocity.x, 0f, pRBody.velocity.z);
                Vector3 horizontalDirection = new Vector3(movement.x, 0f, movement.z);
                Vector3 currentRayPos = transform.position + Vector3.up * 0.375f - Vector3.up * i;
                //Debug.DrawLine(currentRayPos, Quaternion.Euler(0, y, 0) * horizontalDirection + currentRayPos, Color.red, Time.fixedDeltaTime, false);
                //Vector3 currentRayPos = transform.position + Vector3.up * 0.5f - Vector3.up * i;
                if (Physics.Raycast(currentRayPos, Quaternion.Euler(0, y, 0) * horizontalDirection, out hitInfoHorizontal, 0.375f*1.2f + horizontalDirection.magnitude * Time.fixedDeltaTime, collisionLayer))
                {
           
                    return 1;
                }
            }
        }
        return 2;
    }

    private void RotatePlayer(float mouseX)
    {

        Quaternion targetRot = Quaternion.Euler(0f, mouseX, 0f);
        //pRBody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime));
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRot;
    }


    private bool Grounded()
    {
        float multiplier = 1f;
        if (state == PlayerState.GroundState)
        {
            multiplier = 1.2f;
        }
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfoGround, multiplier*clampValue + 0.01f + ((pRBody.velocity.y < 0f) ? Mathf.Abs(pRBody.velocity.y * Time.fixedDeltaTime) : 0f)))
        {

            Vector3 colliderInfo = hitInfoGround.point;

        
            Vector3 tempPosition = transform.position;
            tempPosition.y = hitInfoGround.point.y + clampValue;
            //if (state == PlayerState.AirState)
            //{
                transform.position = tempPosition;
            //}else
            //{
            //    if (Vector3.Distance(hitInfoGround.point, transform.position)>clampValue+0.02f)
            //    {
            //        transform.position = transform.position + Vector3.down * 0.01f;
            //    }
            //}
            
            //transform.position = Vector3.Lerp(transform.position,tempPosition, 5f*Time.deltaTime);
            if (state != PlayerState.GroundState)
            {
                disableTimer = 0f;
                state = PlayerState.GroundState;
                //Debug.Log("Changing to groundstate");
                sp = new StatePointer(MovePlayerPositionGround);
                animStateChanged = 1;
                animGrounded = 1;
                animJump = 0;
                //Debug.Log("Blir grounded");
            }
            
            return true;
        }
        if (state != PlayerState.AirState)
        {
            state = PlayerState.AirState;
            timeInAir = 0f;
            V0Air = 0f;
            //Debug.Log("Changing to airstate");
            sp = new StatePointer(MovePlayerPositionAir);
            animStateChanged = 1;
            animGrounded = 0;
            animJump = 0;
            //Debug.Log("blir air state");
        }
        
        return false;
    }



    private void MovePlayerPositionAir(float vertical, float horizontal)
    {
 
        timeInAir += Time.deltaTime;
        moveUD = V0Air - (gravity * timeInAir);
        if (moveUD > 0f)
        {
            if(Physics.Raycast(transform.position, Vector3.up, out hitInfoGround, 1.1f * clampValue + 0.01f + moveUD * Time.fixedDeltaTime)){
                moveUD = 0f;
                V0Air = 0f;
            }
        }
        SetMovement();
        SetMovementCollision();
        //if (respawning)
        //{
        //    pRBody.MovePosition(transform.position + Vector3.zero * Time.fixedDeltaTime);
        //}else
        //{
        pRBody.MovePosition(transform.position + movement * Time.fixedDeltaTime);
        //}
        //Debug.DrawRay(transform.position, Vector3.up, Color.green,10f );
        
    }

    private void MovePlayerPositionGround(float vertical, float horizontal)
    {
        moveUD = 0f;
        SetMovement();
        if (Vector3.Distance(transform.position, hitInfoGround.point) < clampValue)
        {
            transform.position += (Vector3.up * 0.01f);
        }
        movement = Vector3.ProjectOnPlane(movement, hitInfoGround.normal);
        movement = movement.normalized * moveSpeed;
        SetMovementCollision();
        pRBody.MovePosition(transform.position + movement * Time.fixedDeltaTime);
        if (movement.x > 0 || movement.z > 0 || movement.x < 0 || movement.z < 0)
        {

            footsteps.GeneralFootStep();
        }

    }

    private void SetMovement()
    {
        moveFB = vertical;
        moveLR = horizontal;
      
        movement = new Vector3(moveLR, 0f, moveFB);
        movement = movement.normalized * moveSpeed;
        movement.y = moveUD;

        movement = pRBody.transform.rotation * movement;
    }

    private void SetMovementCollision()
    {
        int collCheck = checkHorizontalMovement(movement);
        if (collCheck == 1)
        {
            //Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 0.5f * transform.localScale.x, stopMovement);
            //if (hitColliders.Length > 1)
            //{
            //    //Debug.Log("Sätter velocity till 0");
            //    movement.x = 0f;
            //    movement.z = 0f;
            //}
            //else
            //{


            movement = Vector3.ProjectOnPlane(movement, hitInfoHorizontal.normal);

        }
        else if (collCheck == 0)
        {
            movement.x = 0f;
            movement.z = 0f;
        }

        //Collider[] hitColliders = Physics.OverlapSphere(this.transform.position+movement*Time.deltaTime, 0.35f * transform.localScale.x, stopMovement);
        //if (hitColliders.Length > 0)
        //{
        //    movement.x = 0f;
        //    movement.z = 0f;
        //}
    }
}
