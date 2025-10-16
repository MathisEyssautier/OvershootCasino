using UnityEngine;

public class ObjectClickManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Objet cliqué : " + hit.collider.gameObject.name);

                // Si tu veux appeler une fonction sur l’objet
                hit.collider.gameObject.SendMessage("OnObjectClicked", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
