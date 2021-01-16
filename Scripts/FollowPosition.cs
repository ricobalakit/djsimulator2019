using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField]
    private Transform m_transformToFollow;

    void Update()
    {
        transform.position = m_transformToFollow.position;
    }
}
