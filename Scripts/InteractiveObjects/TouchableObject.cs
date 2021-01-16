using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TouchableObject : MonoBehaviour
{
    private List<PlayerHandController> m_interactingHandControllers = new List<PlayerHandController>();

    public bool IsHandInteractingWithObject(PlayerHandController handController)
    {
        return m_interactingHandControllers.Contains(handController);
    }

    private void OnTriggerEnter(Collider collider)
    {
        var foundHandController = collider.GetComponent<PlayerHandController>();

        if (foundHandController != null)
        {
            m_interactingHandControllers.Add(foundHandController);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        var foundHandController = collider.GetComponent<PlayerHandController>();

        if (foundHandController != null)
        {
            m_interactingHandControllers.Remove(foundHandController);
        }
    }
}
