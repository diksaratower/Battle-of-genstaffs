using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoadMenuSaveSlotUI : SavesMenuSlotBase
{
    public string SaveSlotName { get; private set; }

    [SerializeField] private TextMeshProUGUI _saveName;
    [SerializeField] private Image _saveCountryFlag;
    [SerializeField] private Outline _selectedOutline;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _deleteSlotButton;


    public void RefreshUI(GameSave.SaveSlotData slotData, LoadSavesMenuUI loadSavesMenu, CountriesDataSO countriesDataSO)
    {
        SaveSlotName = slotData.SaveName;
        _saveName.text = slotData.SaveName;
        _saveCountryFlag.sprite = countriesDataSO.GetCountrySOFromID(slotData.CountryID).CountryFlag;

        SetUpButtons(slotData, loadSavesMenu);

        loadSavesMenu.OnChangeSelected += newSelectedSlot => 
        {
            if (newSelectedSlot == this)
            {
                _selectedOutline.enabled = true;
            }
            else
            {
                _selectedOutline.enabled = false;
            }
        };
    }

    private void SetUpButtons(GameSave.SaveSlotData slotData, LoadSavesMenuUI loadSavesMenu)
    {
        _selectButton.onClick.AddListener(delegate
        {
            if (loadSavesMenu.IsSelected(this))
            {
                loadSavesMenu.SetSelected(null);
            }
            else
            {
                loadSavesMenu.SetSelected(this);
            }
        });
        _deleteSlotButton.onClick.AddListener(delegate
        {
            loadSavesMenu.DeleteSaveSlot(slotData.SaveName);
        });
    }
}
