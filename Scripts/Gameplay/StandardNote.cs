using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardNote : MonoBehaviour
{
    private const float LENGTH_PER_BEAT = 8f;

    private float m_startTimeBeats;
    private MasterMetronome m_metronome;
    private ActionType m_actionType;
    private bool m_isInitialized = false;

    [SerializeField]
    private Material m_clapMaterial;

    [SerializeField]
    private Material m_jumpMaterial;

    public void Initialize(float startTime, ActionType type, MasterMetronome metronome)
    {
        m_startTimeBeats = startTime;
        m_metronome = metronome;
        transform.localPosition = new Vector3(0f, 0f, m_startTimeBeats * LENGTH_PER_BEAT);
        m_actionType = type;

        var foundRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        if (foundRenderers != null)
        {
            foreach (var renderer in foundRenderers)
            {
                if (m_actionType == ActionType.Clap)
                {
                    renderer.material = m_clapMaterial;
                }
                else
                {
                    renderer.material = m_jumpMaterial;
                }
            }
        }

        m_isInitialized = true;
    }

    public void Update()
    {
        if (!m_isInitialized)
        {
            return;
        }

        if (m_metronome.ElapsedBeats() > m_startTimeBeats)
        {
            //kill it
            VisualsManager.Instance.Flash(m_actionType);
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            transform.localPosition = new Vector3(0f, 0f, (m_startTimeBeats - m_metronome.ElapsedBeats()) * LENGTH_PER_BEAT);
        }


    }
}
