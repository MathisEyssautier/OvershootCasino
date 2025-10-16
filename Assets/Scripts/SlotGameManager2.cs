using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlotGameManager2 : MonoBehaviour
{
    [Header("Les trois roues de base")]
    public SlotWheel wheel1;
    public SlotWheel wheel2;
    public SlotWheel wheel3;

    [Header("UI")]
    public Slider moneyBar;
    public Slider ecologyBar;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI spinCostText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ecoText;
    public TextMeshProUGUI spinCountText;

    public Button spinButton;
    public Button industryButton;
    public Button ecologyButton;
    public int ecologyCost;
    public int industryCost;
    public int ecoGain;

    [SerializeField] TMPro.TextMeshProUGUI industryButtonText;
    [SerializeField] TMPro.TextMeshProUGUI ecologyButtonText;
    [SerializeField] TMPro.TextMeshProUGUI MultText;

    [Header("Valeurs de base")]
    public int startingMoney;
    public int startingEcology;
    public int spinCost;
    public float spinTime;
    public int gainMult;

    [Header("Valeurs de gain / perte")]
    public int gainLow = 25;
    public int gainMedium = 50;
    public int gainHigh = 100;
    public int ecoLoss = 30;

    private int currentMoney;
    private int currentEcology;
    private bool isSpinning = false;
    private int spinCount = 0;

    [Header("End")]
    [SerializeField] GameObject endScreen;
    [SerializeField] Button restartButton;
    [SerializeField] TextMeshProUGUI finalSpinCountText;

    [Header("Aniamtions")]
    [SerializeField] AnimationTest animationManager;


    void Start()
    {
        endScreen.SetActive(false);
        currentMoney = startingMoney;
        currentEcology = startingEcology;
        UpdateUI();

        spinButton.onClick.AddListener(StartSpin);
        ecologyButton.onClick.AddListener(BuyEcology);
        industryButton.onClick.AddListener(BuyIndustry);
        restartButton.onClick.AddListener(RestartGame);
    }

    public void StartSpin()
    {
        if (isSpinning) return;
        if (currentEcology < spinCost)
        {
            resultText.text = "Pas assez de jours !";
            SoundManager.Instance.PlayMoneyLoss(); // Son d'échec
            return;
        }

        // SON : Clic de la tireuse
        SoundManager.Instance.PlaySpinButton();

        currentEcology -= spinCost;
        spinCount++;

        // SON : Perte d'écologie
        //SoundManager.Instance.PlayEcoLoss();

        UpdateUI();
        StartCoroutine(SpinAllWheels());
    }

    private IEnumerator SpinAllWheels()
    {
        isSpinning = true;
        animationManager.Lever();
        yield return new WaitForSeconds(0.5f);
        // SON : Roue 1 qui tourne
        SoundManager.Instance.PlayWheelSpin();
        
        wheel1.Spin();
        yield return new WaitForSeconds(spinTime);

        wheel2.Spin();

        yield return new WaitForSeconds(spinTime);

        wheel3.Spin();

        // Attendre que toutes les roues finissent
        while (wheel1.IsSpinning() || wheel2.IsSpinning() || wheel3.IsSpinning())
        {
            yield return null;
        }

        // SON : Toutes les roues s'arrêtent
        SoundManager.Instance.PlayWheelStop();

        int r1 = wheel1.GetFinalSymbol();
        int r2 = wheel2.GetFinalSymbol();
        int r3 = wheel3.GetFinalSymbol();

        resultText.text = "Résultat : " + r1 + " - " + r2 + " - " + r3;

        ApplyResult(r1, r2, r3);

        UpdateUI();
        isSpinning = false;
    }

    void ApplyResult(int a, int b, int c)
    {
        // Fonction pour convertir un symbole en catégorie
        int GetCategory(int symbol)
        {
            if (symbol >= 1 && symbol <= 3) return 1; // Catégorie Bas (1, 2, 3)
            if (symbol >= 4 && symbol <= 5) return 2; // Catégorie Moyen (4, 5)
            if (symbol == 6) return 3; // Catégorie Haut (6)
            if (symbol >= 7 && symbol <= 8) return 4; // Catégorie Écologie (7, 8)
            return 0; // Symbole invalide
        }

        // Conversion des symboles en catégories
        int catA = GetCategory(a);
        int catB = GetCategory(b);
        int catC = GetCategory(c);

        // Comptage des catégories identiques
        int sameCount = 1;
        int mainCategory = catA;

        if (catB == catA && catC == catA)
        {
            sameCount = 3; // Les 3 sont dans la même catégorie
        }
        else if (catB == catA || catB == catC || catC == catA)
        {
            sameCount = 2; // 2 symboles dans la même catégorie
            // On détermine quelle catégorie est en double
            if (catB == catA || catB == catC) mainCategory = catB;
            else mainCategory = catA;
        }

        if (sameCount == 1) return; // Pas de gain/perte

        int multiplier = sameCount == 2 ? 2 : 4;

        // Application des gains/pertes selon la catégorie
        if (mainCategory == 1) // Catégorie BAS (1, 2, 3)
        {
            currentMoney += gainLow * multiplier * gainMult;
            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinSound(multiplier);
            Debug.Log("Gain BAS : " + (gainLow * multiplier * gainMult) + "$");
        }
        else if (mainCategory == 2) // Catégorie MOYEN (4, 5)
        {
            currentMoney += gainMedium * multiplier * gainMult;
            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinSound(multiplier);
            Debug.Log("Gain MOYEN : " + (gainMedium * multiplier * gainMult) + "$");
        }
        else if (mainCategory == 3) // Catégorie HAUT (6)
        {
            currentMoney += gainHigh * multiplier * gainMult;
            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinSound(multiplier);
            Debug.Log("Gain HAUT : " + (gainHigh * multiplier * gainMult) + "$");
        }
        else if (mainCategory == 4) // Catégorie ÉCOLOGIE (7, 8)
        {
            currentEcology -= ecoLoss * multiplier * gainMult;
            currentEcology = Mathf.Max(0, currentEcology);
            SoundManager.Instance.PlayEcoLoss();
            SoundManager.Instance.PlayLoseSound(multiplier);
            Debug.Log("Perte ÉCOLOGIE : " + (ecoLoss * multiplier * gainMult) + " éco !");
        }
    }

    public void BuyEcology()
    {
        if (currentEcology >= startingEcology)
        {
            return; //On peut pas acheter d'éco si on est déjà full
        }
        if (currentMoney >= ecologyCost)
        {
            // SON : Achat
            SoundManager.Instance.PlayBuyButton();

            currentMoney -= ecologyCost;

            // SON : Perte d'argent
            SoundManager.Instance.PlayMoneyLoss();

            currentEcology += ecoGain;
            currentEcology = Mathf.Min(currentEcology, startingEcology);

            // SON : Gain d'écologie
            SoundManager.Instance.PlayEcoGain();

            ecologyCost += ecologyCost / 2;
            animationManager.LeftButton();
        }
        UpdateUI();
    }

    public void BuyIndustry()
    {
        if (currentMoney >= industryCost)
        {
            // SON : Achat
            SoundManager.Instance.PlayBuyButton();

            currentMoney -= industryCost;

            // SON : Perte d'argent
            SoundManager.Instance.PlayMoneyLoss();

            gainMult++;
            industryCost += industryCost / 2;
            animationManager.RightButton();
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        moneyBar.value = currentMoney;
        ecologyBar.value = currentEcology;
        moneyText.text = currentMoney.ToString();
        ecoText.text = currentEcology.ToString();
        spinCostText.text = "Coût : " + spinCost + " Eco ";
        spinCountText.text = "Tirages : " + spinCount;
        industryButtonText.text = industryCost + " $" + " for + " + "1 Mult";
        ecologyButtonText.text = ecologyCost + " $" + " for + " + ecoGain + " Eco";
        MultText.text = "Mult : " + gainMult;

        // IMPORTANT : Mettre à jour la musique en fonction de l'écologie
        SoundManager.Instance.UpdateMusicBasedOnEcology(currentEcology, startingEcology);

        if (currentEcology <= 0)
        {
            spinButton.interactable = false;
            endScreen.SetActive(true);
            finalSpinCountText.text = "Vous avez survécu " + spinCount + " tirages !";
            // SON : Game Over
            SoundManager.Instance.PlayGameOverMusic();
            SoundManager.Instance.StopAllSounds(); // Arrêter les autres sons
        }
        else
        {
            spinButton.interactable = true;
        }
    }

    void RestartGame()
    {
        SoundManager.Instance.PlayBackgroundMusic(); //Pour relancer la musique de fond quand on restart
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}