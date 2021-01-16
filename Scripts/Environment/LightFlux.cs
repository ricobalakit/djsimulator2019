using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlux : MonoBehaviour
{

    [SerializeField]
    private Light m_light;

    [SerializeField]
    private MasterMetronome m_metronome;

    void Update()
    {
        m_light.intensity = 1f-m_metronome.FractionThroughStep(MasterMetronome.StepSize.Quarter)*0.75f;
    }
}
