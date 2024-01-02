using UnityEngine;
using UnityEngine.UI;

public class RemoveShipMenuUI : MonoBehaviour
{
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    private Ship _targetShip;

    public void Awake()
    {
        _yesButton.onClick.AddListener(delegate 
        {
            if (_targetShip == null)
            {
                throw new System.Exception("Ship for remove is null.");
            }
            Map.Instance.MarineRegions.RemoveShip(_targetShip);
            gameObject.SetActive(false);
        });
        _noButton.onClick.AddListener(delegate 
        {
            gameObject.SetActive(false);
        });
    }

    public void SetShip(Ship ship)
    {
        _targetShip = ship;
    }
}
