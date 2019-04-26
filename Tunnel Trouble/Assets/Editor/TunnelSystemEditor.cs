using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TunnelSystem))]
public class TunnelSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TunnelSystem tunnelSystem = (TunnelSystem)target;

        if (GUILayout.Button("Generate Tunnel"))
            tunnelSystem.GenerateTunnel();
        if (GUILayout.Button("Clear"))
            tunnelSystem.Clear();
    }
}
