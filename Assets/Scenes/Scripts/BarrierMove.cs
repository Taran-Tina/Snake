using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierMove : MonoBehaviour
{
    void Update()
    {
        transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(0f,0f,-22f), Time.deltaTime * 0.25f);
    }
}
