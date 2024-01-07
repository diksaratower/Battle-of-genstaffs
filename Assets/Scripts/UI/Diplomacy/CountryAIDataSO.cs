using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CountryAIDataSO", menuName = "ScriptableObjects/CountryAIDataSO", order = 1)]
public class CountryAIDataSO : ScriptableObject
{
    public NationalFocusTree StandartFocusTree;
    public List<CountrAIChoosingIdeologyVariant> ChoosingIdeologyVariantsInStandardTree = new List<CountrAIChoosingIdeologyVariant>();

    private static CountryAIDataSO _instance;


    public static CountryAIDataSO GetInstance()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<CountryAIDataSO>("CountryAIDataSO");
        }
        return _instance;
    }
}

[Serializable]
public class CountrAIChoosingIdeologyVariant
{
    public NationalFocus Focus;
    public Ideology FocusIdealogy;
}