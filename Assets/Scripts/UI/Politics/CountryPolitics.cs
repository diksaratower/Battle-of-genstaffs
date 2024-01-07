using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CountryPolitics 
{
    public static readonly float AdviserAddCost = 20;
    public NationalFocus ExecutingFocus { get; private set; }
    public float BaseStability { get; private set; }
    public float PolitPowerGrowthSpeed => 0.7f;
    public List<Law> EconomicsLaws => Preset.Laws;
    public List<Law> —onscriptionLaws => Preset.ConscriptionLaws;
    public Law CurrentEconomicLaw { get; private set; }
    public Law Current—onscriptionLaw { get; private set; }

    public Ideology CountryIdeology;
    public FormOfGovernment FormGovernment;
    public CuntryElectionsType ElectionsType;
    public float PolitPower;
    public Personage CountryLeader;
    public CountryPoliticsPreset Preset;
    public Action OnFocusExecuted;

    [HideInInspector] public List<Personage> Advisers = new List<Personage>();
    [HideInInspector] public List<PartyPopular> Parties = new List<PartyPopular>();

    private List<NationalFocus> _executedFocuses = new List<NationalFocus>();
    private int _executingFocusProgressDays = 0;
    private Country _country;

    public void Setup(Country country)
    {
        _country = country;
        GameTimer.DayEnd += CalculatePolitPowerGrowth;
        GameTimer.DayEnd += CalculateFocusExecution;
        if (Preset != null)
        {
            foreach (var party in Preset.Parties)
            {
                Parties.Add(new PartyPopular(party.ProcentPopularity, party.Name, party.PartyColor, party.PartyIdeology));
            }
        }
        CurrentEconomicLaw = EconomicsLaws[0];
        Current—onscriptionLaw = —onscriptionLaws[0];
    }

    public void CopyData(CountryPolitics original)
    {
        Preset = original.Preset;
        CountryIdeology = original.CountryIdeology;
        FormGovernment = original.FormGovernment;
        ElectionsType = original.ElectionsType;
        CountryLeader = original.CountryLeader;
        BaseStability = Preset.BaseStability;
    }

    public void SetExecutingFocus(NationalFocus focus)
    {
        _executingFocusProgressDays = 0;
        ExecutingFocus = focus;
    }

    public bool IsExecutedFocus(NationalFocus focus)
    {
        return _executedFocuses.Contains(focus);
    }

    public bool CanExecute(NationalFocus focus)
    {
        if (_executedFocuses.Exists(foc => focus.ConflictWithFocuses.Contains(foc)) == true)
        {
            return false;
        }
        var needFocusesExecuted = 0;
        foreach (var need in focus.NeedsForExecution)
        {
            if(_executedFocuses.Contains(need))
            {
                needFocusesExecuted++;
            }
        }
        return needFocusesExecuted == focus.NeedsForExecution.Count && !_executedFocuses.Contains(focus);
    }

    public float GetProcentOfExecuteFocus()
    {
        if(ExecutingFocus == null)
        {
            return 0;
        }
        return (((float)_executingFocusProgressDays) / ((float)ExecutingFocus.ExecutionDurationDay));
    }

    public void AddAdviser(Personage personage)
    {
        PolitPower -= AdviserAddCost;
        Advisers.Add(personage);
    }

    public void ChangeCurrentEconomicLaw(Law law)
    {
        if (CurrentEconomicLaw == law)
        {
            throw new TryToSetAnAlreadyEstablishedLaw();
        }
        PolitPower -= law.PolitPowerCost;
        CurrentEconomicLaw = law;
    }

    public void ChangeCurrentConscrirtionLaw(Law law)
    {
        if (Current—onscriptionLaw == law)
        {
            throw new TryToSetAnAlreadyEstablishedLaw();
        }
        PolitPower -= law.PolitPowerCost;
        Current—onscriptionLaw = law;
        var newManpower = Mathf.RoundToInt(((float)Player.CurrentCountry.CountryPreset.Population / 100f) * Player.CurrentCountry.Politics.GetConscriptionPercent());
        if(newManpower - _country.EquipmentStorage.GetEquipmentCount(EquipmentType.Manpower) > 0)
        {
            _country.EquipmentStorage.AddEquipment("manpower", newManpower - _country.EquipmentStorage.GetEquipmentCount(EquipmentType.Manpower));
        }
    }

    public float GetConscriptionPercent()
    {
        foreach (var effect in Current—onscriptionLaw.LawEffects)
        {
            if (effect is SetConscriptionProcent)
            {
                return (effect as SetConscriptionProcent).ConscriptionProcent;
            }
        }
        return 0f;
    }

    public class TryToSetAnAlreadyEstablishedLaw : Exception { }

    public void AddPartyPopular(Ideology ideology, float addProcent)
    {
        AddPartyPopular(Parties.Find(pr => pr.PartyIdeology == ideology).Name, addProcent);
    }

    public void AddPartyPopular(string partyName, float addProcent)
    {
        var otherParties = Parties.FindAll(pr => (pr.Name != partyName && pr.ProcentPopularity > 0));
        otherParties.ForEach(otherParty => 
        {
            otherParty.ChangePopular(-(addProcent / otherParties.Count));
        });
        Parties.Find(pr => pr.Name == partyName).ChangePopular(addProcent);
    }

    public float ApplyPolitPowerGrowthEffects(float baseGrowth)
    {
        var correctedGrowth = baseGrowth;
        if (_country == Player.CurrentCountry)
        {
            correctedGrowth += (baseGrowth * (Player.CurrentDifficultie.PolitPowerBonusPercent / 100f));
        }
        var effects = GetAdvisersTraitEffects<PolitPowerGrowthTraitEffect>();
        effects.ForEach((PolitPowerGrowthTraitEffect effect) =>
        {
            correctedGrowth += effect.GetNeedIncreaseValue(baseGrowth);
        });
        return correctedGrowth;
    }

    public float GetPoliticCorrectionMilitaryFabrication(float baseFabrication)
    {
        var result = 0f;
        foreach (var effect in CurrentEconomicLaw.LawEffects)
        {
            if (effect is MilitaryFabricationLawEffect)
            {
                result += (effect as MilitaryFabricationLawEffect).GetNeedIncreaseValue(baseFabrication);
            }
        }
        return result;
    }

    public float GetPoliticCorrectionBuildEfficiency(float baseFabrication)
    {
        var result = 0f;
        var effects = GetAdvisersTraitEffects<BuildSpeedTraitEffect>();
        effects.ForEach((BuildSpeedTraitEffect effect) =>
        {
            result += effect.GetNeedIncreaseValue(baseFabrication);
        });
        return result;
    }

    public float CalculateStability()
    {
        var correctedStability = BaseStability;
        var effects = GetAdvisersTraitEffects<ChangeStabilityTraitEffect>();
        effects.ForEach((ChangeStabilityTraitEffect effect) =>
        {
            correctedStability += effect.GetNeedIncreaseValue(BaseStability);
        });

        if(correctedStability > 100f)
        {
            correctedStability = 100f;
        }
        return correctedStability;
    }

    private void CalculatePolitPowerGrowth()
    {
        PolitPower += ApplyPolitPowerGrowthEffects(PolitPowerGrowthSpeed);
    }

    private List<T> GetAdvisersTraitEffects<T>() where T : TraitEffect
    {
        var result = new List<T>();
        foreach (var adviser in Advisers)
        {
            var effects = adviser.GetTraitEffects<T>();
            foreach (var effect in effects)
            {
                result.Add(effect as T);
            }
        }
        return result;
    }

    private void CalculateFocusExecution()
    {
        if(ExecutingFocus == null)
        {
            return;
        }
        _executingFocusProgressDays += 1;
        if(ExecutingFocus.ExecutionDurationDay == _executingFocusProgressDays || Cheats.InstantFocusesDoing)
        {
            var focus = ExecutingFocus;
            ExecutingFocus = null;
            ExecuteFocus(focus);
        }
    }

    private float GetCorrectedStability(float baseStability)
    {
        var correctedStability = baseStability;

        return correctedStability;
    }

    private void ExecuteFocus(NationalFocus nationalFocus)
    {
        _executedFocuses.Add(nationalFocus);
        nationalFocus.ExecuteFocus(_country);
        OnFocusExecuted?.Invoke();
    }

    public CountryPoliticsSerialize GetSerialize()
    {
        return new CountryPoliticsSerialize(this);
    }

    public void LoadFromSerialize(CountryPoliticsSerialize ser)
    {
        if(ser == null)
        {
            return;
        }
        ser.Load(this);
    }

    [Serializable]
    public class CountryPoliticsSerialize : SerializeForSave
    {
        public float PolitPowerCount;
        public float Stability;
        public PersonageSave Leader;
        public LawSave EconomisLaw;
        public LawSave ConscriptionLaw;
        public List<PersonageSave> Advisers = new List<PersonageSave>();
        public List<FocusSave> ExecutedFocuses = new List<FocusSave>();


        public CountryPoliticsSerialize(CountryPolitics politics)
        {
            PolitPowerCount = politics.PolitPower;
            Stability = politics.BaseStability;
            Leader = new PersonageSave(politics.CountryLeader);
            EconomisLaw = new LawSave(politics.CurrentEconomicLaw);
            ConscriptionLaw = new LawSave(politics.Current—onscriptionLaw);
            foreach (var adviser in politics.Advisers)
            {
                Advisers.Add(new PersonageSave(adviser));
            }
            foreach (var focus in politics._executedFocuses)
            {
                ExecutedFocuses.Add(new FocusSave(focus));
            }
        }

        public override void Load(object objTarget)
        {
            var politics = (CountryPolitics)objTarget;
            politics.PolitPower = PolitPowerCount;
            var politicsData = PoliticsDataSO.GetInstance();
            politics.CountryLeader = politicsData.Personages.Find(personage => personage.ID == Leader.ID);
            LoadLaws(politics);
            LoadAdviser(politics);
            LoadExecutedFocus(politics);
        }

        public override string SaveToJson()
        {
            return JsonUtility.ToJson(this);
        }

        private void LoadLaws(CountryPolitics politics)
        {
            foreach (var economicLaw in politics.EconomicsLaws)
            {
                if (EconomisLaw.ID == economicLaw.ID)
                {
                    politics.CurrentEconomicLaw = economicLaw;
                }
            }
            foreach (var conscriptionLaw in politics.—onscriptionLaws)
            {
                if (EconomisLaw.ID == conscriptionLaw.ID)
                {
                    politics.Current—onscriptionLaw = conscriptionLaw;
                }
            }
        }

        private void LoadAdviser(CountryPolitics politics)
        {
            foreach (var adviseSave in Advisers)
            {
                politics.AddAdviser(politics.Preset.AvailableAdvisers.Find(personage => personage.ID == adviseSave.ID));
            }
        }

        private void LoadExecutedFocus(CountryPolitics politics)
        {
            foreach (var focusSave in ExecutedFocuses)
            {
                var focus = politics.Preset.FocusTree.NationalFocuses.Find(f => focusSave.ID == f.ID);
                if (focus == null)
                {
                    throw new Exception("Focuses load excption.");
                }
                politics._executedFocuses.Add(focus);
            }
        }
    }

    [Serializable]
    public class PersonageSave
    {
        public string ID;

        public PersonageSave(Personage personage) 
        { 
            ID = personage.ID;
        }
    }

    [Serializable]
    public class LawSave
    {
        public string ID;

        public LawSave(Law law) 
        {
            ID = law.ID;
        }
    }

    [Serializable] 
    public class FocusSave
    {
        public string ID;

        public FocusSave(NationalFocus nationalFocus)
        {
            ID = nationalFocus.ID;
        }
    }
}
