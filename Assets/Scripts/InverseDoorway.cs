using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseDoorway : MonoBehaviour {

    [SerializeField]
    private Controller player;

    [SerializeField]
    private Transform Source;

    [SerializeField]
    private Transform Destination;

    private RenderTexture renderTexture;

    private Camera cam;

    // Use this for initialization
    void Start()
    {
        // We must enforce identical FOV and aspect ratio for the effect to work.
        cam = GetComponent<Camera>();
        cam.aspect = player.mainCamera.aspect;
        cam.fieldOfView = player.mainCamera.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
        //Vector3 playerPos = player.transform.position;
        //Vector3 flippedPlayer = new Vector3(playerPos.x, playerPos.y, playerPos.z);
        //Vector3 drawingPos = drawingDoorway.transform.position;
        //Vector3 offset = playerPos;

        //Vector3 flippedOffset = new Vector3(-offset.x, offset.y, -offset.z);

        //transform.position = connectedDoorway.transform.position + flippedPlayer - drawingPos;
        //transform.position = connectedDoorway.transform.position - drawingDoorway.transform.position + player.transform.position;
        //transform.rotation = player.mainCamera.transform.rotation;


        //Vector3 pos = connectedDoorway.transform.InverseTransformPoint(player.transform.position);
        //Vector3 pos = connectedDoorway.transform.position - player.transform.position;
        //transform.localPosition = new Vector3(-pos.x, pos.y, -pos.z);

        //Vector3 euler = Vector3.zero;
        //euler.y = SignedAngle(connectedDoorway.transform.forward, player.mainCamera.transform.forward, Vector3.up);

        //transform.localRotation = Quaternion.Euler(euler);
        //transform.localRotation = Quaternion.LookRotation(player.mainCamera.transform.forward - connectedDoorway.transform.forward, Vector3.up);

        //Transform Source = drawingDoorway.transform;
        //Transform Destination = connectedDoorway.transform;

        Camera MainCamera = player.mainCamera;

        Vector3 cameraPositionInSourceSpace = Source.InverseTransformPoint(MainCamera.transform.position);
        Quaternion cameraRotationInSourceSpace = Quaternion.Inverse(Source.rotation) * MainCamera.transform.rotation;

        this.transform.position = Destination.TransformPoint(cameraPositionInSourceSpace);
        this.transform.rotation = Destination.rotation * cameraRotationInSourceSpace;

    }

    private float SignedAngle(Vector3 a, Vector3 b, Vector3 n)
    {
        //Code stolen from DiegoSLTS
        //http://answers.unity3d.com/questions/992289/portal-effect-using-render-textures-how-should-i-m.html

        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        while (signed_angle < 0) signed_angle += 360;

        return signed_angle;
    }
}
