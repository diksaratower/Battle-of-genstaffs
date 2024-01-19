using UnityEngine;


[CreateAssetMenu(fileName = "AnnexRegionUltimatum", menuName = "ScriptableObjects/Focuses/Effect/AnnexRegionUltimatum", order = 1)]
public class AnnexRegionUltimatumNationalFocusEffect : InstantEffect
{
    public string TargetCountryID;
    public int RegionID;

    public override void DoEffect(Country country)
    {
        var targetCountry = Map.Instance.GetCountryFromId(TargetCountryID);
        targetCountry.CountryDiplomacy.SendUltimatum(new AnnexRegionUltimatum(country, targetCountry, Map.Instance.MapRegions[RegionID]));
    }

    public override string GetEffectDescription()
    {
        return $"В государство {Map.Instance.GetCountryFromId(TargetCountryID).Name} будет отправлен ультиматум для получения региона {Map.Instance.MapRegions[RegionID].Name}.";
    }
}
