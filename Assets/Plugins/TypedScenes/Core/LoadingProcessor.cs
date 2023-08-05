using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IJunior.TypedScenes
{
    public class LoadingProcessor : MonoBehaviour
    {
        private static LoadingProcessor _instance;
        private Action _loadingModelAction;

        public static LoadingProcessor Instance
        {
            get
            {
                if (_instance == null)
                    Initialize();

                return _instance;
            }
        }

        private static void Initialize()
        {
            _instance = new GameObject("LoadingProcessor").AddComponent<LoadingProcessor>();
            _instance.transform.SetParent(null);
            DontDestroyOnLoad(_instance);
        }

        public void ApplyLoadingModel()
        {
            _loadingModelAction?.Invoke();
            _loadingModelAction = null;
        }

        public void RegisterLoadingModel<T>(T loadingModel, string sceneName)
        {
            _loadingModelAction = () =>
            {
                var rootsObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
                foreach (var rootObjects in rootsObjects)//GetActiveScene().GetRootGameObjects())
                {
                    foreach (var handler in rootObjects.GetComponentsInChildren<ISceneLoadHandler<T>>())
                    {
                        handler.OnSceneLoaded(loadingModel);
                    }
                }
            };
        }
    }
}
