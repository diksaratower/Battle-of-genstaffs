using UnityEngine;

public abstract class NationalFocusEffect : ScriptableObject
{
    public abstract void ExecuteFocus(Country country);

    public abstract string GetEffectDescription();
}
