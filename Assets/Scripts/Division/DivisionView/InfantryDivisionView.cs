using UnityEngine;


public class InfantryDivisionView : DivisionModelView
{
    [SerializeField] private Animator _bodyAnimator;
    [SerializeField] private GameObject _putRifle;
    [SerializeField] private GameObject _battleRifle;

    private SoliderAnimationState _state = SoliderAnimationState.Idle;


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

        if (state == DivisionAnimState.Empty && _state != SoliderAnimationState.Idle)
        {
            _bodyAnimator.SetTrigger("Idle");
            _state = SoliderAnimationState.Idle;
            _putRifle.gameObject.SetActive(true);
            _battleRifle.gameObject.SetActive(false);
        }
        if (state == DivisionAnimState.Move && _state != SoliderAnimationState.Walk)
        {
            _bodyAnimator.SetTrigger("Walk");
            _state = SoliderAnimationState.Walk;
            _putRifle.gameObject.SetActive(true);
            _battleRifle.gameObject.SetActive(false);
        }
        if ((state == DivisionAnimState.Attack || state == DivisionAnimState.Defend) && _state != SoliderAnimationState.Battle)
        {
            _bodyAnimator.SetTrigger("Battle");
            _state = SoliderAnimationState.Battle;
            _putRifle.gameObject.SetActive(false);
            _battleRifle.gameObject.SetActive(true);
            RotateDivisionToTarget();
        }
    }

    private enum SoliderAnimationState
    {
        Idle,
        Walk,
        Battle
    }
}
