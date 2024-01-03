using UnityEngine;


[CreateAssetMenu(fileName = "Submarine", menuName = "ScriptableObjects/Fleet/SubmarineSO", order = 1)]
public class SubmarineSO : ShipSO
{
    public override Ship CreateShip()
    {
        throw new System.NotImplementedException();
    }
}
