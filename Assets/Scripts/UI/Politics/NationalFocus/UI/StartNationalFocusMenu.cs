using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartNationalFocusMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _startNationalFocusButton;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _focusImage;
    [SerializeField] private TextMeshProUGUI _focusDuratationText;

    public void RefreshUI(CountryPolitics countryPolitics, NationalFocus focus)
    {
        _startNationalFocusButton.interactable = true; //чтобы бага не было
        _nameText.text = focus.Name;
        _focusImage.sprite = focus.Image;
        foreach (var effect in focus.FocusEffects)
        {
            _descriptionText.text = "\n" + effect.GetEffectDescription();
        }

        if (countryPolitics.CanExecute(focus) == false)
        {
            _startNationalFocusButton.interactable = false;
        }


        _startNationalFocusButton.onClick.AddListener(delegate
        {
            countryPolitics.SetExecutingFocus(focus);
            gameObject.SetActive(false);
        });
        _focusDuratationText.text = "Время выполнения: " + focus.ExecutionDurationDay.ToString() + " дней";
    }
}
