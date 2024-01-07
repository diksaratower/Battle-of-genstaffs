using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Country))]
public class CountryAI : MonoBehaviour
{
    private int _maxDivisionsCount = 10;
    private Country _country;
    private Province _spawnDivisonsProvince;
    private System.Random _randomAI = new System.Random();
    private float _cashedForceFactorInFront = 1.001f;
    private bool _workAI = true;

    private void Start()
    {
        _country = GetComponent<Country>();
        if (NeedAIWork() == false)
        {
            return;
        }
        if (_country.ID == "fra" || _country.ID == "eng")
        {
            Diplomacy.Instance.GuaranteeIndependence(_country, Map.Instance.GetCountryFromId("pol"));
            SetUpFleet();
        }
        _country.CountryDiplomacy.OnAddWarGoal += (WarGoal warGoal) =>
        {
            _country.CountryDiplomacy.DeclareWarToCountry(warGoal.Target);
        };
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Minor)
        {
            _maxDivisionsCount = 4;
        }
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Middle)
        {
            _maxDivisionsCount = 15;
        }
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Major)
        {
            _maxDivisionsCount = 30;
        }
        Country.OnCountryCapitulated += delegate 
        {
            UpdateArmies();
        };
        _spawnDivisonsProvince = GetSpawnDivisionProvince();
        SpawnDivisions();
        _country.CountryDiplomacy.OnGetUltimatum += AnserUltimatum;
        _country.OnAnnexed += delegate
        {
            _workAI = false;
        };
        _country.OnCapitulated += delegate
        {
            _workAI = false;
        };
    }

    private void Update()
    {
        if (NeedAIWork() == false)
        {
            return;
        }
        if (_country.CountryFabrication.EquipmentSlots.Count == 0)
        {
            UpdateFabrication();
        }
        if (_country.CountryArmies.Armies.Count == 0 && UnitsManager.Instance.Divisions.FindAll(div => div.CountyOwner == _country).Count >= _maxDivisionsCount)
        {
            UpdateArmies();
        }
        if (Diplomacy.Instance.CountryIsAtWar(_country))
        {
            foreach (var army in _country.CountryArmies.Armies)
            {
                if (_cashedForceFactorInFront > 1f)
                {
                    army.DoPlanType = DoPlanType.Attack;
                }
                else
                {
                    army.DoPlanType = DoPlanType.Defense;
                }
            }
        }
        if (_country.CountryBuild.BuildingsQueue.Count == 0)
        {
            BuildWork();
        }
        FocusesWork();
    }

    private void SpawnDivisions()
    {
        var template = DivisionTemplateConstructorUI.GetAITemplate(4, 4);
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Major)
        {
            template = DivisionTemplateConstructorUI.GetAITemplate(5, 4);
        }
        _country.Templates.DeleteAllTemplatesWithDivisions();
        _country.Templates.Templates.Add(template);
        if (UnitsManager.Instance.Divisions.FindAll(div => div.CountyOwner == _country).Count < _maxDivisionsCount && _spawnDivisonsProvince != null
           && _spawnDivisonsProvince.Owner == _country)
        {
            for (int i = 0; i < _maxDivisionsCount; i++)
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
        
        plan.OnRecalculatedFront += (List<FrontPlan.FrontData> frontDates) =>
        {
            if (Diplomacy.Instance.CountryIsAtWar(_country))
            {
                _cashedForceFactorInFront = plan.GetForceFactor(frontDates);
            }
        };
    }

    private void BuildWork()
    {
        if (_country.CountryBuild.BuildingsQueue.Count > 0)
        {
            return;
        }
        if (NeedAIWork() == false)
        {
            return;
        }
        var building = BuildingsManagerSO.GetInstance().AvalibleBuildings.Find(b => b.BuildingType == BuildingType.MilitaryFactory);
        var contryRegions = _country.GetCountryRegions();
        foreach (var region in contryRegions)
        {
            _country.CountryBuild.AddBuildingToBuildQueue(building, region);
        }
        UpdateFabrication();
    }

    private void UpdateFabrication()
    {
        var slots = new List<CountryFabricationEquipmentSlot>(_country.CountryFabrication.EquipmentSlots);
        foreach (var slot in slots)
        {
            _country.CountryFabrication.RemoveSlot(slot);
        }
        _country.CountryFabrication.AddSlot(EquipmentManagerSO.GetAllEquipment().Find(equipment => equipment.ID == "ww1_rifle_equipment"),
                _country.CountryBuild.GetCountryBuildings(BuildingType.MilitaryFactory));
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
                if (_country.CountryPreset.Politics.Preset.FocusTree == CountryAIDataSO.GetInstance().StandartFocusTree)
                {
                    var idealogyVariant = CountryAIDataSO.GetInstance().ChoosingIdeologyVariantsInStandardTree.Find(variant => variant.Focus == focus);
                    if (idealogyVariant != null)
                    {
                        if (idealogyVariant.FocusIdealogy != _country.CountryPreset.HistoryIdealogyForAI)
                        {
                            continue;
                        }
                    }
                }
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
        if (_workAI == false)
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
        if (ultimatum.Sender != Player.CurrentCountry)
        {
            seed = 1;
        }
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

    private void SetUpFleet()
    {
        var countryMarineRegions = GetCountryMarineRegions();
        if (countryMarineRegions.Count == 0)
        {
            return;
        }
        var tacticalUnit = new TacticalFleetUnit();
        tacticalUnit.Name = $"Тактическое соеденение ({_country.Fleet.TacticalFleetUnits.Count + 1})";
        _country.Fleet.TacticalFleetUnits.Add(tacticalUnit);
        foreach (var ship in Map.Instance.MarineRegions.Ships)
        {
            if (ship.Country == _country)
            {
                tacticalUnit.AddShip(ship);
            }
        }
        foreach (var region in countryMarineRegions)
        {
            tacticalUnit.AddRegionDominationOrder(region);
        }
    }

    private List<MarineRegion> GetCountryMarineRegions()
    {
        var regions = new List<MarineRegion>();
        foreach (var region in Map.Instance.MarineRegions.MarineRegionsList)
        {
            if (region.Provinces.Exists(province => province.Owner == _country))
            {
                regions.Add(region);
            }
        }
        return regions;
    }
}
