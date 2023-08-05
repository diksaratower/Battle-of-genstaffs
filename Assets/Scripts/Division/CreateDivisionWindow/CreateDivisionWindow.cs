using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreateDivisionWindow : MonoBehaviour
{
    [SerializeField] private Button _choosePlaseButton;
    [SerializeField] private TextMeshProUGUI _spawnPlaceViewText;
    [SerializeField] private bool _isChoosingPlase;
    [SerializeField] private GridLayoutGroup _templatesViewSlotsParent;
    [SerializeField] private CreateDivisionTemplateViewSlot _slotPrefab;
    [SerializeField] private Button _createNewTemplateButton;
    [SerializeField] private DivisionTemplateConstructorUI _templateConstructor;
    [SerializeField] private TMP_Dropdown _chooseTemplatesDropdown;
    [SerializeField] private Button _createDivisionButton;
    [SerializeField] private TMP_InputField _chooseCountField;

    private List<GameObject> _templatesViewSlots = new List<GameObject>();
    private Country _country => Player.CurrentCountry;
    private Province _spawnDivPlace;

    private void Start()
    {
        _choosePlaseButton.onClick.AddListener(delegate
        {
            _isChoosingPlase = true;
            _choosePlaseButton.interactable = false;
        });
        _createNewTemplateButton.onClick.AddListener(delegate
        {
            var newTemp = DivisionTemplateConstructorUI.GetDefaultTemplate("Новый шаблон " + (_country.Templates.Templates.Count + 1));
            _country.Templates.Templates.Add(newTemp);
            RefreshTemplates();
            RefreshTemplatesChooseDropdown();
        });
        _createDivisionButton.onClick.AddListener(delegate
        {
            if (CanAddDivisionsToCreate(out int divisionsCount))
            {
                for (int i = 0; i < divisionsCount; i++)
                {
                    CreateDivisionInPlace(_country.Templates.Templates[_chooseTemplatesDropdown.value]);
                }
            }
        });
        RefreshTemplates();
        RefreshTemplatesChooseDropdown();
    }

    private void OnEnable()
    {
        RefreshTemplates();
        RefreshTemplatesChooseDropdown();
    }

    private void Update()
    {
        _createDivisionButton.interactable = CanAddDivisionsToCreate(out int _);
        if(_spawnDivPlace != null)
        {
            _spawnPlaceViewText.text = "Province ID:" + Map.Instance.Provinces.IndexOf(_spawnDivPlace);
        }
        else
        {
            _spawnPlaceViewText.text = "Место не выбрано";
        }
        if (_isChoosingPlase)
        {
            _spawnPlaceViewText.text = "Нажмите на любую точку карты";
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (GameCamera.Instance.ChekHitToProvinceWithMousePosition(out Province prov))
                {
                    if (prov.Owner == _country)
                    {
                        _spawnDivPlace = prov;
                        _isChoosingPlase = false;
                        _choosePlaseButton.interactable = true;
                    }
                }
            }
        }
    }

    private bool CanAddDivisionsToCreate(out int count)
    {
        count = -1;
        if (int.TryParse(_chooseCountField.text, out int divisionsCount))
        {
            if (divisionsCount <= 0)
            {
                return false;
            }
            if (_spawnDivPlace != null && _chooseTemplatesDropdown.value > -1 &&
            _country.CreationDivisions.MaxQueueSlots >= (_country.CreationDivisions.CreationQueue.Count + divisionsCount))
            {
                if (divisionsCount < 0)
                {
                    divisionsCount = 1;
                }
                count = divisionsCount;
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private void RefreshTemplates()
    {
        _templatesViewSlots.ForEach(sl => Destroy(sl.gameObject));
        _templatesViewSlots.Clear();
        foreach (var templ in _country.Templates.Templates)
        {
            var sl = Instantiate(_slotPrefab, _templatesViewSlotsParent.transform);
            sl.RefreshUI(templ, this);
            _templatesViewSlots.Add(sl.gameObject);
        }
    }

    private void RefreshTemplatesChooseDropdown()
    {
        _chooseTemplatesDropdown.options.Clear();
        foreach (var templ in _country.Templates.Templates)
        {
            _chooseTemplatesDropdown.options.Add(new TMP_Dropdown.OptionData(templ.Name));
        }
    }

    public void CreateDivisionInPlace(DivisionTemplate divisionTemplate)
    {
        if (_spawnDivPlace == null) 
        { 
            throw new NullReferenceException("Spawn place is null.");
        }
        if (divisionTemplate == null)
        {
            throw new NullReferenceException("Template is null.");
        }
        var divisionName = divisionTemplate.Name + " " + "(" +
                    UnitsManager.Instance.Divisions.FindAll(d => (d.Template == divisionTemplate)).Count.ToString() + ")";
        _country.CreationDivisions.AddDivisionCreation(divisionTemplate, _spawnDivPlace, divisionName);
    }

    public void OpenDivisionEditor(DivisionTemplate divisionTemplate)
    {
        _templateConstructor.RefreshBatalions(divisionTemplate);
        _templateConstructor.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
