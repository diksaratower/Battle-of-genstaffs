using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PoliticsPreset", menuName = "ScriptableObjects/CountryPoliticsPreset", order = 1)]
public class CountryPoliticsPreset : ScriptableObject
{
    public List<Personage> AvailableAdvisers = new List<Personage>();
    public List<PartyPopular> Parties = new List<PartyPopular>();
    public List<Decision> Decisions = new List<Decision>();
    public List<CountryTrait> CountryTraits = new List<CountryTrait>();
    public List<Law> Laws = new List<Law>();
    public List<Law> ConscriptionLaws = new List<Law>();
    public NationalFocusTree FocusTree;
    public float BaseStability = 90;
}
