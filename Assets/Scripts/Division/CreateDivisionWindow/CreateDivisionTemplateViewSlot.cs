using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CreateDivisionTemplateViewSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _templateName;
    [SerializeField] private Button _editTemplateButton;
    [SerializeField] private Button _createDivisionButton;

    private CreateDivisionWindow _createDivisionWindow;

    private void Update()
    {
        _createDivisionButton.interactable = false;
    }

    public void RefreshUI(DivisionTemplate templ, CreateDivisionWindow createDivisionWindow)
    {
        _createDivisionWindow = createDivisionWindow;
        _templateName.text = templ.Name;
        _createDivisionButton.onClick.AddListener(delegate {
            createDivisionWindow.CreateDivisionInPlace(templ);
        });
        _editTemplateButton.onClick.AddListener(delegate { 
            createDivisionWindow.OpenDivisionEditor(templ);
        });
    }

}
