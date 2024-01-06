using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Decision", menuName = "ScriptableObjects/Decision/Simple", order = 1)]
public class Decision : ScriptableObject
{
    public string Name;
    public int PolitPowerCost = 10;
    public List<DecisionEffect> Effects = new List<DecisionEffect>(); 


    public void ActivaieDecision(Country country)
    {
        if (country.Politics.PolitPower < PolitPowerCost)
        {
            return;
        }
        country.Politics.PolitPower -= PolitPowerCost;
        foreach (DecisionEffect effect in Effects) 
        {
            effect.ExecuteDecisionEffect(country);
        }
    }
}
