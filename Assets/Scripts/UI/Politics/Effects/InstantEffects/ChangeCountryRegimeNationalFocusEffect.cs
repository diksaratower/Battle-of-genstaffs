using UnityEngine;


[CreateAssetMenu(fileName = "ChangeCountryRegime", menuName = "ScriptableObjects/Focuses/Effect/ChangeCountryRegime", order = 1)]
public class ChangeCountryRegimeNationalFocusEffect : InstantEffect
{
    public PoliticalParty NewParty;

    public override void DoEffect(Country country)
    {
        if (country.Politics.CountryIdeology == NewParty.PartyIdeology)
        {
            return;
        }
        else
        {
            var lastLeader = country.Politics.CountryLeader;
            var newLeader = PoliticsDataSO.GetInstance().MinorLeaders[Random.Range(0, PoliticsDataSO.GetInstance().MinorLeaders.Count - 1)];
            country.Politics.ChangeRegime(newLeader, NewParty);
            if (EventsViewUI.Instance == null)
            {
                return;
            }
            if (NewParty.PartyIdeology == Ideology.Fascism)
            {
                var panel = EventsViewUI.Instance.ViewNewsEvent($@"������� ����� ��������� ��������� � {country.Name}. ������, �������������� ���������� ������, ��������� ����������������� ������ � ���������. ����� ������ {lastLeader.Name} ��� ���� ��� ������� �������������.

������� �������� � �������� ������ �������������, ������� ��������� {country.Politics.CountryLeader.Name}, ��� ����� ��������� ���� �� ���������� ������������ ������������ � �������������� ������������ ���������. ��� ����� ��������� �������� ������ � ������� ������ � �����.

����� ����� ������ �������� ����� ���������. � ���������� ������� ������ �������� ������������, ������� ���� ������� ��������� ��������.");

                panel.AddCloseButton("����...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
            if (NewParty.PartyIdeology == Ideology.Communism)
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
        return $"������ �������� ������ �� {NewParty.Name}";
    }
}
