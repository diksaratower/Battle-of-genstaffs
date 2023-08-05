using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DivisionMovePlanUI : MonoBehaviour
{
    public Division Owner => _divisionView.GetOverageDivision();

    [SerializeField] private DivisionView _divisionView;
    [SerializeField] private Color _color;
    [SerializeField] private GameObject _movePlanEndPrefab;
    [SerializeField] private float _upperMap;
    [SerializeField] private InfantryDivisionView _infantryDivisionView;
    [SerializeField] private TanksDivisionView _tankDivisionView;

    private List<Province> _drawedMovePath = new List<Province>();
    private LineRenderer _movePlanLineRender;
    private GameObject _movePlanEnd;
    private GameIU _gameUI;
    private DivisionModelView _divisionModelView;


    public void Initialize()
    {
        SetTemplateView(Owner.Template);
        Owner.OnSetTemplate += (DivisionTemplate template) =>
        {
            SetTemplateView(template);
        };
      
    }

    private void Update()
    {
        if (_gameUI == null)
        {
            _gameUI = FindObjectOfType<GameIU>();
        }
        if (_gameUI == null)
        {
            return;
        }
        if (Owner == null)
        {
            return;
        }
        CalculateMoveLine();
    }

    private void SetTemplateView(DivisionTemplate template)
    {
        _infantryDivisionView.gameObject.SetActive(false);
        _tankDivisionView.gameObject.SetActive(false);
        if (template.GetAverageBattlion().ViewType != DivisionViewType.Tanks)
        {
            _divisionModelView = _infantryDivisionView;
        }
        if (template.GetAverageBattlion().ViewType == DivisionViewType.Tanks)
        {
            _divisionModelView = _tankDivisionView;
        }
        _divisionModelView.gameObject.SetActive(true);
    }

    private void CalculateMoveLine()
    {
        if (Owner.CountyOwner != Player.CurrentCountry)
        {
            return;
        }
        if (Owner.MovePath.SequenceEqual(_drawedMovePath) == false)
        {
            _drawedMovePath = new List<Province>(Owner.MovePath);
            DrawPlan(_drawedMovePath);
        }
    }

    private void DrawPlan(List<Province> path)
    {
        DeletePlan();
        var drawPath = new List<Province>();
        drawPath.Add(Owner.DivisionProvince);
        drawPath.AddRange(path);
        if (drawPath.Count == 0) return;
        if (drawPath.Count == 1) return;

        GameObject myLine = new GameObject();
        myLine.transform.position = drawPath[0].Position;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("UI/Default"));
        lr.material.color = _color;
        _movePlanLineRender = lr;

#pragma warning disable CS0618 // Тип или член устарел
        _movePlanLineRender.SetColors(Color.red, Color.red);
        _movePlanLineRender.SetWidth(0.7f, 0.7f);
        _movePlanLineRender.SetVertexCount(drawPath.Count);
#pragma warning restore CS0618 // Тип или член устарел
        _movePlanLineRender.alignment = LineAlignment.TransformZ;
        _movePlanLineRender.transform.localEulerAngles = new Vector3(90, 0, 0);

        for (int i = 0; i < drawPath.Count; i++)
        {
            _movePlanLineRender.SetPosition(i, drawPath[i].LandscapeTruePosition + new Vector3(0, 1.5f, 0));
            if (i == drawPath.Count - 1)
            {
                if (i > 0)
                {
                    _movePlanEnd = Instantiate(_movePlanEndPrefab, drawPath[i].LandscapeTruePosition + new Vector3(0, 1.5f, 0), _movePlanEndPrefab.transform.rotation);
                    _movePlanEnd.GetComponentInChildren<UnityEngine.UI.Image>().color = _color;
                    _movePlanEnd.transform.LookAtAxis(drawPath[i - 1].Position, false, true, false);
                    _movePlanEnd.transform.localEulerAngles += new Vector3(0, 180, 0);
                }
            }
        }
        SetMoveLineView();
    }

    private IEnumerator MoveDivisionViewSmooth(Vector3 target, float time)
    {
        yield return null;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(time / 100);
            transform.position = Vector3.MoveTowards(Owner.DivisionProvince.LandscapeTruePosition, target, ((float)i / 100));
        }
    }

    private void SetMoveLineView()
    {
        var selected = _gameUI.IsDivisionSelect(Owner);
        _movePlanLineRender.gameObject.SetActive(selected);
        _movePlanEnd.gameObject.SetActive(selected);
    }

    private Vector3 MapUpOffest()
    {
        return (Vector3.up * (Map.Instance.transform.position.y + 0.1f));
    }

    private void OnDestroy()
    {
        DeletePlan();
    }

    private void DeletePlan()
    {
        if (_movePlanLineRender) { Destroy(_movePlanLineRender.gameObject); }
        if (_movePlanEnd) { Destroy(_movePlanEnd.gameObject); }
    }
}
