using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class OkCancelWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _windowText;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _cancelButton;


    public void RefreshUI(string windowText, Action okClickAction)
    {
        _windowText.text = windowText;
        _okButton.onClick.AddListener(delegate 
        {
            okClickAction?.Invoke();
            CloseWindow();
        });
        _cancelButton.onClick.AddListener(delegate 
        {
            CloseWindow();
        });
    }

    private void CloseWindow()
    {
        Destroy(gameObject);
    }
}
