using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit //this is code for the bear that we control with player input
{
    // Start is called before the first frame update

    //we need to access the camera, the camera's container
    //we need some speed for movement
    //we need something for sensitivity of movement for x and y directions
    //we need a height for jumping
    private Camera playerCam;
    private Transform camContainer; //for movement of the camera
    [SerializeField] 
    float speed = 5;
    [SerializeField]
    float mouseXSensitivity = 1;
    [SerializeField]
    float mouseYSensitivity = 1;
    public float jumpHeight = 15.0f;

    //animation properties
    private const float ANIMATOR_SMOOTHING = 0.4f;
    private const float DISTANCE_LASER_IF_NO_HIT = 300;
    private Vector3 animatorInput;

    protected override void Start()
    {
        base.Start(); //we want to call the Start class for a Unit
        playerCam = GetComponentInChildren<Camera>(); //this allows us to access the camera in the children
        camContainer = playerCam.transform.parent; //we get access to the CamContainer here

    }

    // Update is called once per frame
    protected override void Update()
    { //this is where the magic happens for movement
        base.Update();//update the parent class

        camContainer.Rotate(Input.GetAxis("Mouse Y") * mouseYSensitivity, 0, 0);
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(horizontal, 0, vertical).normalized * speed; //recall that a normalized vector is one of length 1, i.e. it has no magnitude 
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            input.y = jumpHeight;
            animator.SetTrigger("Jumping");
        }
        else
        {
            input.y = GetComponent<Rigidbody>().velocity.y;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger("Kicking");
        }

        GetComponent<Rigidbody>().velocity = transform.TransformVector(input);
        animatorInput = Vector3.Lerp(animatorInput, input, ANIMATOR_SMOOTHING); //we are creating a new animator input based on the previous state of our movement
        animator.SetFloat("HorizontalSpeed", animatorInput.x);
        animator.SetFloat("VerticalSpeed", animatorInput.z);
        float rotationX = Input.GetAxis("Mouse X") * mouseXSensitivity;
        this.transform.Rotate(0, rotationX, 0);

        //laser shooting code
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            LayerMask mask = ~LayerMask.GetMask("Teddy", "Outpost", "Terrain");
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                if(CanSee(hit.transform, hit.point))
                {
                    Debug.Log("We can see the object and the raycast worked");
                    ShootAt(hit.transform);
                    ShowLasers(hit.point);
                }
                else
                {
                    Debug.Log("We cannot see the object but the raycast worked?");
                }
            }
            else
            {
                Vector3 targetPos = playerCam.transform.position + playerCam.transform.forward * DISTANCE_LASER_IF_NO_HIT;
                ShowLasers(targetPos);
            }
        }
    }
}