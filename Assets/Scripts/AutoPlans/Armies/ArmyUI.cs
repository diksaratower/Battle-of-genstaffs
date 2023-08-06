using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class ArmyUI : MonoBehaviour
{
    public Army TargetArmy;
    public ArmiesUI Owner { get; set; }
    public bool Selected { get; private set; }

    [SerializeField] private Outline _selectionOutline;
    [SerializeField] private GameObject _frontLinePrefab;
    [SerializeField] private GameObject _attackLinePrefab;
    [SerializeField] private TextMeshProUGUI _divisionCountText;
    [SerializeField] private TextMeshProUGUI _forceFactorText;
    [SerializeField] private Toggle _doPlansToggle;
    [SerializeField] private Button _deleteArmyButton;

    private List<GameObject> _drawedFrontLines = new List<GameObject>();
    private GameObject _drawedAttackLine;

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
    }
 
    public void Deselect()
    {
        Selected = false;
    }

    public async void AsyncUpdateFront(FrontPlan plan, List<FrontPlan.FrontData> frontDates)
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
    }

    private void DestroyFrontView()
    {
        _drawedFrontLines.ForEach(line => { Destroy(line); });
        _drawedFrontLines.Clear();
        if (_drawedAttackLine != null)
        {
            Destroy(_drawedAttackLine);
        }
    }

    private List<Vector3> SortFromDistance(List<Vector3> allPoints) 
    {
        var result = new List<Vector3>();
        var points = new List<Vector3>();
        points.AddRange(allPoints);
        var current = points[0];
        points.Remove(current);
        result.Add(current);
        while (points.Count != 0)
        {
            current = FindMinDistancePoint(points, current, out _);
            if(current == null)
            {
                break;
            }
            points.Remove(current);
            result.Add(current);
        }
        return result;
    }

    private List<Vector3> SmoothFront(List<Vector3> front)
    {
        var forRemove = new List<Vector3>();
        foreach (var vecOne in front)
        {
            foreach (var vecTwo in front)
            {
                if (vecOne != vecTwo)
                {
                    if (!forRemove.Contains(vecOne) && !forRemove.Contains(vecTwo))
                    {
                        var pointDistance = Vector2.Distance(new Vector2(vecOne.x, vecOne.z), new Vector2(vecTwo.x, vecTwo.z));
                        if (pointDistance < 0.1f)
                        {
                            forRemove.Add(vecOne);
                        }
                    }
                }
            }
        }
        front.RemoveAll(pr => forRemove.Contains(pr));
        return front;
    }

    private List<Vector3> DeleteFrontBugs(List<Vector3> front)
    {
        var forRemove = new List<Vector3>();
        for (int i = 0; i < front.Count; i++)
        {
            if (i == (front.Count - 1))
            {
                break;
            }
            var pointDistance = Vector2.Distance(new Vector2(front[i].x, front[i].z), new Vector2(front[i + 1].x, front[i + 1].z));
            if (pointDistance > (Map.Instance.ProvinceRadius * 2))
            {
                forRemove.Add(front[i]);
            }
        }
        front.RemoveAll(pr => forRemove.Contains(pr));
        return front;
    }

    public void DrawAttackPlanLine(List<Vector3> vectors)
    {
        if (_drawedAttackLine == null)
        {
            _drawedAttackLine = Instantiate(_attackLinePrefab);
        }
        var lr = _drawedAttackLine.GetComponent<LineRenderer>();
        lr.transform.position = vectors[0];
        lr.positionCount = 0;
        lr.SetPositions(new Vector3[0]);
        lr.positionCount = vectors.Count;
        lr.transform.localEulerAngles = new Vector3(90, 0, 0);
        for (int i = 0; i < vectors.Count; i++)
        {
            lr.SetPosition(i, vectors[i] + (Vector3.up * 0.5f));
        }
    }

    public static Vector3 FindMinDistancePoint(List<Vector3> provinces, Vector3 targetProv, out float distance)
    {
        var distances = new List<float>();
        for (int i = 0; i < provinces.Count; i++)
        {
            distances.Add(Vector2.Distance(new Vector2(provinces[i].x, provinces[i].z), new Vector2(targetProv.x, targetProv.z)));//Vector3.Distance(provinces[i], targetProv));
        }
        var min = distances.Min();
        distance = min;
        return provinces.Find(p => Vector2.Distance(new Vector2(p.x, p.z), new Vector2(targetProv.x, targetProv.z)) == min);
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
