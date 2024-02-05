using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SavesMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _saveNameDropdown;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private SavesMenuSaveSlotUI _saveSlotPrefab;
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private GameSave _gameSave;

    private List<SavesMenuSaveSlotUI> _slotsUI = new List<SavesMenuSaveSlotUI>();


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

    private void UpdateSavesSlots()
    {
        _slotsUI.ForEach(slot => 
        {
            Destroy(slot.gameObject);
        });
        _slotsUI.Clear();
        var saves = GameSave.GetSavesData();
        foreach (var save in saves) 
        {
            var slotUI = Instantiate(_saveSlotPrefab, _slotsParent);
            slotUI.RefreshUI(save);
            _slotsUI.Add(slotUI);
        }
    }
}
