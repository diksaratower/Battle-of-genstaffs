using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _singlePlayerModButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private SinglePlayerModMenu _singlePlayerModMenu;

    private void Awake()
    {
        _singlePlayerModButton.onClick.AddListener(delegate 
        {
            _singlePlayerModMenu.gameObject.SetActive(true);
        });
        _exitButton.onClick.AddListener(delegate 
        {
            Application.Quit();
        });
    }
}
