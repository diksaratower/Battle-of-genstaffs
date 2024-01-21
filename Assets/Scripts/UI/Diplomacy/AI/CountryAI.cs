using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Country))]
public class CountryAI : MonoBehaviour
{
    private Country _country;
    private System.Random _randomAI = new System.Random();
    private bool _workAI = true;
    private ArmiesControlCountryAI _countryArmiesAI;


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
            _countryArmiesAI.UpdateArmies();
        };
        _countryArmiesAI = new ArmiesControlCountryAI(_country, this);
        _countryArmiesAI.SetUp();
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
        _countryArmiesAI.UpdateAttcakOrDefens();
        if (_country.CountryBuild.BuildingsQueue.Count == 0)
        {
            BuildWork();
        }
        FocusesWork();
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
                if (_country.Politics.Preset.FocusTree == CountryAIDataSO.GetInstance().StandartFocusTree)
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

    public bool NeedAIWork()
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
            if (_country.ID == "pol" && ultimatum.Sender.ID == "ger")
            {
                ultimatum.SendAnser(UltimatumAnswerType.No);
                return;
            }
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
