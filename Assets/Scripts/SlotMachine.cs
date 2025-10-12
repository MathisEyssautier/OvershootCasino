using UnityEngine;
using System.Collections;

// Ce script contrôle la machine à sous complète avec ses 3 roues
public class SlotMachine : MonoBehaviour
{
    public SlotWheel wheel1;
    public SlotWheel wheel2;
    public SlotWheel wheel3;

    public float delayBetweenWheels = 0.3f;

    private bool canSpin = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canSpin)
        {
            StartCoroutine(LaunchAllWheels());
        }
    }

    private IEnumerator LaunchAllWheels()
    {
        canSpin = false;

        wheel1.Spin();
        yield return new WaitForSeconds(delayBetweenWheels);

        wheel2.Spin();
        yield return new WaitForSeconds(delayBetweenWheels);

        wheel3.Spin();

        while (wheel1.IsSpinning() || wheel2.IsSpinning() || wheel3.IsSpinning())
        {
            yield return null;
        }

        Debug.Log($"{wheel1.GetFinalSymbol()}{wheel2.GetFinalSymbol()}{wheel3.GetFinalSymbol()}");

        canSpin = true;
    }
}