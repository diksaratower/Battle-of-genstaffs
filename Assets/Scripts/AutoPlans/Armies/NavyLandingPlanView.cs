using UnityEngine;


public class NavyLandingPlanView : MonoBehaviour
{
    public SeaLandingPlan TargetPlan { get; private set; }

    [SerializeField] private LineRenderer _startToTargetLine;


    public void Refresh(SeaLandingPlan seaLandingPlan)
    {
        TargetPlan = seaLandingPlan;
        _startToTargetLine.positionCount = 3;
        _startToTargetLine.SetPosition(0, seaLandingPlan.StartNavyBase.Province.Position);
        _startToTargetLine.SetPosition(1, Vector3Extend.GetMiddlePoint(seaLandingPlan.StartNavyBase.Province.Position, seaLandingPlan.TargetProvince.Position) + (Vector3.up * 6));
        _startToTargetLine.SetPosition(2, seaLandingPlan.TargetProvince.Position);
    }
}
