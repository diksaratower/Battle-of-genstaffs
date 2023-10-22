using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseDifficultiesButtonUI : MonoBehaviour
{
    public Difficultie TargetDifficultie { get; private set; }

    [SerializeField] private Button _button;
    [SerializeField] private Outline _buttonOutline;
    [SerializeField] private TextMeshProUGUI _buttonName;
    [SerializeField] private ChooseDifficultiesUI _chooseDifficultiesUI;


    private void Update()
    {
        _buttonOutline.enabled = (_chooseDifficultiesUI.Selected == this);
    }

    public void RefreshUI(Difficultie difficultie, ChooseDifficultiesUI chooseDifficultiesUI)
    {
        TargetDifficultie = difficultie;
        _chooseDifficultiesUI = chooseDifficultiesUI;
        _buttonName.text = difficultie.Name;
        _button.onClick.AddListener(delegate 
        {
            chooseDifficultiesUI.Selected = this;
        });
        var tooltip = gameObject.AddComponent<NotPrefabTooltipHandlerUI>();

        tooltip.Initialize((TooltipViewMenu menu) =>
        {
            menu.AddSimpleText($"Сложность {difficultie.Name}", false);
            menu.AddSimpleText($"Прирост полит. {GameIU.FloatToStringAddPlus(difficultie.PolitPowerBonusPercent)}%");
            menu.AddSimpleText($"Производство {GameIU.FloatToStringAddPlus(difficultie.ProductionFactor * 100)}%");
        });
    }
}
