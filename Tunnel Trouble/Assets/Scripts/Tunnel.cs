using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TunnelTrouble
{
    public class Tunnel : MonoBehaviour
    {
        #region Variables
        private float curveRadius;

        [SerializeField]
        private int tunnelRadius;
        
        private int curveSegmentCount;

        [SerializeField]
        private int tunnelSegmentCount;

        [SerializeField]
        private float ringDistance;

        [SerializeField]
        private float minCurveRadius, maxCurveRadius;

        [SerializeField]
        private int minCurveSegmentCount, maxCurveSegmentCount;

        private Mesh mesh;
        private Vector3[] vertices;
        private int[] triangles;
        private float curveAngle;
        private float relativeRotation;
        #endregion

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = mesh =new Mesh();
            mesh.name = "Tunnel";         
        }

        public void Generate()
        {
            Math3D.CurveRadius = curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
            Math3D.TunnelRadius = tunnelRadius;
            Math3D.CurveSegmentCount = curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
            Math3D.TunnelSegmentCount = tunnelSegmentCount;
            Math3D.RingDistance = ringDistance;

            mesh.Clear();

            vertices = Math3D.SetVertices();
            curveAngle = Math3D.CurveAngle;
            mesh.vertices = vertices;

            triangles = Math3D.SetTriangles();
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        public void AlignWith(Tunnel tunnel)
        {
            relativeRotation = Random.Range(0, curveSegmentCount) * 360f / tunnelSegmentCount;

            transform.SetParent(tunnel.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0f, 0f, -tunnel.curveAngle);
            transform.Translate(0f, tunnel.curveRadius, 0f);
            transform.Rotate(relativeRotation, 0f, 0f);
            transform.Translate(0f, -curveRadius, 0f);
            transform.SetParent(tunnel.transform.parent);

            transform.localScale = Vector3.one;
        }

        public float CurveRadius { get { return curveRadius; } }
        public float CurveAngle { get { return curveAngle; } }
        public float RelativeRotation {get {return relativeRotation;}
        }
    }
}