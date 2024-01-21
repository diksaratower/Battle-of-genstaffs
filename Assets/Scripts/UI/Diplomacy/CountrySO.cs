using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CountrySO", order = 1)]
public class CountrySO : ScriptableObject
{
    public CountryPolitics Politics;
    public string Name;
    public string ID;
    public Color ColorInMap;
    public Sprite CountryFlag;
    public int Population;
    public bool IsAvailableForPlayer = true;
    public bool IsAICountry = false;
    public CountryAISizeData CountrySizeType;
    public Ideology HistoryIdealogyForAI;
    public PoliticalParty RulingPoliticalParty;


#if UNITY_EDITOR
    public static CountrySO CreateMyAsset(string assetName, Country country)
    {
        CountrySO asset = ScriptableObject.CreateInstance<CountrySO>();
        asset.Name = country.Name;
        asset.ID = country.ID;
        asset.ColorInMap = country.ColorInMap;
        asset.CountryFlag = country.Flag;
        //asset.Population = country.Population;
        asset.Politics = country.Politics;

        AssetDatabase.CreateAsset(asset, $"Assets/{assetName}.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        return asset;
    }
#endif
}

public enum CountryAISizeData
{
    Minor,
    Major,
    Middle
}