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

    public int ecologyCost;
    public int industryCost;
    

    [SerializeField] TMPro.TextMeshProUGUI MultText;

    [Header("Valeurs de base")]
    public int startingMoney;
    public int startingEcology;
    public int spinCost;
    public float spinTime;
    public int gainMult;
    public int ecoGain;

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

    [Header("Animations")]
    [SerializeField] AnimationTest animationManager;


    [Header("Positions pour les textes flottants")]
    [SerializeField] Transform leverPosition; // Position du levier
    [SerializeField] Transform moneyTextPosition; // Position du texte d'argent
    [SerializeField] Transform ecoBarPosition; // Position de la barre d'écologie
    [SerializeField] Transform multiPosition; // Position de la barre d'écologie

    [Header("Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI InfoText;
    [SerializeField] private TMPro.TextMeshProUGUI CalculScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI EnergyText;
    [SerializeField] private Renderer JaugeRenderer;
    [SerializeField] private string shaderProperty = "_t";
    int jaugeMult = 1;


    void Start()
    {
        endScreen.SetActive(false);
        currentMoney = startingMoney;
        currentEcology = startingEcology;
        InfoText.text = "";
        UpdateUI();

        restartButton.onClick.AddListener(RestartGame);
    }

    public void StartSpin()
    {
        if (isSpinning) return;
        if (currentEcology < spinCost)
        {
            resultText.text = "Pas assez de jours !";
            SoundManager.Instance.PlayMoneyLoss();
            return;
        }

        SoundManager.Instance.PlaySpinButton();

        currentEcology -= spinCost;
        spinCount++;

        FloatingTextManager.Instance.ShowSpinCost(spinCost, leverPosition);

        UpdateUI();
        StartCoroutine(SpinAllWheels());
    }

    private IEnumerator SpinAllWheels()
    {
        isSpinning = true;
        animationManager.Lever();
        yield return new WaitForSeconds(0.5f);

        SoundManager.Instance.PlayWheelSpin();

        wheel1.Spin();
        yield return new WaitForSeconds(spinTime);

        wheel2.Spin();
        yield return new WaitForSeconds(spinTime);

        wheel3.Spin();

        while (wheel1.IsSpinning() || wheel2.IsSpinning() || wheel3.IsSpinning())
        {
            yield return null;
        }

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
        int GetCategory(int symbol)
        {
            if (symbol == 1 || symbol == 4 || symbol == 7) return 1;
            if (symbol == 2 || symbol == 5) return 2;
            if (symbol == 8) return 3;
            if (symbol == 3 || symbol == 6) return 4;
            return 0;
        }

        int catA = GetCategory(a);
        int catB = GetCategory(b);
        int catC = GetCategory(c);

        int sameCount = 1;
        int mainCategory = catA;

        if (catB == catA && catC == catA)
        {
            sameCount = 3;
        }
        else if (catB == catA || catB == catC || catC == catA)
        {
            sameCount = 2;
            if (catB == catA || catB == catC) mainCategory = catB;
            else mainCategory = catA;
        }


        if (sameCount == 1) return;

        if (sameCount == 1) {
            
            return;
                } // Pas de gain/perte


        int multiplier = sameCount == 2 ? 2 : 4;

        if (mainCategory == 1) // BAS
        {
            int gain = gainLow * multiplier * gainMult * jaugeMult;
            currentMoney += gain;

            // TEXTE FLOTTANT : +X$ en jaune
            FloatingTextManager.Instance.ShowMoneyGain(gain, moneyTextPosition);

            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinSound(multiplier);
            Debug.Log("Gain BAS : " + gain + "$");
        }
        else if (mainCategory == 2) // MOYEN
        {
            int gain = gainMedium * multiplier * gainMult * jaugeMult;
            currentMoney += gain;

            // TEXTE FLOTTANT : +X$ en jaune
            FloatingTextManager.Instance.ShowMoneyGain(gain, moneyTextPosition);

            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinSound(multiplier);
            Debug.Log("Gain MOYEN : " + gain + "$");
        }
        else if (mainCategory == 3) // HAUT
        {
            int gain = gainHigh * multiplier * gainMult * jaugeMult;
            currentMoney += gain;

            // TEXTE FLOTTANT : +X$ en jaune
            FloatingTextManager.Instance.ShowMoneyGain(gain, moneyTextPosition);

            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinSound(multiplier);
            Debug.Log("Gain HAUT : " + gain + "$");
        }
        else if (mainCategory == 4) // ÉCOLOGIE
        {
            int loss = ecoLoss * multiplier * gainMult * jaugeMult;
            currentEcology -= loss;
            currentEcology = Mathf.Max(0, currentEcology);

            // TEXTE FLOTTANT : -X en rouge à côté de la barre d'écologie
            FloatingTextManager.Instance.ShowEcoLoss(loss, ecoBarPosition);

            SoundManager.Instance.PlayEcoLoss();
            SoundManager.Instance.PlayLoseSound(multiplier);
            Debug.Log("Perte ÉCOLOGIE : " + loss + " éco !");
        }
    }

    public void BuyEcology()
    {
        if (currentEcology >= startingEcology)
        {
            return;
        }
        if (currentMoney >= ecologyCost)
        {
            SoundManager.Instance.PlayBuyButton();

            currentMoney -= ecologyCost;

            // TEXTE FLOTTANT : -X$ en jaune (perte d'argent)
            FloatingTextManager.Instance.ShowMoneyLoss(ecologyCost, moneyTextPosition);

            SoundManager.Instance.PlayMoneyLoss();

            currentEcology += ecoGain;
            currentEcology = Mathf.Min(currentEcology, 365);

            // TEXTE FLOTTANT : +X en vert (gain d'écologie)
            FloatingTextManager.Instance.ShowEcoGain(ecoGain, ecoBarPosition);

            SoundManager.Instance.PlayEcoGain();

            ecologyCost += ecologyCost / 2;
            animationManager.LeftButton();
        }
        HoveringLeftButton();
        UpdateUI();
    }

    public void BuyIndustry()
    {
        if (currentMoney >= industryCost)
        {
            SoundManager.Instance.PlayBuyButton();

            currentMoney -= industryCost;

            // TEXTE FLOTTANT : -X$ en jaune (perte d'argent)
            FloatingTextManager.Instance.ShowMoneyLoss(industryCost, moneyTextPosition);
            FloatingTextManager.Instance.ShowMultGain(multiPosition);

            SoundManager.Instance.PlayMoneyLoss();

            gainMult++;
            industryCost += industryCost / 2;
            animationManager.RightButton();
        }
        HoveringRightButton();
        UpdateUI();
    }

    void UpdateUI()
    {
        moneyBar.value = currentMoney;
        ecologyBar.value = currentEcology;
        moneyText.text = currentMoney.ToString() + " $";
        ecoText.text = currentEcology.ToString();
        spinCostText.text = "Coût : " + spinCost + " Eco ";
        spinCountText.text = "Tirages : " + spinCount;
        MultText.text = "x " + gainMult;
        UpdateJauge();
        SoundManager.Instance.UpdateMusicBasedOnEcology(currentEcology, startingEcology);
        CheckJaugeBonus();
        if (currentEcology <= 0)
        {
            endScreen.SetActive(true);
            finalSpinCountText.text = "Vous avez survécu " + spinCount + " tirages !";
            SoundManager.Instance.PlayGameOverMusic();
            SoundManager.Instance.StopAllSounds();
        }
    }

    void CheckJaugeBonus()
    {
        if (currentEcology >= 330)
        {
            jaugeMult = 2;
        }
        else if (currentEcology <= 80)
        {
            jaugeMult = 4;

        }
        else
        {
            jaugeMult = 1;
        }
    }

     void UpdateJauge()
    {

        float t = Mathf.Clamp01(currentEcology / 365f);
        // On envoie la valeur au shader
        JaugeRenderer.material.SetFloat(shaderProperty, t);
    }

    void RestartGame()
    {
        SoundManager.Instance.PlayBackgroundMusic();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }




    //Mouse Over Text
    

    public void HoveringLever()
    {
        InfoText.text = "Spin for " + spinCost + " energy";
    }

    public void HoveringRightButton()
    {
        InfoText.text = "+ 1 Mult for " + industryCost + " $";
    }

    public void HoveringLeftButton()
    {
        InfoText.text = "+ " + ecoGain + " energy for " + ecologyCost + " $";
    }

    public void StopHovering()
    {
        InfoText.text = "";
    }
}