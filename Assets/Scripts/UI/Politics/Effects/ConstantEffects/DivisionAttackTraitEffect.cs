using UnityEngine;


[CreateAssetMenu(fileName = "DivisionAttackTraitEffect", menuName = "ScriptableObjects/Personages/Traits/Effects/DivisionAttack", order = 1)]
public class DivisionAttackTraitEffect : ConstantEffect
{
    public float AddedAttackPercent;

    public override string GetEffectDescription()
    {
        return $"Атака дивизий {GameIU.FloatToStringAddPlus(AddedAttackPercent)}%";
    }
}
