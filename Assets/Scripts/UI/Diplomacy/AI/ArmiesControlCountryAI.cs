using System.Collections.Generic;
using System.Linq;


public class ArmiesControlCountryAI
{
    private Country _country;
    private CountryAI _countryAI;
    private int _maxDivisionsCount;
    private float _cashedForceFactorInFront = 1.01f;
    private Province _spawnDivisonsProvince;
    private DivisionTemplate _mainTemplate;
    private List<TypedEquipmentCountIdPair> _cashedNeedEquipments = new List<TypedEquipmentCountIdPair>();

    private const int _divisionsAtOneTime = 5;


    public ArmiesControlCountryAI(Country country, CountryAI countryAI)
    {
        _country = country;
        _countryAI = countryAI;
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Minor)
        {
            _maxDivisionsCount = 10;
        }
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Middle)
        {
            _maxDivisionsCount = 30;
        }
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Major)
        {
            _maxDivisionsCount = 100;
        }
    }

    public void SetUp()
    {
        Country.OnCountryCapitulated += delegate
        {
            UpdateArmies();
        };
        _country.CountryDiplomacy.OnDeclaredWarToCountry += delegate
        {
            UpdateArmies();
        };
        _spawnDivisonsProvince = GetSpawnDivisionProvince();
        CreateMainTemplate();
        _cashedNeedEquipments = _mainTemplate.GetTemplateNeedEquipment();
        UpdateArmies();
    }

    public void UpdateAttcakOrDefens()
    {
        if (Diplomacy.Instance.CountryIsAtWar(_country))
        {
            foreach (var army in _country.CountryArmies.Armies)
            {
                if (army.CashedForceFactorInFront > 1f)
                {
                    army.DoPlanType = DoPlanType.Attack;
                }
                else
                {
                    army.DoPlanType = DoPlanType.Defense;
                }
            }
        }
    }

    public void UpdateArmies()
    {
        if (_countryAI.NeedAIWork() == false)
        {
            return;
        }
        var divisions = UnitsManager.Instance.Divisions.FindAll(division => division.CountyOwner == _country);
        if (divisions.Count == 0)
        {
            return;
        }
        var countryWars = Diplomacy.Instance.GetCountryWars(_country);
        var enemies = new List<Country>();

        _country.CountryArmies.RemoveAllArmies();
        if (countryWars.Count == 0)
        {
            enemies.Add(Player.CurrentCountry);
        }
        else
        {
            foreach (var war in countryWars)
            {
                foreach (var member in war.GetMembers())
                {
                    if (member.Country != _country)
                    {
                        if (FrontPlan.FrontCanExist(_country, member.Country))
                        {
                            enemies.Add(member.Country);
                        }
                    }
                }
            }
        }
        DivideDivisionsBetweenEnemies(divisions, enemies);
    }

    public void CreateDivisionsIfNeed()
    {
        var divisions = UnitsManager.Instance.Divisions.FindAll(division => division.CountyOwner == _country);
        if (divisions.Count >= _maxDivisionsCount)
        {
            return;
        }
        if (_country.CreationDivisions.CreationQueue.Count > 0)
        {
            return;
        }

        if (_spawnDivisonsProvince.Owner != _country)
        {
            return;
        }

        if (_spawnDivisonsProvince == null)
        {
            if (GetSpawnDivisionProvince() == null)
            {
                return;
            }
            else
            {
                _spawnDivisonsProvince = GetSpawnDivisionProvince();
            }
        }

        var haveEquipments = new List<TypedEquipmentCountIdPair>();
        foreach (var equipment in _cashedNeedEquipments)
        {
            var equipmentCount = _country.EquipmentStorage.GetEquipmentCountWithDeficit(equipment.EqType);
            if (equipmentCount <= 0)
            {
                return;
            }
            if (equipmentCount < equipment.Count)
            {
                return;
            }
            haveEquipments.Add(new TypedEquipmentCountIdPair(equipment.EqType, equipmentCount));
        }
        for (int i = 0; i < 5; i++)
        {
            _country.CreationDivisions.AddDivisionCreation(_mainTemplate, _spawnDivisonsProvince, $"division {divisions.Count + i}");
            foreach (var needEquipment in _cashedNeedEquipments)
            {
                var haveThisTypeEquiemnet = haveEquipments.Find(eq => eq.EqType == needEquipment.EqType);
                haveThisTypeEquiemnet.Count -= needEquipment.Count;
                if (haveThisTypeEquiemnet.Count <= 0)
                {
                    return;
                }
            }
        }
    }

    private void CreateMainTemplate()
    {
        var template = DivisionTemplateConstructorUI.GetAITemplate(4, 4);
        if (_country.CountryPreset.CountrySizeType == CountryAISizeData.Major)
        {
            template = DivisionTemplateConstructorUI.GetAITemplate(5, 4);
        }
        _country.Templates.Templates.Add(template);
        _mainTemplate = template;
    }

    private void DivideDivisionsBetweenEnemies(List<Division> divisions, List<Country> enemies)
    {
        if (divisions.Count == 0 || enemies.Count == 0)
        {
            return;
        }
        var divisionsAndCountryPair = new List<EnemyDivisionsPair>();
        foreach (var enemy in enemies)
        {
            divisionsAndCountryPair.Add(new EnemyDivisionsPair(enemy, new List<Division>()));
        }
        while (true)
        {
            if (divisions.Count == 0)
            {
                break;
            }
            var currnet = divisions[0];
            divisions.Remove(currnet);
            GetEnemyDivisionsPairWithLeastDivisions(divisionsAndCountryPair, currnet).Divisions.Add(currnet);
        }

        foreach (var pair in divisionsAndCountryPair)
        {
            if (pair.Divisions.Count == 0)
            {
                continue;
            }
            CreateArmy(pair.Divisions, pair.Country);
        }
    }

    private EnemyDivisionsPair GetEnemyDivisionsPairWithLeastDivisions(List<EnemyDivisionsPair> enemyDivisionsPairs, Division division)
    {
        int minDivisionsCount = int.MaxValue;
        EnemyDivisionsPair minDivisionsPair = enemyDivisionsPairs[0];

        foreach (EnemyDivisionsPair enemyDivisionsPair in enemyDivisionsPairs)
        {
            if (enemyDivisionsPair.Divisions.Count < minDivisionsCount && !enemyDivisionsPair.Divisions.Contains(division))
            {
                minDivisionsCount = enemyDivisionsPair.Divisions.Count;
                minDivisionsPair = enemyDivisionsPair;
            }
        }

        return minDivisionsPair;
    }

    private void CreateArmy(List<Division> divisions, Country enemy)
    {
        var army = _country.CountryArmies.AddArmy(divisions);
        army.DoPlanType = DoPlanType.Defense;
        var plan = new FrontPlan(army.Divisions.ToList(), enemy, _country);
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

    private class EnemyDivisionsPair
    {
        public Country Country { get; }
        public List<Division> Divisions { get; }

        public EnemyDivisionsPair(Country country, List<Division> divisions)
        {
            Country = country;
            Divisions = divisions;
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
}
