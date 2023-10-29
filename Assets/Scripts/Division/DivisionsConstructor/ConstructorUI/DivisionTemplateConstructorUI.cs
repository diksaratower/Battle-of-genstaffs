using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DivisionTemplateConstructorUI : MonoBehaviour
{
    public List<Battalion> _availableBattalions => TechnologiesManagerSO.GetInstance().AvailableBattalions;
    public DivisionTemplate TargetTemplate { get; private set; }

    [SerializeField] private TextMeshProUGUI _divisionCharacteristicTextPrefab;
    [SerializeField] private GridLayoutGroup _divisionCharacteristicsParent;
    [SerializeField] private Vector2Int _templateGridSize = new Vector2Int(6, 5);
    [SerializeField] private DivisionTemplateConstructorAddBatalionMenu _addBatalionMenu;
    [SerializeField] private DivisionLineUI _divisionLineUIPrefab;
    [SerializeField] private HorizontalLayoutGroup _divisionsLineUILayot;
    [SerializeField] private TMP_InputField _templateNameInputField;

    private List<DivisionLineUI> _divisionLinesUI = new List<DivisionLineUI>();
    private List<TextMeshProUGUI> _divisionCharacteristics = new List<TextMeshProUGUI>();

    private void Update()
    {
        TargetTemplate.Name = _templateNameInputField.text;
    }

    public void RefreshBatalions(DivisionTemplate template)
    {
        TargetTemplate = template;
        _templateNameInputField.text = template.Name;
        _divisionLinesUI.ForEach(ln => { Destroy(ln.gameObject); });
        _divisionLinesUI.Clear();
        foreach (var line in TargetTemplate.DivisionLines)
        {
            var lineUI = Instantiate(_divisionLineUIPrefab, _divisionsLineUILayot.transform);
            lineUI.RefreshUI(line, this, _templateGridSize.y);
            _divisionLinesUI.Add(lineUI);
        }
        RefreshTemplateCharacteristicsUI();
    }

    public void RefreshTemplateCharacteristicsUI()
    {
        _divisionCharacteristics.ForEach(ch => { Destroy(ch.gameObject); });
        _divisionCharacteristics.Clear();
        var attackCh = SpawnTemplateCharacteristic();
        attackCh.text = "Атака: " + TargetTemplate.Attack.ToString();
        _divisionCharacteristics.Add(attackCh);
        var defendCh = SpawnTemplateCharacteristic();
        defendCh.text = "Защита: " + TargetTemplate.Defend.ToString();
        _divisionCharacteristics.Add(defendCh);
        var organizationCh = SpawnTemplateCharacteristic(); 
        organizationCh.text = "Организация: " + TargetTemplate.Organization.ToString();
        _divisionCharacteristics.Add(organizationCh);
        var speedCh = SpawnTemplateCharacteristic(); 
        speedCh.text = "Скорость: " + TargetTemplate.Speed.ToString();
        _divisionCharacteristics.Add(speedCh);


        var needEquipment = TargetTemplate.GetTemplateNeedEquipmentKeyValPair();
        foreach (var ne in needEquipment)
        {
            var needEquipmentCh = SpawnTemplateCharacteristic();
            needEquipmentCh.text = ne.Key + ": " + ne.Value;
            _divisionCharacteristics.Add(needEquipmentCh);
        }
    }

    public TextMeshProUGUI SpawnTemplateCharacteristic()
    {
        return Instantiate<TextMeshProUGUI>(_divisionCharacteristicTextPrefab, _divisionCharacteristicsParent.transform);
    }

    public void OpenAddBattalionMenu(DivisionLine divisionLine)
    {
        _addBatalionMenu.RefreshUI(divisionLine, this, Player.CurrentCountry);
        _addBatalionMenu.gameObject.SetActive(true);
    }

    public void AddBatalion(DivisionLine line, Battalion battalion)
    {
        line.Battalions.Add(battalion);
        RefreshBatalions(TargetTemplate);
    }

    public void RemoveBatalion(DivisionLine line, Battalion battalion)
    {
        if(TargetTemplate.Battalions.Count == 1)
        {
            return;
        }
        line.Battalions.Remove(battalion);
        RefreshBatalions(TargetTemplate);
    }

    public static DivisionTemplate GetDefaultTemplate(string tName)
    {
        var newTemp = new DivisionTemplate(tName);
        for (int i = 0; i < 6; i++)
        {
            newTemp.DivisionLines.Add(new DivisionLine());
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                newTemp.DivisionLines[i].Battalions.Add(TechnologiesManagerSO.GetInstance().AvailableBattalions[0]);
            }
        }
        return newTemp;
    }

    public static DivisionTemplate GetAITemplate(int linesCount, int battalionsInLine)
    {
        var newTemp = new DivisionTemplate("infantry_1");
        for (int i = 0; i < 6; i++)
        {
            newTemp.DivisionLines.Add(new DivisionLine());
        }
        for (int i = 0; i < linesCount; i++)
        {
            for (int j = 0; j < battalionsInLine; j++)
            {
                newTemp.DivisionLines[i].Battalions.Add(TechnologiesManagerSO.GetInstance().AvailableBattalions[0]);
            }
        }
        return newTemp;
    }

}
