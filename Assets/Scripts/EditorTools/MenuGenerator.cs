using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
#if UNITY_EDITOR
public class MenuGenerator : EditorWindow
{
    private GameObject provincePrefab;
    private int sizeX;
    private int sizeY;
    private GameObject rootObject;
    private GameObject contactsGenerateCollider;
    private Province[,] map = new Province[0, 0];

    [MenuItem("Window/Hoi Map Gen Tool")]
    public static void ShowWindow()
    {

        EditorWindow.GetWindow(typeof(MenuGenerator));
    }

    void OnGUI()
    {
        /*
        GUILayout.Label("Map generator", EditorStyles.boldLabel);

        sizeX = EditorGUILayout.DelayedIntField("Size X:", sizeX);
        sizeY = EditorGUILayout.DelayedIntField("Size Y:", sizeY);
        Vector2Int size = new Vector2Int(sizeX, sizeY);

        GUILayout.Label("Province prefab: ", EditorStyles.boldLabel);
        provincePrefab = (GameObject)EditorGUILayout.ObjectField(provincePrefab, typeof(GameObject), true);
        GUILayout.Label("Root object: ", EditorStyles.boldLabel);
        rootObject = (GameObject)EditorGUILayout.ObjectField(rootObject, typeof(GameObject), true);
        GUILayout.Label("Contacts collider object: ", EditorStyles.boldLabel);
        contactsGenerateCollider = (GameObject)EditorGUILayout.ObjectField(contactsGenerateCollider, typeof(GameObject), true);


        if (GUILayout.Button("Generate Map"))
        {
            GenerateMap(new Vector2Int(sizeX, sizeY), out List<Line> lines);
           
        }
        if (GUILayout.Button("Generate contacts"))
        {
            var map = GetMap();
            var colliders = new List<FindProvinceContactCollider>();
            foreach (var province in map.Provinces)
            {
                var col = Instantiate(contactsGenerateCollider, province.Position, contactsGenerateCollider.transform.rotation).GetComponent<FindProvinceContactCollider>();
                col.Prov = province;
                colliders.Add(col);
            }
            for (int i = 0; i < map.Provinces.Count; i++)
            {
                map.Provinces[i].FindContacts(colliders[i], map);
            }
            foreach (var c in colliders)
            {
                DestroyImmediate(c.gameObject);
            }
        }
        if (GUILayout.Button("Delete map"))
        {
            while (rootObject.transform.childCount > 0)
            {
                DestroyImmediate(rootObject.transform.GetChild(0).gameObject);
            }
        }/*
        if(GUILayout.Button("Copy provinces"))
        {
            var testcol = rootObject.GetComponent<TestCol>();
            var map = GetMap();
            foreach (var p in testcol.provinces)
            {
                map.Provinces.Add(p);
            }
        }
        */
        EditorGUILayout.EndToggleGroup();
    }
    /*
    private Map GetMap()
    {
        return FindObjectOfType<Map>();
    }

    private IEnumerator GenerateCorotine()
    {
        GenerateMap(new Vector2Int(sizeX, sizeY), out List<Line> lines);
        yield return null;
    }

    private void GenerateMap(Vector2Int size, out List<Line> lines)
    {
        Debug.Log(size);
        lines = new List<Line>();
        map = new Province[size.x, size.y];
        var offest = new Vector3(0.88f, 0);
        offest *= 2;
        for (int i = 0; i < size.x; i++)
        {
            lines.Add(DrawLine(offest * i, size.y, i));
        }
    }

    private Line DrawLine(Vector3 lineOffest, int lenght, int lineNumber)
    {
        var ln = new Line();
        var offest = new Vector3(0.44f, 0, 0.88f);
        offest *= 2;
        bool forwardAndUp = false;
        for (int elNum = 0; elNum < lenght; elNum++)
        {

            var provPos = new Vector3();
            if (forwardAndUp)
            {
                provPos = new Vector3(rootObject.transform.position.x + offest.x, rootObject.transform.position.y
                    , rootObject.transform.position.z + (offest.z  * elNum)) + lineOffest;
            }
            if (!forwardAndUp)
            {
                provPos = new Vector3(rootObject.transform.position.x, rootObject.transform.position.y,
                    rootObject.transform.position.z + (offest.z * elNum)) + lineOffest;
            }
            forwardAndUp = !forwardAndUp;
            if (!ProvIsMap(provPos)) 
            {
                ln.Points.Add(null);
                continue;
            }
            Debug.Log("instance");
            var kek = new Province();
            kek.Position = provPos;
            kek.mesh = provincePrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
            rootObject.GetComponent<TestCol>().provinces.Add(kek);
            /*
            GameObject go = Instantiate(provincePrefab);
            
            go.transform.position = provPos;
            go.transform.SetParent(parentObject.transform);

            map[lineNumber, elNum] = go.GetComponent<Province>();
           
            ln.Points.Add(go.GetComponent<Province>());
            */
     /*   }

        return ln;
    }

    private bool ProvIsMap(Vector3 provincePos)
    {
        Ray ray = new Ray(provincePos, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.collider.GetComponent<MeshCollider>())
            {
                return true;
            }
        }
        return false;
    }*/

}
[Serializable]
public class Line
{
    public Line()
    {

    }
    public List<Province> Points = new List<Province>();
}
#endif