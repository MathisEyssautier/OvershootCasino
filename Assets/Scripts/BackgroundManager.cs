using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BackgroundManager : MonoBehaviour
{
    
    [SerializeField] GameObject treesContainer;
    [SerializeField] GameObject buildingsContainer;
    [SerializeField] Image sky;
    [SerializeField] private float maxHeight = 3f;
    [SerializeField] private Texture2D[] sunTextures;
    [SerializeField] private MeshRenderer grid;
    

    public float T = 0;
    
    private Material _skyMat;
    private Vector3[] _startPositions;
    private Vector3[] _startPositionsTrees;
    private float[] _endPositions;
    private float[] _endPositionsTrees;
    private float[] _speedOffsets;
    private float[] _speedOffsetsTrees;
    private List<GameObject> _buildings = new();
    private List<GameObject> _trees = new();
    
    [SerializeField] private Color _skyColorStart;
    [SerializeField] private Color _horizonColorStart;
    [SerializeField] private Color _sunColorStart1;
    [SerializeField] private Color _sunColorStart2;
    
    [SerializeField] private Color gridColorStart;
    [SerializeField] private Color groundColorStart;
    
    private Color _skyColorEnd;
    private Color _horizonColorEnd;
    private Color _sunColorEnd1;
    private Color _sunColorEnd2;
    
    private Color _gridColorEnd;
    private Color _groundColorEnd;
    
    private Material _skyboxMaterial;
    private Material _gridMaterial;
    
    void Start()
    {
        _skyboxMaterial = RenderSettings.skybox;
        _gridMaterial = grid.material;

        var coroutine = UpdateSun(0.04f);
        StartCoroutine(coroutine);
        
        _skyboxMaterial.SetColor("_SkyColor", _skyColorStart);
        _skyboxMaterial.SetColor("_HorizonColor", _horizonColorStart);
        _skyboxMaterial.SetColor("_SunColorOne", _sunColorStart1);
        _skyboxMaterial.SetColor("_SunColorTwo", _sunColorStart2);
        
        _skyColorEnd = new Color(_skyColorStart.grayscale, _skyColorStart.grayscale, _skyColorStart.grayscale);
        _horizonColorEnd = new Color(_horizonColorStart.grayscale, _horizonColorStart.grayscale, _horizonColorStart.grayscale);
        _sunColorEnd1 = new Color(_sunColorStart1.grayscale, _sunColorStart1.grayscale, _sunColorStart1.grayscale);
        _sunColorEnd2 = new Color(_sunColorStart2.grayscale, _sunColorStart2.grayscale, _sunColorStart2.grayscale);

        _gridColorEnd = new Color(gridColorStart.grayscale, gridColorStart.grayscale, gridColorStart.grayscale);
        _groundColorEnd = new Color(groundColorStart.grayscale, groundColorStart.grayscale, groundColorStart.grayscale);
        
        for (int i = 0; i < buildingsContainer.transform.childCount; i++)
        {
            _buildings.Add(buildingsContainer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < treesContainer.transform.childCount; i++)
        {
            _trees.Add(treesContainer.transform.GetChild(i).gameObject);
        }
        
        _startPositions = new Vector3[_buildings.Count];
        _speedOffsets = new float[_buildings.Count];
        _endPositions = new float[_buildings.Count];
        
        _startPositionsTrees = new Vector3[_trees.Count];
        _speedOffsetsTrees = new float[_trees.Count];
        _endPositionsTrees = new float[_trees.Count];

        for (int i = 0; i < _buildings.Count; i++)
        {
            if (_buildings[i] != null)
            {
                _startPositions[i] = _buildings[i].transform.position;
                _speedOffsets[i] = Random.Range(0.5f, 5f);
                _endPositions[i] = maxHeight + Random.Range(-0.5f, 0.5f);
            }
        }
        for (int i = 0; i < _trees.Count; i++)
        {
            if (_trees[i] != null)
            {
                _startPositionsTrees[i] = _trees[i].transform.position;
                _speedOffsetsTrees[i] = 1f;
                _endPositionsTrees[i] = _startPositionsTrees[i].y - maxHeight/8;
            }
        }
    }
    
    void Update()
    {
        T += Time.deltaTime/10;
        
        _skyboxMaterial.SetColor("_SkyColor", Color.Lerp(_skyColorStart, _skyColorEnd, T));
        _skyboxMaterial.SetColor("_HorizonColor", Color.Lerp(_horizonColorStart, _horizonColorEnd, T));
        _skyboxMaterial.SetColor("_SunColorOne", Color.Lerp(_sunColorStart1, _sunColorEnd1, T));
        _skyboxMaterial.SetColor("_SunColorTwo", Color.Lerp(_sunColorStart2, _sunColorEnd2, T));

        _gridMaterial.SetColor("_GridColor", Color.Lerp(gridColorStart, _gridColorEnd, T));
        _gridMaterial.SetColor("_GroundColor", Color.Lerp(groundColorStart, _groundColorEnd, T));
        
        for (int i = 0; i < _buildings.Count; i++)
        {
            float t = Mathf.Clamp01(Mathf.Pow(T, _speedOffsets[i]));

            Vector3 pos = _startPositions[i];
            pos.y += Mathf.Lerp(0f, _endPositions[i], t);
            _buildings[i].transform.position = pos;
        }
        
        for (int i = 0; i < _trees.Count; i++)
        {
            float t = Mathf.Clamp01(Mathf.Pow(T, _speedOffsetsTrees[i]));

            Vector3 pos = _startPositionsTrees[i];
            pos.y += Mathf.Lerp(0f, _endPositionsTrees[i], t);
            _trees[i].transform.position = pos;
        }
    }
    
    private IEnumerator UpdateSun(float waitTime) {
        int i = 0;
        while (true) {
            _skyboxMaterial.SetTexture("_SunMask", sunTextures[i]);
            i++;
            if (i > sunTextures.Length - 1) i = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
