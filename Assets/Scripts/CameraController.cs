using UnityEngine;

using System.Collections;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera camStart;
    [SerializeField] private CinemachineCamera camGame;
    [SerializeField] private float transitionDelay = 0f;

    private void Start()
    {
        StartCoroutine(SwitchCameras());
    }

    private IEnumerator SwitchCameras()
    {
        

        // Change simplement la priorité — Cinemachine gère le blend automatiquement
        camStart.Priority = 0;
        camGame.Priority = 10;
        yield return new WaitForSeconds(transitionDelay);
        
    }
}
