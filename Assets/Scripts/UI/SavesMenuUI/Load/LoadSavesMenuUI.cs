using System;
using UnityEngine;
using UnityEngine.UI;


public class LoadSavesMenuUI : SavesMenuBase
{
    public Action<LoadMenuSaveSlotUI> OnChangeSelected;

    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private bool _menuMod;
    [SerializeField] private SinglePlayerModMenu _singlePlayerModMenu;

    private bool _isLoadingScene = false;
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
            if (_menuMod == true)
            {
                _singlePlayerModMenu.LoadGameSceneLoadSave(_seletedSlot.SaveSlotName);
            }
            else
            {
                if (_isLoadingScene == true)
                {
                    return;
                }
                _isLoadingScene = true;
                MainMenu.LoadMenuAsync(new MenuEntryData(MenuLoadType.Hub, _seletedSlot.SaveSlotName));
            }
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

    protected override void UpdateSavesSlots()
    {
        DeleteSlots();
        var saves = GameSave.GetSavesData().FindAll(slot => slot.SaveName != "standard");
        foreach (var save in saves)
        {
            var slotUI = Instantiate(_slotPrefab as LoadMenuSaveSlotUI, _slotsParent);
            slotUI.RefreshUI(save, this, _countriesData);
            _slotsUI.Add(slotUI);
        }
    }
}
