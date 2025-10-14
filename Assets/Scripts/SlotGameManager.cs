using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotGameManager : MonoBehaviour
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

    public Button spinButton;
    public Button industryButton;
    public Button ecologyButton;
    public int ecologyCost;
    public int industryCost;
    public int ecoGain;

    //Button
    [SerializeField] TMPro.TextMeshProUGUI industryButtonText;
    [SerializeField] TMPro.TextMeshProUGUI ecologyButtonText;
    [SerializeField] TMPro.TextMeshProUGUI MultText;

    [Header("Valeurs de base")]
    public int startingMoney; //La thune de base
    public int startingEcology; //La valeur de base de notre éco
    public int spinCost; //le coût en jours de notre lancement
    public float spinTime; //le temps entre nos roues qui tournent
    public int gainMult;

    [Header("Valeurs de gain / perte")]
    public int gainLow = 25;     // symboles 1, 2, 3
    public int gainMedium = 50; // symboles 4, 5
    public int gainHigh = 100;   // symbole 6
    public int ecoLoss = 30;     // symboles 7, 8


    private int currentMoney;
    private int currentEcology;
    private bool isSpinning = false;

    void Start()
    {
        currentMoney = startingMoney; //On réinitialise tout à chaque session
        currentEcology = startingEcology;
        UpdateUI(); //L'UI aussi

        spinButton.onClick.AddListener(StartSpin); //On assimile nos fonctions aux boutons
        ecologyButton.onClick.AddListener(BuyEcology);
        industryButton.onClick.AddListener(BuyIndustry);
    }

    void StartSpin() //La fonction qui envoie les roues
    {
        if (isSpinning) return; //On vérifie que c'est pas déjà en train de tourner
        if (currentEcology < spinCost) //On vérifie qu'il nous reste assez de jours
        {
            resultText.text = "Pas assez de jours !";
            return;
        }

        currentEcology -= spinCost; 
        UpdateUI();
        StartCoroutine(SpinAllWheels()); //On lance les roues (la fonction est dans nos wheels)
    }

    private IEnumerator SpinAllWheels()
    {
        isSpinning = true;

        wheel1.Spin();
        yield return new WaitForSeconds(spinTime); //On attend avant de lancer la deuxième
        wheel2.Spin();
        yield return new WaitForSeconds(spinTime);
        wheel3.Spin();

        while (wheel1.IsSpinning() || wheel2.IsSpinning() || wheel3.IsSpinning()) //On attend que les roues finissent de tourner
        {
            yield return null;
        }

        int r1 = wheel1.GetFinalSymbol();
        int r2 = wheel2.GetFinalSymbol();
        int r3 = wheel3.GetFinalSymbol();

        resultText.text = "Résultat : " + r1 + " - " + r2 + " - " + r3; //On affiche le résultat (on pourra l'enlever c'est surtout pour débug)

        ApplyResult(r1, r2, r3); //On envoie notre résultat à notre fonction qui fait le vrai travail

        UpdateUI();
        isSpinning = false; //On indique qu'on a fini de tourner
    }

    void ApplyResult(int a, int b, int c) //La fonction qui interprète nos résultats de roues
    {
        int sameCount = 1; //Pour l'instant on part du principe qu'on a qu'un symbole apreil
        int symbol = a; //Et le symbole de référence c'est le premier

        if (b == a && c == a) //Là on a trois symboles identiques
        {
            sameCount = 3;
        }
        else if (b == a || b == c || c == a) //Si on a deux symboles identiques
        {
            sameCount = 2;
            if (b == a || b == c) symbol = b; //Soit c'est b qui est en double
            else symbol = a; //Soit c'est a
        }

        if (sameCount == 1) return; //Si on a pas de symboles en double ou triple osef

        int multiplier = sameCount == 2 ? 2 : 4; //Si on a deux symboles pareils notre multiplier passe à x2, sinon à x4

        if (symbol >= 1 && symbol <= 6) //Si les symboles en commun c'est des symboles thunes
        {
            int gain = 0;
            if (symbol <= 3) gain = gainLow; //On adapte les gains
            else if (symbol <= 5) gain = gainMedium;
            else gain = gainHigh;

            currentMoney += gain * multiplier * gainMult; //On incrémente notre thune
            Debug.Log("Gain de " + (gain * multiplier) + "$");
        }
        else if (symbol == 7 || symbol == 8) //Dans le cas où notre symbole en double ou en triple c'est les symboles d'écologie
        {
            currentEcology -= ecoLoss * multiplier * gainMult;
            Debug.Log("Perte de " + (ecoLoss * multiplier) + "d'éco !");
        }
    }

    public void BuyEcology()
    {
        
        if (currentMoney >= ecologyCost)
        {
            currentMoney -= ecologyCost;
            currentEcology += ecoGain;
            currentEcology = Mathf.Min(currentEcology, startingEcology);

            ecologyCost += ecologyCost/2;
        }
        UpdateUI();
    }

    public void BuyIndustry()
    {
        if(currentMoney >= industryCost)
        {
            currentMoney -= industryCost;
            gainMult++;
            industryCost += industryCost/2;
        }
        UpdateUI();
    }

    void UpdateUI() //Ici on update toute notre UI
    {
        moneyBar.value = currentMoney;
        ecologyBar.value = currentEcology;
        moneyText.text = currentMoney.ToString();
        ecoText.text = currentEcology.ToString();
        spinCostText.text = "Coût : " + spinCost + "jours";
        industryButtonText.text = industryCost + " $"+ " for + " + gainMult + " Mult";
        ecologyButtonText.text = ecologyCost + " $" + " for + " + ecoGain + " Eco";
        MultText.text = "Mult : " + gainMult;
    }
}


