using UnityEngine;
using UnityEngine.UI;

public class DivisionTemplateModelPreviewUI : MonoBehaviour
{
    [SerializeField] private Camera _previewCamera;
    [SerializeField] private GameObject _infantaryModelPrefab;
    [SerializeField] private GameObject _tankModelPrefab;
    [SerializeField] private string _divisionPreviewLayerMask;
    [SerializeField] private Transform _divisionsModelsParent;
    [SerializeField] private Image _divisionPreviewImage;

    private GameObject _divisionModelGameObject;


    public void UpdateModelView(DivisionTemplate divisionTemplate)
    {
        _divisionPreviewImage.gameObject.SetActive(true);
        _previewCamera.gameObject.SetActive(true);
        if (_divisionModelGameObject != null)
        {
            Destroy(_divisionModelGameObject);
        }

        if (divisionTemplate.GetAverageBattlion().ViewType == DivisionViewType.Infantry)
        {
            _divisionModelGameObject = Instantiate(_infantaryModelPrefab, _divisionsModelsParent);
            SetLayerAllChildren(_divisionModelGameObject.transform, LayerMask.NameToLayer(_divisionPreviewLayerMask));
        }
        if (divisionTemplate.GetAverageBattlion().ViewType == DivisionViewType.Tanks)
        {
            _divisionModelGameObject = Instantiate(_tankModelPrefab, _divisionsModelsParent);
            SetLayerAllChildren(_divisionModelGameObject.transform, LayerMask.NameToLayer(_divisionPreviewLayerMask));
        }

        if (_divisionModelGameObject != null)
        {
            _divisionModelGameObject.GetComponent<DivisionModelView>().enabled = false;
            _divisionModelGameObject.AddComponent<DivisionTemplateModelPreviewModelGO>();
        }
        else
        {
            _divisionPreviewImage.gameObject.SetActive(false);
        }
    }

    public void DisableCamera()
    {
        _previewCamera.gameObject.SetActive(false);
        if (_divisionModelGameObject != null)
        {
            Destroy(_divisionModelGameObject);
        }
    }

    private void SetLayerAllChildren(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    private void OnDisable()
    {
        if (_previewCamera != null)//на случай закрытия игры
        {
            DisableCamera();
        }
    }
}
