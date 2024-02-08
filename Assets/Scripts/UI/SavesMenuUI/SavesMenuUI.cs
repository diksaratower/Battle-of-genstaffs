using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SavesMenuUI : SavesMenuBase
{
    [SerializeField] private TMP_InputField _saveNameDropdown;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] protected GameSave _gameSave;


    private void Update()
    {
        _saveButton.interactable = _saveNameDropdown.text != "";
    }

    public void RefreshUI()
    {
        var dateString = $"{GameTimer.Instance.DTime.Day}-{GameTimer.Instance.DTime.Month}-{GameTimer.Instance.DTime.Year}";
        _saveNameDropdown.text = $"{Player.CurrentCountry.ID}_{dateString}";
        UpdateSavesSlots();
        _saveButton.onClick.RemoveAllListeners();
        _saveButton.onClick.AddListener(delegate
        {
            _gameSave.Save(_saveNameDropdown.text);
            UpdateSavesSlots();
        });
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(delegate 
        {
            gameObject.SetActive(false);
        });
    }

    protected override void UpdateSavesSlots()
    {
        DeleteSlots();
        var saves = GameSave.GetSavesData().FindAll(slot => slot.SaveName != "standard");
        foreach (var save in saves)
        {
            var slotUI = Instantiate(_slotPrefab as SavesMenuSaveSlotUI, _slotsParent);
            slotUI.RefreshUI(save, this, _countriesData);
            _slotsUI.Add(slotUI);
        }
    }
}
