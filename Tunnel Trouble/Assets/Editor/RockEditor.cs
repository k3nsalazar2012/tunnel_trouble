using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rock))]
public class RockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Rock rock = (Rock)target;

        if (GUILayout.Button("Generate"))
        {
            rock.Generate();
        }
    }
}
