using System.Collections.Generic;
using UnityEngine;


public class SavesMenuBase : MonoBehaviour
{
    [SerializeField] protected Transform _slotsParent;
    [SerializeField] protected OkCancelWindowUI _okCancelWindowPrefab;
    [SerializeField] protected SavesMenuSlotBase _slotPrefab;
    [SerializeField] protected CountriesDataSO _countriesData;

    protected List<SavesMenuSlotBase> _slotsUI = new List<SavesMenuSlotBase>();
    protected OkCancelWindowUI _deleteSaveOkCancelWindow;


    public void DeleteSaveSlot(string saveSlotName)
    {
        if (_deleteSaveOkCancelWindow != null)
        {
            Destroy(_deleteSaveOkCancelWindow.gameObject);
        }
        var okCancelWindow = Instantiate(_okCancelWindowPrefab, transform);
        okCancelWindow.RefreshUI($"Вы уверены, что хотите удалить сохранение {saveSlotName}? Это действие необратимо.", delegate
        {
            GameSave.DeleteSave(saveSlotName);
            UpdateSavesSlots();
        });
        _deleteSaveOkCancelWindow = okCancelWindow;
    }

    protected virtual void UpdateSavesSlots()
    {
       
    }

    protected void DeleteSlots()
    {
        _slotsUI.ForEach(slot =>
        {
            Destroy(slot.gameObject);
        });
        _slotsUI.Clear();
    }
}
