using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Personages/Simple", order = 1)]
public class Personage : ScriptableObject
{
    public Sprite Portrait => _standartPortrait;
    public string Name => _name;
    public List<PersonageTrait> Traits = new List<PersonageTrait>();

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

    public List<TraitEffect> GetTraitEffects<T>() where T : TraitEffect
    {
        var result = new List<TraitEffect>();
        foreach (var trait in Traits)
        {
            var effects = trait.TraitEffects.FindAll(effect => effect.GetType() == typeof(T));
            if (effects.Count > 0)
            {
                result.AddRange(effects);
            }
        }
        return result;
    }
}
