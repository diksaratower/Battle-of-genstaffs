using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoliticsAdviserUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _adviserAvatar;

    public void RefreshUI(Personage personage)
    {
        _nameText.text = personage.GetName();
        _adviserAvatar.sprite = personage.GetPortrait();
    }
}
