using IJunior.TypedScenes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour, ISceneLoadHandler<MenuEntryData>
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

    public void OnSceneLoaded(MenuEntryData argument)
    {
        if (argument.LoadType == MenuLoadType.Simple)
        {
            return;
        }
        if (argument.LoadType == MenuLoadType.Hub)
        {
            _singlePlayerModMenu.LoadGameSceneLoadSave(argument.SaveName);
        }
    }

    public static void LoadMenuAsync(MenuEntryData entryData)
    {
        Menu.Load(entryData);
    }
}

public class MenuEntryData
{
    public MenuLoadType LoadType { get; }
    public string SaveName { get; }

    public MenuEntryData(MenuLoadType menuLoadType, string saveName)
    {
        LoadType = menuLoadType;
        SaveName = saveName;
    }
}

public enum MenuLoadType
{
    Simple,
    Hub
}