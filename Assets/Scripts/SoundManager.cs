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

    [Header("=== RÉSULTATS - CATÉGORIE BAS ===")]
    [SerializeField] private AudioSource resultSource; // Source pour les résultats
    [SerializeField] private AudioClip winLowX2Sound; // 2 symboles bas
    [SerializeField] private AudioClip winLowX4Sound; // 3 symboles bas (jackpot)

    [Header("=== RÉSULTATS - CATÉGORIE MOYEN ===")]
    [SerializeField] private AudioClip winMediumX2Sound; // 2 symboles moyens
    [SerializeField] private AudioClip winMediumX4Sound; // 3 symboles moyens (jackpot)

    [Header("=== RÉSULTATS - CATÉGORIE HAUT ===")]
    [SerializeField] private AudioClip winHighX2Sound; // 2 symboles hauts
    [SerializeField] private AudioClip winHighX4Sound; // 3 symboles hauts (jackpot)

    [Header("=== RÉSULTATS - CATÉGORIE ÉCOLOGIE (PERTES) ===")]
    [SerializeField] private AudioClip loseEcoX2Sound; // 2 symboles écologie
    [SerializeField] private AudioClip loseEcoX4Sound; // 3 symboles écologie (jackpot négatif)

    [Header("=== RÉSULTATS - AUCUN COMBO ===")]
    [SerializeField] private AudioClip noCombo1Sound; // Son neutre 1
    [SerializeField] private AudioClip noCombo2Sound; // Son neutre 2
    [SerializeField] private AudioClip noCombo3Sound; // Son neutre 3

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
    [SerializeField] private AudioClip energyBoostSound; // Quand on entre en zone haute d'énergie
    [SerializeField] private AudioClip criticalBoostSound; // Quand on entre en zone critique d'énergie

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

    [Header("Text")]
    [SerializeField] private TMPro.TextMeshProUGUI comboText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (useLowPassFilter && musicSource != null)
        {
            lowPassFilter = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
            lowPassFilter.cutoffFrequency = normalCutoffFreq;
        }
        comboText.text = "";
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

            yield return new WaitForSeconds(145f);
        }

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

    public void UpdateMusicBasedOnEcology(int currentEco, int maxEco)
    {
        if (musicSource == null || !musicSource.isPlaying) return;

        currentEcoPercentage = (float)currentEco / maxEco * 100f;

        if (currentEcoPercentage <= criticalEcoThreshold)
        {
            float severity = 1f - (currentEcoPercentage / criticalEcoThreshold);

            musicSource.pitch = Mathf.Lerp(normalPitch, minPitch, severity);
            musicSource.volume = Mathf.Lerp(normalVolume, maxVolume, severity);

            if (useLowPassFilter && lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = Mathf.Lerp(normalCutoffFreq, minCutoffFreq, severity);
            }

            if (!isCritical)
            {
                isCritical = true;
                PlaySound(feedbackSource, criticalWarningSound);
            }
        }
        else
        {
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

    // Sons pour catégorie BAS
    public void PlayWinLowSound(int multiplier)
    {
        if (multiplier == 2)
        {
            PlaySound(resultSource, winLowX2Sound);
            comboText.text = "COMBO";
        }
        else if (multiplier == 4)
        {
            PlaySound(resultSource, winLowX4Sound);
            comboText.text = "MONEY COMBO";
        }
    }

    // Sons pour catégorie MOYEN
    public void PlayWinMediumSound(int multiplier)
    {
        if (multiplier == 2)
        {
            PlaySound(resultSource, winMediumX2Sound);
            comboText.text = "GOLDEN COMBO";
        }
        else if (multiplier == 4)
        {
            PlaySound(resultSource, winMediumX4Sound);
            comboText.text = "CASINO COMBO";
        }
    }

    // Sons pour catégorie HAUT
    public void PlayWinHighSound(int multiplier)
    {
        if (multiplier == 2)
        {
            PlaySound(resultSource, winHighX2Sound);
            comboText.text = "DADDY'S COMBO";
        }
        else if (multiplier == 4)
        {
            PlaySound(resultSource, winHighX4Sound);
            comboText.text = "INCREDIBLE COMBO";
        }
    }

    // Sons pour catégorie ÉCOLOGIE (pertes)
    public void PlayLoseEcoSound(int multiplier)
    {
        if (multiplier == 2)
        {
            PlaySound(resultSource, loseEcoX2Sound);
            comboText.text = "FIRE COMBO";
        }
        else if (multiplier == 4)
        {
            PlaySound(resultSource, loseEcoX4Sound);
            comboText.text = "DISASTER COMBO";
        }
    }

    // Son aléatoire quand aucun combo
    public void PlayNoComboSound()
    {
        int randomIndex = Random.Range(0, 3);
        AudioClip selectedClip = null;

        switch (randomIndex)
        {
            case 0:
                selectedClip = noCombo1Sound;
                comboText.text = "DO BETTER";
                break;
            case 1:
                selectedClip = noCombo2Sound;
                comboText.text = "BORING";
                break;
            case 2:
                selectedClip = noCombo3Sound;
                comboText.text = "BOUHOUHOU";
                break;
        }

        PlaySound(resultSource, selectedClip);
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

    public void PlayEnergyBoost()
    {
        PlaySound(feedbackSource, energyBoostSound);
    }

    public void PlayCriticalBoost()
    {
        PlaySound(feedbackSource, criticalBoostSound);
    }

    // ==================== UTILITAIRE ====================

    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (source != null && clip != null)
        {
            source.PlayOneShot(clip);
        }
    }

    public void StopAllSounds()
    {
        wheelSource?.Stop();
        resultSource?.Stop();
        uiSource?.Stop();
        feedbackSource?.Stop();
    }
}