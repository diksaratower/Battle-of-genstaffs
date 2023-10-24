using System;
using UnityEngine;

public class Player : MonoBehaviour, ISaveble
{
    public static Country CurrentCountry;
    public static Difficultie CurrentDifficultie;

    string ISaveble.GetFileName()
    {
        return "player";
    }

    string ISaveble.Save()
    {
        return JsonUtility.ToJson(new PlayerSerialize(this));
    }

    void ISaveble.Load(string data)
    {
        var ser = JsonUtility.FromJson<PlayerSerialize>(data);
        CurrentCountry = Map.Instance.GetCountryFromId(ser.CountryID);
        if (DifficultiesData.GetInstance().Difficulties.Exists(d => d.ID == ser.DifficultieID))
        {
            CurrentDifficultie = DifficultiesData.GetInstance().Difficulties.Find(d => d.ID == ser.DifficultieID);
        }
        else
        {
            CurrentDifficultie = DifficultiesData.GetInstance().StandartDifficultie;
        }
    }

    Type ISaveble.GetSaveType()
    {
        throw new NotImplementedException();
    }

    [Serializable]
    public class PlayerSerialize
    {
        public string CountryID;
        public string DifficultieID;

        public PlayerSerialize(Player player)
        {
            CountryID = Player.CurrentCountry.ID;
            DifficultieID = Player.CurrentDifficultie.ID;
        }

        public PlayerSerialize(string countryId)
        {
            CountryID = countryId;
        }
    }
}
