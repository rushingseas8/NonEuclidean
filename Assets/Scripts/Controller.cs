using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A basic first person player controller.
/// Allows for flight mode and walking around on a surface.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour {

    [SerializeField]
    private bool flyingMode = true;

    [SerializeField]
    private float movementScale = 3f;

    [SerializeField]
    private float rotationScale = 5f;

    [SerializeField]
    private float jumpHeight = 2f;

    public Camera mainCamera;

    private Rigidbody body;

    private float thirdPersonDistance = 0;

    private readonly Vector3 cameraOffset = Vector3.up;

    private bool onGround = false;
    private bool jumping = false;

    private float lastGravity = float.NaN;
    private bool updatedGravity = false;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
        //forward    = new KeyCode[]{ KeyCode.W, KeyCode.UpArrow };
        //backward = new KeyCode[]{ KeyCode.S, KeyCode.DownArrow };
        //left = new KeyCode[]{ KeyCode.A, KeyCode.LeftArrow };
        //right = new KeyCode[]{ KeyCode.D, KeyCode.RightArrow };
        //up = new KeyCode[]{ KeyCode.LeftShift, KeyCode.Space };
        //down = new KeyCode[]{ KeyCode.LeftControl, KeyCode.LeftAlt };

        //mainCamera = Camera.main;

        //Cursor.lockState = CursorLockMode.Locked;

        /*
        if (flyingMode)
            movementScale = 2.5f;
        else
            movementScale = 0.2f;*/
    }

    bool keycodePressed(KeyCode[] arr) {
        for(int i = 0; i < arr.Length; i++) {
            if(Input.GetKey(arr[i])) {
                return true;
            }
        }
        return false;
    }

    bool keycodeDown(KeyCode[] arr) {
        for(int i = 0; i < arr.Length; i++) {
            if(Input.GetKeyDown(arr[i])) {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        onGround = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        onGround = false;
    }

    // Update is called once per frame
    void Update () {
        Vector3 oldPosition = this.gameObject.transform.position;
        Quaternion oldRotation = mainCamera.transform.rotation;

        Vector3 newPosition = oldPosition;
        Quaternion newRotation = oldRotation;

        if (Input.GetKeyDown (KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
        }

        float planeRotation = mainCamera.transform.rotation.eulerAngles.y;
        Quaternion quat = Quaternion.Euler (new Vector3 (0, planeRotation, 0));

        Quaternion ang = Quaternion.identity;

        //onGround = false;

        /*
         * Raycast down and if we hit terrain, ensure movement is constrained to be normal to
         * the terrain. This ensures we only move on the ground, instead of clipping in/floating.
         */
        RaycastHit hit;
        if (Physics.Raycast (this.gameObject.transform.position, -gameObject.transform.up, out hit))
        {
            if (hit.distance <= 1.5f)
            {
                //onGround = true;

                Vector3 fbn = new Vector3(0, hit.normal.y, hit.normal.z);
                float xAngle = 90 - Vector3.Angle(fbn, Vector3.forward);

                Vector3 lrn = new Vector3(hit.normal.x, hit.normal.y, 0);
                float zAngle = 90 - Vector3.Angle(lrn, Vector3.left);

                ang = Quaternion.Euler(xAngle, 0, zAngle);
            }
            Debug.Log("On ground: " + onGround);
        }

        // Forward/backward motion
        float vertical = Input.GetAxis("Vertical");

        // Left/right motion (strafe)
        float horizontal = Input.GetAxis("Horizontal");

        // Up/down motion (flying or jump/crouch)
        float jump = Input.GetAxis("Jump");

        // Combined motion vector (excluding up/down movement)
        Vector3 motionVector = (vertical * Vector3.forward) + (horizontal * Vector3.right);

        // Grab the y component of the current velocity
        float gravity = body.velocity.y;

        if (flyingMode)
        {
            motionVector += jump * Vector3.up;
        }
        else
        {

            /*
            if (onGround && jumping && gravity < 0.01f && updatedGravity)
            {
                Debug.Log("Stopping jumping.");
                jumping = false;
                updatedGravity = false;
            }

            //Debug.Log("Jump vector magnitude: " + jumpVector.sqrMagnitude);
            if (jump > 0 && onGround && !jumping) {
                body.AddForce(Vector3.up * Mathf.Sqrt(2f * 9.8f * jumpHeight) * body.mass, ForceMode.Impulse);
                Debug.Log("Adding jump force");
                jumping = true;
            }
            */
        }

        // Apply all the velocity changes
        Vector3 newVelocity = ang * quat * motionVector * movementScale;
        newVelocity = new Vector3(newVelocity.x, gravity, newVelocity.z);
        //newVelocity += Physics.gravity * Time.deltaTime;
        body.velocity = newVelocity;


        /*
        if (keycodePressed (forward)) {
            newPosition += (ang * quat * Vector3.forward * movementScale);
        }
        if (keycodePressed (backward)) {
            newPosition += (ang * quat * Vector3.back * movementScale);
        }
        if (keycodePressed (left)) {
            newPosition += (ang * quat * Vector3.left * movementScale);
        }
        if (keycodePressed (right)) {
            newPosition += (ang * quat * Vector3.right * movementScale);
        }

        if (flyingMode) {
            if (keycodePressed (up)) {
                newPosition += (Vector3.up * movementScale);
            }
        } else {
            //Only jump if we're on the ground (or very close)
            if (keycodeDown (up)) {    
                if (onGround) {
                    this.gameObject.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 5, 0);
                }
            }
            //this.gameObject.GetComponent<Rigidbody>().AddForce(-Vector3.down * 9.81f, ForceMode.Acceleration);
        }
            
        if (flyingMode) {
            if (keycodePressed (down)) {
                newPosition += (Vector3.down * movementScale);
            }
        }
        */

        float mouseX = Input.GetAxis ("Mouse X");
        float mouseY = -Input.GetAxis ("Mouse Y");
        if (mouseX != 0 || mouseY != 0) {
            Vector3 rot = oldRotation.eulerAngles;

            float xRot = rot.x;

            float tent = rot.x + (rotationScale * mouseY);

            if (xRot > 270 && tent < 270) {
                xRot = 270;
            } else if (xRot < 90 && tent > 90) {
                xRot = 90;
            } else {
                xRot += rotationScale * mouseY;
            }

            newRotation = Quaternion.Euler (
                new Vector3 (
                    xRot,
                    rot.y + (rotationScale * mouseX),
                    0));
        }

        thirdPersonDistance -= Input.GetAxis ("Mouse ScrollWheel");
        if (thirdPersonDistance < 0)
            thirdPersonDistance = 0;

        this.gameObject.transform.position = newPosition;
        this.gameObject.transform.rotation = ang;

        mainCamera.transform.position = newPosition + (ang * cameraOffset) + (newRotation * new Vector3 (0, 0, -thirdPersonDistance));
        mainCamera.transform.rotation = newRotation;
    }

    // Checking the jump needs to happen in sync with the physics timestep. Otherwise, since
    // Update() is called more often than FixedUpdate(), we can apply a force without seeing
    // a change in velocity the next frame. We'd have to use locks to guide the flow; moving
    // the code to FixedUpdate means we can check fewer things.
    // 
    // This code works, except that moving on non-level surfaces causes movement in the y axis
    // that looks like jumping. TODO have two separate fields for movement along the surface
    // and movement normal to the surface, so we can query just one or the other.
    private void FixedUpdate()
    {
        // Up/down motion (flying or jump/crouch)
        float jump = Input.GetAxis("Jump");

        // Grab the y component of the current velocity
        float gravity = body.velocity.y;

        updatedGravity = false;
        if (!float.IsNaN(lastGravity))
        {
            // If gravity hasn't changed (i.e., physics hasn't updated between frames),
            // then we can't conclude if we've stopped or not.
            if (!Mathf.Approximately(lastGravity, gravity))
            {
                updatedGravity = true;
            }
        }

        lastGravity = gravity;

        bool verticallyStationary = Mathf.Abs(gravity) < 0.1f;

        //if (onGround) {
        //    Debug.Log("1. On ground.");
        //    if (jumping) {
        //        Debug.Log("2. Jumping.");
        //        if (verticallyStationary) {
        //            Debug.Log("3. Vertically stationary.");
        //            if (updatedGravity) {
        //                Debug.Log("4. Gravity has updated since last frame.");

        //                Debug.Log("Good to go- stopping jumping.");
        //                jumping = false;
        //                updatedGravity = false;
        //            }
        //        }
        //    }
        //}

        Debug.Log("Vertical velocity: " + gravity);
        if (onGround && jumping && verticallyStationary && updatedGravity)
        {
            Debug.Log("Stopping jumping.");
            jumping = false;
            updatedGravity = false;
        }

        //Debug.Log("Jump vector magnitude: " + jumpVector.sqrMagnitude);
        if (jump > 0 && onGround && !jumping && verticallyStationary)
        {
            body.AddForce(Vector3.up * Mathf.Sqrt(2f * 9.8f * jumpHeight) * body.mass, ForceMode.Impulse);
            Debug.Log("Adding jump force");
            jumping = true;
            updatedGravity = false;
        }
    }
}
