using UnityEngine;
using UnityEngine.UI;


public class ExitMenuUI : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _openDebugConsoleButton;
    [SerializeField] private Button _openSettingsButton;
    [SerializeField] private Button _openSavesMenuButton;
    [SerializeField] private Button _openLoadMenuButton;
    [SerializeField] private DebugConsoleUI _debugConsoleUI;
    [SerializeField] private SettingsMenu _settingsMenu;
    [SerializeField] private SavesMenuUI _savesMenu;
    [SerializeField] private LoadSavesMenuUI _loadMenu;


    private void Start()
    {
        _exitButton.onClick.AddListener(delegate 
        {
            Application.Quit();
        });
        _continueButton.onClick.AddListener(delegate 
        {
            gameObject.SetActive(false);
        });
        _openDebugConsoleButton.onClick.AddListener(delegate 
        {
            _debugConsoleUI.gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
        _openSettingsButton.onClick.AddListener(delegate 
        {
            _settingsMenu.gameObject.SetActive(true);
        });
        _openSavesMenuButton.onClick.AddListener(delegate 
        {
            _savesMenu.gameObject.SetActive(true);
            _savesMenu.RefreshUI();
        });
        _openLoadMenuButton.onClick.AddListener(delegate 
        {
            _loadMenu.gameObject.SetActive(true); 
            _loadMenu.RefreshUI();
        });
    }
}
