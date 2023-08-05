using UnityEngine;


public class DivisionModelView : MonoBehaviour
{
    [SerializeField] protected DivisionView _divisionView;
    protected Division _owner => _divisionView.GetOverageDivision();

    [SerializeField] private GameObject _divModel;


    protected virtual void Update()
    {
        if (_divModel.activeSelf == true)
        {
            if (_owner != null)
            {
                if (_owner.MovePath.Count != 0)
                {
                    RotateDivisionToTarget();
                }
            }
        }
    }

    protected void RotateDivisionToTarget()
    {
        var viewPos = GetViewPosition();
        if (viewPos == Vector3.zero)
        {
            return;
        }
        _divModel.transform.LookAtAxis(viewPos, false, true, false);
    }

    private Vector3 GetViewPosition()
    {
        var viewPos = Vector3.zero;
        if (_owner.MovePath.Count == 0 || _owner.DivisionState == DivisionAnimState.Defend)
        {
            var divisionCombat = _owner.Combats.Find(combat => combat.Defenders.Contains(_owner));
            viewPos = divisionCombat.Attackers[0].DivisionProvince.Position;
            return viewPos;
        }
        if (_owner.MovePath.Count > 0)
        {
            viewPos = _owner.MovePath[0].Position;
            if (_owner.MovePath[0] == _owner.DivisionProvince)
            {
                if (_owner.MovePath.Count > 1)
                {
                    viewPos = _owner.MovePath[1].Position;
                }
            }
        }
        return viewPos;
    }
}
