using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseDoorway : MonoBehaviour {

    /// <summary>
    /// The main camera in the scene, that the player sees rendered to their screen.
    /// The position of this relative to Source is used to render the texture.
    /// </summary>
    [SerializeField]
    private Camera MainCamera;

    /// <summary>
    /// The source doorway. Where the texture gets rendered.
    /// </summary>
    [SerializeField]
    private Transform Source;

    /// <summary>
    /// The target doorway. Where the source is connected to.
    /// </summary>
    [SerializeField]
    private Transform Destination;

    private RenderTexture renderTexture;

    private Camera cam;

    // Use this for initialization
    void Start()
    {
        // We must enforce identical FOV and aspect ratio for the effect to work.
        cam = GetComponent<Camera>();
        if (cam == null) {
            cam = gameObject.AddComponent<Camera>();
        }
        cam.aspect = MainCamera.aspect;
        cam.fieldOfView = MainCamera.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
        // TODO: make the camera constantly check for changes in main camera FOV/aspect.
        // TODO: change the resolution of renderTexture to always result in at least as much
        // resolution as the player's screen. Based on distance(player, source) and size of
        // player window.

        Vector3 cameraPositionInSourceSpace = Source.InverseTransformPoint(MainCamera.transform.position);
        Quaternion cameraRotationInSourceSpace = Quaternion.Inverse(Source.rotation) * MainCamera.transform.rotation;

        this.transform.position = Destination.TransformPoint(cameraPositionInSourceSpace);
        this.transform.rotation = Destination.rotation * cameraRotationInSourceSpace;

    }
}
