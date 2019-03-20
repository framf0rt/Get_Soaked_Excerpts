using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraController3D : MonoBehaviour
{
    public float rotationSpeed;
    private float mouseY;
    private CharacterInput input;
    public bool invertMouse;
    public float maxUpRotation, maxDownRotation;
    // Use this for initialization
    public Transform cam;

    private GameObject domPad;
    private Camera fpsCamera;
    private Vector3 targetPos;
    private Vector3 screenMiddle;
    public GameObject UIPointer;
    public Transform realCam;


    //values that will be set in the Inspector
    public Transform Target;
    public float RotationSpeed;

    //values for internal use
    private Quaternion _lookRotation;
    private Vector3 _direction;
    public LayerMask lm;



    void Start()
    {

        //if (!isLocalPlayer)
        //{
        //    return;
        //}
        input = transform.GetComponentInParent<CharacterInput>();

        //cam = transform.GetChild(1).transform;
        maxUpRotation *= -1f;

        domPad = GameObject.FindGameObjectWithTag("Domination Spawn");
      

        UIPointer.SetActive(true);
        fpsCamera = cam.gameObject.GetComponent<Camera>();
        //Get the middle of the screen into a Vector3
        screenMiddle = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isLocalPlayer)
        //{
        //    return;
        //}
        mouseY = input.getMouseY();

        if (invertMouse)
        {
            mouseY *= -1f;
        }
        RotateCamera(mouseY);

        IsVisible();

    }

    private void FixedUpdate()
    {

    }

    private void RotateCamera(float mouseY)
    {

        if (mouseY > maxDownRotation)
        {
            input.setMouseY(-1f * maxDownRotation);
        }
        if (mouseY < maxUpRotation)
        {
            input.setMouseY(-1f * maxUpRotation);
        }



        Quaternion targetRot = Quaternion.Euler(mouseY, 0f, 0f);

        //cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, targetRot, rotationSpeed * Time.deltaTime);
        cam.transform.localRotation = targetRot;

    }
   

    public void IsVisible()
    {
        //Vector3 targetDir = domPad.transform.position - transform.position;
        //float angle = Vector3.Angle(targetDir, transform.forward);
        //Vector3 cross = Vector3.Cross(targetDir, transform.forward);
        //if (cross.y < 0) angle = -angle;
        //Debug.Log(angle);
        if (domPad.GetComponent<Renderer>().IsVisibleFrom(realCam.transform.gameObject.GetComponent<Camera>())) UIPointer.SetActive(false);
        else
        {
            // UIPointer.SetActive(true);

            //Vector3 targetDir = domPad.transform.position - transform.position;
            //float angle = Vector3.Angle(targetDir, transform.forward);
            //Vector3 cross = Vector3.Cross(targetDir, transform.forward);
            //if (cross.y < 0) angle = -angle;
            // UIPointer.transform.rotation = Quaternion.Euler(0, 0, angle);


            // angle = UIPointer.transform.rotation.z;
            //if (angle < 271 && angle > 225)
            // {
            //     UIPointer.transform.localPosition = new Vector3(900, -250, 0);
            //   //  Debug.Log("1");
            // }

            // else if (angle < -44 && angle > -90)
            // {
            //     UIPointer.transform.localPosition = new Vector3(900, 250, 0);
            //    // Debug.Log("2");
            // }

            // else if (angle < 224 && angle > 180)
            // {
            //     UIPointer.transform.localPosition = new Vector3(900, -500, 0);
            //    // Debug.Log("3");
            // }
            // else if (angle < 179 && angle > 135 )
            // {
            //     UIPointer.transform.localPosition = new Vector3(-900, -500, 0);
            //   //  Debug.Log("4");
            // }
            // else if (angle < 135 && angle > 134)
            // {
            //     UIPointer.transform.localPosition = new Vector3(-900, -250, 0);
            //   //  Debug.Log("5");
            // }
            // else if (angle < 90 && angle > 44)
            // {
            //     UIPointer.transform.localPosition = new Vector3(-500, 500, 0);
            //   //  Debug.Log("6");
            // }
            // else if (angle < 44 && angle > 0)
            // {
            //     UIPointer.transform.localPosition = new Vector3(-900, 250, 0);
            //    // Debug.Log("7");
            // }
            // else if (angle < 0 && angle > -44)
            // {
            //   //  Debug.Log("8");
            //     UIPointer.transform.localPosition = new Vector3(500, 500, 0);
            // }


            //UIPointer.transform.position = screenPos;
        }

    }



}
