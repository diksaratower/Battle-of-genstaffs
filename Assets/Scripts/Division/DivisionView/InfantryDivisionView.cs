using UnityEngine;


public class InfantryDivisionView : DivisionModelView
{
    [SerializeField] private Animator _bodyAnimator;

    protected override void Update()
    {
        base.Update();
        if (_owner != null)
        {
            InfantryAnimate(_owner.DivisionState);
        }
    }

    private void InfantryAnimate(DivisionAnimState state) 
    {
        if (_owner == null)
        {
            return;
        }

        if (state == DivisionAnimState.Empty)
        {
            _bodyAnimator.SetTrigger("Idle");
        }
        if (state == DivisionAnimState.Move)
        {
            _bodyAnimator.SetTrigger("Walk");
        }
        if (state == DivisionAnimState.Attack || state == DivisionAnimState.Defend)
        {
            _bodyAnimator.SetTrigger("Battle");
            RotateDivisionToTarget();
        }
    }
}
