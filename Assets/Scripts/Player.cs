using System;
using UnityEngine;

public class Player : MonoBehaviour, ISaveble
{
    public static Country CurrentCountry;
    public static Difficultie CurrentDifficultie;


    public void InitializePlayer(Country startCountry)
    {
        CurrentCountry = startCountry;
    }

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
    }

    Type ISaveble.GetSaveType()
    {
        throw new NotImplementedException();
    }

    [Serializable]
    public class PlayerSerialize
    {
        public string CountryID;

        public PlayerSerialize(Player player)
        {
            CountryID = Player.CurrentCountry.ID;
        }

        public PlayerSerialize(string countryId)
        {
            CountryID = countryId;
        }
    }
}
