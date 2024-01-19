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
                var panel = EventsViewUI.Instance.ViewNewsEvent($@"—егодн€ ночью произошЄл переворот в {country.Name}. ¬ойска, подконтрольные фашистской партии, захватили правительственные здани€ и телецентр. Ћидер страны {lastLeader.Name} был убит при попытке сопротивлени€.

‘ашисты объ€вили о создании нового правительства, которое возглавил {country.Politics.CountryLeader.Name}, оно будет проводить курс на укрепление национальной безопасности и восстановление традиционных ценностей. ќни также пообещали провести чистку в органах власти и армии.

Ќовый режим вызвал протесты среди населени€. ¬ нескольких городах прошли массовые демонстрации, которые были жестоко подавлены полицией.");

                panel.AddCloseButton("”жас...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
            if (NewIdeology == Ideology.Communism)
            {
                var panel = EventsViewUI.Instance.ViewNewsEvent($@"ѕереворот в {country.Name}. ¬осстание началось в нескольких крупных городах государства. –абочие, недовольные низкими зарплатами, плохими услови€ми труда и отсутствием социальных гарантий, вышли на улицы. »х вЄл ранее неизвестный {country.Politics.CountryLeader.Name}

ѕравительство попыталось подавить восстание силой, но оно оказалось успешным. –абочие захватили правительственные здани€ и телецентры. Ћидер страны {lastLeader.Name} был свергнут и арестован.

 оммунисты объ€вили о создании нового правительства, которое будет проводить курс на социальную справедливость и равенство. ќни также пообещали провести национализацию промышленности и сельского хоз€йства.");
                panel.AddCloseButton("„то же будет дальше?").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
        }
    }

    public override string GetEffectDescription()
    {
        return $"ћен€ет прав€щую партию на {NewIdeology}";
    }
}
