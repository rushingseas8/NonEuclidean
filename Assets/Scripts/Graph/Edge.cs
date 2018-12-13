using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Edge : MonoBehaviour {

    private Rigidbody2D body;

    public Vertex Begin { get; set; }
    public Vertex End { get; set; }
	
    public bool Sleeping { get; set; }

    void Start() {
        this.body = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
	void Update () {
        transform.position = (Begin.transform.position + End.transform.position) / 2f;
        transform.localScale = new Vector3(0.1f, 7 * (End.transform.position - Begin.transform.position).magnitude, 1f);
        transform.rotation = Quaternion.FromToRotation(Vector3.up, End.transform.position - Begin.transform.position);
	}

    public void AddImpulse(Vector2 force)
    {
        if (Sleeping)
        {
            return;
        }

        //body.AddForce(force, ForceMode2D.Impulse);
        Begin.AddImpulse(force);
        End.AddImpulse(force);
    }
}
