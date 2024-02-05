using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IJunior.TypedScenes
{
    public class LoadingProcessor : MonoBehaviour
    {
        private static LoadingProcessor _instance;
       // private Action _loadingModelAction;
        private LoadingModelData _loadingModelData;

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

        public void ApplyLoadingModel(bool hui = false)
        {
            if (_loadingModelData == null)
            {
                return;
            }
            _loadingModelData.LoadingModelAction?.Invoke();
            if (hui != false)
            {
                _loadingModelData = null;
            }
            //_loadingModelAction?.Invoke();
            //_loadingModelAction = null;
        }

        public void RegisterLoadingModel<T>(T loadingModel, string sceneName)
        {
            Action loadingModelAction = () =>
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
            _loadingModelData = new LoadingModelData(loadingModelAction, sceneName);
        }
    }

    public class LoadingModelData
    {
        public string SceneName { get; }
        public Action LoadingModelAction { get; }

        public LoadingModelData(Action loadingModelAction, string sceneName)
        {
            SceneName = sceneName;
            LoadingModelAction = loadingModelAction;
        }
    }
}
