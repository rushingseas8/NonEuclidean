using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Vertex : MonoBehaviour {

    public Rigidbody2D body;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;

    private Color color = Color.white;

    //private VertexData vertexData;
    public List<Vertex> neighbors = new List<Vertex>();

    public bool Sleeping { get; set; }

	// Use this for initialization
	void Start () {
        this.body = GetComponent<Rigidbody2D>();
        this.circleCollider = GetComponent<CircleCollider2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = this.color;
    }

    // Update is called once per frame
    void Update () {
	}

    public void SetColor(Color color) 
    {
        this.color = color;
    }

    public void AddImpulse(Vector2 force) 
    {
        if (Sleeping) 
        {
            return;
        }

        body.AddForce(force, ForceMode2D.Impulse);
    }
}
