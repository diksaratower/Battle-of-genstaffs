using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersonageTrait : ScriptableObject
{
    public string TraitName;
    public List<ConstantEffect> TraitEffects = new List<ConstantEffect>();
}
