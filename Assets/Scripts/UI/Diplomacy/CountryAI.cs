using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Country))]
public class CountryAI : MonoBehaviour
{
    private int MaxDivisionsCount = 10;
    private Country _country;
    private Province _spawnDivisonsProvince;
    private System.Random _randomAI = new System.Random();

    private void Start()
    {
        _country = GetComponent<Country>();
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Minor)
        {
            MaxDivisionsCount = 4;
        }
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Major)
        {
            MaxDivisionsCount = 22;
        }
        Country.OnCountryCapitulated += delegate 
        {
            UpdateArmies();
        };
        _spawnDivisonsProvince = GetSpawnDivisionProvince();
        SpawnDivisions();
        _country.CountryDiplomacy.OnGetUltimatum += AnserUltimatum;
    }


    private void Update()
    {
        if (NeedAIWork() == false)
        {
            return;
        }
        if (_country.CountryFabrication.EquipmentSlots.Count == 0)
        {
            _country.CountryFabrication.AddSlot("ww1_rifle_equipment", _country.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory));
        }
        if (_country.CountryArmies.Armies.Count == 0 && UnitsManager.Instance.Divisions.FindAll(div => div.CountyOwner == _country).Count >= MaxDivisionsCount)
        {
            UpdateArmies();
        }
        FocusesWork();
    }

    private void SpawnDivisions()
    {
        if (_country.Templates.Templates.Count == 0)
        {
            _country.Templates.Templates.Add(DivisionTemplateConstructorUI.GetAITemplateWeak());
        }
        if (UnitsManager.Instance.Divisions.FindAll(div => div.CountyOwner == _country).Count < MaxDivisionsCount && _spawnDivisonsProvince != null
           && _spawnDivisonsProvince.Owner == _country)
        {
            for (int i = 0; i < MaxDivisionsCount; i++)
            {
                UnitsManager.Instance.AddDivision(_spawnDivisonsProvince, _country.Templates.Templates[0], _country);
            }
        }
    }

    private Province GetSpawnDivisionProvince()
    {
        var regions = _country.GetCountryRegions();
        if (regions.Count == 0)
        {
            return null;
        }
        return regions.Find(region => region.RegionCapital != null).RegionCapital.CityProvince;
    }

    private void UpdateArmies()
    {
        if (NeedAIWork() == false)
        {
            return;
        }
        var divs = UnitsManager.Instance.Divisions.FindAll(div => div.CountyOwner == _country);
        if (divs.Count == 0)
        {
            return;
        }

        _country.CountryArmies.RemoveAllArmies();
        var army = _country.CountryArmies.AddArmy(divs);
        army.DoPlanType = DoPlanType.Defense;
        var plan = new FrontPlan(army.Divisions.ToList(), Player.CurrentCountry, _country);
        army.AddPlan(plan);
        plan.Initialize();
    }

    private void FocusesWork()
    {
        if (NeedAIWork() == false)
        {
            return;
        }
        if (_country.Politics.ExecutingFocus == null)
        {
            foreach (var focus in _country.Politics.Preset.FocusTree.NationalFocuses)
            {
                if (_country.Politics.CanExecute(focus))
                {
                    _country.Politics.SetExecutingFocus(focus);
                }
            }
        }
    }

    private bool NeedAIWork()
    {
        if (Cheats.DisableAI == true)
        {
            return false;
        }
        if (_country == Player.CurrentCountry)
        {
            return false;
        }
        if (_country.Is—apitulated)
        {
            return false;
        }
        return true;
    }

    private void AnserUltimatum(Ultimatum ultimatum)
    {
        if (NeedAIWork() == false)
        {
            return;
        }
        var seed = _randomAI.Next(0, 100);
        if (ultimatum is AnnexCountryUltimatum)
        {
            if (seed < 95)
            {
                ultimatum.SendAnser(UltimatumAnswerType.Yes);
            }
            else 
            {
                ultimatum.SendAnser(UltimatumAnswerType.No);
            }
        }
        if (ultimatum is AnnexRegionUltimatum)
        {
            if (seed < 95)
            {
                ultimatum.SendAnser(UltimatumAnswerType.Yes);
            }
            else
            {
                ultimatum.SendAnser(UltimatumAnswerType.No);
            }
        }
    }
}
