using UnityEngine;


[CreateAssetMenu(fileName = "AnnexCountryUltimatum", menuName = "ScriptableObjects/Focuses/Effect/AnnexCountryUltimatum", order = 1)]
public class AnnexCountryUltimatumNationalFocusEffect : NationalFocusEffect
{
    public string TargetCountryID;

    public override void ExecuteFocus(Country country)
    {
        var targetCountry = Map.Instance.GetCountryFromId(TargetCountryID);
        targetCountry.CountryDiplomacy.SendUltimatum(new AnnexCountryUltimatum(country, targetCountry));
    }

    public override string GetEffectDescription()
    {
        return $"� ����������� {Map.Instance.GetCountryFromId(TargetCountryID).Name} ����� ��������� ���������� �� ��������� � ��� ������.";
    }
}
