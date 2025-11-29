using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("=== MUSIQUES ===")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip victoryMusic;

    [Header("=== ROUES ===")]
    [SerializeField] private AudioSource wheelSource;
    [SerializeField] private AudioClip wheelSpinSound;
    [SerializeField] private AudioClip wheelStopSound;

    [Header("=== RÉSULTATS - CATÉGORIE BAS ===")]
    [SerializeField] private AudioSource resultSource;
    [SerializeField] private AudioClip winLowX2Sound;
    [SerializeField] private AudioClip winLowX4Sound;

    [Header("=== RÉSULTATS - CATÉGORIE MOYEN ===")]
    [SerializeField] private AudioClip winMediumX2Sound;
    [SerializeField] private AudioClip winMediumX4Sound;

    [Header("=== RÉSULTATS - CATÉGORIE HAUT ===")]
    [SerializeField] private AudioClip winHighX2Sound;
    [SerializeField] private AudioClip winHighX4Sound;

    [Header("=== RÉSULTATS - CATÉGORIE ÉCOLOGIE (PERTES) ===")]
    [SerializeField] private AudioClip loseEcoX2Sound;
    [SerializeField] private AudioClip loseEcoX4Sound;

    [Header("=== RÉSULTATS - AUCUN COMBO ===")]
    [SerializeField] private AudioClip noCombo1Sound;
    [SerializeField] private AudioClip noCombo2Sound;
    [SerializeField] private AudioClip noCombo3Sound;

    [Header("=== UI / BOUTONS ===")]
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioClip spinButtonSound;
    [SerializeField] private AudioClip buyButtonSound;

    [Header("=== FEEDBACK RESSOURCES ===")]
    [SerializeField] private AudioSource feedbackSource;
    [SerializeField] private AudioClip moneyGainSound;
    [SerializeField] private AudioClip moneyLossSound;
    [SerializeField] private AudioClip ecoGainSound;
    [SerializeField] private AudioClip ecoLossSound;
    [SerializeField] private AudioClip energyBoostSound;
    [SerializeField] private AudioClip criticalBoostSound;

    [Header("=== PARAMÈTRES MUSIQUE ===")]
    [SerializeField] private float normalPitch = 1f;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float normalVolume = 0.5f;
    [SerializeField] private float maxVolume = 0.8f;
    [SerializeField] private float criticalEcoThreshold = 30f;
    [SerializeField] private bool useLowPassFilter = true;
    [SerializeField] private float normalCutoffFreq = 22000f;
    [SerializeField] private float minCutoffFreq = 500f;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeSoundManager();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SoundManager - Scène rechargée");
        StartCoroutine(InitAfterSceneLoad());
    }

    private IEnumerator InitAfterSceneLoad()
    {
        yield return null;
        FindComboText();
        InitializeSoundManager();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void FindComboText()
    {
        GameObject comboObj = GameObject.Find("ComboText");
        if (comboObj != null)
        {
            comboText = comboObj.GetComponent<TMPro.TextMeshProUGUI>();
            Debug.Log($"ComboText trouvé: {comboText != null}");
        }
        else
        {
            Debug.LogWarning("GameObject ComboText introuvable!");
        }
    }

    public void SetComboText(TMPro.TextMeshProUGUI text)
    {
        comboText = text;
        if (comboText != null)
        {
            comboText.text = "";
            Debug.Log("ComboText assigné manuellement");
        }
    }

    private void InitializeSoundManager()
    {
        if (useLowPassFilter && musicSource != null)
        {
            lowPassFilter = musicSource.gameObject.GetComponent<AudioLowPassFilter>();
            if (lowPassFilter == null)
            {
                lowPassFilter = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
            }
            lowPassFilter.cutoffFrequency = normalCutoffFreq;
        }

        if (comboText != null)
        {
            comboText.text = "";
            Debug.Log("ComboText réinitialisé");
        }

        isCritical = false;
        currentEcoPercentage = 100f;

        if (musicSource != null)
        {
            musicSource.pitch = normalPitch;
            musicSource.volume = normalVolume;
        }

        StopAllCoroutines();
        StartCoroutine(PlayIntroThenLoop());
    }

    public void ResetSoundManager()
    {
        StopAllSounds();
        musicSource?.Stop();
        StopAllCoroutines();

        isCritical = false;
        currentEcoPercentage = 100f;

        if (musicSource != null)
        {
            musicSource.pitch = normalPitch;
            musicSource.volume = normalVolume;
        }

        if (useLowPassFilter && lowPassFilter != null)
        {
            lowPassFilter.cutoffFrequency = normalCutoffFreq;
        }

        InitializeSoundManager();
    }

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

            if (useLowPassFilter && lowPassFilter != null)
            {
                lowPassFilter.cutoffFrequency = normalCutoffFreq;
            }

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

    public void PlayWheelSpin()
    {
        PlaySound(wheelSource, wheelSpinSound);
    }

    public void PlayWheelStop()
    {
        PlaySound(wheelSource, wheelStopSound);
    }

    public void PlayWinLowSound(int multiplier)
    {
        if (multiplier == 2)
        {
            PlaySound(resultSource, winLowX2Sound);
            if (comboText != null)
            {
                comboText.text = "COMBO";
                Debug.Log("Texte mis à jour: COMBO");
            }
            else
            {
                Debug.LogWarning("comboText NULL!");
                FindComboText();
                if (comboText != null) comboText.text = "COMBO";
            }
        }
        else if (multiplier == 4)
        {
            PlaySound(resultSource, winLowX4Sound);
            if (comboText != null)
            {
                comboText.text = "MONEY COMBO";
            }
            else
            {
                FindComboText();
                if (comboText != null) comboText.text = "MONEY COMBO";
            }
        }
    }

    public void PlayWinMediumSound(int multiplier)
    {
        if (multiplier == 2)
        {
            PlaySound(resultSource, winMediumX2Sound);
            if (comboText != null)
            {
                comboText.text = "GOLDEN COMBO";
            }
            else
            {
                FindComboText();
                if (comboText != null) comboText.text = "GOLDEN COMBO";
            }
        }
        else if (multiplier == 4)
        {
            PlaySound(resultSource, winMediumX4Sound);
            if (comboText != null)
            {
                comboText.text = "CASINO COMBO";
            }
            else
            {
                FindComboText();
                if (comboText != null) comboText.text = "CASINO COMBO";
            }
        }
    }

    public void PlayWinHighSound(int multiplier)
    {
        if (multiplier == 2)
        {
            PlaySound(resultSource, winHighX2Sound);
            if (comboText != null)
            {
                comboText.text = "DADDY'S COMBO";
            }
            else
            {
                FindComboText();
                if (comboText != null) comboText.text = "DADDY'S COMBO";
            }
        }
        else if (multiplier == 4)
        {
            PlaySound(resultSource, winHighX4Sound);
            if (comboText != null)
            {
                comboText.text = "INCREDIBLE COMBO";
            }
            else
            {
                FindComboText();
                if (comboText != null) comboText.text = "INCREDIBLE COMBO";
            }
        }
    }

    public void PlayLoseEcoSound(int multiplier)
    {
        if (multiplier == 2)
        {
            PlaySound(resultSource, loseEcoX2Sound);
            if (comboText != null)
            {
                comboText.text = "FIRE COMBO";
            }
            else
            {
                FindComboText();
                if (comboText != null) comboText.text = "FIRE COMBO";
            }
        }
        else if (multiplier == 4)
        {
            PlaySound(resultSource, loseEcoX4Sound);
            if (comboText != null)
            {
                comboText.text = "DISASTER COMBO";
            }
            else
            {
                FindComboText();
                if (comboText != null) comboText.text = "DISASTER COMBO";
            }
        }
    }

    public void PlayNoComboSound()
    {
        int randomIndex = Random.Range(0, 3);
        AudioClip selectedClip = null;

        switch (randomIndex)
        {
            case 0:
                selectedClip = noCombo1Sound;
                if (comboText != null)
                {
                    comboText.text = "DO BETTER";
                }
                else
                {
                    FindComboText();
                    if (comboText != null) comboText.text = "DO BETTER";
                }
                break;
            case 1:
                selectedClip = noCombo2Sound;
                if (comboText != null)
                {
                    comboText.text = "BORING";
                }
                else
                {
                    FindComboText();
                    if (comboText != null) comboText.text = "BORING";
                }
                break;
            case 2:
                selectedClip = noCombo3Sound;
                if (comboText != null)
                {
                    comboText.text = "BOUHOUHOU";
                }
                else
                {
                    FindComboText();
                    if (comboText != null) comboText.text = "BOUHOUHOU";
                }
                break;
        }

        PlaySound(resultSource, selectedClip);
    }

    public void PlaySpinButton()
    {
        PlaySound(uiSource, spinButtonSound);
    }

    public void PlayBuyButton()
    {
        PlaySound(uiSource, buyButtonSound);
    }

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