using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TunnelTrouble
{
    public static class Math3D
    {
        public static float CurveRadius { set; get; }
        public static int TunnelRadius { set; get; }
        public static int CurveSegmentCount { set; get; }
        public static int TunnelSegmentCount { set; get; }
        public static float RingDistance { set; get; }
        public static float CurveAngle { set; get; }

        private static Vector3[] vertices;
        private static int[] triangles;
        
        #region 3D sinusoid function
        // formula to get 3D sinusoid function
        // x = (R + r cos v) cos u
        // y = (R + r cos v) sin u
        // z = r cos v
        public static Vector3 GetPointOnTorus(float u, float v)
        {
            Vector3 p;
            float r = (CurveRadius + TunnelRadius * Mathf.Cos(v));
            p.x = r * Mathf.Sin(u);
            p.y = r * Mathf.Cos(u);
            p.z = TunnelRadius * Mathf.Sin(v);
            return p;
        }
        #endregion

        #region Set Vertices of the Mesh
        public static Vector3[] SetVertices()
        {
            vertices = new Vector3[TunnelSegmentCount * CurveSegmentCount * 4];
            float uStep = RingDistance / CurveRadius;
            CurveAngle = uStep * CurveSegmentCount * (360f / (2f * Mathf.PI));
            CreateFirstQuadRing(uStep);
            int iDelta = TunnelSegmentCount * 4;
            for(int u = 2, i = iDelta; u <= CurveSegmentCount; u++, i += iDelta)
            {
                CreateQuadRing(u * uStep, i);
            }
            return vertices;
        }

        private static void CreateFirstQuadRing(float u)
        {
            float vStep = (2f * Mathf.PI) / TunnelSegmentCount;

            Vector3 vertexA = GetPointOnTorus(0f, 0f);
            Vector3 vertexB = GetPointOnTorus(u, 0f);

            for (int v = 1, i = 0; v <= TunnelSegmentCount; v++, i += 4)
            {
                vertices[i] = vertexA;
                vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
                vertices[i + 2] = vertexB;
                vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
            }
        }

        private static void CreateQuadRing(float u, int i)
        {
            float vStep = (2f * Mathf.PI) / TunnelSegmentCount;
            int ringOffset = TunnelSegmentCount * 4;

            Vector3 vertex = GetPointOnTorus(u, 0f);
            for (int v = 1; v <= TunnelSegmentCount; v++, i += 4)
            {
                vertices[i] = vertices[i - ringOffset + 2];
                vertices[i + 1] = vertices[i - ringOffset + 3];
                vertices[i + 2] = vertex;
                vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
            }
        }
        #endregion

        #region Set Triangles of the Mesh
        public static int[] SetTriangles()
        {
            triangles = new int[TunnelSegmentCount * CurveSegmentCount * 6];
            for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
            {
                triangles[t] = i;
                triangles[t + 1] = triangles[t + 4] = i + 2;
                triangles[t + 2] = triangles[t + 3] = i + 1;
                triangles[t + 5] = i + 3;
            }
            return triangles;
        } 
        #endregion
    }
}