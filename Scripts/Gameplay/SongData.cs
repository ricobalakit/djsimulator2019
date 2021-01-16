using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SongNoteData
{
    public List<NoteData> NoteData = new List<NoteData>();
    public float MaxScore = 0f;

    public void InitializeNoteDataFromInput(List<NoteInputData> noteInputList)
    {
        for (int i = 0; i < noteInputList.Count; i++)
        {
            if (noteInputList[i].Type == ActionType.Clap || noteInputList[i].Type == ActionType.ClapDouble)
            {
                //convert to beat notes
                var length = noteInputList[i].EndBeat - noteInputList[i].StartBeat;

                for (int j = 0; j <= length; j++)
                {
                    if (noteInputList[i].Type == ActionType.Clap)
                    {
                        if (j % 2 == 0)
                        {
                            continue;
                        }
                    }

                    var noteScoreMax = 2f;

                    NoteData noteData = new NoteData(noteInputList[i].StartBeat + j, noteInputList[i].StartBeat + j, ActionType.Clap, NoteType.Beat, noteScoreMax);
                    NoteData.Add(noteData);

                    MaxScore += noteScoreMax;
                }
            }
            else if(noteInputList[i].Type == ActionType.Jump)
            {

                //convert to beat notes
                var length = noteInputList[i].EndBeat - noteInputList[i].StartBeat;

                for (int j = 0; j < length; j++)
                {
                    var noteScoreMax = 1f;

                    NoteData noteData = new NoteData(noteInputList[i].StartBeat + j, noteInputList[i].StartBeat + j, noteInputList[i].Type, NoteType.Beat, noteScoreMax);
                    NoteData.Add(noteData);

                    MaxScore += noteScoreMax;
                }

            }
            else
            {
                //convert to hold notes
                var noteScoreMax = (noteInputList[i].EndBeat - noteInputList[i].StartBeat);

                NoteData noteData = new NoteData(noteInputList[i].StartBeat, noteInputList[i].EndBeat, noteInputList[i].Type, NoteType.Hold, noteScoreMax);
                NoteData.Add(noteData);

                MaxScore += noteScoreMax;
            }
        }
    }

    public float GetCurrentScore()
    {
        float currentScore = 0f;

        for (int i = 0; i < NoteData.Count; i++)
        {
            currentScore += NoteData[i].CurrentScore;
        }

        return currentScore;
    }

    public float GetMaxScoreForBeatTime(float currentBeat)
    {
        float totalUntil = 0f;

        for (int i = 0; i < NoteData.Count; i++)
        {
            if(currentBeat > NoteData[i].EndBeat)
            {
                totalUntil += NoteData[i].MaxScore;
            }
        }

        return totalUntil;
    }
}

[Serializable]
public class NoteInputData
{
    public float StartBeat;
    public float EndBeat;
    public ActionType Type;
}

public class NoteData
{
    public float StartBeat;
    public float EndBeat;
    public ActionType Action;
    public NoteType NoteShape;
    public float m_currentScore = 0f;
    public float MaxScore = 0f;

    public float CurrentScore
    {
        get { return m_currentScore; }
        set { m_currentScore = Mathf.Clamp(value, 0f, MaxScore); }
    }

    public NoteData(float startBeat, float endBeat, ActionType action, NoteType noteShape, float maxScore)
    {
        StartBeat = startBeat;
        EndBeat = endBeat;
        Action = action;
        NoteShape = noteShape;
        MaxScore = maxScore;
    }
}

public enum ActionType
{
    Clap,
    Jump,
    JesusPose,
    Lyric,
    Mix,
    ClapDouble
}

public enum NoteType
{
    Beat,
    Hold
}