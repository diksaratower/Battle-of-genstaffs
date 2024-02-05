using IJunior.TypedScenes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadSavesMenuUI : MonoBehaviour
{
    public Action<LoadMenuSaveSlotUI> OnChangeSelected;

    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private LoadMenuSaveSlotUI _saveSlotPrefab;
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private GameSave _gameSave;

    private bool _isLoadingScene = false;
    private List<LoadMenuSaveSlotUI> _slotsUI = new List<LoadMenuSaveSlotUI>();
    private LoadMenuSaveSlotUI _seletedSlot;


    private void Update()
    {
        _loadButton.interactable = _seletedSlot != null;
    }

    public void RefreshUI()
    {
        UpdateSavesSlots();
        _loadButton.onClick.RemoveAllListeners();
        _loadButton.onClick.AddListener(delegate
        {
            if (_isLoadingScene == true)
            {
                return;
            }
            _isLoadingScene = true;
            MainMenu.LoadMenuAsync(new MenuEntryData(MenuLoadType.Hub, _seletedSlot.SaveSlotName));
        });
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(delegate 
        {
            gameObject.SetActive(false);
        });
    }

    public bool IsSelected(LoadMenuSaveSlotUI selectedSlotUI)
    {
        return _seletedSlot == selectedSlotUI;
    }

    public void SetSelected(LoadMenuSaveSlotUI newSelectedSlotUI)
    {
        _seletedSlot = newSelectedSlotUI;
        OnChangeSelected?.Invoke(newSelectedSlotUI);
    }

    private void UpdateSavesSlots()
    {
        _slotsUI.ForEach(slot =>
        {
            Destroy(slot.gameObject);
        });
        var saves = GameSave.GetSavesData();
        foreach (var save in saves)
        {
            var slotUI = Instantiate(_saveSlotPrefab, _slotsParent);
            slotUI.RefreshUI(save, this);
            _slotsUI.Add(slotUI);
        }
    }
}
