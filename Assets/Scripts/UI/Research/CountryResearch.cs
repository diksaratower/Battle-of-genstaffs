using System;
using System.Collections.Generic;
using UnityEngine;

public class CountryResearch
{
    public Action<Technology> OnResearchedTech;
    public float ResearchPointsPerHour = 0.05f;
    public float ResearchPointCount = 0;

    private List<Technology> _openedTechnologies = new List<Technology>();

    public CountryResearch() 
    {
        GameTimer.HourEnd += () => { ResearchPointCount += ResearchPointsPerHour; };
    }

    public bool AlreadyResearched(Technology technology)
    {
        return _openedTechnologies.Contains(technology);
    }

    public void BuyTech(Technology technology)
    {
        if(_openedTechnologies.Contains(technology))
        {
            throw new System.Exception("You cannot learn the same technology twice.");
        }
        if((ResearchPointCount - technology.OpenCost) < 0)
        {
            throw new System.Exception("Not enough science points.");
        }
        ResearchPointCount -= technology.OpenCost;
        _openedTechnologies.Add(technology);
        OnResearchedTech?.Invoke(technology);
    }

    public void ResearchAllTechForFreeCheat()
    {
        ResearchPointCount += 0.1f;
        foreach (var tech in TechnologiesManagerSO.GetAllTechs())
        {
            if(!AlreadyResearched(tech))
            {
                ResearchPointCount += tech.OpenCost;
                BuyTech(tech);
            }
        }
    }

    public List<Technology> GetOpenedTechnologies()
    {
        var list = new List<Technology>();
        list.AddRange(_openedTechnologies);
        return list;
    }

    public CountryResearchSerialize GetSerialize()
    {
        return new CountryResearchSerialize(this);
    }

    public void LoadFromSerialize(CountryResearchSerialize ser)
    {
        ser.Load(this);
    }

    [Serializable]
    public class CountryResearchSerialize : SerializeForSave
    {
        public List<OpenTechnologySave> OpenTechnologySaves = new List<OpenTechnologySave>();
        public float ResearchPointCount;

        public CountryResearchSerialize(CountryResearch countryResearch)
        {
            foreach (var technology in countryResearch._openedTechnologies)
            {
                OpenTechnologySaves.Add(new OpenTechnologySave(technology.ID));
            }
            ResearchPointCount = countryResearch.ResearchPointCount;
        }

        public override void Load(object objTarget)
        {
            if (OpenTechnologySaves == null)
            {
                return;
            }
            var research = (CountryResearch)objTarget;
            var manager = TechnologiesManagerSO.GetInstance();
            foreach (var techSave in OpenTechnologySaves)
            {
                var tech = manager.TechnologyList.Find(t => t.ID == techSave.ID);
                if (tech != null)
                {
                    research._openedTechnologies.Add(tech);
                }
            }
            research.ResearchPointCount = ResearchPointCount;
        }

        public override string SaveToJson()
        {
            return JsonUtility.ToJson(this);
        }

        [Serializable]
        public class OpenTechnologySave
        {
            public string ID;

            public OpenTechnologySave(string id)
            {
                ID = id;
            }
        }
    }
}
