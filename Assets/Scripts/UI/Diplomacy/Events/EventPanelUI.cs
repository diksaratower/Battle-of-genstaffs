using System;
using TMPro;
using UnityEngine;


public class EventPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _eventText;
    [SerializeField] private TextMeshProUGUI _eventName;
    [SerializeField] private TextMeshProUGUI _eventTimeText;
    [SerializeField] private EventPanelCloseButton _panelButtonPrefab;
    [SerializeField] private Transform _buttonsLayoutGroup;

    private Func<int> _getCurrentDate;

    private void Update()
    {
        if (_getCurrentDate != null && _eventTimeText.gameObject.activeSelf)
        {
            _eventTimeText.text = $"Осталось {_getCurrentDate?.Invoke()} дней.";
        }
    }

    public void RefreshUI(string eventName, string eventText)
    {
        _eventName.text = eventName;
        _eventText.text = eventText;
    }
    
    public void SetEventDataTracking(Func<int> getCurrentDateFunc)
    {
        _getCurrentDate = getCurrentDateFunc;
        _eventTimeText.gameObject.SetActive(true);
    }

    public EventPanelCloseButton AddCloseButton(string text)
    {
        var button = Instantiate(_panelButtonPrefab, _buttonsLayoutGroup.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        return button;
    }
}
