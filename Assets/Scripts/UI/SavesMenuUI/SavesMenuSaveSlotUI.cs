using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SavesMenuSaveSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _saveName;
    [SerializeField] private Image _saveCountryFlag;


    public void RefreshUI(GameSave.SaveSlotData slotData)
    {
        _saveName.text = slotData.SaveName;
        _saveCountryFlag.sprite = Map.Instance.GetCountryFromId(slotData.CountryID).Flag;
    }
}
