using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManagerInstancer : MonoBehaviour
{
    [SerializeField] private EquipmentManagerSO _equipmentManager;

    public static EquipmentManagerSO GetInstance()
    {
        var go = Resources.Load("EquipmentManagerInst", typeof(GameObject)) as GameObject;
        var em = Instantiate(go);
        DontDestroyOnLoad(em);
        return em.GetComponent<EquipmentManagerInstancer>()._equipmentManager;
    }
}
