using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bend : MonoBehaviour
{
    public enum BendAxis { X, Y, Z };

    public float minRotate = -45;
    public float maxRotate = 45;
    public float fromPosition = 0.5F; //from 0 to 1
    public BendAxis axis = BendAxis.X;
    Mesh mesh;
    Vector3[] vertices;
    Vector3[] arcPoints;
    private int rotate;

    void Start()
    {
        //BendMesh();       
    }

    public void BendMesh()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        vertices = mesh.vertices;

        rotate = (int) Random.Range(minRotate, maxRotate);

        if (axis == BendAxis.X)
        {
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
        }
        else if (axis == BendAxis.Y)
        {
            float meshWidth = mesh.bounds.size.z;
            for (var i = 0; i < vertices.Length; i++)
            {
                float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
                float zeroPos = vertices[i].z + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

                zeroPos -= 2 * vertices[i].y * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

                vertices[i].y += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                vertices[i].z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
            }
        }
        else if (axis == BendAxis.Z)
        {
            float meshWidth = mesh.bounds.size.x;
            for (var i = 0; i < vertices.Length; i++)
            {
                float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
                float zeroPos = vertices[i].x + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

                zeroPos -= 2 * vertices[i].y * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

                vertices[i].y += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                vertices[i].x = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        if(GetComponent<MeshCollider>() != null)
            DestroyImmediate(GetComponent<MeshCollider>());

        gameObject.AddComponent<MeshCollider>();
        /*for(int i=30; i>0; i--)
        print(vertices[vertices.Length-i]);

        print("last: " + vertices[vertices.Length-1]);
        print("center: " + GetComponent<MeshRenderer>().bounds.);*/

        int length = 50;

        arcPoints = new Vector3[length];
        for (int i= 0; i < length; i++)
        {
            float formPos = Mathf.Lerp(length / 2, -length / 2, fromPosition);
            float zeroPos = i + formPos;
            float rotateValue = (-rotate / 2) * (zeroPos / length);
            zeroPos -= 2 * 0f * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

            float x = 0;
            float z = i;
            x += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
            z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
            arcPoints[i] = new Vector3(x, 0f, z);
        }

        print(arcPoints[arcPoints.Length - 1]);
    }

    void OnDrawGizmos()
    {
        if (arcPoints == null)
            return;

        foreach (var point in arcPoints)
        {
            Gizmos.DrawSphere(point, 0.5f);
        }

        
        //Gizmos.DrawLine(Vector3.zero, Vector3.forward * 50f);
    }
}
