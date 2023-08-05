using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "TooltipData", menuName = "ScriptableObjects/TooltipsDataSO", order = 1)]
public class TooltipsDataSO : ScriptableObject
{
    public GameObject MenuStandartPrefab;
    public TextMeshProUGUI SimpleTextPrefab;
}
