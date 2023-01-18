using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{

    NavMeshAgent eneymy;
    GameObject player;
    Animator anim;
    void Start()
    {
        eneymy=GetComponent<NavMeshAgent>();
        anim=transform.GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player");
        anim.SetBool("Run", true);
    }
    
    void Update()
    {
        //플레이어를 쫓아다님!
        eneymy.SetDestination(player.transform.position);
    }
}
