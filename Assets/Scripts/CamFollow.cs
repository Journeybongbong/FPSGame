using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    //component 변수 -> 초록색 
    public Transform target;
    
    void Update()
    {
        transform.position = target.position;
    }
}
