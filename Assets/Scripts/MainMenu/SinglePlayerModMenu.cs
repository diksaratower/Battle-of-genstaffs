using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IJunior;
using IJunior.TypedScenes;
using UnityEngine.SceneManagement;

public class SinglePlayerModMenu : MonoBehaviour
{
    [SerializeField] private Button _continueGameButon;
    [SerializeField] private Image _continueGameCountryFlagImage;
    [SerializeField] private Button _exitFromMenu;
    [SerializeField] private LoadScreen _loadScreen;
    [SerializeField] private CountriesDataSO _countriesData;

    private bool _isLoadingScene;

    private void Awake()
    {
        _exitFromMenu.onClick.AddListener(delegate 
        {
            gameObject.SetActive(false);
        });
        _continueGameButon.onClick.AddListener(delegate 
        {
            if (GameSave.GetSaves().Exists(save => save == "quicksave"))
            {
                LoadGameSceneContinue();
            }
        });
        if (GameSave.GetSaves().Exists(save => save == "quicksave") == false)
        {
            _continueGameButon.interactable = false;
            _continueGameCountryFlagImage.gameObject.SetActive(false);
        }
        else
        {
            _continueGameCountryFlagImage.sprite = _countriesData.Countries.Find(country => country.ID == GameSave.GetSavePlayerCountryID("quicksave")).CountryFlag;
        }
    }

    public void LoadGameSceneUpdateQuick(string id, Difficultie difficultie)
    {
        if (_isLoadingScene)//чтобы избежать двойной загрузки
        {
            return;
        }
        _isLoadingScene = true;
        var operation = Main.LoadAsync(new GameEntryData(GameEntryType.StartFromStandart, id, difficultie), LoadSceneMode.Additive);
        _loadScreen.StartMonitoringLoading(operation);
    }

    private void LoadGameSceneContinue()
    {
        if (_isLoadingScene)//чтобы избежать двойной загрузки
        {
            return;
        }
        //Debug.Log("scene loading");
        _isLoadingScene = true;
        var operation = Main.LoadAsync(new GameEntryData(GameEntryType.Continue, ""), LoadSceneMode.Additive);
        _loadScreen.StartMonitoringLoading(operation);
    }
}
