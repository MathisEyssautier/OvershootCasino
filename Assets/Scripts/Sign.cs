using UnityEngine;

public class Sign : MonoBehaviour
{
    
    [SerializeField] private MeshRenderer logo;
    [SerializeField] private Texture2D onTexture;
    [SerializeField] private Texture2D offTexture;
    
    private Material _logoMaterial;
    private bool _isOn = true;
    
    void Start() {
        _logoMaterial = logo.material;
        _logoMaterial.SetTexture("_EmissionMap", onTexture);
    }
    
    public void ToggleLogo() {
        //if (_isOn) {
            _logoMaterial.SetTexture("_EmissionMap", offTexture);
            _isOn = false;
        //}
        //else {
        //    _logoMaterial.SetTexture("_EmissionMap", onTexture);
        //    _isOn = true;
        //}
    }
}
