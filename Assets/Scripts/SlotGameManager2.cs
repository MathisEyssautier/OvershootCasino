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
    [SerializeField] Transform leverPosition;
    [SerializeField] Transform moneyTextPosition;
    [SerializeField] Transform ecoBarPosition;
    [SerializeField] Transform multiPosition;

    [Header("Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI InfoText;
    [SerializeField] private TMPro.TextMeshProUGUI CalculScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI EnergyText;
    [SerializeField] private Renderer JaugeRenderer;
    [SerializeField] private string shaderProperty = "_t";

    private int jaugeMult = 1;
    private int previousJaugeBonus = 1; // Pour détecter les changements de zone

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

        // AUCUN COMBO : Jouer un son neutre aléatoire
        if (sameCount == 1)
        {
            SoundManager.Instance.PlayNoComboSound();
            return;
        }

        int multiplier = sameCount == 2 ? 2 : 4;

        if (mainCategory == 1) // CATÉGORIE BAS
        {
            int gain = gainLow * multiplier * gainMult * jaugeMult;
            currentMoney += gain;

            FloatingTextManager.Instance.ShowMoneyGain(gain, moneyTextPosition);
            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinLowSound(multiplier);
            Debug.Log("Gain BAS : " + gain + "$");
        }
        else if (mainCategory == 2) // CATÉGORIE MOYEN
        {
            int gain = gainMedium * multiplier * gainMult * jaugeMult;
            currentMoney += gain;

            FloatingTextManager.Instance.ShowMoneyGain(gain, moneyTextPosition);
            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinMediumSound(multiplier);
            Debug.Log("Gain MOYEN : " + gain + "$");
        }
        else if (mainCategory == 3) // CATÉGORIE HAUT
        {
            int gain = gainHigh * multiplier * gainMult * jaugeMult;
            currentMoney += gain;

            FloatingTextManager.Instance.ShowMoneyGain(gain, moneyTextPosition);
            SoundManager.Instance.PlayMoneyGain();
            SoundManager.Instance.PlayWinHighSound(multiplier);
            Debug.Log("Gain HAUT : " + gain + "$");
        }
        else if (mainCategory == 4) // CATÉGORIE ÉCOLOGIE (PERTES)
        {
            int loss = ecoLoss * multiplier * gainMult * jaugeMult;
            currentEcology -= loss;
            currentEcology = Mathf.Max(0, currentEcology);

            FloatingTextManager.Instance.ShowEcoLoss(loss, ecoBarPosition);
            SoundManager.Instance.PlayEcoLoss();
            SoundManager.Instance.PlayLoseEcoSound(multiplier);
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
            FloatingTextManager.Instance.ShowMoneyLoss(ecologyCost, moneyTextPosition);
            SoundManager.Instance.PlayMoneyLoss();

            currentEcology += ecoGain;
            currentEcology = Mathf.Min(currentEcology, startingEcology);

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
        CheckJaugeBonus();

        SoundManager.Instance.UpdateMusicBasedOnEcology(currentEcology, startingEcology);

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
        JaugeRenderer.material.SetFloat(shaderProperty, t);

        int currentBonus = 1; // Valeur par défaut (zone normale)

        if (currentEcology >= 330)
        {
            JaugeRenderer.material.color = new Color(1f, 0.937f, 0f); // Jaune
            currentBonus = 2;

            // Si on vient juste d'entrer dans la zone haute
            if (previousJaugeBonus != 2)
            {
                SoundManager.Instance.PlayEnergyBoost();
            }
        }
        else if (currentEcology <= 80)
        {
            JaugeRenderer.material.color = new Color32(255, 31, 19, 255); // Rouge
            currentBonus = 0;

            // Si on vient juste d'entrer dans la zone critique
            if (previousJaugeBonus != 0)
            {
                SoundManager.Instance.PlayCriticalBoost();
            }
        }
        else
        {
            JaugeRenderer.material.color = new Color32(0, 229, 3, 255); // Vert
            currentBonus = 1;
        }

        previousJaugeBonus = currentBonus;
    }

    void RestartGame()
    {
        SoundManager.Instance.PlayBackgroundMusic();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Mouse Over Text
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