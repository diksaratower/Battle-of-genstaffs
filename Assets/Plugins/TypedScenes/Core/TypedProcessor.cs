using System;
using System.Linq;
using UnityEngine;

namespace IJunior.TypedScenes
{
    public class TypedProcessor : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("ApplyLoadingModel");
            foreach(var handler in FindObjectsOfType<MonoBehaviour>().OfType<ITypedAwakeHandler>())
            {
                handler.OnSceneAwake();
            }
            LoadingProcessor.Instance.ApplyLoadingModel();
        }
    }
}
