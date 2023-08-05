using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DivisionLineUI : MonoBehaviour
{
    [SerializeField] private BattalionUI _battalionPrefab;
    [SerializeField] private Button _addBattlionButtonPrefab;
    [SerializeField] private VerticalLayoutGroup _verticalBattalionsLayout;

    public void RefreshUI(DivisionLine divisionLine, DivisionTemplateConstructorUI constructorUI, int maxBatsCount)
    {
        foreach (var bat in divisionLine.Battalions)
        {
            var batUI = Instantiate(_battalionPrefab, _verticalBattalionsLayout.transform);
            batUI.RefreshUI(bat, divisionLine, constructorUI);
        }
        if (divisionLine.Battalions.Count < maxBatsCount)
        {
            var addButton = Instantiate(_addBattlionButtonPrefab, _verticalBattalionsLayout.transform);
            addButton.onClick.AddListener(() =>
            {
                constructorUI.OpenAddBattalionMenu(divisionLine);
            });
        }

    }
}
