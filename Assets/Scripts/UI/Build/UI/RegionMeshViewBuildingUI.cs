using System;
using System.Collections.Generic;
using UnityEngine;


public class RegionMeshViewBuildingUI : MonoBehaviour
{
    public Region RegionTarget { get; private set; }

    [SerializeField] private Mesh _meshPrefab;
    [SerializeField] private GameObject _linePrefab;

    private Material _insideProvincesMaterial;
    private Color _insideViewColorInNormal;
    private Color _insideViewColorInBuildProcess;


    public void RefreshView(Region region, Color boardViewColor, Color insideViewColor, Color insideViewColorInBuildProcess, CountryBuild countryBuild)
    {
        RegionTarget = region;
        _insideViewColorInNormal = insideViewColor;
        _insideViewColorInBuildProcess = insideViewColorInBuildProcess;
        DrawRegionMesh(region, boardViewColor, insideViewColor);
        Action updateProgressBuildingsCountDelegate = delegate
        {
            UpdateColorFromSituation(countryBuild);
        };
        countryBuild.OnAddedBuildingToQueue += updateProgressBuildingsCountDelegate;
        countryBuild.OnRemovedBuildingFromQueue += updateProgressBuildingsCountDelegate;
    }

    private void UpdateColorFromSituation(CountryBuild countryBuild)
    {
        var inBuildProcess = countryBuild.BuildingsQueue.Exists(slot => slot.BuildRegion == RegionTarget);
        if(inBuildProcess) 
        {
            _insideProvincesMaterial.color = _insideViewColorInBuildProcess;
        }
        else 
        {
            _insideProvincesMaterial.color = _insideViewColorInNormal;
        }
    }

    private void DrawRegionMesh(Region region, Color boardViewColor, Color insideViewColor)
    {
        gameObject.AddComponent<MeshFilter>().mesh = GetRegionMesh(region);
        var insideMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _insideProvincesMaterial = insideMat;
        insideMat.color = insideViewColor;
        var boardMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        boardMat.color = boardViewColor;
        gameObject.AddComponent<MeshRenderer>().materials = new Material[2] { boardMat, insideMat };
        gameObject.AddComponent<MeshCollider>();
        RegionTarget = region;
    }

    public Mesh GetRegionMesh(Region region)
    {
        List<CombineInstance> combines = new List<CombineInstance>();
        var board = region.GetRegionBoard();
        var boardOutSide = new List<Province>();
        foreach (var province in board)
        {
            boardOutSide.AddRange(province.Contacts.FindAll(con => (region.Provinces.Contains(con) == false && boardOutSide.Contains(con) == false)));
        }

        DrawFrontPlanLine(board, boardOutSide);

        var instance = new CombineInstance();
        instance.mesh = GetMeshFromProvinces(region.Provinces);
        instance.transform = Matrix4x4.Scale(Vector3.one);
        combines.Add(instance);
        var mesh = new Mesh();
        mesh.CombineMeshes(combines.ToArray());
        return mesh;
    }

    private void DrawFrontPlanLine(List<Province> front, List<Province> front2)
    {
        foreach (var frontProvince in front)
        {
            foreach (var frontProvince2 in front2)
            {
                var points = frontProvince.GetIntersectionsPoints(frontProvince2);
                if (points.Count == 0 || points.Count == 1)
                {
                    continue;
                }

                var drawedFront = Instantiate(_linePrefab, transform).GetComponent<LineRenderer>();
                drawedFront.positionCount = points.Count;
                for (int j = 0; j < points.Count; j++)
                {
                    drawedFront.SetPosition(j, points[j] + (Vector3.up * 3f));
                }

            }
        }
    }

    private Mesh GetMeshFromProvinces(List<Province> provinces)
    {
        List<CombineInstance> combines = new List<CombineInstance>();

        foreach (Province province in provinces)
        {
            var combine = new CombineInstance();
            var q = new Quaternion
            {
                eulerAngles = new Vector3(0, 0, -90)
            };

            combine.mesh = Map.Instance.CreateProvinceMeshFromPosition(Matrix4x4.Rotate(q), province.Position, true);
            float scale = 1f;
            combine.transform = Matrix4x4.Translate(province.Position + new Vector3(0, 1.5f, 0)) * Matrix4x4.Scale(new Vector3(scale, scale, scale));//* Matrix4x4.Rotate(q);
            combines.Add(combine);
        }
        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combines.ToArray());
        return mesh;
    }
}
