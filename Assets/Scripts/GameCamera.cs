using System.Collections.Generic;
using System.Drawing.Design;
using UnityEngine;
using UnityEngine.UI;


public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance;
    public CanvasScaler CanvasScaler;
    public Camera GCamera { get => _camera; }

    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraSpeed;

    private float _minCameraHeight = 20;
    private float _maxCameraHeight = 250;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetCameraWithCurrentCountry();
    }

    private void Update()
    {
        MoveCameraFromDirection(Vector3.forward * Input.GetAxis("Vertical"), true);
        MoveCameraFromDirection(Vector3.right * Input.GetAxis("Horizontal"), true);
        ChangeCameraHeightWithWorldBounds();
    }

    public bool ChekHitToProvinceWithMousePosition(out Province result)
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.GetComponent<TerrainCollider>())
            {
                var prov = Map.Instance.Provinces.Find(p => Vector2.Distance(new Vector2(p.Position.x, p.Position.z), new Vector2(hit.point.x, hit.point.z)) <=Map.Instance.ProvinceRadius);
                if (prov != null)
                {
                    result = prov;
                    return true;
                }
            }
        }
        result = null;
        return false;
    }

    public bool ChekProvincesWithRadius(out List<Province> result, float radius)
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.GetComponent<TerrainCollider>())
            {
                var provs = Map.Instance.Provinces.FindAll(p => Vector2.Distance(new Vector2(p.Position.x, p.Position.z), new Vector2(hit.point.x, hit.point.z)) < radius);
                if (provs != null)
                {
                    result = provs;
                    return true;
                }
            }
        }
        result = null;
        return false;
    }

    public Vector3 WorldToScreenPointResolutionTrue(Vector3 worldPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        /*float refWidth = CanvasScaler.referenceResolution.x;
        float refHeight = CanvasScaler.referenceResolution.y; 
        bool matchWidth = true; //true if screen match mode is set to match the width, 
                                //false if is set to match the height
        if(Screen.width == refWidth && Screen.height == refHeight)
        {
            return screenPos;
        }

        if (matchWidth)
        {
            screenPos.x *= refWidth / Screen.width;
            screenPos.y *= refWidth / Screen.width;
        }
        else
        {
            screenPos.x *= refHeight / Screen.height;
            screenPos.y *= refHeight / Screen.height;
        }
        return screenPos;*/
        return CorrectScreenPointResolutionTrue(screenPos);
    }

    public Vector3 CorrectScreenPointResolutionTrue(Vector3 screenPoint)
    {
        float refWidth = CanvasScaler.referenceResolution.x;
        float refHeight = CanvasScaler.referenceResolution.y;
        bool matchWidth = true; 
        var correctedScreenPos = screenPoint;
        if (Screen.width == refWidth && Screen.height == refHeight)
        {
            return correctedScreenPos;
        }

        if (matchWidth)
        {
            correctedScreenPos.x *= refWidth / Screen.width;
            correctedScreenPos.y *= refWidth / Screen.width;
        }
        else
        {
            correctedScreenPos.x *= refHeight / Screen.height;
            correctedScreenPos.y *= refHeight / Screen.height;
        }
        return correctedScreenPos;
    }

    private void SetCameraWithCurrentCountry()
    {
        foreach (var region in Player.CurrentCountry.GetCountryRegions())
        {
            if (region.RegionCapital != null)
            {
                transform.position = region.RegionCapital.CityProvince.Position + new Vector3(4f, 74f, -65.3f);
                break;
            }
        }
    }    

    private void ChangeCameraHeightWithWorldBounds()
    {
        var heightChange = Input.mouseScrollDelta.y;
        if(transform.position.y <= _minCameraHeight && heightChange < 0) 
        {
            heightChange = 0;
        }
        if(transform.position.y >= _maxCameraHeight && heightChange > 0)
        {
            heightChange = 0;
        }
        transform.position += Vector3.up * heightChange;
    }

    private void MoveCameraFromDirection(Vector3 direction, bool useWorldBounds)
    {
        Vector3 correctedDirection;
        if (useWorldBounds == true)
        {
            correctedDirection = GetCorrectedDirectionFromWorldBounds(direction);
        }
        else
        {
            correctedDirection = direction;
        }
        transform.position += correctedDirection * Time.deltaTime * _cameraSpeed;
    }

    private Vector3 GetCorrectedDirectionFromWorldBounds(Vector3 direction)
    {
        var correctedDirection = direction;
        if (transform.position.x <= 0 && direction.x < 0)
        {
            correctedDirection.x = 0;
        }
        if (transform.position.z <= 0 && direction.z < 0)
        {
            correctedDirection.z = 0;
        }

        if (transform.position.x >= Map.Instance.GetWorldBounds().x && direction.x > 0)
        {
            correctedDirection.x = 0;
        }
        if (transform.position.z >= Map.Instance.GetWorldBounds().y && direction.z > 0)
        {
            correctedDirection.z = 0;
        }

        return correctedDirection;
    }
}