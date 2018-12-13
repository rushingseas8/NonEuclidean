using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour {

    public float MovementSpeed = 1f;

    public float ZoomSpeed = 2f;

    public float MinZoom = 1f;
    public float MaxZoom = 3f;

    // The camera we're going to use for the minimap.
    // We create this on the fly with our own set of settings.
    private Camera graphCamera;

    // TODO make a rendertexture dynamically
    // TODO add GUI manager to connect camera -> rendertexture -> UI.

    private float linearZoom = 1f;
    private float actualZoomFactor;

	// Use this for initialization
	void Start () 
    {
        // Create the camera
        GameObject cameraObj = new GameObject();
        cameraObj.transform.position = Vector3.back;
        cameraObj.name = "Minimap Viewing Camera";
        graphCamera = cameraObj.AddComponent<Camera>();

        // Set it up
        graphCamera.orthographic = true;
        graphCamera.orthographicSize = 2f;
        graphCamera.cullingMask = 1 << LayerMask.NameToLayer("Graph");
        graphCamera.clearFlags = CameraClearFlags.Color;
        graphCamera.backgroundColor = Color.black;
    }
	
	void Update () 
    {
        if (Input.GetKey(KeyCode.Equals)) {
            Debug.Log("Plus pressed");
            linearZoom -= ZoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Minus))
        {
            Debug.Log("Minus pressed");
            linearZoom += ZoomSpeed * Time.deltaTime;
        }

        linearZoom = Mathf.Clamp(linearZoom, MinZoom, MaxZoom);

        actualZoomFactor = linearZoom * linearZoom;
        graphCamera.orthographicSize = actualZoomFactor;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movementVector = Vector2.right * horizontal + Vector2.up * vertical;
        graphCamera.transform.Translate(MovementSpeed * linearZoom * movementVector * Time.deltaTime);

	}
}
