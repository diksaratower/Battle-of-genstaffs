using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PartiesPopularityChartUI : MonoBehaviour
{
    [SerializeField] private Image _chartElementPrefab;

    public void RefreshUI(List<PartyPopular> populars)
    {
        if(populars.Count == 0) 
        {
            return;
        }
        float startProcent = 0;
        foreach (var popular in populars)
        {
            var el = Instantiate(_chartElementPrefab, transform);
            el.fillAmount = (popular.ProcentPopularity / 100);
            el.transform.localEulerAngles = new Vector3(0, 0, ((startProcent / 100) * 360));
            el.color = popular.PartyColor;
            startProcent -= popular.ProcentPopularity;
        }
    }

}

[Serializable]
public class PartyPopular
{
    [field:SerializeField] public float ProcentPopularity { get; private set; }
    public Ideology PartyIdeology;
    public string Name;
    public Color PartyColor;

    public PartyPopular(float procentPopularity, string name, Color partyColor, Ideology partyIdeology)
    {
        ProcentPopularity = procentPopularity;
        Name = name;
        PartyColor = partyColor;
        PartyIdeology = partyIdeology;
    }

    public void ChangePopular(float offest)
    {
        ProcentPopularity += offest;
        if(ProcentPopularity > 100)
        {
            ProcentPopularity = 100;
        }
        if (ProcentPopularity < 0)
        {
            ProcentPopularity = 0;
        }
    }
}