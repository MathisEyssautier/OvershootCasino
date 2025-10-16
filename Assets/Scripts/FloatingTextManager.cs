using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingTextManager : MonoBehaviour
{
    // Instance Singleton
    public static FloatingTextManager Instance { get; private set; }

    [Header("Prefab du texte flottant")]
    [SerializeField] private GameObject floatingTextPrefab;

    [Header("Canvas parent (pour l'UI)")]
    [SerializeField] private Canvas canvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fonction pour créer un texte flottant
    public void ShowFloatingText(string text, Vector3 uiPosition, Color color)
    {
        if (floatingTextPrefab == null || canvas == null) return;

        GameObject textObj = Instantiate(floatingTextPrefab, canvas.transform);
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();

        //On place directement dans le Canvas
        rectTransform.position = uiPosition;

        TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = text;
            textComponent.color = color;
        }

        StartCoroutine(AnimateFloatingText(textObj));
    }

    // Surcharge pour utiliser directement un Transform au lieu d'une position
    public void ShowFloatingText(string text, Transform target, Color color, Vector3 offset = default)
    {
        ShowFloatingText(text, target.position + offset, color);
    }

    private IEnumerator AnimateFloatingText(GameObject textObj)
    {
        TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();

        float duration = 0.5f; // Durée de l'animation
        float elapsed = 0f;
        Vector2 startPos = rectTransform.position;
        Vector2 endPos = startPos + Vector2.up * 50f; // Monte de 50 pixels

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Déplacement vers le haut
            rectTransform.position = Vector2.Lerp(startPos, endPos, t);

            // Fade out
            Color color = textComponent.color;
            color.a = 1f - t; // Alpha passe de 1 à 0
            textComponent.color = color;

            yield return null;
        }

        // Détruire l'objet à la fin
        Destroy(textObj);
    }

    // Fonctions pratiques pour les différents types de feedback
    public void ShowEcoLoss(int amount, Transform target)
    {
        ShowFloatingText("-" + amount, target, Color.red, Vector3.down * 0.5f);
    }

    public void ShowEcoGain(int amount, Transform target)
    {
        ShowFloatingText("+" + amount, target, Color.green, Vector3.up * 0.5f);
    }

    public void ShowMoneyLoss(int amount, Transform target)
    {
        ShowFloatingText("-" + amount + "$", target, new Color(0.7f, 0.8f, 0f), Vector3.down * 0.5f); // Jaune
    }

    public void ShowMoneyGain(int amount, Transform target)
    {
        ShowFloatingText("+" + amount + "$", target, new Color(1f, 0.8f, 0f), Vector3.up * 0.5f); // Jaune
    }

    public void ShowSpinCost(int amount, Transform target)
    {
        ShowFloatingText("-" + amount, target, Color.green, Vector3.down * 0.5f);
    }
    public void ShowMultGain(Transform target)
    {
        ShowFloatingText("+1", target, Color.red, Vector3.up * 0.5f);
    }
}