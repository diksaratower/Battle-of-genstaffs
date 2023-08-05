using IJunior.TypedScenes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionInitializer : MonoBehaviour, ISceneLoadHandler<GameEntryData>
{
    public bool IsEndInit;

    [SerializeField] private Map _map;
    [SerializeField] private GameSave _gameSave;
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private Player _player;

    private bool _gameInitialized;

    private void Awake()
    {
        if (_gameInitialized == true)
        {
            return;
        }
        InitializeGame(new GameEntryData(GameEntryType.Continue, "ger"));
    }

    public void OnSceneLoaded(GameEntryData entry)
    {
        InitializeGame(entry);
        SceneManager.sceneLoaded += UnloadMenu;
        _gameInitialized = true;
    }

    private void InitializeGame(GameEntryData entry)
    {
        _map.InitializeMap();
        _map.CreateCountries();
        if (entry.EntryType == GameEntryType.Continue)
        {
            _gameSave.QuickLoad();
        }
        if (entry.EntryType == GameEntryType.StartFromStandart)
        {
            _gameSave.UpdateQuickSaveFromStandart();
            GameSave.SetSavePlayerCountryIDInQuickSave(entry.CountryID);
            _gameSave.QuickLoad();
        }
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

    public GameEntryData(GameEntryType entryType, string countryID)
    {
        EntryType = entryType;
        CountryID = countryID;
    }
}

public enum GameEntryType
{
    Continue,
    StartFromStandart
}


