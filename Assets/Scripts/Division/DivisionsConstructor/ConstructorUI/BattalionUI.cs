using UnityEngine;
using UnityEngine.UI;

public class BattalionUI : MonoBehaviour
{
    [SerializeField] private Image _battalionImage;
    [SerializeField] private Button _removeBattlionButton;

    public void RefreshUI(Battalion battalion, DivisionLine divisionLine, DivisionTemplateConstructorUI constructorUI)
    {
        _battalionImage.sprite = battalion.BatImage;
        _removeBattlionButton.onClick.AddListener(delegate 
        {
            constructorUI.RemoveBatalion(divisionLine, battalion);
        });
    }
}
