using UnityEngine;


[CreateAssetMenu(fileName = "MilitaryFabrication", menuName = "ScriptableObjects/Law/Effects/MilitaryFabrication", order = 1)]
public class MilitaryFabricationLawEffect : LawEffect
{
    public float MilitaryFabricationIncreaseProcent = 10;

    public float GetNeedIncreaseValue(float baseValue)
    {
        return (baseValue * (MilitaryFabricationIncreaseProcent / 100f));
    }

    public override string GetEffectDescription()
    {
        return $"Производство на военных заводах {GameIU.FloatToStringAddPlus(MilitaryFabricationIncreaseProcent)}%";
    }
}
