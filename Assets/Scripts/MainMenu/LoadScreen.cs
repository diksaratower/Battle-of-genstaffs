using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoadScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _loadProgressProcentText;
    [SerializeField] private Image _loadProgressBar;

    private AsyncOperation _loadOperation;

    public void StartMonitoringLoading(AsyncOperation operation)
    {
        if (gameObject.activeSelf != true)
        {
            gameObject.SetActive(true);
        }
        _loadOperation = operation;
    }

    private void Update()
    {
        if(_loadOperation == null)
        {
            return;
        }
        _loadProgressProcentText.text = Mathf.RoundToInt(_loadOperation.progress * 100).ToString() + "%";
        _loadProgressBar.fillAmount = _loadOperation.progress;
    }
}
