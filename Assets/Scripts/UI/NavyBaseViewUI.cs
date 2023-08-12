using UnityEngine;


public class NavyBaseViewUI : MonoBehaviour
{
    public BuildingSlotProvince Target { get; private set; }

    private RectTransform _rectTransform;
    private NavyBasesViewerUI _navybasesViewerUI;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.anchoredPosition = GameCamera.Instance.WorldToScreenPointResolutionTrue(Target.Province.Position);
    }

    public void RefreshUI(BuildingSlotProvince aviabase, NavyBasesViewerUI navybasesViewerUI)
    {
        Target = aviabase;
        _navybasesViewerUI = navybasesViewerUI;
    }
}
