using UnityEngine;


public class EventsViewUI : MonoBehaviour
{
    public static EventsViewUI Instance { get; private set; }

    [SerializeField] private EventPanelUI _panelPrefab;

    private string _communismCapitulateText = @"����������� {1} ������� ��� ������������ ��������. ����� ����������� {0} ��������, ��� {1} ����������� ��� � {2} ����� ����� �� ��� ������������� � ��������� �������� ����������";


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Player.CurrentCountry.CountryDiplomacy.OnGetUltimatum += (Ultimatum ultimatum) =>
        {
            var panel = ViewNewsEvent(EventsViewUI.GetUltimatumText(ultimatum), "��� ��������...");

            panel.AddCloseButton("�����������").CloseButton.onClick.AddListener(delegate 
            {
                ultimatum.SendAnser(UltimatumAnswerType.Yes);
                panel.gameObject.SetActive(false);
            });
            panel.AddCloseButton("����������").CloseButton.onClick.AddListener(delegate 
            {
                ultimatum.SendAnser(UltimatumAnswerType.No);
                panel.gameObject.SetActive(false);
            });
            panel.SetEventDataTracking(() => (ultimatum.GetAutoAnserTimeDays - ultimatum.GetAutoAnserProgressDays));
        };
        Player.CurrentCountry.CountryDiplomacy.OnGetUltimatumAnser += (UltimatumAnswerType ultimatumAnswerType, Ultimatum ultimatum) => 
        {
            if (ultimatumAnswerType == UltimatumAnswerType.No)
            {
                var panel = ViewNewsEvent($"����������� {ultimatum.Target.Name} ��������� ���� ����������.", "��� �� ��� ����������.");
                panel.AddCloseButton("������� ������� �� �����...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
        };
        Map.Instance.GetCountryFromId("aust").OnAnnexed += (Country annexer) =>
        {
            if (annexer.ID == "ger")
            {
                var panel = ViewNewsEvent(@"������� ���������� ������, ������� ��������������� �� ������� � ������������ � ������ �����, ����� �� ���������� �������. ����������� �����, ���������� ������ �� ��������� �������������, ��������������.", "������ �������");

                panel.AddCloseButton("��� �� � �����...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
        };
        Map.Instance.GetCountryFromId("cze").OnAnnexed += (Country annexer) =>
        {
            if (annexer.ID == "ger")
            {
                var panel = ViewNewsEvent(@"������� ���������� ������ ����� �� ���������� ������������. ������������� ����� �� ������ ������ ������� ������ ������� ��������.", "�������� ������������");

                panel.AddCloseButton("��� ����������...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
        };
    }

    public EventPanelUI ViewNewsEvent(string eventText, string eventName = "�������")
    {
        var panel = Instantiate(_panelPrefab, transform);
        panel.RefreshUI(eventName, eventText);
        return panel;
    }

    public static string GetCapitulationText(Country country, Country invader)
    {
        return $"��� ����� {invader.Politics.CountryLeader.Name} ������� ������, ��� ����������� {country.Name} ������� ��� ������ � �������.";
    }

    public static string GetUltimatumText(Ultimatum ultimatum)
    {
        var ultimatumText = "";
        if (ultimatum is AnnexCountryUltimatum)
        {
            ultimatumText = $@"����� ����������� {ultimatum.Sender.Name} {ultimatum.Sender.Politics.CountryLeader.Name} ������, ��� �����, ����� ���� ����������� ����� � ������ �� ������. � ��������� ������ ��� �������� ���������
���� ���-�� ������, �� ������ - ��� �������� ���������������� ����� � ����������";
        }
        if (ultimatum is AnnexRegionUltimatum)
        {
            ultimatumText = $"����� ����������� {ultimatum.Sender.Name} {ultimatum.Sender.Politics.CountryLeader.Name} ������, ��� ����� ����� �� ������ ������ {(ultimatum as AnnexRegionUltimatum).AnnexedRegion.Name}.";
        }
        return ultimatumText; 
    }
}
