using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    // Instance Singleton pour accéder au SoundManager de partout
    public static SoundManager Instance { get; private set; }

    [Header("=== MUSIQUES ===")]
    [SerializeField] private AudioSource musicSource; // Source pour la musique de fond
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip victoryMusic;

    [Header("=== ROUES ===")]
    [SerializeField] private AudioSource wheelSource; // Source dédiée aux roues
    [SerializeField] private AudioClip wheelSpinSound;
    [SerializeField] private AudioClip wheelStopSound;

    [Header("=== RÉSULTATS ===")]
    [SerializeField] private AudioSource resultSource; // Source pour les résultats
    [SerializeField] private AudioClip winX1Sound; // Gain simple (2 symboles)
    [SerializeField] private AudioClip winX2Sound; // Jackpot (3 symboles)
    [SerializeField] private AudioClip loseX1Sound; // Perte simple (2 symboles écologie)
    [SerializeField] private AudioClip loseJackpotSound; // Perte jackpot (3 symboles écologie)

    [Header("=== UI / BOUTONS ===")]
    [SerializeField] private AudioSource uiSource; // Source pour l'UI
    [SerializeField] private AudioClip spinButtonSound; // Clic tireuse
    [SerializeField] private AudioClip buyButtonSound; // Achat (cha-ching)

    [Header("=== FEEDBACK RESSOURCES ===")]
    [SerializeField] private AudioSource feedbackSource; // Source pour feedback
    [SerializeField] private AudioClip moneyGainSound;
    [SerializeField] private AudioClip moneyLossSound;
    [SerializeField] private AudioClip ecoGainSound;
    [SerializeField] private AudioClip ecoLossSound;
    [SerializeField] private AudioClip criticalWarningSound; // Quand écologie devient critique

    [Header("=== PARAMÈTRES MUSIQUE ===")]
    [SerializeField] private float normalPitch = 1f; // Pitch normal
    [SerializeField] private float minPitch = 0.5f; // Pitch minimum (situation critique)
    [SerializeField] private float normalVolume = 0.5f; // Volume normal
    [SerializeField] private float maxVolume = 0.8f; // Volume maximum (situation critique)
    [SerializeField] private float criticalEcoThreshold = 30f; // Seuil d'écologie critique (%)
    [SerializeField] private bool useLowPassFilter = true; // Activer le filtre Low Pass
    [SerializeField] private float normalCutoffFreq = 22000f; // Fréquence normale (pas de filtre)
    [SerializeField] private float minCutoffFreq = 500f; // Fréquence minimum (son étouffé)

    private float currentEcoPercentage = 100f;
    private bool isCritical = false;
    private AudioLowPassFilter lowPassFilter;

    void Awake()
    {
        // Singleton : une seule instance de SoundManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Ajouter un filtre Low Pass si activé
        if (useLowPassFilter && musicSource != null)
        {
            lowPassFilter = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
            lowPassFilter.cutoffFrequency = normalCutoffFreq;
        }

        // Démarrer la musique de fond
        StartCoroutine(PlayIntroThenLoop());
    }

    // ==================== MUSIQUES ====================

    private IEnumerator PlayIntroThenLoop()
    {
        if (introMusic != null)
        {
            musicSource.clip = introMusic;
            musicSource.loop = false;
            musicSource.volume = normalVolume;
            musicSource.Play();

            // Attendre la durée de l’intro (145 secondes = 2 min 25)
            yield return new WaitForSeconds(145f);
        }

        // Ensuite, lancer la musique principale
        PlayBackgroundMusic();
    }
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = normalVolume;
            musicSource.pitch = normalPitch;
            // Réinitialiser le filtre Low Pass
            if (useLowPassFilter && lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = normalCutoffFreq;
            }
            musicSource.Play();
        }
    }

    public void PlayGameOverMusic()
    {
        if (gameOverMusic != null && musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = gameOverMusic;
            musicSource.loop = false;
            musicSource.volume = 0.7f;
            musicSource.pitch = 1f;
            musicSource.Play();
        }
    }

    public void PlayVictoryMusic()
    {
        if (victoryMusic != null && musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = victoryMusic;
            musicSource.loop = false;
            musicSource.volume = 0.7f;
            musicSource.pitch = 1f;
            musicSource.Play();
        }
    }

    // Fonction pour mettre à jour la musique en fonction de l'écologie
    public void UpdateMusicBasedOnEcology(int currentEco, int maxEco)
    {
        if (musicSource == null || !musicSource.isPlaying) return;

        // Calcul du pourcentage d'écologie
        currentEcoPercentage = (float)currentEco / maxEco * 100f;

        // Si on est en dessous du seuil critique
        if (currentEcoPercentage <= criticalEcoThreshold)
        {
            // Interpolation : plus l'écologie est basse, plus le pitch baisse et le volume monte
            float severity = 1f - (currentEcoPercentage / criticalEcoThreshold); // 0 à 1

            musicSource.pitch = Mathf.Lerp(normalPitch, minPitch, severity);
            musicSource.volume = Mathf.Lerp(normalVolume, maxVolume, severity);

            // Appliquer le filtre Low Pass pour étouffer le son
            if (useLowPassFilter && lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = Mathf.Lerp(normalCutoffFreq, minCutoffFreq, severity);
            }

            // Son d'alerte si on vient juste de devenir critique
            if (!isCritical)
            {
                isCritical = true;
                PlaySound(feedbackSource, criticalWarningSound);
            }
        }
        else
        {
            // Retour à la normale
            musicSource.pitch = normalPitch;
            musicSource.volume = normalVolume;

            if (useLowPassFilter && lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = normalCutoffFreq;
            }

            isCritical = false;
        }
    }

    // ==================== ROUES ====================

    public void PlayWheelSpin()
    {
        PlaySound(wheelSource, wheelSpinSound);
    }

    public void PlayWheelStop()
    {
        PlaySound(wheelSource, wheelStopSound);
    }

    // ==================== RÉSULTATS ====================

    public void PlayWinSound(int multiplier)
    {
        if (multiplier == 2) // 2 symboles identiques
        {
            PlaySound(resultSource, winX1Sound);
        }
        else if (multiplier == 4) // 3 symboles identiques (jackpot)
        {
            PlaySound(resultSource, winX2Sound);
        }
    }

    public void PlayLoseSound(int multiplier)
    {
        if (multiplier == 2) // 2 symboles écologie
        {
            PlaySound(resultSource, loseX1Sound);
        }
        else if (multiplier == 4) // 3 symboles écologie (jackpot négatif)
        {
            PlaySound(resultSource, loseJackpotSound);
        }
    }

    // ==================== UI ====================

    public void PlaySpinButton()
    {
        PlaySound(uiSource, spinButtonSound);
    }

    public void PlayBuyButton()
    {
        PlaySound(uiSource, buyButtonSound);
    }

    // ==================== FEEDBACK RESSOURCES ====================

    public void PlayMoneyGain()
    {
        PlaySound(feedbackSource, moneyGainSound);
    }

    public void PlayMoneyLoss()
    {
        PlaySound(feedbackSource, moneyLossSound);
    }

    public void PlayEcoGain()
    {
        PlaySound(feedbackSource, ecoGainSound);
    }

    public void PlayEcoLoss()
    {
        PlaySound(feedbackSource, ecoLossSound);
    }

    // ==================== UTILITAIRE ====================

    // Fonction générique pour jouer un son
    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (source != null && clip != null)
        {
            source.PlayOneShot(clip);
        }
    }

    // Fonction pour arrêter tous les sons (utile pour le game over)
    public void StopAllSounds()
    {
        wheelSource?.Stop();
        resultSource?.Stop();
        uiSource?.Stop();
        feedbackSource?.Stop();
    }
}