using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Focuses/NationalFocusTree", order = 1)]
public class NationalFocusTree : ScriptableObject
{
    public List<NationalFocus> NationalFocuses = new List<NationalFocus>();
    public NationalFocus BaseFocus;
}
