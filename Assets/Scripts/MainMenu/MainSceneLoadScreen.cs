using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MainSceneLoadScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _loadProgressProcentText;
    [SerializeField] private TextMeshProUGUI _loadProgressStatusText;
    [SerializeField] private Image _loadProgressBar;
    [SerializeField] private GameSessionInitializer _gameSessionInitializer;
    [SerializeField] private GameSave _gameSave;

    private void Update()
    {
        /*
        if(_gameSessionInitializer.IsEndInit && Map.Instance.IsEndAsyncInit)
        {
            gameObject.SetActive(false);
        }
        if (_gameSessionInitializer.InitializeStatus == GameInitializeStatus.InitializeMap)
        {
            _loadProgressStatusText.text = "Создание карты";
        }
        if (_gameSessionInitializer.InitializeStatus == GameInitializeStatus.CreatingCountries)
        {
            _loadProgressStatusText.text = "Создание стран";
        }
        if (_gameSessionInitializer.InitializeStatus == GameInitializeStatus.LoadingSaves)
        {
            _loadProgressStatusText.text = "Загрузка сохранения";
        }
        //_loadProgressProcentText.text = Mathf.RoundToInt(_loadOperation.progress * 100).ToString() + "%";
        _loadProgressBar.fillAmount = _gameSave.ProcentOffAsyncLoad;*/
    }
}
