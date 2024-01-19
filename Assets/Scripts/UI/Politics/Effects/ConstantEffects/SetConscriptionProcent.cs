using UnityEngine;


[CreateAssetMenu(fileName = "SetConscriptionProcent", menuName = "ScriptableObjects/Law/Effects/SetConscriptionProcent", order = 1)]
public class SetConscriptionProcent : ConstantEffect
{
    public float ConscriptionProcent;

    public override string GetEffectDescription()
    {
        return $"Призыв от общего населения {ConscriptionProcent}%";
    }
}
