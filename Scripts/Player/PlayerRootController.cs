using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lasp;

public class PlayerRootController : MonoBehaviour
{
    private const float HAND_CLAP_DISTANCE = 0.25f;
    private const float HANDS_ABOVE_HEAD_DISTANCE = 0.05f;
    private const float AUDIO_THRESHOLD = 0.25f;
    private const float JUMP_TOLERANCE = 0.5f;
    private const int JUMP_FRAMES = 3;

    [SerializeField]
    private PlayerHandController m_leftHandController;

    [SerializeField]
    private PlayerHandController m_rightHandController;

    [SerializeField]
    private GameObject m_headObject;

    [SerializeField]
    private ObjectsDirectory m_objectsDirectory;

    [SerializeField]
    private GameplayManager m_gameplayManager;

    [SerializeField]
    private AudioLevelTracker m_audioLevelTrackerComponent;

    private bool m_handsAreCloseTogether = false;
    private bool m_handsAreUp = false;
    private bool m_micIsLoud = false;

    private float m_accumulatedDJingMovement = 0f;
    private float m_previousCameraY = 0f;
    private int m_accumulatedJumpData = 0;

    private float m_leftVibrate = 0f;
    private float m_rightVibrate = 0f;

    private DateTime m_lastClapTime = DateTime.MinValue;

    public void OutputAudioLevel(float level)
    {
        Debug.Log($"Audio level is currently: {level} at {DateTime.UtcNow}");
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_leftHandController.Initialize(true);
        m_rightHandController.Initialize(false);
    }

    // Update is called once per frame
    private void Update()
    {
        m_leftVibrate = 0f;
        m_rightVibrate = 0f;

        UpdateHandsUpStatus();
        UpdateMicStatus();
        UpdateObjectInteractionStatus();
        UpdateJumpStatus();
        UpdateClapStatus();

        OVRInput.SetControllerVibration(1f, m_leftVibrate, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(1f, m_rightVibrate, OVRInput.Controller.RTouch);
    }

    private void UpdateClapStatus()
    {
        if((DateTime.Now-m_lastClapTime).TotalMilliseconds < 50)
        {
            m_leftVibrate = 1f;
            m_rightVibrate = 1f;
        }

        var handDistance = Vector3.Distance(m_leftHandController.transform.position, m_rightHandController.transform.position);

        if (handDistance < HAND_CLAP_DISTANCE && !m_handsAreCloseTogether)
        {
            m_handsAreCloseTogether = true;
            OnHandsClapped();
        }
        else if (handDistance >= HAND_CLAP_DISTANCE && m_handsAreCloseTogether)
        {
            m_handsAreCloseTogether = false;
        }
    }

    private void UpdateHandsUpStatus()
    {

        var bothHandsAboveHead = m_leftHandController.transform.position.y > (m_headObject.transform.position.y + HANDS_ABOVE_HEAD_DISTANCE) &&
            m_rightHandController.transform.position.y > (m_headObject.transform.position.y + HANDS_ABOVE_HEAD_DISTANCE);

        if (bothHandsAboveHead)
        {
            m_handsAreUp = true;
            OnHandsUp();
        }
        else if (!bothHandsAboveHead)
        {
            m_handsAreUp = false;
        }
    }

    private void UpdateMicStatus()
    {
        var defaultMicrophone = Microphone.devices[0];

        
        if (m_audioLevelTrackerComponent.normalizedLevel > AUDIO_THRESHOLD && !m_micIsLoud)
        {
            m_micIsLoud = true;
            OnMicLoud();
        }
        else if (m_audioLevelTrackerComponent.normalizedLevel <= AUDIO_THRESHOLD && m_micIsLoud)
        {
            m_micIsLoud = false;
        }
        
    }

    private void UpdateObjectInteractionStatus()
    {
        CheckObjectsNearHand(m_leftHandController);
        CheckObjectsNearHand(m_rightHandController);
    }

    private void CheckObjectsNearHand(PlayerHandController handController)
    {
        if (m_objectsDirectory.MainDJMixer.IsHandInteractingWithObject(handController) || m_objectsDirectory.LeftDeck.IsHandInteractingWithObject(handController) || m_objectsDirectory.RightDeck.IsHandInteractingWithObject(handController))
        {
            m_accumulatedDJingMovement += handController.DistanceMovedLastFrame;
            m_gameplayManager.ReceiveMix(handController.DistanceMovedLastFrame);

            if(handController.IsLeft)
            {
                m_leftVibrate = 0.6f;
            }
            else
            {
                m_rightVibrate = 0.6f;
            }
            //Debug.LogError($"{handController} interacting with mainDJMixer, last frame delta {handController.DistanceMovedLastFrame}, accumulated distance {m_accumulatedMixerMovement}");
        }

        if (m_objectsDirectory.LeftPlayButton.IsHandInteractingWithObject(handController))
        {
            // Play the game
            //Debug.LogError($"{handController} interacting with play button");
            m_gameplayManager.StartGame();
        }

        if (m_objectsDirectory.TutorialNextButton.IsHandInteractingWithObject(handController))
        {
            // Play the game
            //Debug.LogError($"{handController} interacting with play button");
            m_gameplayManager.NextSlide();
        }
    }

    private void UpdateJumpStatus()
    {
        var headYDelta = m_headObject.transform.position.y - m_previousCameraY;

        var headVelocity = headYDelta / Time.deltaTime;

        if(Mathf.Abs(headVelocity) > JUMP_TOLERANCE)
        {
            m_accumulatedJumpData++;
        }
        else
        {
            m_accumulatedJumpData = 0;
        }

        if(m_accumulatedJumpData > JUMP_FRAMES )
        {
            OnJump();
        }

        m_previousCameraY = m_headObject.transform.position.y;
    }

    private void OnHandsClapped()
    {
        m_leftVibrate = 1f;
        m_rightVibrate = 1f;

        m_lastClapTime = DateTime.Now;

        Debug.Log($"Hands are clapped at {DateTime.UtcNow}");
        m_gameplayManager.ReceiveClap();
    }

    private void OnHandsUp()
    {
        Debug.Log($"Hands are up at {DateTime.UtcNow}");
        m_gameplayManager.ReceiveJesus();

        m_leftVibrate = 0.3f;
        m_rightVibrate = 0.3f;
    }

    private void OnMicLoud()
    {
        Debug.Log($"Mic was loud at at {DateTime.UtcNow}");
        m_gameplayManager.ReceiveLyric(1f);
    }

    private void OnJump()
    {
        Debug.Log($"jumped  at {DateTime.UtcNow}");
        m_gameplayManager.ReceiveJump();
    }
}
