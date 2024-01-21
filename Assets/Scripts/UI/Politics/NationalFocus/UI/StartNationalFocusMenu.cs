using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StartNationalFocusMenu : MonoBehaviour
{
    [SerializeField] private Button _startNationalFocusButton;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _focusImage;
    [SerializeField] private TextMeshProUGUI _focusDuratationText;
    [SerializeField] private RectTransform _focusesEffectsUIParent;
    [SerializeField] private StartNationalFocusMenuFocusDescriptionSlot _focusDescriptionSlotPrefab;

    private List<StartNationalFocusMenuFocusDescriptionSlot> _descriptionSlotsUI = new List<StartNationalFocusMenuFocusDescriptionSlot>();


    public void RefreshUI(CountryPolitics countryPolitics, NationalFocus focus)
    {
        _startNationalFocusButton.interactable = true; //����� ���� �� ����
        _nameText.text = focus.Name;
        _focusImage.sprite = focus.Image;

        ClearDescription();
        AddConditionsDescription(focus);
        AddDescriptionText("\n");
        AddEffectesDescription(focus);

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
        _focusDuratationText.text = "����� ����������: " + focus.ExecutionDurationDay.ToString() + " ����";
    }

    private void AddConditionsDescription(NationalFocus focus)
    {
        if (focus.FocusConditions.Count != 0)
        {
            AddDescriptionText("����������� �������:");
            foreach (var condition in focus.FocusConditions)
            {
                AddDescriptionText(condition.GetConditionDescription());
            }
        }
    }

    private void AddEffectesDescription(NationalFocus focus)
    {
        AddDescriptionText("������� ����� ����������:");
        if (focus.FocusEffects.Count == 0)
        {
            AddDescriptionText("� ������ ������ �� ����� ��������.");
        }
        else
        {
            foreach (var effect in focus.FocusEffects)
            {
                AddDescriptionText(effect.GetEffectDescription());
            }
        }
    }

    private void AddDescriptionText(string text)
    {
        var slotUI = Instantiate(_focusDescriptionSlotPrefab, _focusesEffectsUIParent);
        slotUI.RefreshUI(text);
        _descriptionSlotsUI.Add(slotUI);
    }

    private void ClearDescription()
    {
        foreach (var slot in _descriptionSlotsUI)
        {
            Destroy(slot.gameObject);
        }
        _descriptionSlotsUI.Clear();
    }
}
