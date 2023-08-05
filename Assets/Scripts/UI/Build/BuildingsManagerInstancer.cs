using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManagerInstancer : MonoBehaviour
{
    [SerializeField] private BuildingsManagerSO _equipmentManager;

    public static BuildingsManagerSO GetInstance()
    {
        var go = Resources.Load("BuildManagerInst", typeof(GameObject)) as GameObject;
        var em = Instantiate(go);
        DontDestroyOnLoad(em);
        return em.GetComponent<BuildingsManagerInstancer>()._equipmentManager;
    }
}
