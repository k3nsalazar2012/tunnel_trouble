using UnityEngine;
using UnityEditor;

public class RockGenerator : EditorWindow
{
    int xSize = 2, ySize = 2, zSize = 2;
    Material material;
    float roundness = 1f, minBumpiness = 0.1f, maxBumpiness = 0.5f;
    Color checkColor;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector3[] normals;
    Editor editor;
    GameObject previewObject;

    [MenuItem("Window/Rock Generator")]
    public static void ShowWindow()
    {
        GetWindow<RockGenerator>("Rock Generator");
    }

    void OnGUI()
    {
        /*EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Dimensions");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("x");
        xSize = EditorGUILayout.IntField(xSize);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("y");
        ySize = EditorGUILayout.IntField(ySize);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("z");
        zSize = EditorGUILayout.IntField(zSize);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Material");
        material = (Material) EditorGUILayout.ObjectField(material, typeof(Material), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Roundness");
        roundness = EditorGUILayout.FloatField(roundness);
        EditorGUILayout.EndHorizontal();
      
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Mesh Preview");

        GUIStyle color = new GUIStyle();
        Color newColor = new Color(0.192f, 0.192f, 0.192f);
        color.normal.background = Texture2D.whiteTexture;
        
        if (EditorGUI.EndChangeCheck())
        {
            UpdateMesh();

            editor = Editor.CreateEditor(previewObject);
        }

        if (previewObject != null)
        {
           if (editor == null)
                editor = Editor.CreateEditor(previewObject);
            editor.OnPreviewGUI(GUILayoutUtility.GetRect(256, 256), color);
        }
         
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Min Bumpiness");
        minBumpiness = EditorGUILayout.FloatField(minBumpiness);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Max Bumpiness");
        maxBumpiness = EditorGUILayout.FloatField(maxBumpiness);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Update Bumpiness"))
        {
            UpdateMesh();
            editor = Editor.CreateEditor(previewObject);
        } 

        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate Rock"))
        {
            GenerateRock();
        }*/
    }

    void OnEnable()
    {
      // Cleanup();
      // UpdateMesh();
    }

    void UpdateMesh()
    {
        GameObject obj = EditorUtility.CreateGameObjectWithHideFlags("rock", HideFlags.HideInHierarchy);

        mesh = new Mesh();
        mesh.Clear();

        int cornerVertices = 8; // cube has 8 corner virtices
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];
        SetVertices();
        SetTriangles();

        previewObject = obj;
        previewObject.AddComponent<MeshFilter>().mesh = mesh;
        previewObject.AddComponent<MeshRenderer>();
        if (material)
            previewObject.GetComponent<MeshRenderer>().material = material;
    }

    void OnDisable()
    {
       // Cleanup();
    }

    void GenerateRock()
    {
        GameObject rock = Instantiate(previewObject);
        rock.name = "rock";
    }

    void Cleanup()
    {
        PreviewRenderUtility render = new PreviewRenderUtility();
        render.Cleanup();

        var hidden = FindObjectsOfType<GameObject>();

        if (hidden.Length == 0) return;
        foreach (GameObject go in hidden)
        {
            if ((go.hideFlags & HideFlags.HideInHierarchy) != 0)
            {
                DestroyImmediate(go);
            }
        }
    }

    void SetVertices()
    {
        int v = 0;

        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
                SetVertex(v++, x, y, 0);//vertices[v++] = new Vector3(x, y, 0 - Noise(1f));

            for (int z = 1; z <= zSize; z++)
                SetVertex(v++, xSize, y, z);//vertices[v++] = new Vector3(xSize, y, z);

            for (int x = xSize - 1; x >= 0; x--)
                SetVertex(v++, x, y, zSize);//vertices[v++] = new Vector3(x, y, zSize);

            for (int z = zSize - 1; z > 0; z--)
                SetVertex(v++, 0, y, z);//vertices[v++] = new Vector3(0, y, z);
        }

        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
                SetVertex(v++, x, ySize, z);//vertices[v++] = new Vector3(x, ySize + Noise(0.25f), z);
        }

        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
                SetVertex(v++, x, 0, z);//vertices[v++] = new Vector3(x, 0 - Noise(5f), z);
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
    }

    void SetTriangles()
    {
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        triangles = new int[quads * 6];

        int ring = (xSize + zSize) * 2;
        int t = 0, v = 0;

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
            }
            t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
        }
        t = CreateTopFace(triangles, t, ring);
        t = CreateBottomFace(triangles, t, ring);
        mesh.triangles = triangles;
    }

    void SetVertex(int i, float x, float y, float z)
    {
        Vector3 inner = vertices[i] = new Vector3(x, y, z);

        if (x < roundness)
        {
            inner.x = Random.Range(roundness - 0.2f, roundness + 0.2f);
        }
        else if (x > xSize - roundness)
        {
            inner.x = xSize - Random.Range(roundness - 0.2f, roundness + 0.2f);
        }


        if (y < roundness)
        {
            inner.y = Random.Range(roundness - 0.1f, roundness + 0.1f);
        }
        else if (y > ySize - roundness)
        {
            inner.y = ySize - Random.Range(roundness - 0.1f, roundness + 0.1f);
        }
        if (z < roundness)
        {
            inner.z = Random.Range(roundness - 0.2f, roundness + 0.2f);
        }
        else if (z > zSize - roundness)
        {
            inner.z = zSize - Random.Range(roundness - 0.2f, roundness + 0.2f);
        }

        normals[i] = (vertices[i] - inner).normalized;
        vertices[i] = inner + normals[i] * roundness;

    }

    int CreateTopFace(int[] triangles, int t, int ring)
    {
        int v = ring * ySize;
        for (int x = 0; x < xSize - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);

        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);
        return t;
    }

    int CreateBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= xSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
        return t;
    }

    int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }
}
