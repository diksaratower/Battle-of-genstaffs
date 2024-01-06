using UnityEngine;


public class NavyLandingPlanView : MonoBehaviour
{
    public SeaLandingPlan TargetPlan { get; private set; }

    [SerializeField] private LineRenderer _startToTargetLine;


    public void Refresh(SeaLandingPlan seaLandingPlan)
    {
        TargetPlan = seaLandingPlan;
        var landingMarinePath = seaLandingPlan.FindPathMarineLandingWithSea();
        _startToTargetLine.positionCount = (2 + landingMarinePath.Count);
        _startToTargetLine.SetPosition(0, seaLandingPlan.StartNavyBase.Province.Position);
        for (int i = 0; i < landingMarinePath.Count ; i++)
        {
            _startToTargetLine.SetPosition(i + 1, landingMarinePath[i].Center.position + (Vector3.up * 3));
        }
        _startToTargetLine.SetPosition((2 + landingMarinePath.Count) - 1, seaLandingPlan.TargetProvince.Position);
    }
}
