using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class MenuSaveCountries : EditorWindow
{
    private Map _map;
    private List<string> _countrieTags = new List<string>();
    private Country _country;
    [MenuItem("Window/Save load countries")]
    public static void ShowWindow()
    {

        EditorWindow.GetWindow(typeof(MenuSaveCountries));
    }

    void OnGUI()
    {
        GUILayout.Label("Map generator", EditorStyles.boldLabel);

        GUILayout.Label("Province prefab: ", EditorStyles.boldLabel);
        _map = (Map)EditorGUILayout.ObjectField(_map, typeof(Map), true);
        _country = (Country)EditorGUILayout.ObjectField(_country, typeof(Country), true);
        //var a  = (Country)EditorGUILayout.ObjectField(_countrieTags.Count, typeof(int), true);
        GUILayout.Label($"Saved provs {_countrieTags.Count}", EditorStyles.boldLabel);

        if (GUILayout.Button("Save countries"))
        {
            _countrieTags.Clear();
            foreach (var prov in _map.Provinces)
            {
                _countrieTags.Add(prov.Owner.ID);
            }
        }
        if (GUILayout.Button("Load countries"))
        {
            for (int i = 0; i < _map.Provinces.Count; i++)
            {
                _map.Provinces[i].SetOwner(_map.GetCountryFromId(_countrieTags[i]));
            }
            _countrieTags.Clear();
        }
        if (GUILayout.Button("Set all"))
        {
            foreach (var prov in _map.Provinces)
            {
                prov.SetOwner(_country);
            }
            _countrieTags.Clear();
        }
        if (GUILayout.Button("Clear"))
        {
            _countrieTags.Clear();
        }

        EditorGUILayout.EndToggleGroup();
    }

}
#endif