using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologiesManagerInstancer : MonoBehaviour
{
    [SerializeField] private TechnologiesManagerSO _techManager;

    public static TechnologiesManagerSO GetInstance()
    {
        var go = Resources.Load("TechnologiesManagerInst", typeof(GameObject)) as GameObject;
        var em = Instantiate(go);
        DontDestroyOnLoad(em);
        return em.GetComponent<TechnologiesManagerInstancer>()._techManager;
    }
}
