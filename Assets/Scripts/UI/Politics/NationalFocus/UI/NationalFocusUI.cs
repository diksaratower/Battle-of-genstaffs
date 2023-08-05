using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NationalFocusUI : MonoBehaviour
{
    public NationalFocus Focus { get; private set; }

    [SerializeField] private TextMeshProUGUI _focusName;
    [SerializeField] private Image _focusImage;
    [SerializeField] protected Button _executeButton;
    [SerializeField] private Image _executedIcone;

    public void RefreshUI(NationalFocus focus, CountryPolitics countryPolitics, StartNationalFocusMenu startNationalFocusMenu)
    {
        Focus = focus;
        _focusName.text = focus.Name;
        _focusImage.sprite = focus.Image;
        _executeButton.onClick.AddListener(delegate { 
            startNationalFocusMenu.gameObject.SetActive(true);
            startNationalFocusMenu.RefreshUI(countryPolitics, focus);
        });
        var executed = countryPolitics.IsExecutedFocus(focus);
        if (executed == true)
        {
            _executedIcone.gameObject.gameObject.SetActive(true);
        }
        if(executed == false) 
        {
            if(countryPolitics.CanExecute(focus) == false)
            {
                //_executeButton.interactable = false;
            }
        }
    }
}
