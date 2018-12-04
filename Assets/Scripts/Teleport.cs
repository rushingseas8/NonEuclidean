using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private Transform targetPoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.x > startPoint.position.x && transform.position.x < targetPoint.position.x) {
            transform.position = targetPoint.position;
        }
	}
}
