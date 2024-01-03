using UnityEngine;


[CreateAssetMenu(fileName = "EquipmentTechnology", menuName = "ScriptableObjects/Technologies/EquipmentTechnology", order = 1)]
public class EquipmentTechnology : Technology
{
    public Equipment UnlockEquipment;
    public override string ID
    {
        get 
        {
            if (_id == "*use_equipment")
            {
                return UnlockEquipment.ID;
            }
            else
            {
                return _id;
            }
        }
        set 
        { 
            throw new System.NotImplementedException();
        }
    }
    public override string TechName
    {
        get
        {
            if (_id == "*use_equipment")
            {
                return UnlockEquipment.Name;
            }
            else
            {
                return _id;
            }
        }
        set
        {
            throw new System.NotImplementedException();
        }
    }
    public override Sprite TechImage
    {
        get
        {
            if (_techImage == null)
            {
                return UnlockEquipment.EquipmentImage;
            }
            else
            {
                return null;
            }
        }
        set
        {
            throw new System.NotImplementedException();
        }
    }
}
