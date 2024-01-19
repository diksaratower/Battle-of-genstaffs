using UnityEngine;


[CreateAssetMenu(fileName = "ResearchPointsGrowthConstantEffect", menuName = "ScriptableObjects/Effects/Constant/ResearchPointsGrowthConstantEffect", order = 1)]
public class ResearchPointsGrowthConstantEffect : ConstantEffect
{
    public float ResearchPointsGrothIncreaseProcent = 10;

    public override string GetEffectDescription()
    {
        return $"Прирост очков науки {GameIU.FloatToStringAddPlus(ResearchPointsGrothIncreaseProcent)}%";
    }
}
