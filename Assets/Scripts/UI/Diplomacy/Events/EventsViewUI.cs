using UnityEngine;


public class EventsViewUI : MonoBehaviour
{
    public static EventsViewUI Instance { get; private set; }

    [SerializeField] private EventPanelUI _panelPrefab;

    private string _communismCapitulateText = @"Государство {1} сдалось под ожесточённым натиском. Глава государства {0} заявляет, что {1} суверенитет пал и {2} будет велик всё своё существование и уничтожит западный капитализм";


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Player.CurrentCountry.CountryDiplomacy.OnGetUltimatum += (Ultimatum ultimatum) =>
        {
            var panel = ViewNewsEvent(EventsViewUI.GetUltimatumText(ultimatum), "Это тревожно...");

            panel.AddCloseButton("Согласиться").CloseButton.onClick.AddListener(delegate 
            {
                ultimatum.SendAnser(UltimatumAnswerType.Yes);
                panel.gameObject.SetActive(false);
            });
            panel.AddCloseButton("Отказаться").CloseButton.onClick.AddListener(delegate 
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
                var panel = ViewNewsEvent($"Государство {ultimatum.Target.Name} отклонило наши требования.", "Они за это поплатятся.");
                panel.AddCloseButton("Мирного решения не будет...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
        };
        Map.Instance.GetCountryFromId("aust").OnAnnexed += (Country annexer) =>
        {
            if (annexer.ID == "ger")
            {
                var panel = ViewNewsEvent(@"Сегодня германские войска, заранее сосредоточенные на границе в соответствии с планом «Отто», вошли на территорию Австрии. Австрийская армия, получившая приказ не оказывать сопротивления, капитулировала.", "Аншлюс Австрии");

                panel.AddCloseButton("Это не к добру...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
        };
        Map.Instance.GetCountryFromId("cze").OnAnnexed += (Country annexer) =>
        {
            if (annexer.ID == "ger")
            {
                var panel = ViewNewsEvent(@"Сегодня германские войска вошли на территорию Чехословакии. Чехословацкая армия не смогла ничего сделать против натиска вермахта.", "Аннексия Чехословакии");

                panel.AddCloseButton("Это катастрофа...").CloseButton.onClick.AddListener(delegate
                {
                    panel.gameObject.SetActive(false);
                });
            }
        };
    }

    public EventPanelUI ViewNewsEvent(string eventText, string eventName = "Новости")
    {
        var panel = Instantiate(_panelPrefab, transform);
        panel.RefreshUI(eventName, eventText);
        return panel;
    }

    public static string GetCapitulationText(Country country, Country invader)
    {
        return $"Наш лидер {invader.Politics.CountryLeader.Name} сегодня заявил, что государство {country.Name} сложило своё оружие и сдалось.";
    }

    public static string GetUltimatumText(Ultimatum ultimatum)
    {
        var ultimatumText = "";
        if (ultimatum is AnnexCountryUltimatum)
        {
            ultimatumText = $@"Лидер государства {ultimatum.Sender.Name} {ultimatum.Sender.Politics.CountryLeader.Name} заявил, что хочет, чтобы наше государство вошло в состав их страны. В противном случае нам угрожает вторжение
«Они что-то мошнят, ща влетят» - так ситуацию прокомментировал некто в парламенте";
        }
        if (ultimatum is AnnexRegionUltimatum)
        {
            ultimatumText = $"Лидер государства {ultimatum.Sender.Name} {ultimatum.Sender.Politics.CountryLeader.Name} заявил, что хочет чтобы мы отдали регион {(ultimatum as AnnexRegionUltimatum).AnnexedRegion.Name}.";
        }
        return ultimatumText; 
    }
}
