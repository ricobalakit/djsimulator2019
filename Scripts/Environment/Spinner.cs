using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0f, Time.time *10f, 0f);
    }
}
