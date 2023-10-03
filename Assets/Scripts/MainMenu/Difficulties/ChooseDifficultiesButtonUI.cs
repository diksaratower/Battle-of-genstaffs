using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseDifficultiesButtonUI : MonoBehaviour
{
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
        _chooseDifficultiesUI = chooseDifficultiesUI;
        _buttonName.text = difficultie.Name;
        _button.onClick.AddListener(delegate 
        {
            chooseDifficultiesUI.Selected = this;
        });
    }
}
