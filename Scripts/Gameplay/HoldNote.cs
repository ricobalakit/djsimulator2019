using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNote : MonoBehaviour
{
    private const float LENGTH_PER_BEAT = 8f;

    [SerializeField]
    private GameObject m_noteTail;

    private float m_startTimeBeats;
    private float m_endTimeBeats;
    private MasterMetronome m_metronome;
    private ActionType m_actionType;

    [SerializeField]
    private Material m_jesusMaterial;

    [SerializeField]
    private Material m_mixMaterial;

    [SerializeField]
    private Material m_lyricsMaterial;

    [SerializeField]
    private TextMesh m_lyrics;

    private bool m_isInitialized = false;

    public void Initialize(float startTime, float endTime, ActionType type, MasterMetronome metronome)
    {
        m_startTimeBeats = startTime;
        m_endTimeBeats = endTime;
        m_metronome = metronome;
        m_noteTail.transform.localScale = new Vector3(1f, 1f, (m_endTimeBeats - m_startTimeBeats) * LENGTH_PER_BEAT);
        transform.localPosition = new Vector3(0f, 0f, m_startTimeBeats * LENGTH_PER_BEAT);
        m_actionType = type;
        m_lyrics.text = "One! Two! Three! Jump!";
        var foundRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(false);
        if (foundRenderers != null)
        {
            foreach (var renderer in foundRenderers)
            {
                if (m_actionType == ActionType.JesusPose)
                {
                    renderer.material = m_jesusMaterial;
                }
                else if (m_actionType == ActionType.Lyric)
                {
                    renderer.material = m_lyricsMaterial;
                }
                else
                {
                    renderer.material = m_mixMaterial;
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

        if (m_metronome.ElapsedBeats() > m_endTimeBeats)
        {
            //kill it
            VisualsManager.Instance.SetLight(m_actionType, false);
            GameObject.Destroy(this.gameObject);
        }
        else if (m_metronome.ElapsedBeats() > m_startTimeBeats && m_metronome.ElapsedBeats() < m_endTimeBeats)
        {
            VisualsManager.Instance.SetLight(m_actionType, true);
            transform.localPosition = Vector3.zero;
            m_noteTail.transform.localScale = new Vector3(1f, 1f, (m_endTimeBeats - m_metronome.ElapsedBeats())* LENGTH_PER_BEAT);

            if(m_actionType == ActionType.Lyric)
            {
                if(m_endTimeBeats - m_metronome.ElapsedBeats() > 3f)
                {
                    m_lyrics.text = "One! Two! Three! Jump!";
                }
                else if (m_endTimeBeats - m_metronome.ElapsedBeats() > 2f)
                {
                    m_lyrics.text = "Two! Three! Jump!";
                }
                else if (m_endTimeBeats - m_metronome.ElapsedBeats() > 1f)
                {
                    m_lyrics.text = "Three! Jump!";
                }
                else
                {
                    m_lyrics.text = "Jump!";
                }
            }
        }
        else
        {
            m_lyrics.gameObject.SetActive((m_actionType == ActionType.Lyric) && (m_startTimeBeats - m_metronome.ElapsedBeats() < 16f));

            transform.localPosition = new Vector3(0f, 0f, (m_startTimeBeats - m_metronome.ElapsedBeats()) * LENGTH_PER_BEAT);
        }


    }
}
