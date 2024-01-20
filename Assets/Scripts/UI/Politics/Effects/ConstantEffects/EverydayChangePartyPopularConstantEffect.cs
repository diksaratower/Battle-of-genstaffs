using UnityEngine;


[CreateAssetMenu(fileName = "EverydayChangePartyPopularConstantEffect", menuName = "ScriptableObjects/Effects/Constant/EverydayChangePartyPopularConstantEffect", order = 1)]
public class EverydayChangePartyPopularConstantEffect : ConstantEffect
{
    public float EverydayChangePopular;
    public PoliticalParty Party;


    public override string GetEffectDescription()
    {
        return $"������. ������� ������������ {Party.Name} {GameIU.FloatToStringAddPlus(EverydayChangePopular)}%";
    }
}
