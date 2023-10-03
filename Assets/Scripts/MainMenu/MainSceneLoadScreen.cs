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

}
