using UnityEngine;


[CreateAssetMenu(fileName = "HireMercenary", menuName = "ScriptableObjects/Decision/Effects/HireMercenary", order = 1)]
public class HireMercenaryDecisionEffect : DecisionEffect
{
    public int MercenaryCount;

    public override void ExecuteDecisionEffect(Country country)
    {
        country.EquipmentStorage.AddEquipment("manpower", MercenaryCount);
    }
}
