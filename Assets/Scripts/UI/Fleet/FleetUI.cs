using TMPro;
using UnityEngine;


public class FleetUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _shipsCountText;

    private void Update()
    {
        _shipsCountText.text = "В нашем славном флоте 100 кораблей!";
    }
}
