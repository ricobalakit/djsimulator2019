using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualsManager : MonoBehaviour
{
    public static VisualsManager Instance;

    [SerializeField]
    private GameObject m_clapFlashlight;

    [SerializeField]
    private GameObject m_jumpFlashlight;

    [SerializeField]
    private GameObject m_lyricGlow;

    [SerializeField]
    private GameObject m_jesusGlow;

    [SerializeField]
    private GameObject m_mixGlow;

    private void Start()
    {
        Instance = this;
    }

    public void Flash(ActionType lane)
    {
        if(lane == ActionType.Clap)
        {
            StartCoroutine(FlashClapLight());
        }
        else
        {
            StartCoroutine(FlashJumpLight());
        }
    }

    public void SetLight(ActionType lane, bool on)
    {
        GameObject laneLight = m_lyricGlow;

        if(lane == ActionType.JesusPose)
        {
            laneLight = m_jesusGlow;
        }
        else if(lane == ActionType.Mix)
        {
            laneLight = m_mixGlow;
        }

        if(on)
        {
            laneLight.SetActive(on);
        }
        else
        {
            laneLight.SetActive(on);
        }
    }

    private IEnumerator FlashClapLight()
    {
        m_clapFlashlight.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        m_clapFlashlight.SetActive(false);
    }

    private IEnumerator FlashJumpLight()
    {
        m_jumpFlashlight.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        m_jumpFlashlight.SetActive(false);
    }
}
