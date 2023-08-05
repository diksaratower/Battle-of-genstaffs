using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class WarMemberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _memberName;
    [SerializeField] private Image _memberFlag;
    [SerializeField] private TextMeshProUGUI _percentOfCapitulate;
    [SerializeField] private TextMeshProUGUI _manpowerLosses;

    private WarMember _warMember;

    private void Update()
    {
        if (_warMember != null)
        {
            UpdatePercentCapitulation();
            _manpowerLosses.text = ToRoundThousand(_warMember.ManPowerLosses) + "ê";
        }
    }

    public void RefreshUI(WarMember warMember)
    {
        _warMember = warMember;
        _memberName.text = warMember.Country.Name;
        _memberFlag.sprite = warMember.Country.Flag;
        UpdatePercentCapitulation();
    }

    private void UpdatePercentCapitulation()
    {
        _percentOfCapitulate.text = Math.Round(_warMember.Country.CapitulatePercent * 100f, 2) + "%";
    }

    private float ToRoundThousand(float value)
    {
        return (float)Math.Round(value / 1000, 1);
    }
}
