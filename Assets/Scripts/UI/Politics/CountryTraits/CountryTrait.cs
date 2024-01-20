using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CountryTrait", menuName = "ScriptableObjects/CountryTrait", order = 1)]
public class CountryTrait : ScriptableObject, IHavingConstantPoliticsEffect
{
    public List<ConstantEffect> CountryTraitEffects = new List<ConstantEffect>();
    public string Name;
    public Sprite TraitImage;
    public bool TemporaryTrait;
    public int WorkTime = 1;


    public List<T> GetEffects<T>() where T : ConstantEffect
    {
        var result = new List<T>();
        foreach (var effect in CountryTraitEffects)
        {
            if (effect is T)
            {
                result.Add(effect as T);
            }
        }
        return result;
    }
}
