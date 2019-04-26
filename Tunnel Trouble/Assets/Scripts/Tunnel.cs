using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Tunnel : MonoBehaviour
{
    [Header("Mesh Generation")]
    [SerializeField]
    private int tunnelSides;
    [SerializeField]
    private int tunnelRadius;
    [SerializeField]
    private int tunnelLength;
    [SerializeField]
    private float minBump, maxBump;

    [Header("Mesh Bending")]
    [SerializeField]
    private int minRotate;
    [SerializeField]
    private int maxRotate;

    // private variables for mesh generation
    private const float thetaScale = 0.1f;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    // private variables for mesh bending
    private const float fromPosition = 0.5f;
    private int rotate;
    private Vector3[] wayPoints;

    void Awake()
    {
        //GenerateMesh();
        //BendMesh();
    }

    public void Generate()
    {
        GenerateMesh();
        BendMesh();
    }

    #region Creating a Randomly Generated Tunnel Mesh
    public void GenerateMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();

        mesh.Clear();

        SetVertices();
        SetTriangles();


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    #endregion

    #region Setting Mesh Vertices
    void SetVertices()
    {
        vertices = new Vector3[(tunnelSides + 1) * (tunnelLength + 1)];

        Vector3 pos = Vector3.zero;
        float theta = 0f;

        for (int z = 0, i = 0; z <= tunnelLength; z++)
        {
            for (int x = 0; x <= tunnelSides; x++)
            {
                theta += (2f * Mathf.PI * thetaScale);
                pos.x = tunnelRadius * Mathf.Cos(theta);
                pos.y = tunnelRadius * Mathf.Sin(theta) +
                    (Mathf.PerlinNoise(
                        x * UnityEngine.Random.Range(minBump, maxBump),
                        x * UnityEngine.Random.Range(minBump, maxBump)) * 3f);
                pos.z = z;
                vertices[i] = pos;
                i++;
            }
        }
    }
    #endregion

    #region Setting Mesh Triangles
    void SetTriangles()
    {
        triangles = new int[tunnelSides * tunnelLength * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < tunnelLength; z++)
        {
            for (int x = 0; x < tunnelSides; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + tunnelSides + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + tunnelSides + 1;
                triangles[tris + 5] = vert + tunnelSides + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }

    }
    #endregion

    #region Bending the Mesh
    void BendMesh()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        rotate = (int)UnityEngine.Random.Range(minRotate, maxRotate);

        float meshWidth = mesh.bounds.size.z;
        for (var i = 0; i < vertices.Length; i++)
        {
            float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
            float zeroPos = vertices[i].z + formPos;
            float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

            zeroPos -= 2 * vertices[i].x * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

            vertices[i].x += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
            vertices[i].z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        if (GetComponent<MeshCollider>() != null)
            DestroyImmediate(GetComponent<MeshCollider>());

        gameObject.AddComponent<MeshCollider>();
        wayPoints = new Vector3[tunnelLength];

        for (int i = 0; i < tunnelLength; i++)
        {
            float formPos = Mathf.Lerp(tunnelLength / 2, -tunnelLength / 2, fromPosition);
            float zeroPos = i + formPos;
            float rotateValue = (-rotate / 2) * (zeroPos / tunnelLength);
            zeroPos -= 2 * 0f * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

            float x = transform.localPosition.x;
            float z = i;
            x += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
            z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;

            Vector3 point = new Vector3(x, 0f, z + transform.localPosition.z);
            wayPoints[i] = RotatePointAroundPivot(point, transform.localPosition, transform.localEulerAngles.y);
        }
    }
    #endregion

    public Vector3 EdgePosition
    {
        get {return wayPoints[wayPoints.Length - 1];}
    }

    public Vector3 EdgeRotation
    {
        get {return transform.localEulerAngles - (Vector3.up * rotate); }
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angles)
    {
        var dir = point - pivot;
        dir = Quaternion.Euler(Vector3.up * angles) * dir;
        point = dir + pivot;
        return point;
    }

    public Vector3[] Waypoints
    {
        get { return wayPoints; }
    }

    void OnDrawGizmos()
    {
        if (wayPoints == null)
            return;

        foreach (var point in wayPoints)
        {
            Gizmos.DrawSphere(point, 0.5f);
        }
    }
}
