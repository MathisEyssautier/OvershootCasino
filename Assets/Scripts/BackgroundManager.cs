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
    
    private Color _skyColorStart;
    private Color _horizonColorStart;
    private Color _sunColorStart1;
    private Color _sunColorStart2;
    
    private Material _skyboxMaterial;
    
    void Start()
    {
        _skyboxMaterial = RenderSettings.skybox;

        var coroutine = UpdateSun(0.05f);
        StartCoroutine(coroutine);

        _skyColorStart = _skyboxMaterial.GetColor("_SkyColor");
        _horizonColorStart = _skyboxMaterial.GetColor("_HorizonColor");
        _sunColorStart1 = _skyboxMaterial.GetColor("_SunColorOne");
        _sunColorStart2 = _skyboxMaterial.GetColor("_SunColorTwo");

        

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
            if (i > 3) i = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
