using UnityEngine;
using System.Collections;

public class SlotWheel : MonoBehaviour
{
    public Transform cylinderTransform;

    public float spinDuration = 2f;

    private bool isSpinning = false;
    private int finalSymbol; // Le symbole final (entre 1 et 8)
    private int symbolCount = 8; // Nombre total de symboles
    private float finalRotation; // La rotation finale en degr�s

    public bool IsSpinning()
    {
        return isSpinning;
    }

    public int GetFinalSymbol()
    {
        return finalSymbol;
    }

    public void Spin()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinAnimation());
        }
    }

    private IEnumerator SpinAnimation()
    {
        isSpinning = true;

        // On r�cup�re la rotation de départ
        float startRotation = cylinderTransform.localEulerAngles.z;

        // Calcul des degr�s par symbole (360degrés divisé par 8 symboles = 45degrés)
        float degreesPerSymbol = 360f / symbolCount;

        int randomSymbol = Random.Range(1, symbolCount + 1);

        // Calcul de la rotation finale
        float targetRotation = startRotation + 1080f + (randomSymbol - 1) * degreesPerSymbol;

        float timeElapsed = 0f;

        // Boucle d'animation qui tourne pendant toute la durée définie
        while (timeElapsed < spinDuration)
        {
            // On avance le temps
            timeElapsed += Time.deltaTime;

            // Calcul du pourcentage de progression (0 à 1) pour la ligne suivante
            float progress = timeElapsed / spinDuration;

            // Cette formule fait ralentir la roue à la fin
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

            // Interpolation entre la rotation de depart et la rotation finale --> de combien on doit tourner la roue
            float currentRotation = Mathf.Lerp(startRotation, targetRotation, easedProgress);

            // Application de la rotation au cylindre ( là on fait vraiment rtourner la roue )
            cylinderTransform.localEulerAngles = new Vector3(0f, 0f, currentRotation);

            // On attend la prochaine frame
            yield return null;
        }

        // � la fin, on s'assure que la rotation finale est exacte
        cylinderTransform.localEulerAngles = new Vector3(0f, 0f, targetRotation);

        // On stocke la rotation finale
        finalRotation = targetRotation;

        // On calcule le symbole basé sur la rotation finale
        // On prend le modulo pour obtenir la position entre 0 et 360
        float normalizedRotation = finalRotation % 360f;
        // On divise par les degrés par symbole et on arrondit
        finalSymbol = Mathf.FloorToInt(normalizedRotation / degreesPerSymbol) + 1;

        isSpinning = false;
    }
}