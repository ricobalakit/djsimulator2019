using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDisplayController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_laneMix;

    [SerializeField]
    private GameObject m_laneClap;

    [SerializeField]
    private GameObject m_laneJesus;

    [SerializeField]
    private GameObject m_laneJump;

    [SerializeField]
    private GameObject m_laneLyric;

    [SerializeField]
    private GameObject m_holdNotePrefab;

    [SerializeField]
    private GameObject m_standardNotePrefab;

    [SerializeField]
    private MasterMetronome m_metronome;

    [SerializeField]
    private HitboxIndicator m_hitboxMix;

    [SerializeField]
    private HitboxIndicator m_hitboxClap;

    [SerializeField]
    private HitboxIndicator m_hitboxJesus;

    [SerializeField]
    private HitboxIndicator m_hitboxJump;

    [SerializeField]
    private HitboxIndicator m_hitboxLyric;

    public void Initialize(SongNoteData songData)
    {
        for(int i = 0; i < songData.NoteData.Count; i++)
        {
            if(songData.NoteData[i].NoteShape == NoteType.Hold)
            {
                GameObject lane = null;

                switch(songData.NoteData[i].Action)
                {
                    case ActionType.JesusPose:
                        lane = m_laneJesus;
                        break;
                    case ActionType.Lyric:
                        lane = m_laneLyric;
                        break;
                    case ActionType.Mix:
                        lane = m_laneMix;
                        break;
                }

                var newNote = GameObject.Instantiate(m_holdNotePrefab, lane.transform);
                newNote.GetComponent<HoldNote>().Initialize(songData.NoteData[i].StartBeat, songData.NoteData[i].EndBeat, songData.NoteData[i].Action, m_metronome);
            }
            else
            {
                GameObject lane = null;

                switch (songData.NoteData[i].Action)
                {
                    case ActionType.Jump:
                        lane = m_laneJump;
                        break;
                    case ActionType.Clap:
                        lane = m_laneClap;
                        break;
                }

                var newNote = GameObject.Instantiate(m_standardNotePrefab, lane.transform);
                newNote.GetComponent<StandardNote>().Initialize(songData.NoteData[i].StartBeat, songData.NoteData[i].Action, m_metronome);
            }
            
        }
    }

    public void SetIndicatorHit(ActionType action)
    {
        switch(action)
        {
            case ActionType.Clap:
                m_hitboxClap.SetHitNow();
                break;
            case ActionType.JesusPose:
                m_hitboxJesus.SetHitNow();
                break;
            case ActionType.Jump:
                m_hitboxJump.SetHitNow();
                break;
            case ActionType.Mix:
                m_hitboxMix.SetHitNow();
                break;
            case ActionType.Lyric:
                m_hitboxLyric.SetHitNow();
                break;
        }
    }
}
