using UnityEngine;


public class ColoredCountriesTool : MonoBehaviour
{
    [SerializeField] private GameIU _gameIU;

    private Rect _windowRect = new Rect(20, 80, 165, 210);
    private string _countryTag;
    private bool _coloringProvs;
    private bool _clearingProvs;
    private float _brushSize = 3;

    private void Update()
    {
        _gameIU.BlockDivisionSelecting = _coloringProvs;
        if (_coloringProvs)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (GameCamera.Instance.ChekProvincesWithRadius(out var newProvinces, _brushSize))
                {
                    
                    foreach (var province in newProvinces)
                    {
                        if (_countryTag != "")
                        {
                            if (_clearingProvs)
                            {
                                province.SetOwner(Map.Instance.GetCountryFromId("null"));
                            }
                            if (province.Owner == Map.Instance.GetCountryFromId("null") && province.Owner != Map.Instance.GetCountryFromId(_countryTag))
                            {
                                if (!_clearingProvs)
                                {
                                    province.SetOwner(Map.Instance.GetCountryFromId(_countryTag));
                                }
                                Map.Instance.ColoredProvince(province);
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnGUI()
    {
        _windowRect = GUI.Window(0, _windowRect, DoMyWindow, "Regions create");
    }

    private void DoMyWindow(int windowID)
    {
        _coloringProvs = GUI.Toggle(new Rect(10, 40, 150, 20), _coloringProvs, "Coloring provinces");
        GUI.Label(new Rect(10, 60, 150, 20), "Select country tag:");
        _countryTag = GUI.TextField(new Rect(10, 80, 100, 20), _countryTag);

        _clearingProvs = GUI.Toggle(new Rect(10, 120, 150, 20), _clearingProvs, "Clear provinces");

        GUI.Label(new Rect(10, 140, 150, 20), $"Brush size: {_brushSize}");
        _brushSize = GUI.HorizontalSlider(new Rect(10, 160, 100, 20), _brushSize, 0, 20);

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));

    }
    private void OnDrawGizmos()
    {
        if(!GameCamera.Instance || !this.enabled)
        {
            return;
        }
        Gizmos.color = Color.gray;

        if (GameCamera.Instance.ChekProvincesWithRadius(out var newProvinces, _brushSize))
        {

            foreach (var province in newProvinces)
            {
               
               Gizmos.DrawSphere(province.Position + Vector3.up * 2, 1f);
            }
        }
    }

}
