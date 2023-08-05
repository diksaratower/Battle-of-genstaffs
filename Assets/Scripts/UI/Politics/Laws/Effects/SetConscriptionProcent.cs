using UnityEngine;


[CreateAssetMenu(fileName = "SetConscriptionProcent", menuName = "ScriptableObjects/Law/Effects/SetConscriptionProcent", order = 1)]
public class SetConscriptionProcent : LawEffect
{
    public float ConscriptionProcent;

    public override string GetEffectDescription()
    {
        return $"������ �� ������ ��������� {ConscriptionProcent}%";
    }
}
