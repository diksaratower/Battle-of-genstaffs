using UnityEngine;


[CreateAssetMenu(fileName = "AddPolitPowerEffect", menuName = "ScriptableObjects/Focuses/Effect/AddPolitPower", order = 1)]
public class AddPolitPowerNationalFocusEffect : InstantEffect
{
    public float AddPolitPower = 100;

    public override void DoEffect(Country country)
    {
        country.Politics.PolitPower += AddPolitPower;
    }

    public override string GetEffectDescription()
    {
        return $"Полит. власть {GameIU.FloatToStringAddPlus(AddPolitPower)}";
    }
}
