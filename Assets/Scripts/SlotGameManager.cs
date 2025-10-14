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

    [Header("Valeurs de base")]
    public int startingMoney = 1000; //La thune de base
    public int startingEcology = 365; //La valeur de base de notre �co
    public int spinCost = 20; //le co�t en jours de notre lancement
    public float spinTime = 0.3f; //le temps entre nos roues qui tournent

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
        currentMoney = startingMoney; //On r�initialise tout � chaque session
        currentEcology = startingEcology;
        UpdateUI(); //L'UI aussi

        spinButton.onClick.AddListener(StartSpin); //On assimile nos fonctions aux boutons
        ecologyButton.onClick.AddListener(BuyEcology);
    }

    void StartSpin() //La fonction qui envoie les roues
    {
        if (isSpinning) return; //On v�rifie que c'est pas d�j� en train de tourner
        if (currentEcology < spinCost) //On v�rifie qu'il nous reste assez de jours
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
        yield return new WaitForSeconds(spinTime); //On attend avant de lancer la deuxi�me
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

        resultText.text = "R�sultat : " + r1 + " - " + r2 + " - " + r3; //On affiche le r�sultat (on pourra l'enlever c'est surtout pour d�bug)

        ApplyResult(r1, r2, r3); //On envoie notre r�sultat � notre fonction qui fait le vrai travail

        UpdateUI();
        isSpinning = false; //On indique qu'on a fini de tourner
    }

    void ApplyResult(int a, int b, int c) //La fonction qui interpr�te nos r�sultats de roues
    {
        int sameCount = 1; //Pour l'instant on part du principe qu'on a qu'un symbole apreil
        int symbol = a; //Et le symbole de r�f�rence c'est le premier

        if (b == a && c == a) //L� on a trois symboles identiques
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

        int multiplier = sameCount == 2 ? 2 : 4; //Si on a deux symboles pareils notre multiplier passe � x2, sinon � x4

        if (symbol >= 1 && symbol <= 6) //Si les symboles en commun c'est des symboles thunes
        {
            int gain = 0;
            if (symbol <= 3) gain = gainLow; //On adapte les gains
            else if (symbol <= 5) gain = gainMedium;
            else gain = gainHigh;

            currentMoney += gain * multiplier; //On incr�mente notre thune
            Debug.Log("Gain de " + (gain * multiplier) + "$");
        }
        else if (symbol == 7 || symbol == 8) //Dans le cas o� notre symbole en double ou en triple c'est les symboles d'�cologie
        {
            currentEcology -= ecoLoss * multiplier;
            Debug.Log("Perte de " + (ecoLoss * multiplier) + "d'�co !");
        }
    }

    void BuyEcology()
    {
        int costMoney = 50;
        int ecoGain = 30;

        if (currentMoney >= costMoney)
        {
            currentMoney -= costMoney;
            currentEcology += ecoGain;
        }
        UpdateUI();
    }

    void UpdateUI() //Ici on update toute notre UI
    {
        moneyBar.value = currentMoney;
        ecologyBar.value = currentEcology;
        moneyText.text = currentMoney.ToString();
        ecoText.text = currentEcology.ToString();
        spinCostText.text = "Co�t : " + spinCost + "jours";
    }
}


