using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    private bool m_isLeftHand = false;
    private bool m_isInitialized = false;
    private Vector3 m_currentPosition = Vector3.zero;
    private float m_distanceMovedLastFrame = 0f;

    public float DistanceMovedLastFrame
    {
        get { return m_distanceMovedLastFrame; }
    }

    public bool IsLeft
    {
        get { return m_isLeftHand; }
    }

    public void Initialize(bool isLeftHand)
    {
        m_isLeftHand = isLeftHand;
        m_isInitialized = true;
    }

    private void Update()
    {
        var distanceMoved = Vector3.Distance(transform.localPosition, m_currentPosition);
        m_distanceMovedLastFrame = distanceMoved;
        m_currentPosition = transform.localPosition;
    }
}
