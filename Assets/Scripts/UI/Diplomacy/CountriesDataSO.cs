using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CountriesData", menuName = "ScriptableObjects/CountriesData", order = 1)]
public class CountriesDataSO : ScriptableObject
{
    public List<CountrySO> Countries = new List<CountrySO>();
}
