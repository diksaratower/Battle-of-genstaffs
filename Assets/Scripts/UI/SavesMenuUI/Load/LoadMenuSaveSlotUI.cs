using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoadMenuSaveSlotUI : MonoBehaviour
{
    public string SaveSlotName { get; private set; }

    [SerializeField] private TextMeshProUGUI _saveName;
    [SerializeField] private Image _saveCountryFlag;
    [SerializeField] private Outline _selectedOutline;
    [SerializeField] private Button _selectButton;


    public void RefreshUI(GameSave.SaveSlotData slotData, LoadSavesMenuUI loadSavesMenu)
    {
        SaveSlotName = slotData.SaveName;
        _saveName.text = slotData.SaveName;
        _saveCountryFlag.sprite = Map.Instance.GetCountryFromId(slotData.CountryID).Flag;
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
}
