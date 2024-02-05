using IJunior.TypedScenes;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameSessionInitializer : MonoBehaviour, ISceneLoadHandler<GameEntryData>
{
    public bool IsEndInit;

    [SerializeField] private Map _map;
    [SerializeField] private GameSave _gameSave;
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private Player _player;
    [SerializeField] private MarineRegions _marineRegions;

    private bool _gameInitialized;

    private void Awake()
    {
        LoadingProcessor.Instance.ApplyLoadingModel(true);
        if (_gameInitialized == true)
        {
            return;
        }
        InitializeGame(new GameEntryData(GameEntryType.Continue, "ger"));
    }

    public void OnSceneLoaded(GameEntryData entry)
    {
        if (_gameInitialized == true)
        {
            return;
        }
        InitializeGame(entry);
        SceneManager.sceneLoaded += UnloadMenu;
        _gameInitialized = true;
    }

    private void InitializeGame(GameEntryData entry)
    {
        _map.InitializeMap();
        _map.CreateCountries();
        _marineRegions.Initialize();
        if (entry.EntryType == GameEntryType.Continue)
        {
            _gameSave.QuickLoad();
        }
        if (entry.EntryType == GameEntryType.LoadSave)
        {
            _gameSave.Load(entry.SaveName);
        }
        if (entry.EntryType == GameEntryType.StartFromStandart)
        {
            _gameSave.UpdateQuickSaveFromStandart();
            GameSave.SetSavePlayerCountryIDInQuickSave(entry.CountryID);
            _gameSave.QuickLoad();
        }
        Player.CurrentDifficultie = entry.Difficultie;
        _gameTimer.StartTimer();
    }

    private void UnloadMenu(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Main" || loadSceneMode == LoadSceneMode.Additive)
        {
            SceneManager.UnloadSceneAsync("Menu");
        }
    }
}

public class GameEntryData
{
    public GameEntryType EntryType { get; }
    public string CountryID { get; }
    public Difficultie Difficultie { get; }
    public string SaveName { get; }

    public GameEntryData(GameEntryType entryType, string countryID, Difficultie difficultie = null, string saveName = "")
    {
        EntryType = entryType;
        CountryID = countryID;
        if (difficultie != null)
        {
            Difficultie = difficultie;
        }
        else
        {
            Difficultie = DifficultiesData.GetInstance().StandartDifficultie;
        }
        SaveName = saveName;
    }
}

public enum GameEntryType
{
    Continue,
    StartFromStandart,
    LoadSave
}


