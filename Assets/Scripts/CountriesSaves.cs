using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountriesSaves : MonoBehaviour, ISaveble
{
    private List<Country> _countries => Map.Instance.Countries;

    string ISaveble.GetFileName()
    {
        return "countries";
    }

    Type ISaveble.GetSaveType()
    {
        throw new NotImplementedException();
    }

    void ISaveble.Load(string data)
    {
        CountriesSavesSerialize.OverwriteFromJson(data, this);
    }

    string ISaveble.Save()
    {
        return CountriesSavesSerialize.GetJsonString(this);
    }

    public class CountriesSavesSerialize
    {
        public List<Country.CountrySerialize> Countries = new List<Country.CountrySerialize>();

        public static string GetJsonString(CountriesSaves countriesSaves)
        {
            var ser = new CountriesSavesSerialize();
            foreach (var country in countriesSaves._countries)
            {
                ser.Countries.Add(country.GetSerialize());
            }
            return JsonUtility.ToJson(ser, true);
        }

        public static void OverwriteFromJson(string json, CountriesSaves forOverwrite) 
        {
            var ser = new CountriesSavesSerialize();
            JsonUtility.FromJsonOverwrite(json, ser);
            for (int i = 0; i < ser.Countries.Count; i++)
            {
                var country = forOverwrite._countries.Find(country => (ser.Countries[i].ID == country.ID));
                if(country.ID == "ger")
                {

                }
                country.LoadFromSerialize(ser.Countries[i]);
                //forOverwrite._countries[i].LoadFromSerialize(ser.Countries[i]);
            }
        }
    }
}
