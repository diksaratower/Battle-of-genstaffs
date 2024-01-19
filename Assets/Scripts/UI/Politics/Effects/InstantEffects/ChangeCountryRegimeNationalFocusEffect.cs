using UnityEngine;


[CreateAssetMenu(fileName = "ChangeCountryRegime", menuName = "ScriptableObjects/Focuses/Effect/ChangeCountryRegime", order = 1)]
public class ChangeCountryRegimeNationalFocusEffect : InstantEffect
{
    public Ideology NewIdeology;

    public override void DoEffect(Country country)
    {
        if (country.Politics.CountryIdeology == NewIdeology)
        {
            return;
        }
        else
        {
            var lastLeader = country.Politics.CountryLeader;
            country.Politics.CountryLeader = PoliticsDataSO.GetInstance().MinorLeaders[Random.Range(0, PoliticsDataSO.GetInstance().MinorLeaders.Count - 1)];
            if (EventsViewUI.Instance == null)
            {
                return;
            }
            if (NewIdeology == Ideology.Fascism)
            {
                var panel = EventsViewUI.Instance.ViewNewsEvent($@"������� ����� ��������� ��������� � {country.Name}. ������, �������������� ���������� ������, ��������� ����������������� ������ � ���������. ����� ������ {lastLeader.Name} ��� ���� ��� ������� �������������.

������� �������� � �������� ������ �������������, ������� ��������� {country.Politics.CountryLeader.Name}, ��� ����� ��������� ���� �� ���������� ������������ ������������ � �������������� ������������ ���������. ��� ����� ��������� �������� ������ � ������� ������ � �����.

����� ����� ������ �������� ����� ���������. � ���������� ������� ������ �������� ������������, ������� ���� ������� ��������� ��������.");

                panel.AddCloseButton("����...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
            if (NewIdeology == Ideology.Communism)
            {
                var panel = EventsViewUI.Instance.ViewNewsEvent($@"��������� � {country.Name}. ��������� �������� � ���������� ������� ������� �����������. �������, ����������� ������� ����������, ������� ��������� ����� � ����������� ���������� ��������, ����� �� �����. �� �� ����� ����������� {country.Politics.CountryLeader.Name}

������������� ���������� �������� ��������� �����, �� ��� ��������� ��������. ������� ��������� ����������������� ������ � ����������. ����� ������ {lastLeader.Name} ��� �������� � ���������.

���������� �������� � �������� ������ �������������, ������� ����� ��������� ���� �� ���������� �������������� � ���������. ��� ����� ��������� �������� �������������� �������������� � ��������� ���������.");
                panel.AddCloseButton("��� �� ����� ������?").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
        }
    }

    public override string GetEffectDescription()
    {
        return $"������ �������� ������ �� {NewIdeology}";
    }
}
