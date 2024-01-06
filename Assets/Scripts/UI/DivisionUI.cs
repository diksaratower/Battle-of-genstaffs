using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DivisionUI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Outline _outLineImage;
    [SerializeField] private Image _organisationFill;
    [SerializeField] private Image _equipmentFill;
    [SerializeField] private Image _divisionAvatar;
    [SerializeField] private Image _flagImage;
    [SerializeField] private TextMeshProUGUI _divisionsCountText;

    public DivisionView _targetView;
    private RectTransform _rectTransform;
    private GameIU _gameIU;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //RemoveNotNeedDivisions();
        if (_targetView.Divisions.Count == 0)
        {
            return;
        }
        _divisionsCountText.text = _targetView.Divisions.Count.ToString();
        _rectTransform.anchoredPosition = GameCamera.Instance.WorldToScreenPointResolutionTrue(_targetView.Divisions[0].DivisionProvince.Position);
        //(GameCamera.Instance._camera.WorldToScreenPoint(Divisions[0].transform.position) * GameCamera.Instance.CanvasScaler.referenceResolution);
        
        
        _organisationFill.fillAmount = GetAverageOrganisation();
        _equipmentFill.fillAmount = GetOverageEquipmentProcent();
        _outLineImage.enabled = _targetView.Selected;
    }
    /*
    private void RemoveNotNeedDivisions()
    {
        if(Divisions.Count == 0)
        {
            return;
        }
        Divisions.RemoveAll(division => division == null);
        var notNeededDivisions = new List<Division>();
        foreach (var division in Divisions)
        {
            if (division != null)
            {
                if (division.NeedDrawDivisionUI() == false)
                {
                    notNeededDivisions.Add(division);
                }
            }
        }
        notNeededDivisions.ForEach(division => 
        {
            if (Divisions.Contains(division)) 
            {
                Divisions.Remove(division);
            }
        });
    }*/

    private float GetAverageOrganisation()
    {
        var amountList = new List<float>();
        var amountsSumm = 0f;
        foreach (Division division in _targetView.Divisions)
        {
            var amount = division.Organization / division.MaxOrganization;
            amountsSumm += amount;
            amountList.Add(amount);
        }
        float averageOrg = amountsSumm / amountList.Count;
        return averageOrg;
    }

    private float GetOverageEquipmentProcent()
    {
        var amountList = new List<float>();
        var amountsSumm = 0f;
        foreach (Division division in _targetView.Divisions)
        {
            var amount = division.GetBattleStrengh();
            amountsSumm += amount;
            amountList.Add(amount);
        }
        float averageEqp = amountsSumm / amountList.Count;
        return averageEqp;
    }

    public void RefreshUI(DivisionView divisionView, GameIU UI)
    {
        _targetView = divisionView;
        /*
        Divisions.Clear();
        Divisions.AddRange(target);
        TargetProvince = province;
        foreach (Division division in Divisions)
        {
            division.OnDivisionRemove += delegate 
            {
                if (Divisions.Contains(division))
                {
                    Divisions.Remove(division);
                }
            };
            division.OnDivisionEnterToProvince += (Province pr) =>
            {
                if (pr != TargetProvince)
                {
                    if (Divisions.Contains(division))
                    {
                        Divisions.Remove(division);
                    }
                    if (divisionsUI.Exists(dUI => dUI.TargetProvince == pr))
                    {
                        divisionsUI.Find(dUI => dUI.TargetProvince == pr).Divisions.Add(division);
                    }
                }
            };
        }*/
        _rectTransform.anchoredPosition = GameCamera.Instance.WorldToScreenPointResolutionTrue(_targetView.Divisions[0].DivisionProvince.Position);
        _divisionsCountText.text = _targetView.Divisions.Count.ToString();
        _gameIU = UI;
        _divisionAvatar.sprite = _targetView.Divisions[0].DivisionAvatar;
        _outLineImage.enabled = _targetView.Selected;
        if (_targetView.Divisions[0].CountyOwner) _flagImage.sprite = _targetView.Divisions[0].CountyOwner.Flag;
    }

    public void SelectButtonClick()
    {
        foreach (Division division in _targetView.Divisions)
        {
            _gameIU.OnDivisionClick(division, _targetView);
        }
    }
}
