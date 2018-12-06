using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class defining a single Portal object. 
/// </summary>
public class Portal : MonoBehaviour {

    /// <summary>
    /// The main camera in the scene, that the player sees rendered to their screen.
    /// The position of this relative to Source is used to render the texture.
    /// </summary>
    [SerializeField]
    private Camera MainCamera;

    /// <summary>
    /// The Portal we're connected to.
    /// </summary>
    [SerializeField]
    private Portal Destination;

    private static readonly Quaternion _mirror = Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(180, Vector3.left);

    /// <summary>
    /// Should we normalize the transformation? If false, we use the attached
    /// object's Transform as-is. If true, we pretend the attached object has
    /// an identity Transform instead of a rotated one.
    /// </summary>
    [SerializeField]
    private bool ShouldRotate = false;

    private GameObject childObject;

    // A camera we create for the render texture.
    private Camera renderCamera;
    private RenderTexture renderTexture;
    private Material renderMaterial;
    private static Shader portalShader;

    private bool shouldTeleport = true;

	// Use this for initialization
	void Start () {
        // Get player camera (TODO: use the GameManager to assign this)
        if (MainCamera == null) {
            MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        // Find the portal shader, if it's not already set
        if (portalShader == null)
        {
            portalShader = Shader.Find("Custom/Perspective");
        }

        childObject = new GameObject();
        //childObject.transform.parent = this.transform;
        childObject.name = "Camera looking at " + Destination.name;

        // Set up our custom camera/renderTexture/material
        renderCamera = childObject.AddComponent<Camera>();

        // Don't render portals (or else things don't look great!)
        renderCamera.cullingMask &= ~LayerMask.GetMask("Portal");


        // TODO make this size adaptive based on screen size + player distance from this portal
        // (can also completely stop rendering if player faces away from the object, i.e. the angle from
        // MainCamera.transform.forward to (transform.position - player.position) > 180)
        renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
        renderTexture.antiAliasing = 8;
        
        renderCamera.forceIntoRenderTexture = true;
        renderCamera.targetTexture = renderTexture;

        renderMaterial = new Material(portalShader);
        renderMaterial.mainTexture = renderTexture;

        // Assign this object's material to be the render material.
        gameObject.GetComponent<Renderer>().material = renderMaterial;

        // Update the render camera to always have same FOV + aspect ratio as the 
        // player's camera. Otherwise we'll get weird discrepencies that are immediately
        // visible.
        UpdateRenderCamera();
    }

	// Update is called once per frame
	void Update () {
        UpdateRenderCamera();

        Vector3 cameraPositionInSourceSpace;
        Quaternion cameraRotationInSourceSpace;

        cameraPositionInSourceSpace = transform.InverseTransformPoint(MainCamera.transform.position);
        cameraRotationInSourceSpace = Quaternion.Inverse(transform.rotation) * MainCamera.transform.rotation;

        //cameraPositionInSourceSpace = new Vector3(cameraPositionInSourceSpace.x, -cameraPositionInSourceSpace.y, cameraPositionInSourceSpace.z);

        // Pretend for a moment the portal is facing the opposite direction
        Destination.transform.Rotate(Vector3.forward, 180);

        renderCamera.transform.position = Destination.transform.TransformPoint(cameraPositionInSourceSpace);
        renderCamera.transform.rotation = Destination.transform.rotation * cameraRotationInSourceSpace;

        // Make sure we don't actually keep it that way, though
        Destination.transform.Rotate(Vector3.forward, -180);

    }

    private void OnTriggerEnter(Collider other)
    {
        // Technically this should be a property of the colliding object; after being
        // teleported, they shouldn't teleport again until they leave a portal zone.
        // This is a shortcut allowing only the player to teleport. If many objects should
        // also teleport, then make this a per-object property (Teleportable?).
        if (!shouldTeleport)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit)) {
            //Debug.Log("Contact point: " + hit.point);
            //Vector3 localPos = transform.InverseTransformPoint(hit.point);
            //Debug.Log("Local contact: " + localPos);

            Vector3 closestPos = other.ClosestPoint(transform.position);
            Vector3 flipped = new Vector3(closestPos.x, -closestPos.y, -closestPos.z);


            Vector3 dest = Destination.transform.position;
            float destY = dest.y;
            //dest = new Vector3(dest.x, 0, dest.z);
            Vector3 src = transform.position;
            float srcY = src.y;

            Debug.Log("Moved by " + (dest - src));
            //other.transform.position = Destination.transform.InverseTransformPoint(closestPos);
            other.transform.position += dest - src;//dest - (flipped - src);
            //other.transform.rotation *= (Destination.transform.rotation * Quaternion.Inverse(transform.rotation));

            Quaternion offset = Quaternion.AngleAxis(180, Vector3.up) * Destination.transform.rotation * Quaternion.Inverse(transform.rotation);

            if (ShouldRotate)
            {
                Debug.Log("Rotating by " + offset.eulerAngles);
                //MainCamera.transform.rotation = MainCamera.transform.rotation * (transform.rotation * Quaternion.Inverse(Destination.transform.rotation));
                //MainCamera.transform.rotation *= transform.rotation * Quaternion.Inverse(Destination.transform.rotation);
                MainCamera.transform.rotation = offset * MainCamera.transform.rotation;
            }

            Destination.shouldTeleport = false;
            this.shouldTeleport = true;

            //Debug.Log(other.ClosestPointOnBounds(transform.position));
            //Debug.Log(other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position));
        }


        //other.transform.position = Destination.transform.position - new Vector3(0f, 4, 0);
        //shouldTeleport = true;
    }

    private void OnTriggerExit(Collider other)
    {
        this.shouldTeleport = true;
    }

    // Ensures that the settings of our custom rendering camera match the main 
    // camera in all the right ways- aspect + FOV. Other settings might go here later.
    private void UpdateRenderCamera()
    {
        renderCamera.aspect = MainCamera.aspect;
        renderCamera.fieldOfView = MainCamera.fieldOfView;
    }
}
