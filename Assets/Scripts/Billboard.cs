using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;
    // Update is called once per frame
    void Update()
    {
        //캔버스의 방향과 카메라의 방향을 일치
        transform.forward = target.forward;
    }
}
