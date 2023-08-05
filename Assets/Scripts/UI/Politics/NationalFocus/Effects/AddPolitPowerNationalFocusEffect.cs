using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AddPolitPowerEffect", menuName = "ScriptableObjects/Focuses/Effect/AddPolitPower", order = 1)]
public class AddPolitPowerNationalFocusEffect : NationalFocusEffect
{
    public float AddPolitPower = 100;

    public override void ExecuteFocus(Country country)
    {
        country.Politics.PolitPower += AddPolitPower;
    }

    public override string GetEffectDescription()
    {
        return $"Полит. власть {GameIU.FloatToStringAddPlus(AddPolitPower)}";
    }
}
