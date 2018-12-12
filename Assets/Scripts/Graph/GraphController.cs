using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A data structure representing a graph.
/// </summary>
public class GraphController : MonoBehaviour
{
    public List<Vertex> vertices;
    public List<Edge> edges;

    public float IdealVertexDistance = 2f;

    public float IdealEdgeDistance = 3f;

    public float DeadZone = 0.1f;

    [Range(0, 1f)]
    public float Damper = 0.1f;
    private float actualDamper;

    [Tooltip("When the velocity of a vertex is below this value, it will freeze.")]
    public float MovementThreshold = 0.02f;

    private static Vertex vertexPrefab;
    private static Edge edgePrefab;

    public void Awake()
    {
        vertices = new List<Vertex>();
        edges = new List<Edge>();
        if (vertexPrefab == null)
        {
            vertexPrefab = Resources.Load<Vertex>("Prefabs/Graph/Vertex");
        }
        if (edgePrefab == null)
        {
            edgePrefab = Resources.Load<Edge>("Prefabs/Graph/Edge");
        }

        // Choice of x^0.25 is arbitrary; we want to squish all values down,
        // and values close to 0 closer down to make the useful range larger.
        actualDamper = Mathf.Sqrt(Mathf.Sqrt(1 - Damper));


        //Vertex v0 = this.AddVertex(Vector3.zero);
        //Vertex v1 = this.AddVertex(new Vector3(1, 1, 0));
        //Vertex v2 = this.AddVertex(new Vector3(-2, 2, 0));
        //Vertex v3 = this.AddVertex(new Vector3(-2, -2, 0));

        //// 3-cycle
        //this.Connect(v0, v1);
        //this.Connect(v1, v2);
        //this.Connect(v2, v0);

        //// Connect one vertex to two others
        //this.Connect(v3, v0);
        //this.Connect(v3, v1);


    }

    public void Start() {
        for (int i = 0; i < 50; i++)
        {
            if (Random.value < 0.4f)
            {
                AddVertex(new Vector3(Random.value * 10 - 5, Random.value * 10 - 5));
                if (vertices.Count == 1) {
                    vertices[0].Sleeping = true;
                    vertices[0].SetColor(Color.red);
                    Camera.main.transform.position = vertices[0].transform.position + Vector3.back;
                }
            }
            else
            {
                int first = (int)(Random.value * vertices.Count);
                int second = (int)(Random.value * vertices.Count);
                if (first == second)
                {
                    continue;
                }
                Connect(vertices[first], vertices[second]);
            }
        }
    }

    int count = 0;
    public void Update() 
    {
        actualDamper = Mathf.Sqrt(Mathf.Sqrt(1 - Damper));

        for (int i = 0; i < vertices.Count; i++) {
            // Compute attraction to all neighbors
            UpdateAttraction(vertices[i]);

            // Compute repulsion from all vertices
            UpdateRepulsion(vertices[i]);

            // Add some damper so we don't oscillate forever. 
            vertices[i].body.velocity = actualDamper * vertices[i].body.velocity;

            if (vertices[i].body.velocity.magnitude < MovementThreshold) 
            {
                vertices[i].Sleeping = true;
            }
        }

        for (int i = 0; i < edges.Count; i++)
        {
            UpdateRepulsion(edges[i]);
        }

        //count++;
        //if (count % 10 == 0) {
        //    count = 0;

        //    if (Random.value < 0.4f)
        //    {
        //        AddVertex(new Vector3(Random.value * 10 - 5, Random.value * 10 - 5));
        //    }
        //    else
        //    {
        //        int first = (int)(Random.value * vertices.Count);
        //        int second = (int)(Random.value * vertices.Count);
        //        if (first == second)
        //        {
        //            return;
        //        }
        //        Connect(vertices[first], vertices[second]);
        //    }
        //}
    }

    // A vertex is only attracted to its neighbors.
    private void UpdateAttraction(Vertex vertex)
    {
        //Debug.Log("Updating attractive force");
        Vector3 pos = vertex.transform.position;
        for (int i = 0; i < vertex.neighbors.Count; i++) {
            Vertex neighbor = vertex.neighbors[i];

            // Which direction is our neighbor?
            Vector3 towardsVector = neighbor.transform.position - pos;

            // Grab the distance. We use this to determine forces.
            float dSquared = towardsVector.sqrMagnitude;
            float distance = towardsVector.magnitude;

            if (Mathf.Approximately(IdealVertexDistance, 0))
            {
                continue;
            }

            // Too far, come back!
            if (distance > IdealVertexDistance + DeadZone)
            {
                vertex.AddImpulse((dSquared / IdealVertexDistance) * towardsVector.normalized * Time.deltaTime);
            }
        }
    }

    // Go through all vertices and compute repulsive force
    private void UpdateRepulsion(Vertex vertex) 
    {
        Vector3 pos = vertex.transform.position;
        for (int i = 0; i < vertices.Count; i++)
        {
            Vertex neighbor = vertices[i];

            // Which direction is our neighbor?
            Vector3 towardsVector = neighbor.transform.position - pos;

            // Grab the distance. We use this to determine forces.
            float dSquared = towardsVector.sqrMagnitude;
            float distance = towardsVector.magnitude;

            // Don't divide by zero
            if (Mathf.Approximately(distance, 0)) 
            {
                continue;
            }

            // Too close, get away!
            if (distance < IdealVertexDistance - DeadZone)
            {
                vertex.AddImpulse(-(IdealVertexDistance * IdealVertexDistance / distance) * towardsVector.normalized * Time.deltaTime);
            }
        }
    }

    //TODO: make the repulsive force much stronger if edges are crossing?
    private void UpdateRepulsion(Edge edge) {
        Vector3 pos = edge.transform.position;
        for (int i = 0; i < edges.Count; i++)
        {
            Edge neighbor = edges[i];

            // Which direction is our neighbor?
            Vector3 towardsVector = neighbor.transform.position - pos;

            // Grab the distance. We use this to determine forces.
            float dSquared = towardsVector.sqrMagnitude;
            float distance = towardsVector.magnitude;

            // Don't divide by zero
            if (Mathf.Approximately(distance, 0))
            {
                continue;
            }

            // Too close, get away!
            if (distance < IdealEdgeDistance - DeadZone)
            {
                edge.AddImpulse(-(IdealEdgeDistance * IdealEdgeDistance / distance) * towardsVector.normalized * Time.deltaTime);
            }
        }
    }

    public Vertex AddVertex(Vector3 position) 
    {
        Vertex vertex = GameObject.Instantiate<Vertex>(vertexPrefab, position, Quaternion.identity);
        vertices.Add(vertex);
        return vertex; 
    }

    public void Connect(Vertex a, Vertex b) {
        //if (edgePrefab == null) {
        //    edgePrefab = Resources.Load<GameObject>("Prefabs/Graph/Edge");
        //}

        a.neighbors.Add(b);
        b.neighbors.Add(a);

        Edge e = GameObject.Instantiate<Edge>(edgePrefab);
        e.Begin = a;
        e.End = b;
        edges.Add(e);
    }
}
