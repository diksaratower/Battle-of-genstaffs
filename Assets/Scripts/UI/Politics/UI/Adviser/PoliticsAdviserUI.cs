using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PoliticsAdviserUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _adviserAvatar;
    [SerializeField] private Button _removeAdviserButton;


    public void RefreshUI(Personage personage, Country country)
    {
        _nameText.text = personage.GetName();
        _adviserAvatar.sprite = personage.GetPortrait();
        _removeAdviserButton.onClick.AddListener(delegate
        {
            country.Politics.RemoveAdviser(personage);
        });
    }
}
