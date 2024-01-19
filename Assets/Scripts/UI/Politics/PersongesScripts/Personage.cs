using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Personages/Simple", order = 1)]
public class Personage : ScriptableObject, IHavingConstantPoliticsEffect
{
    public Sprite Portrait => _standartPortrait;
    public string Name => _name;
    public List<PersonageTrait> Traits = new List<PersonageTrait>();
    public string ID;

    [SerializeField] private Sprite _standartPortrait;
    [SerializeField] private string _name;

    public string GetName()
    {
        return _name;
    }

    public Sprite GetPortrait()
    {
        return _standartPortrait;
    }

    public List<T> GetEffects<T>() where T : ConstantEffect
    {
        var result = new List<T>();
        foreach (var trait in Traits)
        {
            var effects = trait.TraitEffects.FindAll(effect => effect.GetType() == typeof(T));
            if (effects.Count > 0)
            {
                result.AddRange(effects.Cast<T>());
            }
        }
        return result;
    }
}
