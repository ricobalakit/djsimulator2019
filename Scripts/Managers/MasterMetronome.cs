using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MasterMetronome : MonoBehaviour
{

    #region Private Constants

    private const float SLIDER_DEFAULT_MINIMUM_TEMPO = 100f;
    private const float SLIDER_DEFAULT_MAXIMUM_TEMPO = 220f;

    #endregion

    #region Public Enums

    public enum StepSize
    {
        Thirtysecondth = 0,
        Sixteenth = 1,
        Eighth = 2,
        Quarter = 3,
        Half = 4,
        OneNote = 5,
        TwoNote = 6,
        FourNote = 7,
        EightNote = 8,
        SixteenNote = 9,
        ThirtyTwoNote = 10,
        Off = 11
    }

    #endregion

    #region Private Variables

    [SerializeField]
    private DateTime m_timeReferencePoint = DateTime.MinValue;

    [SerializeField]
    private float m_tempoBPM = 170;

    [SerializeField]
    private Text m_tempoIndicator = null;

    #endregion

    #region Private Static Variables

    private static MasterMetronome s_instance = null;

    #endregion

    #region Public Static Properties

    public static MasterMetronome Instance
    {
        get { return s_instance; }
    }

    #endregion

    #region Public Properties

    public float Tempo
    {
        get { return m_tempoBPM; }
        set
        {
            m_tempoBPM = value;
            AdjustTimePoint(m_tempoBPM, value);
            
            
            //m_tempoIndicator.text = String.Format("Tempo: {0} BPM", m_tempoBPM);
        }
    }

#endregion

    #region Public Methods

    public void RoundTempo()
    {
        Tempo = Mathf.Round(Tempo);
    }

    public void IncreaseTempo()
    {
        Tempo = Tempo+1;
    }

    public void DecreaseTempo()
    {
        Tempo = Tempo-1;
    }

    public void DoubleTempo()
    {
        Tempo = Tempo * 2f;
    }
    
    public void HalfTempo()
    {
        Tempo = Tempo / 2f;
    }

    public void NudgeForwardFour()
    {
        NudgeBeats(4f);
    }

    public void NudgeBackwardsFour()
    {
        NudgeBeats(-4f);
    }

    public void NudgeForwardOne()
    {
        NudgeBeats(1f);
    }

    public void NudgeBackwardsOne()
    {
        NudgeBeats(-1f);
    }

    public void NudgeForwardSmall()
    {
        NudgeBeats(1/8f);
    }

    public void NudgeBackwardsSmall()
    {
        NudgeBeats(-1/8f);
    }


    public void NudgeBeats(float beatsToNudge)
    {
        var secondsToNudge = TempoToSecondsPerBeat(m_tempoBPM) * -beatsToNudge;
        m_timeReferencePoint = m_timeReferencePoint.AddSeconds(secondsToNudge);

        if (m_timeReferencePoint > DateTime.Now)
        {
            m_timeReferencePoint.AddSeconds(TempoToSecondsPerBeat(m_tempoBPM) * -32f);
        }
    }

    public float ElapsedBeatsModulized()
    {
        var elapsedTime = (DateTime.Now - m_timeReferencePoint).TotalSeconds;
        var divisor = TempoToSecondsPerBeat() * 32f;
        var modulus = (float)elapsedTime % divisor;
        return modulus;

    }

    public float ElapsedBeats()
    {
        var elapsedTime = (DateTime.Now - m_timeReferencePoint).TotalSeconds;

        return (float)elapsedTime / TempoToSecondsPerBeat();
    }

    public void AdjustTimePoint(float oldTempo, float newTempo)
    {
        var oldFraction = FractionThroughStep(MasterMetronome.StepSize.EightNote);

        m_timeReferencePoint = DateTime.Now.AddSeconds( -oldFraction * 32f * TempoToSecondsPerBeat(newTempo));

        m_tempoBPM = newTempo;

        var newFraction = FractionThroughStep(MasterMetronome.StepSize.EightNote);


    }

    public void Start()
    {
        m_timeReferencePoint = DateTime.Now;
        Tempo = m_tempoBPM;

        if(s_instance != null)
        {
            Debug.LogErrorFormat("There are more than 1 master metronomes! this is bad!");
            this.enabled = false;
        }
        else
        {
            s_instance = this;
        }
    }

    public float FractionThroughStep(StepSize stepSize)
    {
        var elapsedTime = (DateTime.Now - m_timeReferencePoint).TotalSeconds;

        var divisor = TempoToSecondsPerBeat() * StepSizeToFloat(stepSize) * 4f;

        var modulus = elapsedTime % divisor;

        var fractionModulusThroughDivisor = modulus / divisor;

        return (float)fractionModulusThroughDivisor;
    }

    public bool WithinStep(StepSize stepSize)
    {
        var fractionThroughStep = FractionThroughStep(stepSize);
        return fractionThroughStep < 0.5f;
    }

    public void SetMetronomeReferencePoint()
    {
        m_timeReferencePoint = DateTime.Now;
        Debug.Log("Setting reference point to now");
    }

    public void SetMetronomeReferencePoint(float beatsToMove)
    {
        m_timeReferencePoint.AddSeconds(TempoToSecondsPerBeat() * beatsToMove);
    }

    public float StepSizeToFloat(StepSize stepSize)
    {
        switch (stepSize)
        {
            case StepSize.Thirtysecondth:
                return 1f / 32f;
            case StepSize.Sixteenth:
                return 1f / 16f;
            case StepSize.Eighth:
                return 1f / 8f;
            case StepSize.Quarter:
                return 1f / 4f;
            case StepSize.Half:
                return 1f / 2f;
            case StepSize.OneNote:
                return 1f;
            case StepSize.TwoNote:
                return 2f;
            case StepSize.FourNote:
                return 4f;
            case StepSize.EightNote:
                return 8f;
            case StepSize.SixteenNote:
                return 16f;
            case StepSize.ThirtyTwoNote:
                return 32f;
            case StepSize.Off:
                return float.MaxValue;
        }

        Debug.LogErrorFormat("Unhandled StepSize input {0}", stepSize);
        return 1f;
    }

    public float TempoToSecondsPerBeat()
    {
        return TempoToSecondsPerBeat(m_tempoBPM);
    }

    public static float TempoToSecondsPerBeat(float tempoBPM)
    {
        return 60f / tempoBPM;
    }

#endregion

}
