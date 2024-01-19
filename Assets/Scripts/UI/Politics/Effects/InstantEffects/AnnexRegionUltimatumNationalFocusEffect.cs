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
        return $"� ����������� {Map.Instance.GetCountryFromId(TargetCountryID).Name} ����� ��������� ���������� ��� ��������� ������� {Map.Instance.MapRegions[RegionID].Name}.";
    }
}
