using UnityEngine;


[CreateAssetMenu(fileName = "RifleEquipment", menuName = "ScriptableObjects/RifleEquipment", order = 1)]
public class RifleEquipment : Equipment, IGroundCombatEquipment
{
    public float Attack;
    public float Defens;

    float IGroundCombatEquipment.Attack => Attack;
    float IGroundCombatEquipment.Defens => Defens;
}
