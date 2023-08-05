using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DivisionTemplateConstructorAddBatalionMenuSlotUI : MonoBehaviour
{
    [SerializeField] private Image _battalionImage;
    [SerializeField] private TextMeshProUGUI _battalionName;
    [SerializeField] private Button _clickButton;

    public void RefreshUI(Battalion battalion, DivisionLine divisionLine, DivisionTemplateConstructorUI constructorUI, DivisionTemplateConstructorAddBatalionMenu addBatalionMenu)
    {
        _battalionImage.sprite = battalion.BatImage;
        _battalionName.text = battalion.Name;
        _clickButton.onClick.AddListener(delegate 
        {
            constructorUI.AddBatalion(divisionLine, battalion);
            addBatalionMenu.gameObject.SetActive(false);
        });
    }
}
