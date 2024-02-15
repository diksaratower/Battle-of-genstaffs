using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Map : MonoBehaviour, ISaveble
{
    public static Map Instance { get; private set; }
    public List<Country> Countries = new List<Country>();
    public List<Province> Provinces => _provinces;
    public float ProvinceRadius;
    public List<Region> MapRegions = new List<Region>();
    public MarineRegions MarineRegions;

    [SerializeField] private Vector2Int _gridSize = new Vector2Int(2, 2);
    [SerializeField] private Vector3 _meshesRotation = new Vector3(0, 0, -90);
    [SerializeField] private Terrain _terrain;
    [SerializeField] private Transform _countriesParent;
    [SerializeField] private CountriesDataSO _countriesData;
    [SerializeField] private float _seaLevel;
    [SerializeField] private bool _drawMapSize;
    [SerializeField] private Factory _factory;
    [SerializeField] private MilitaryFactory _militaryFactory;
    [SerializeField] private GameObject _cityModelPrefab;
    [SerializeField] protected Transform _citiesParents;

    private List<Province> _provinces = new List<Province>();
    private Texture2D _mainTexture;
    private Province[,] _map = null;

    public void InitializeMap()
    {
        Instance = this;
        GenerateHexGrid(_gridSize, true);
        _mainTexture = GenerateRandomTexture(_gridSize);
        GetComponent<MeshRenderer>().material.mainTexture = _mainTexture;
    }

    public void CreateCountries()
    {
        foreach (var preset in _countriesData.Countries)
        {
            var countryGameObject = new GameObject(preset.ID + "_country");
            countryGameObject.transform.SetParent(_countriesParent);
            var country = countryGameObject.AddComponent<Country>();
            country.SetPreset(preset);
            Countries.Add(country);
        }
    }
    
    public void ColoredProvince(Province province)
    {
        _mainTexture.SetPixel(province.PositionInTexture.x, province.PositionInTexture.y, province.Owner.ColorInMap);
        _mainTexture.Apply();
    }

    public void UpdateColorAllProvincesFast()
    {
        foreach (var province in Provinces)
        {
            if (province.Owner != null)
            {
                _mainTexture.SetPixel(province.PositionInTexture.x, province.PositionInTexture.y, province.Owner.ColorInMap);
            }
        }
        _mainTexture.Apply();
    }

    public Province GetProvinceById(int ID)
    {
        return Provinces[ID];
    }

    public Country GetCountryFromId(string id)
    {
        foreach (var country in Countries)
        {
            if (country.ID == id)
            {
                return country;
            }
        }
        throw new System.Exception("Invalid country ID");
    }

    public Vector2 GetWorldBounds()
    {
        var hex = GenerateHex(ProvinceRadius);
        var insideRadius = GetHexInsideRadius(hex);
        var X = (_gridSize.x * (insideRadius * 2));
        var Z = (ProvinceRadius * 1.5f) * _gridSize.y;
        return new Vector2(X, Z);
    }

    public Vector3 GetWorldPosition(SpriteRenderer spriteRenderer, Vector2Int pixelCoord)
    {
        Texture2D texture = spriteRenderer.sprite.texture;
        Vector2 textureCoord = new Vector2((float)pixelCoord.x / (float)texture.width, (float)pixelCoord.y / (float)texture.height); // вычисляем текстурные координаты
        Vector3 localPosition = new Vector3(spriteRenderer.sprite.bounds.min.x + textureCoord.x * spriteRenderer.sprite.bounds.size.x,
                                            spriteRenderer.sprite.bounds.min.y + textureCoord.y * spriteRenderer.sprite.bounds.size.y,
                                            0f); // вычисляем локальные координаты пикселя
        return spriteRenderer.transform.TransformPoint(localPosition); // переводим локальные координаты в мировые
    }

    public Mesh CreateProvinceMeshFromPosition(Matrix4x4 matrix, Vector3 provincePosition, bool landScapeTrue = false)
    {
        var hex = GenerateHex(ProvinceRadius);
        Mesh mesh = null;
        if (landScapeTrue)
        {
            mesh = UpdateMeshLandscapeTrue(hex, provincePosition, matrix);
        }
        else
        {
            mesh = GetMeshFromHex(hex);
        }    
        return mesh;
    }
  
    public static Province GetProvinceFromPosition(Vector3 position)
    {
        foreach (var prov in Instance.Provinces)
        {
            if (prov.Position == position)
            {
                return prov;
            }
        }
        return null;
    }

    private void GenerateHexGrid(Vector2Int size, bool usePregenerated = false)
    {
        _provinces.Clear();
        _map = new Province[size.x, size.y];
        List<CombineInstance> combine = new List<CombineInstance>();
        var hex = GenerateHex(ProvinceRadius);
        var insideRadius = GetHexInsideRadius(hex);
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var X = (x * (insideRadius * 2));

                var Z = (ProvinceRadius * 1.5f) * y;

                if (y % 2 == 0)
                {
                    X += insideRadius;
                }

                var provincePosition = new Vector3(X, 0, Z) + transform.position;

                if (!IsProvinceLandSeaLevel(provincePosition))
                {
                    continue;
                }
                var province = CreateProvince(provincePosition, new Vector2Int(x, y), hex);

                _map[x, y] = province;
                _provinces.Add(province);
                
                if (usePregenerated == false)
                {
                    var comb = CreateProvinceCombineInstance(province, hex, size, provincePosition);
                    combine.Add(comb);
                }
            }
        }
        if (usePregenerated == false)
        {
            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.CombineMeshes(combine.ToArray());

            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
        }
    }

    private CombineInstance CreateProvinceCombineInstance(Province province, List<Vector3> hex, Vector2Int size, Vector3 provincePosition)
    {
        var provinceHeight = GetProvinceHeight(provincePosition);
        var combineInstane = new CombineInstance();
        var mesh = GetMeshFromHex(hex);
        SetUvToMesh(mesh, province.PositionInGrid, size.x, size.y);

        var provinceLocalPostion = provincePosition - transform.position;
        var q = new Quaternion
        {
            eulerAngles = _meshesRotation
        };
        combineInstane.transform = Matrix4x4.Translate(provinceLocalPostion) * Matrix4x4.Scale(new Vector3(0.99f, 0.99f, 0.99f));//Matrix4x4.Scale(new Vector3(0.95f, 0.95f, 0.95f));
        mesh = UpdateMeshLandscapeTrue(hex, province.Position, Matrix4x4.Rotate(q));
        SetUvToMesh(mesh, province.PositionInGrid, size.x, size.y);
        hex.ForEach(ver =>
        {
            Matrix4x4 m = Matrix4x4.Translate(new Vector3(provinceLocalPostion.x, provinceHeight, provinceLocalPostion.z))
            * Matrix4x4.Rotate(new Quaternion() { eulerAngles = new Vector3(0, 0, 90) });
            province.Vertices.Add(m.MultiplyPoint3x4(ver));
        });
        combineInstane.mesh = mesh;
        return combineInstane;
    }

    private Province CreateProvince(Vector3 provincePosition, Vector2Int positionInGrid, List<Vector3> hex)
    {
        var provinceHeight = GetProvinceHeight(provincePosition);
        var province = new Province(positionInGrid, _provinces.Count, provincePosition, new Vector3(provincePosition.x, provinceHeight, provincePosition.z));
        var provinceLocalPostion = provincePosition - transform.position;
        hex.ForEach(ver =>
        {
            Matrix4x4 m = Matrix4x4.Translate(new Vector3(provinceLocalPostion.x, provinceHeight, provinceLocalPostion.z))
            * Matrix4x4.Rotate(new Quaternion() { eulerAngles = new Vector3(0, 0, 90) });
            province.Vertices.Add(m.MultiplyPoint3x4(ver));
        });
        return province;
    }

    private void SpawnCities()
    {
        foreach (var region in MapRegions)
        {
            foreach (var city in region.Cities)
            {
                var cityModel = Instantiate(_cityModelPrefab, _citiesParents);
                cityModel.transform.position = city.CityProvince.LandscapeTruePosition;
            }
        }
    }

    private float GetProvinceHeight(Vector3 provincePos)
    {
        return _terrain.SampleHeight(provincePos) + _terrain.transform.position.y;
    }

    private List<int> GetProvinceContacts(Province province)
    {
        var neighbours = new List<Province>();
        neighbours.Add(GetProvinceByGridCoords(province.PositionInGrid.x, province.PositionInGrid.y + 1));
        neighbours.Add(GetProvinceByGridCoords(province.PositionInGrid.x, province.PositionInGrid.y - 1));
        int koof;
        if (province.PositionInGrid.y % 2 == 0)
        {
            koof = -1;
        }
        else
        {
            koof = 1;
        }
        neighbours.Add(GetProvinceByGridCoords(province.PositionInGrid.x + koof, province.PositionInGrid.y));
        neighbours.Add(GetProvinceByGridCoords(province.PositionInGrid.x - koof, province.PositionInGrid.y));

        neighbours.Add(GetProvinceByGridCoords(province.PositionInGrid.x - koof, province.PositionInGrid.y - 1));
        neighbours.Add(GetProvinceByGridCoords(province.PositionInGrid.x - koof, province.PositionInGrid.y + 1));
        var ids = new List<int>();
        foreach (var neigh in neighbours)
        {
            if (neigh != null)
            {
                ids.Add(neigh.ID);
            }
        }
        return ids;
    }

    private Province GetProvinceByGridCoords(int x, int y)
    {
        var prov = GetProvinceByGridCoords(new Vector2Int(x, y));
        return prov;
    }

    private Province GetProvinceByGridCoords(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= _gridSize.x || coords.x >= _gridSize.y)
        {
            return null;
        }
        return _map[coords.x, coords.y];
    }
    
    private List<Vector3> GenerateHex(float radius)
    {
        var vertecs = new List<Vector3>();
        for (int i = 0; i < 6; i++)
        {
            var q = new Quaternion();
            q.eulerAngles = new Vector3(60 * i, 0, 0);
            var vect = Matrix4x4.Rotate(q).MultiplyVector(Vector3.forward * radius);
            vertecs.Add(vect);
        }
        return vertecs;
    }

    private float GetHexInsideRadius(List<Vector3> verteces)
    {
        var rectBottom = Vector3.Distance(verteces[0], verteces[1]) / 2;
        var rectSide = ProvinceRadius;
        //Теорема пифагора
        return Mathf.Sqrt(Mathf.Pow(rectSide, 2) - Mathf.Pow(rectBottom, 2));
    }

    private Mesh GetMeshFromHex(List<Vector3> hexVerteces)
    {
        List<Vector3> verteces = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int i = 0; i < hexVerteces.Count; i++)
        {
            if (i == 0)
            {
                verteces.Add(hexVerteces[0]);
                verteces.Add(hexVerteces[hexVerteces.Count - 1]);
                verteces.Add(Vector3.zero);
                //triangles.Add(i);
            }
            if (i > 0)
            {
                verteces.Add(hexVerteces[i]);
                verteces.Add(hexVerteces[i - 1]);
                verteces.Add(Vector3.zero);
                //triangles.Add(i);
            }
        }
        for (int i = 0; i < 18; i++)
        {
            triangles.Add(i);
        }
        var mesh = new Mesh();
        mesh.vertices = verteces.ToArray();
        mesh.triangles = triangles.ToArray();


        return mesh;
    }

    private Mesh UpdateMeshLandscapeTrue(List<Vector3> hexVerteces, Vector3 provincePosition, Matrix4x4 matrix)
    {
        var mesh = GetMeshFromHex(hexVerteces);
        var vertices = new List<Vector3>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            var newVert = matrix.MultiplyPoint3x4(new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z));
            newVert.y = GetProvinceHeight(provincePosition + newVert);
            vertices.Add(newVert);
        }
        mesh.vertices = vertices.ToArray();
        return mesh;
    }

    private Mesh FastCopyMesh(Mesh mesh)
    {
        Mesh newmesh = new Mesh();
        newmesh.vertices = mesh.vertices;
        newmesh.triangles = mesh.triangles;
        newmesh.normals = mesh.normals;
        newmesh.tangents = mesh.tangents;
        return newmesh;
    }

    private void ColoredProvincesFast(List<Province> forRecolor)
    {
        foreach (var province in forRecolor)
        {
            _mainTexture.SetPixel(province.PositionInTexture.x, province.PositionInTexture.y, province.Owner.ColorInMap);
        }
        _mainTexture.Apply();
    }

    public void SetUvToMesh(Mesh mesh, Vector2Int pixcelPos, int texWidth, int texHeight)
    {
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            var seedWight = ((1 / (float)texWidth) * 0.001f);
            var seedHeight = ((1 / (float)texHeight) * 0.001f);
            var uvVect = new Vector2((pixcelPos.x / (float)texWidth) + seedWight, (pixcelPos.y / (float)texHeight) + seedHeight);
            uvs.Add(uvVect);
        }

        mesh.uv = uvs.ToArray();
    }
    private Texture2D GenerateRandomTexture(Vector2Int textureSize)
    {
        var generatedTexture = new Texture2D(textureSize.x, textureSize.y);
        generatedTexture.filterMode = FilterMode.Point;
        for (int x = 0; x < textureSize.x; x++)
        {
            for (int y = 0; y < textureSize.y; y++)
            {
                Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                generatedTexture.SetPixel(x, y, randomColor);
            }
        }

        generatedTexture.Apply();
        return generatedTexture;
    }

    private bool IsProvinceLandSeaLevel(Vector3 provincePoition)
    {
        return GetProvinceHeight(provincePoition) > _seaLevel;
    }

    string ISaveble.GetFileName()
    {
        return "map";
    }

    string ISaveble.Save()
    {
        return new MapSerialize(this).SaveToJson();
    }

    void ISaveble.Load(string data)
    {
        ///LoadAsync(data);
        MapSerialize ser = null;
        ser = new MapSerialize(data); 
        ser.Load(this);
    }

    public async void LoadAsync(string data)
    {
        MapSerialize ser = null;
        await Task.Run(delegate { ser = new MapSerialize(data); });
        ser.Load(this);
    }

    System.Type ISaveble.GetSaveType()
    {
        throw new System.NotImplementedException();
    }

    public class MapSerialize : SerializeForSave
    {
        public List<ProvinceSave> Provinces = new List<ProvinceSave>();
        public List<RegionSave> Regions = new List<RegionSave>();

        public MapSerialize(Map map)
        {
            foreach (var province in map.Provinces)
            {
                Provinces.Add(new ProvinceSave() { CountryOwnerID = province.Owner.ID, ContactsIDs = province.ContactsIDs, ID = province.ID });
            }
            foreach (var mapReg in map.MapRegions)
            {     
                Regions.Add(Region.SaveRegion(mapReg));
            }
        }

        public MapSerialize(string jsonSave)
        {
            JsonUtility.FromJsonOverwrite(jsonSave, this);
        }

        public override void Load(object objTarget)
        {
            var map = (Map)objTarget;
            var forRecolor = new List<Province>();
            foreach (var provinceSave in Provinces)
            {
                var province = map.Provinces[Provinces.IndexOf(provinceSave)];//map.Provinces.Find(pr => pr.ID == provinceSave.ID);
                province.SetOwner(map.GetCountryFromId(provinceSave.CountryOwnerID), false);
                province.ContactsIDs = provinceSave.ContactsIDs;
                forRecolor.Add(province);
            }
            map.ColoredProvincesFast(forRecolor);

            foreach (var reg in Regions)
            {
                map.MapRegions.Add(Region.LoadRegion(reg));
            }
            map.SpawnCities();
        }

        public override string SaveToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        [Serializable]
        public class ProvinceSave
        {
            public string CountryOwnerID;
            public int ID;
            public List<int> ContactsIDs = new List<int>();
        }
      
    }

}
