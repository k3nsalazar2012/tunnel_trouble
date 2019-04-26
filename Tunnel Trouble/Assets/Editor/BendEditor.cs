using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bend))]
public class BendEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Bend bend = (Bend)target;

        if (GUILayout.Button("Bend Mesh"))
        {
            bend.BendMesh();
        }
    }
}
