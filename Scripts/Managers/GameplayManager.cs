using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{

    private const float BEAT_NOTE_TOLERANCE = 0.33f;
    private const float MIX_MULTIPLIER = 1.5f;
    private const float JESUS_MULTIPLIER = 10f;
    private const float MC_MULTIPLIER = 10f;

    [SerializeField]
    private AudioSource m_gameMusic;

    [SerializeField]
    private MasterMetronome m_metronome;

    [SerializeField]
    private TextMesh m_debugBeats;

    [SerializeField]
    private Text[] m_debugScore;

    [SerializeField]
    private Text m_secondaryDisplay;

    [SerializeField]
    private List<NoteInputData> m_noteInput = new List<NoteInputData>();

    [SerializeField]
    private NoteDisplayController m_noteDisplayController; //lol

    [SerializeField]
    private GameObject m_tutorialRoot1;

    [SerializeField]
    private GameObject m_tutorialRoot2;

    [SerializeField]
    private Text m_finalScoreDisplay;

    [SerializeField]
    private Text m_finalScoreDisplaySubtitle;

    [SerializeField]
    private Camera m_smoothCamera;

    [SerializeField]
    private AudioSource m_booingAudio;

    [SerializeField]
    private GameObject m_peakStateVisuals;

    private bool m_tutorial1passed = false;
    private bool m_gameStarted = false;
    private SongNoteData m_songData = new SongNoteData();

    public void Start()
    {
        m_tutorialRoot1.SetActive(true);
        m_tutorialRoot2.SetActive(false);

        m_finalScoreDisplay.gameObject.SetActive(false);
        m_finalScoreDisplaySubtitle.gameObject.SetActive(false);

        StartCoroutine(CameraInitialization());
    }

    public void NextSlide()
    {
        m_tutorialRoot1.SetActive(false);
        m_tutorialRoot2.SetActive(true);
        m_tutorial1passed = true;
    }

    public void StartGame()
    {
        if (!m_gameStarted && m_tutorial1passed)
        {
            m_tutorialRoot1.SetActive(false);
            m_tutorialRoot2.SetActive(false);
            //Debug.LogError("Game has started!");
            m_gameStarted = true;
            m_gameMusic.Play();

            m_metronome.SetMetronomeReferencePoint();

            m_songData.InitializeNoteDataFromInput(m_noteInput);

            m_noteDisplayController.Initialize(m_songData);
        }
    }

    private void Update()
    {
        if (m_gameStarted)
        {
            m_tutorialRoot2.SetActive(false);
            m_debugBeats.text = m_metronome.ElapsedBeats().ToString();
            UpdateScore();

            if(m_gameMusic.isPlaying)
            {
            }
            else
            {
                IsDone();
            }
        }
        else
        {
            m_debugBeats.text = string.Empty;
        }
    }

    public void ReceiveClap()
    {
        var foundClapNow = GetNoteByTypeNow(ActionType.Clap);
        m_noteDisplayController.SetIndicatorHit(ActionType.Clap);
        if (foundClapNow != null)
        {
            foundClapNow.CurrentScore = 2f;
            //set score if not yet set
            //Debug.LogError($"found a clap! score is set to {foundClapNow.CurrentScore}");
        }
    }

    public void ReceiveJump()
    {
        var foundJumpNow = GetNoteByTypeNow(ActionType.Jump);
        m_noteDisplayController.SetIndicatorHit(ActionType.Jump);
        if (foundJumpNow != null)
        {
            
            foundJumpNow.CurrentScore = 1f;
            //Debug.LogError($"found a jump! score set to {foundJumpNow.CurrentScore}");
            //set score if not yet set
        }
    }

    public void ReceiveMix(float moveDistance)
    {
        var foundMixNote = GetNoteByTypeNow(ActionType.Mix);
        m_noteDisplayController.SetIndicatorHit(ActionType.Mix);
        if (foundMixNote != null)
        {  
            foundMixNote.CurrentScore = foundMixNote.CurrentScore + moveDistance * MIX_MULTIPLIER; // rpb: find the multiplier here.
            //Debug.LogError($"found mix note, current score is {foundMixNote.CurrentScore}");
        }
    }

    public void ReceiveJesus()
    {
        var foundJesusScore = GetNoteByTypeNow(ActionType.JesusPose);
        m_noteDisplayController.SetIndicatorHit(ActionType.JesusPose);
        if (foundJesusScore != null)
        {
            foundJesusScore.CurrentScore = foundJesusScore.CurrentScore + Time.deltaTime / m_metronome.TempoToSecondsPerBeat(); // RPB: needs a multiplier
            //Debug.LogError($"found mix note, current score is {foundJesusScore.CurrentScore}");
        }
    }

    public void ReceiveLyric(float volume)
    {
        var foundLyricScore = GetNoteByTypeNow(ActionType.Lyric);
        m_noteDisplayController.SetIndicatorHit(ActionType.Lyric);

        if (foundLyricScore != null)
        {
            foundLyricScore.CurrentScore = foundLyricScore.CurrentScore + volume; // RPB: needs a multiplier
        }
    }

    public NoteData GetNoteByTypeNow(ActionType type)
    {
        for (int i = 0; i < m_songData.NoteData.Count; i++)
        {
            var note = m_songData.NoteData[i];

            if (note.Action != type)
            {
                continue;
            }

            var currentBeat = m_metronome.ElapsedBeats();

            if (note.NoteShape == NoteType.Beat)
            {
                //get something around a tolerance
                if (currentBeat < note.StartBeat + BEAT_NOTE_TOLERANCE && currentBeat > note.StartBeat - BEAT_NOTE_TOLERANCE)
                {
                    return note;
                }
            }

            else
            {
                //get exactly between

                if (currentBeat < note.EndBeat && currentBeat > note.StartBeat)
                {
                    return note;
                }
            }
        }

        return null;
    }

    private void UpdateScore()
    {
        var currentScore = m_songData.GetCurrentScore();

        for (int i = 0; i < m_debugScore.Length; i++)
        {
            m_debugScore[i].text = $"SCORE: {currentScore / m_songData.MaxScore * 1000000:0}";
        }

        var maxScoreUntil = m_songData.GetMaxScoreForBeatTime(m_metronome.ElapsedBeats());

        var scoreFraction = currentScore / maxScoreUntil;

        //Debug.LogError($"maximum score to this point: {maxScoreUntil}, percent to current: {scoreFraction * 100f}%");

        if (maxScoreUntil > 50)
        {
            if (scoreFraction > 0.8f)
            {
                //show peakstate
                m_peakStateVisuals.SetActive(true);
                m_booingAudio.volume = Mathf.Clamp(m_booingAudio.volume - 0.01f, 0f, 0.7f);
                m_secondaryDisplay.text = "Crowd is ecstatic!";
            }
            else if (scoreFraction > 0.5f)
            {
                //rpb:threshold for booing to happen!
                m_peakStateVisuals.SetActive(false);
                m_booingAudio.volume = Mathf.Clamp(m_booingAudio.volume - 0.01f, 0f, 0.7f);
                m_secondaryDisplay.text = "Crowd is satisfied!";

            }
            else
            {
                m_peakStateVisuals.SetActive(false);
                m_booingAudio.volume = Mathf.Clamp(m_booingAudio.volume + 0.01f, 0f, 0.7f);
                m_secondaryDisplay.text = "Crowd is unhappy!";
            }
        }
    }

    private void IsDone()
    {
        StartCoroutine(IsDoneCoroutine());
    }

    private IEnumerator IsDoneCoroutine()
    {
        yield return new WaitForSeconds(1f);

        

        var finalScore = m_songData.GetCurrentScore();
        var finalScoreFraction = finalScore / m_songData.MaxScore;

        if (finalScoreFraction > 0.95f)
        {
            m_finalScoreDisplay.text = $"{finalScore / m_songData.MaxScore * 1000000:0} - S";
            m_finalScoreDisplaySubtitle.text = "Best (fake) DJ Ever!";
        }
        else if (finalScoreFraction > 0.9f)
        {
            m_finalScoreDisplay.text = $"{finalScore / m_songData.MaxScore * 1000000:0} - A";
            m_finalScoreDisplaySubtitle.text = "You're a headliner!";
        }
        else if (finalScoreFraction > 0.8f)
        {
            m_finalScoreDisplay.text = $"{finalScore / m_songData.MaxScore * 1000000:0} - B";
            m_finalScoreDisplaySubtitle.text = "What a pro!";
        }
        else if (finalScoreFraction > 0.65f)
        {
            m_finalScoreDisplay.text = $"{finalScore / m_songData.MaxScore * 1000000:0} - C";
            m_finalScoreDisplaySubtitle.text = "Ready for the club!";
        }
        else if (finalScoreFraction > 0.5f)
        {
            m_finalScoreDisplay.text = $"{finalScore / m_songData.MaxScore * 1000000:0} - D";
            m_finalScoreDisplaySubtitle.text = "Keep practicing!";
        }
        else
        {
            m_finalScoreDisplay.text = $"{finalScore} - E";
            m_finalScoreDisplaySubtitle.text = "The crowd left your show."; //wow
        }

        m_finalScoreDisplay.gameObject.SetActive(true);
        m_finalScoreDisplaySubtitle.gameObject.SetActive(true);

        yield return new WaitForSeconds(30f);

        Application.LoadLevel(0);
    }

    private IEnumerator CameraInitialization()
    {
        yield return new WaitForSeconds(0.1f);
        m_smoothCamera.enabled = true;
    }
}
