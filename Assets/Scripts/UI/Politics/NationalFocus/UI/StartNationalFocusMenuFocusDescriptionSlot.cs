using TMPro;
using UnityEngine;


public class StartNationalFocusMenuFocusDescriptionSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _slotText;


    public void RefreshUI(string text)
    {
        _slotText.text = text;
    }
}
