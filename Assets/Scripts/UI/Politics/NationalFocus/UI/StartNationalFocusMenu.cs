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
    [SerializeField] private RectTransform _focusesEffectsUIParent;


    public void RefreshUI(CountryPolitics countryPolitics, NationalFocus focus)
    {
        _startNationalFocusButton.interactable = true; //чтобы бага не было
        _nameText.text = focus.Name;
        _focusImage.sprite = focus.Image;

        if (focus.FocusEffects.Count == 0)
        {
            _descriptionText.text = "В данный момент не имеет эффектов.";
        }
        var focusEffectsText = "";
        foreach (var effect in focus.FocusEffects)
        {
            focusEffectsText += effect.GetEffectDescription() + "\n";
        }
        _descriptionText.text = focusEffectsText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_focusesEffectsUIParent);

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
