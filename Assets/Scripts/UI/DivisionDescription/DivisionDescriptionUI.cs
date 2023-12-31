using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DivisionDescriptionUI : MonoBehaviour
{
    [SerializeField] private GameObject _windowGO;
    [SerializeField] private TextMeshProUGUI _divisionName;
    [SerializeField] private Image _divisonImage;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _defensText;
    [SerializeField] private TextMeshProUGUI _organizationText;
    [SerializeField] private TextMeshProUGUI _equipmentPercentText;
    [SerializeField] private TextMeshProUGUI _manpowerPercentText;
    [SerializeField] private TextMeshProUGUI _battleStrenghPercentText;
    [SerializeField] private Transform _equipmentSlotsParent;
    [SerializeField] private EquipmentSlotDivisionDescriptionUI _equipmentSlotDivisionDescriptionUIPrefab;

    private List<EquipmentSlotDivisionDescriptionUI> _equipmentSlots = new List<EquipmentSlotDivisionDescriptionUI>();
    private Division _division;


    private void Update()
    {
        if (_division != null && _windowGO.activeSelf == true)
        {
            var equipmentPercent = _division.GetEquipmentProcent(eqType => eqType != EquipmentType.Manpower);
            var manpowerPercent = _division.GetEquipmentProcent(eqType => eqType == EquipmentType.Manpower);
            _equipmentPercentText.text = "������� ���. �����.: " + Math.Round(((equipmentPercent) * 100f), 2).ToString() + "%";
            _manpowerPercentText.text = "������ ������: " + Math.Round(manpowerPercent * 100f, 2).ToString() + "%";
            _battleStrenghPercentText.text = "�������������: " + Math.Round((manpowerPercent * equipmentPercent) * 100f, 2).ToString() + "%";
        }
    }

    public void RefreshUI(Division division)
    {
        _division = division;
        _windowGO.SetActive(true);
        
        _divisionName.text = division.Name;
        _divisonImage.sprite = division.Template.GetAvatar();
        _attackText.text = "�����: " + division.GetAttack();
        _defensText.text = "������: " + division.GetDefense();
        _organizationText.text = "���� �����������: " + division.MaxOrganization;
        UpdateEquipmentDetails(division);
        division.OnGetSupply += delegate
        {
            if (_windowGO.activeSelf == true)
            {
                if (division == _division)
                {
                    UpdateEquipmentDetails(division);
                }
            }
        };
    }

    private void UpdateEquipmentDetails(Division division)
    {
        if (division != _division)
        {
            return;
        }
        _equipmentSlots.ForEach(slotUI =>
        {
            Destroy(slotUI.gameObject);
        });
        _equipmentSlots.Clear();
        foreach (var equipment in division.EquipmentInDivision)
        {
            AddEquipmentSlotUI(equipment);
        }
    }

    private void AddEquipmentSlotUI(EquipmentCountIdPair countIdPair)
    {
        var slotUI = Instantiate(_equipmentSlotDivisionDescriptionUIPrefab, _equipmentSlotsParent);
        slotUI.RefreshUI(countIdPair);
        _equipmentSlots.Add(slotUI);
    }
}
