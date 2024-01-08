using UnityEngine;


[CreateAssetMenu(fileName = "TankEquipment", menuName = "ScriptableObjects/TankEquipment", order = 1)]
public class TankEquipment : Equipment, IGroundCombatEquipment
{
    public float Attack;
    public float Defens;
    public TanksDivisionView EquipmentViewPrefab;

    float IGroundCombatEquipment.Attack => Attack;
    float IGroundCombatEquipment.Defens => Defens;
}
