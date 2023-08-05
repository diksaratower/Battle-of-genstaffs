using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CombatUI : MonoBehaviour
{
    public DivisionCombat Target { get; private set; }
    public bool IsCombatEnd { get; private set; }

    [SerializeField] private TextMeshProUGUI _combatProcentText;
    [SerializeField] private Transform _rotateTransform;
    [SerializeField] private Button _combatDetailsButton;

    public void RefreshUI(DivisionCombat target)
    {
        Target = target;
        target.OnEnd += delegate
        {
            IsCombatEnd = true;
        };
        _combatDetailsButton.onClick.AddListener(delegate {
            GameIU.OpenCombatDetailsWindow().RefreshUI(Target);
        });
    }

    public void Update()
    {
        if(IsCombatEnd) 
        {
            return;
        }
        if (Target.Defenders.Count == 0 || Target.Attackers.Count == 0)
        {
            return;
        }
        if (Target.Defenders[0] == null || Target.Attackers[0] == null)
        {
            return;
        }
        transform.position = Vector3Extend.GetMiddlePoint(GameCamera.Instance.WorldToScreenPointResolutionTrue(Target.Defenders[0].DivisionProvince.Position),
    GameCamera.Instance.WorldToScreenPointResolutionTrue(Target.Attackers[0].DivisionProvince.Position));
        var defenderScreenPos = GameCamera.Instance.WorldToScreenPointResolutionTrue(Target.Defenders[0].DivisionProvince.Position);
        _rotateTransform.LookAtAxis(defenderScreenPos, true, true, true);
        _combatProcentText.text = Math.Round(100 - (Target.GetProcentOfCombat() * 100)).ToString();
    }
}
