using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using System.Linq;


public class ArmyUI : MonoBehaviour
{
    public Army TargetArmy;
    public bool CreationSeaLanding { get; set; }
    public bool CreationFrontUI { get; set; }
    public ArmiesUI Owner { get; set; }
    public bool Selected { get; private set; }

    [SerializeField] private Outline _selectionOutline;
    [SerializeField] private GameObject _frontLinePrefab;
    [SerializeField] private GameObject _attackLinePrefab;
    [SerializeField] private TextMeshProUGUI _divisionCountText;
    [SerializeField] private TextMeshProUGUI _forceFactorText;
    [SerializeField] private Toggle _doPlansToggle;
    [SerializeField] private Button _deleteArmyButton;
    [SerializeField] private NavyLandingPlanView _landingPlanViewPrefab;

    private List<GameObject> _drawedFrontLines = new List<GameObject>();
    private List<NavyLandingPlanView> _navyLandingPlanViews = new List<NavyLandingPlanView>();


    private void Start()
    {
        _doPlansToggle.onValueChanged.AddListener((bool chekBoxValue) => {
            if (chekBoxValue == true) 
            {
                TargetArmy.DoPlanType = DoPlanType.Attack;
            }
            else
            {
                TargetArmy.DoPlanType = DoPlanType.Defense;
            }
        });
        _deleteArmyButton.onClick.AddListener(() => {
            Player.CurrentCountry.CountryArmies.RemoveArmy(TargetArmy);
        });
        TargetArmy.OnAddedPlan += delegate 
        {
            if (TargetArmy.Plans.Find(pl => pl is FrontPlan) != null)
            {
                var plan = (FrontPlan)TargetArmy.Plans.Find(pl => pl is FrontPlan);
                plan.OnRecalculatedFront += (List<FrontPlan.FrontData> frontDates)  =>
                {
                    AsyncUpdateFront(plan, frontDates);
                    _forceFactorText.text = "Фактор силы: " + Math.Round(plan.GetForceFactor(frontDates), 1);
                };
                plan.OnStoppedFront += delegate 
                { 
                    DestroyFrontView();
                };
            }
            else
            {
                DestroyFrontView();
            }
        };
    }

    private void Update()
    {
        _selectionOutline.enabled = Selected;
        _divisionCountText.text = TargetArmy.DivisionsCount + "/" + TargetArmy.MaxDivisionsCount;
        if (CreationFrontUI && Selected)
        {
            CreatingNewFrontLine();
        }
        if (CreationSeaLanding && Selected)
        {
            CreatingNewSeaLanding();
        }
    }

    public void Deselect()
    {
        Selected = false;
    }

    private void CreatingNewFrontLine()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (GameCamera.Instance.ChekHitToProvinceWithMousePosition(out var prov))
            {
                if (prov.Owner == Player.CurrentCountry && prov.Owner.ID != "null")
                {
                    if (prov.Contacts.Find(pr => pr.Owner != prov.Owner) != null)
                    {
                        CreationFrontUI = false;
                        Deselect();
                        var enemyCountry = prov.Contacts.Find(pr => pr.Owner != prov.Owner).Owner;
                        TargetArmy.RemoveAllPlans();
                        var plan = new FrontPlan(TargetArmy.Divisions.ToList(), enemyCountry, Player.CurrentCountry);
                        TargetArmy.AddPlan(plan);
                        plan.Initialize();
                    }
                }
            }
        }
    }

    private void CreatingNewSeaLanding()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (GameCamera.Instance.ChekHitToProvinceWithMousePosition(out var province))
            {
                if (province.Owner.ID != "null")
                {
                    if (province.Contacts.Count < 6)
                    {
                        if (Map.Instance.MarineRegions.MarineRegionsList.Exists(region => region.Provinces.Contains(province)))
                        {
                            CreationSeaLanding = false;
                            if (SeaLandingPlan.ArmyCanExecuteSeaLanding(TargetArmy, out var startNavyBase))
                            {
                                Deselect();
                                TargetArmy.RemoveAllPlans();
                                var seaLandingPlan = new SeaLandingPlan(new List<Division>(TargetArmy.Divisions), province, startNavyBase, Player.CurrentCountry);
                                var sealandingUI = DrawSeaLandingPlan(seaLandingPlan);
                                seaLandingPlan.OnRemoveLanding += delegate
                                {
                                    if (sealandingUI != null)
                                    {
                                        Destroy(sealandingUI.gameObject);
                                    }
                                };
                                TargetArmy.AddPlan(seaLandingPlan);
                            }
                        }
                    }
                }
            }
        }
    }

    private NavyLandingPlanView DrawSeaLandingPlan(SeaLandingPlan seaLandingPlan)
    {
        var view = Instantiate(_landingPlanViewPrefab);
        view.Refresh(seaLandingPlan);
        _navyLandingPlanViews.Add(view);
        return view;
    }

    private async void AsyncUpdateFront(FrontPlan plan, List<FrontPlan.FrontData> frontDates)
    {
        await Task.Delay(40);
        if (frontDates.Count != 0)
        {
            if (plan.IsFrontStopped != true)
            {
                DestroyFrontView();
                foreach (var date in frontDates)
                {
                    if (plan.IsFrontStopped == true)
                    {
                        DestroyFrontView();
                        break;
                    }
                    if (date.FrontProvinces.Count == 0 || date.FrontAllyContacts.Count == 0)
                    {
                        continue;
                    }
                    DrawFrontPlanLine(date.FrontProvinces, date.FrontAllyContacts);
                }
            }
        }
    }

    private void DrawFrontPlanLine(List<Province> front, List<Province> front2)
    {
        foreach (var frontProvince in front)
        {
            foreach (var frontProvince2 in front2)
            {
                var points = frontProvince.GetIntersectionsPoints(frontProvince2);
                if (points.Count == 0 || points.Count == 1)
                {
                    continue;
                }

                var drawedFront = Instantiate(_frontLinePrefab).GetComponent<LineRenderer>();
                drawedFront.positionCount = points.Count;
                for (int j = 0; j < points.Count; j++)
                {
                    drawedFront.SetPosition(j, points[j] + (Vector3.up * 1.7f));
                }
                _drawedFrontLines.Add(drawedFront.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        _frontLinePrefab = null;
        DestroyFrontView();
        DestroyNavyLandingViews();
    }

    private void DestroyFrontView()
    {
        _drawedFrontLines.ForEach(line => { Destroy(line); });
        _drawedFrontLines.Clear();
    }

    private void DestroyNavyLandingViews()
    {
        _navyLandingPlanViews.ForEach(planView =>
        {
            if (planView != null)
            {
                Destroy(planView.gameObject);
            }
        });
        _navyLandingPlanViews.Clear();
    }

    public void OnArmyClick()
    {
        if (Selected == false)
        {
            Owner.DeselectAllArmies();
        }
        Selected = !Selected;
        var gUI = FindObjectOfType<GameIU>();
        gUI.DeselectedAllDivisions();
        gUI.SelectDivisions(TargetArmy.Divisions);
    }
}
