using UnityEngine;

using System.Collections;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera camStart;
    [SerializeField] private CinemachineCamera camGame;
    [SerializeField] private float transitionDelay = 0f;
    [SerializeField] private GameObject Sign;
    [SerializeField] private float moveDuration = 2f; // durée du déplacement
    [SerializeField] private float targetY = 5f;

    private void Start()
    {
        camStart.Priority = 10;
        camGame.Priority = 0;
        StartCoroutine(SwitchCameras());
    }

    private IEnumerator SwitchCameras()
    {
        

        // Change simplement la priorité — Cinemachine gère le blend automatiquement
        camStart.Priority = 0;
        camGame.Priority = 10;
        yield return new WaitForSeconds(transitionDelay);
        Sign.SetActive(true);
        StartCoroutine(MoveSignUp());
    }

    private IEnumerator MoveSignUp()
    {
        Vector3 startPos = Sign.transform.position;
        Vector3 endPos = new Vector3(startPos.x, targetY, startPos.z);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            // Interpolation fluide (EaseInOut)
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            Sign.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Assure la position finale exacte
        Sign.transform.position = endPos;
    }
}
