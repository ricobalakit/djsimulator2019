using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxIndicator : MonoBehaviour
{

    private const float MAX_BRIGHTNESS = 3f;
    private const float FADE_TIME_SECONDS = 0.5f;

    [SerializeField]
    private ParticleSystem m_particleSystem;

    private DateTime m_lastHitTime = DateTime.MinValue;
    // Start is called before the first frame update
    private Material m_materialCopy;

    private void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        m_materialCopy = new Material(renderer.material);
        renderer.material = m_materialCopy;
    }

    // Update is called once per frame
    private void Update()
    {
        if ((float)(DateTime.Now - m_lastHitTime).TotalSeconds < 0.5f)
        {
            m_particleSystem.gameObject.SetActive(true);
        }
        else
        {
            m_particleSystem.gameObject.SetActive(false);
        }

        var fractionBrightness = Mathf.Clamp01(FADE_TIME_SECONDS - (float)(DateTime.Now - m_lastHitTime).TotalSeconds);

        m_materialCopy.SetColor("_EmissionColor", new Color(fractionBrightness, fractionBrightness, fractionBrightness, 1.0f) * fractionBrightness * MAX_BRIGHTNESS) ;
    }

    public void SetHitNow()
    {
        m_lastHitTime = DateTime.Now;
    }
}
