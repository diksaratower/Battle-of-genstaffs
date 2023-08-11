using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AirbaseViewUI : MonoBehaviour, IPointerClickHandler
{
    public BuildingSlotRegion Target { get; private set; }

    [SerializeField] private Button _baseClickButton;
    [SerializeField] private GameObject _addtionalInformationPanel;
    [SerializeField] private TextMeshProUGUI _aviationDivsionsCountText;

    private Vector3 _regionPosition;
    private RectTransform _rectTransform;
    private AirbasesViewerUI _airbasesViewerUI;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _addtionalInformationPanel.SetActive(_airbasesViewerUI.ViewAdttionalInfo);
        if (_airbasesViewerUI.ViewAdttionalInfo)
        {
            var avibaseDivisionsCount = UnitsManager.Instance.AviationDivisions.FindAll(aviationDivision => aviationDivision.PositionAviabase == Target).Count;
            var baseCapacity = (Target.TargetBuilding as Airbase).BaseCapacity;
            if (avibaseDivisionsCount <= baseCapacity)
            {
                _aviationDivsionsCountText.color = Color.black;
            }
            else 
            {
                _aviationDivsionsCountText.color = Color.red;
            }
            _aviationDivsionsCountText.text = $"{avibaseDivisionsCount}/{baseCapacity}";
        }
        _rectTransform.anchoredPosition = GameCamera.Instance.WorldToScreenPointResolutionTrue(_regionPosition + Vector3.up);
    }

    public void RefreshUI(BuildingSlotRegion aviabase, AirbasesViewerUI airbasesViewerUI)
    {
        Target = aviabase;
        _regionPosition = Target.Region.GetProvincesAveragePostion();
        _airbasesViewerUI = airbasesViewerUI;
        _baseClickButton.onClick.AddListener(delegate 
        {
            airbasesViewerUI.ActivateAviasionMode(Target);
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            _airbasesViewerUI.OnAirBaseGetRightClick(this);
        }
    }
}
