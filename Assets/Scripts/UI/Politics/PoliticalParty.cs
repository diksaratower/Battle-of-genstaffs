﻿using UnityEngine;


[CreateAssetMenu(fileName = "PoliticalParty", menuName = "ScriptableObjects/PoliticalParty", order = 1)]
public class PoliticalParty : ScriptableObject
{
    public Ideology PartyIdeology;
    public CuntryElectionsType ElectionType;
    public string Name;
    public Color PartyColor;
}