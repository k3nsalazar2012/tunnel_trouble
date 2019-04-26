using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject tunnelPrefab;

    [SerializeField]
    private int segment;

    private Tunnel[] tunnels;
    private List<Vector3> waypoints;

    public void GenerateTunnel()
    {
        waypoints = new List<Vector3>();
       
        tunnels = new Tunnel[segment];
        for (int i = 0; i < segment; i++)
        {
            GameObject tunnelObject = Instantiate(tunnelPrefab);
            Tunnel tunnel = tunnelObject.GetComponent<Tunnel>();
            tunnelObject.transform.SetParent(transform);
            tunnelObject.transform.localScale = Vector3.one;

            if (i == 0)
            {
                tunnelObject.transform.localPosition = Vector3.zero;
                tunnelObject.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                tunnelObject.transform.localPosition = tunnels[i - 1].EdgePosition;// + new Vector3(0f, 0f, tunnels[i-1].transform.localPosition.z);
                tunnelObject.transform.localEulerAngles = tunnels[i - 1].EdgeRotation;
            }

            tunnel.Generate();
            foreach (var point in tunnel.Waypoints)
            {
                waypoints.Add(point);
            }
            
            tunnels[i] = tunnel;
        }

        Debug.Log(waypoints.Count);
        GenerateWaypoints();
    }

    void GenerateWaypoints()
    {
        GameObject points = new GameObject();
        points.name = "Waypoints";
        points.transform.SetParent(transform);
        points.transform.localPosition = Vector3.zero;
        points.transform.localScale = Vector3.one;

        for (int i = 0; i < waypoints.Count; i++)
        {
            GameObject point = new GameObject();
            point.transform.SetParent(points.transform);
            point.transform.localPosition = waypoints[i];
            point.transform.localScale = Vector3.one;
            point.name = "Waypoint";

            point.AddComponent<SphereCollider>().isTrigger = true;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < transform.childCount; i++)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    public List<Vector3> Waypoints
    {
        get { return waypoints; }
    }
}
