using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddAdviserMenuUI : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _addAdvisersLayoutParent;
    [SerializeField] private AddAdviserButtonUI _addAdviserUIPrefab;
    [SerializeField] private PolticsUI _poiliticsUI;

    private List<AddAdviserButtonUI> _addAdvisersUI = new List<AddAdviserButtonUI>();

    public void RefreshUI(List<Personage> advisers)
    {
        _addAdvisersUI.ForEach(adv => Destroy(adv.gameObject));
        _addAdvisersUI.Clear();
        foreach (var pers in advisers)
        {
            var slot = Instantiate(_addAdviserUIPrefab, _addAdvisersLayoutParent.transform);
            slot.RefreshUI(pers, _poiliticsUI);
            _addAdvisersUI.Add(slot);
        }
    }
}
