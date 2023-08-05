using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelCloseButton : MonoBehaviour
{
    public Button CloseButton { get => _button; }

    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private Button _button;
}
