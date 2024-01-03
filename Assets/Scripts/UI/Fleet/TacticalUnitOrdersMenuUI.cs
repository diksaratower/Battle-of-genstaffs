using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TacticalUnitOrdersMenuUI : MonoBehaviour
{
    [SerializeField] private Button _seaDominationOrder;
    [SerializeField] private TextMeshProUGUI _seasControlCount;
    [SerializeField] private Button _dropAllOrders;
    [SerializeField] private Button _closeMenu;
    [SerializeField] private LayerMask _selectSeaMask;

    private bool _isSelectingRegion;
    private TacticalFleetUnit _targetUnit;

    private void Start()
    {
        _closeMenu.onClick.AddListener(delegate 
        {
            gameObject.SetActive(false);
        });
        _seaDominationOrder.onClick.AddListener(delegate 
        {
            _isSelectingRegion = true;
        });
        _dropAllOrders.onClick.AddListener(delegate 
        {
            _targetUnit.DropOrder();
        });
    }

    private void Update()
    {
        _seaDominationOrder.interactable = !_isSelectingRegion;
        SelectNewRegion();
        UpdateTextDescription();
    }

    public void SetTargetUnit(TacticalFleetUnit targetUnit)
    {
        _targetUnit = targetUnit;
    }

    private void SelectNewRegion()
    {
        if (_isSelectingRegion)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100000f, _selectSeaMask))
                {
                    if (hit.collider.TryGetComponent<MarineRegion>(out var marineRegion))
                    {
                        _targetUnit.AddRegionDominationOrder(marineRegion);
                    }
                }
            }
        }
    }

    private void UpdateTextDescription()
    {
        if (_targetUnit.Order == FleetOrders.Domination)
        {
            _seasControlCount.text = $"Дествуем в {_targetUnit.WorkingSeasCount} морях.";
        }
        if (_targetUnit.Order == FleetOrders.None)
        {
            _seasControlCount.text = "Нет приказа";
        }
    }
}
