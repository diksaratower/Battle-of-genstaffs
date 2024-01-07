using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Focuses/NationalFocus", order = 1)]
public class NationalFocus : ScriptableObject
{
    public string Name;
    public string Description;
    public string ID;
    public Sprite Image;
    public int ExecutionDurationDay;
    public List<NationalFocus> NeedsForExecution = new List<NationalFocus>();
    public List<NationalFocus> ConflictWithFocuses = new List<NationalFocus>();
    public List<NationalFocusEffect> FocusEffects = new List<NationalFocusEffect>();

    public void ExecuteFocus(Country country)
    {
        foreach (var effect in FocusEffects)
        {
            effect.ExecuteFocus(country);
        }
    }
}
