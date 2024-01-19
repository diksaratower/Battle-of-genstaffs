using UnityEngine;


public abstract class InstantEffect : ScriptableObject
{
    public abstract void DoEffect(Country country);
    public abstract string GetEffectDescription();
}
