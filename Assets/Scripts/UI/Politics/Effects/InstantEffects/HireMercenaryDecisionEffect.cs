using UnityEngine;


[CreateAssetMenu(fileName = "HireMercenary", menuName = "ScriptableObjects/Decision/Effects/HireMercenary", order = 1)]
public class HireMercenaryDecisionEffect : InstantEffect
{
    public int MercenaryCount;

    public override void DoEffect(Country country)
    {
        country.EquipmentStorage.AddEquipment("manpower", MercenaryCount);
    }

    public override string GetEffectDescription()
    {
        return "";
    }
}
