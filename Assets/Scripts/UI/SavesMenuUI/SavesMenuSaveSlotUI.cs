using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SavesMenuSaveSlotUI : SavesMenuSlotBase
{
    [SerializeField] private TextMeshProUGUI _saveName;
    [SerializeField] private Image _saveCountryFlag;
    [SerializeField] private Button _deleteSlotButton;


    public void RefreshUI(GameSave.SaveSlotData slotData, SavesMenuUI savesMenuUI, CountriesDataSO countriesDataSO)
    {
        _saveName.text = slotData.SaveName;
        _saveCountryFlag.sprite = countriesDataSO.GetCountrySOFromID(slotData.CountryID).CountryFlag;
        _deleteSlotButton.onClick.AddListener(delegate 
        {
            savesMenuUI.DeleteSaveSlot(slotData.SaveName);
        });
    }
}
