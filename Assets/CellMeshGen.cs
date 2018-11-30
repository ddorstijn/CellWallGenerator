using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMeshGen : MonoBehaviour {

    Mesh mesh;
    List<int> triangles;
    VertexPoint[] points;

    class VertexPoint
    {
        public Vector3 position;
        List<int[]> connections = new List<int[]>();

        public void Process(VertexPoint[] points)
        {
            float total_x;
            float total_y;
            float total_z;

            foreach (var c in connections) {
                total_x = (points[c[0]].position.x + points[c[1]].position.x + points[c[2]].position.x) / 3f;
                total_y = (points[c[0]].position.y + points[c[1]].position.y + points[c[2]].position.y) / 3f;
                total_z = (points[c[0]].position.z + points[c[1]].position.z + points[c[2]].position.z) / 3f;

                position = new Vector3(total_x / connections.Count, total_y / connections.Count, total_z / connections.Count);
            }
        }

        public void Connect(VertexPoint[] points, int index, List<int> tris) {
            while (connections.Count < 2 || Random.Range(0, 10) < 3) {
                var v2 = RandomNode(index, points.Length);
                var v3 = RandomNode(index, points.Length);

                int[] triangle = { index, v2, v3 };
                System.Array.Sort(triangle);

                if (!connections.Contains(triangle)) {
                    connections.Add(triangle);
                    points[v2].connections.Add(triangle);
                    points[v3].connections.Add(triangle);
                    tris.Add(index);
                    tris.Add(v2);
                    tris.Add(v3);
                }
            }
        }

        int RandomNode(int index, int array_size)
        {
            int point;

            do {
                point = Random.Range(0, array_size);
            } while (point == index);

            return point;
        }

        public void ClearConnections()
        {
            connections.Clear();
        }
    }

    // Use this for initialization
    void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        triangles = new List<int>();
        points = new VertexPoint[20];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new VertexPoint();

            float x = Random.Range(-10f, 10f);
            float y = Random.Range(-10f, 10f);
            float z = Random.Range(-10f, 10f);

            points[i].position = new Vector3(x, y, z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] verts = new Vector3[points.Length];
        
        for (int i = 0; i < points.Length; i++) {
            points[i].ClearConnections();
        }

        for (int i = 0; i < points.Length; i++) {
            points[i].Connect(points, i, triangles);
        }

        for (int i = 0; i < points.Length; i++) {
            points[i].Process(points);
            verts[i] = points[i].position;
        }

        mesh.vertices = verts;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
