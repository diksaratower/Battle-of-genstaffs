using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MapExperementalFunctions : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _countriesSpriteRenderer;
    [SerializeField] private GameObject _countryNameTextPrefab;

    private void LoadCountriesColorsFromTexture()
    {
        float smooth = 5f;
        foreach (var province in Map.Instance.Provinces)
        {

            var pixCoords = CalculatePixelCoordFromWorldPosition(new Vector3(province.Position.x, _countriesSpriteRenderer.transform.position.y,
                province.Position.z), _countriesSpriteRenderer);
            var color = _countriesSpriteRenderer.sprite.texture.GetPixel(pixCoords.x, pixCoords.y);

            if (GetCountryByColor(color, smooth) != Map.Instance.GetCountryFromId("null"))
            {
                province.SetOwner(GetCountryByColor(color, smooth));
            }
            else
            {
                province.SetOwner(GetCountryByColor(color, smooth));
                var pixels = GetPixelNeighbors(pixCoords.x, pixCoords.y, _countriesSpriteRenderer.sprite.texture);
                foreach (var pix in pixels)
                {
                    if (GetCountryByColor(pix, smooth) != Map.Instance.GetCountryFromId("null"))
                    {
                        province.SetOwner(GetCountryByColor(color, smooth));
                        break;
                    }
                }
            }

        }
        _countriesSpriteRenderer.gameObject.SetActive(false);
    }

    private Texture2D ToTexture2D(RenderTexture renderTexture)
    {
        var texture2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        Graphics.ConvertTexture(renderTexture, texture2d);
        //Graphics.CopyTexture(renderTexture, texture2d);
        return texture2d;
    }


    private Country GetCountryByColor(Color color, float smooth)
    {
        if (Map.Instance.Countries.Find(cou => CompareColors(cou.ColorInMap, color, smooth)) != null)
        {
            //Debug.Log(Countries.Find(cou => CompareColors(cou.color, color)).ID);
            return Map.Instance.Countries.Find(cou => CompareColors(cou.ColorInMap, color, smooth));
        }
        else
        {
            return Map.Instance.GetCountryFromId("null");
        }
    }


    public bool CompareColors(Color color1, Color color2, float smoothKoof)
    {
        int redDiff = Mathf.Abs((int)(color1.r * 255) - (int)(color2.r * 255));
        int greenDiff = Mathf.Abs((int)(color1.g * 255) - (int)(color2.g * 255));
        int blueDiff = Mathf.Abs((int)(color1.b * 255) - (int)(color2.b * 255));

        return redDiff <= smoothKoof && greenDiff <= smoothKoof && blueDiff <= smoothKoof;
    }

    public List<Color> GetPixelNeighbors(int x, int y, Texture2D texture)
    {
        List<Color> neighbors = new List<Color>(new Color[4]);

        // получаем цвета соседних пикселей
        if (x > 0)
            neighbors[0] = texture.GetPixel(x - 1, y);  // левый пиксель
        if (x < texture.width - 1)
            neighbors[1] = texture.GetPixel(x + 1, y);  // правый пиксель
        if (y > 0)
            neighbors[2] = texture.GetPixel(x, y - 1);  // верхний пиксель
        if (y < texture.height - 1)
            neighbors[3] = texture.GetPixel(x, y + 1);  // нижний пиксель

        return neighbors;
    }

    private GameObject DrawCountryText(Country country)
    {
        var countryNameTextGO = Instantiate(_countryNameTextPrefab);
        //_polandText = countryNameTextGO;
        var countryNameTransform = countryNameTextGO.transform;
        var text = countryNameTextGO.GetComponentInChildren<TextMeshProUGUI>();
        text.text = country.Name;//Countries[0].Name;
        List<KeyValuePair<float, Province[]>> distances = new List<KeyValuePair<float, Province[]>>();
        for (int i = 0; i < Map.Instance.Provinces.Count; i++)
        {
            for (int j = 0; j < Map.Instance.Provinces.Count; j++)
            {
                if (Map.Instance.Provinces[i].Owner == country && Map.Instance.Provinces[j].Owner == country)
                {
                    distances.Add(new KeyValuePair<float, Province[]>(Vector2.Distance(Map.Instance.Provinces[i].Position, Map.Instance.Provinces[j].Position)
                    , new Province[2] { Map.Instance.Provinces[i], Map.Instance.Provinces[j] }));
                }
            }
        }
        var distancesFloat = new List<float>();
        for (int i = 0; i < distances.Count; i++)
        {
            distancesFloat.Add(distances[i].Key);
        }
        var distMax = distancesFloat.Max();
        var provs = distances.Find(p_arr => p_arr.Key == distMax);

        var point = provs.Value[0].Position + ((provs.Value[1].Position - provs.Value[0].Position) / 2);
        point.y += 1;
        countryNameTransform.transform.position = point;
        //var last = countryNameTransform.transform.localEulerAngles;
        //countryNameTransform.LookAt(provs.Value[0].transform.position, Vector3.right);
        countryNameTransform.LookAtAxis(point, false, true, true);
        //countryNameTransform.transform.localEulerAngles = new Vector3(last.x, countryNameTransform.transform.localEulerAngles.y, countryNameTransform.transform.localEulerAngles.z);
        countryNameTransform.localScale = new Vector3(provs.Key, provs.Key * 0.8f, 1);

        return countryNameTextGO;
        //Destroy(countryNameTextGO);
        //return new Vector3[2] { provs.Value[0].transform.position, provs.Value[1].transform.position };
    }

    private Vector2Int CalculatePixelCoordFromWorldPosition(Vector3 position, SpriteRenderer spriteRenderer)
    {
        var sprite = spriteRenderer.sprite;
        var texture = sprite.texture;
        Vector3 spritePos = spriteRenderer.worldToLocalMatrix.MultiplyPoint3x4(position);
        float pixelsPerUnit = sprite.pixelsPerUnit;
        float halfRealTexWidth = texture.width * 0.5f;
        float halfRealTexHeight = texture.height * 0.5f;
        int texPosX = (int)(spritePos.x * pixelsPerUnit + halfRealTexWidth);
        int texPosY = (int)(spritePos.y * pixelsPerUnit + halfRealTexHeight);
        return new Vector2Int(texPosX, texPosY);
    }
}
