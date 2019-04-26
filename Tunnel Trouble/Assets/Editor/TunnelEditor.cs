using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tunnel))]
public class TunnelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Tunnel tunnel = (Tunnel) target;

        if (GUILayout.Button("Generate"))
            tunnel.Generate();
    }
}
