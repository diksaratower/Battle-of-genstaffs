using UnityEngine;


public class AviationRadiusViewUI : MonoBehaviour
{
    public float TragetRadius { get; private set; }

    [SerializeField] private Transform _radiusTransform;

    public void UpdateRadiusView(AviationDivision aviationDivision)
    {
        float raduis = aviationDivision.AttackDistance;
        transform.position = aviationDivision.PositionAviabase.Region.GetProvincesAveragePostion() + (Vector3.up * 5);
        _radiusTransform.localScale = new Vector3(raduis, _radiusTransform.localScale.y, raduis);
    }
}
