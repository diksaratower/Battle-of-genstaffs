using UnityEngine;
using UnityEngine.UI;


public class ChangeCategoryButtonResearchUI : MonoBehaviour
{
    [SerializeField] private Button _targetButton;
    [SerializeField] private ResearchUI _researchUI;
    [SerializeField] private TechnologyCategory _category;


    private void Awake()
    {
        _researchUI.OnChangeCategory += (TechnologyCategory category) => 
        {
            _targetButton.interactable = category != _category;
        };
    }

    private void Start()
    {
        _targetButton.onClick.AddListener(delegate
        {
            _researchUI.RefreshUI(_category);
        });
    }
}
