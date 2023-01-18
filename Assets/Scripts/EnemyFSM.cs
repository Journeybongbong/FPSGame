using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    enum EnemyState //몬스터 상태 저장
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }
    EnemyState m_State;
    
    public float findDistance = 8f; // 플레이어 발견 범위
    Transform player; // 플레이어 위치
    public float attackDistance = 2f; // 플레이어 공격 범위
    public float moveSpeed = 4f; // 몬스터 이동 속도

    private PlayerMove script; // 플레이어가 없애야하는 몬스터의 수를 받아오기 위함

    CharacterController cc;

    float currentTime = 0;
    float attackDelay = 2f;

    public int attackPower = 3; //몬스터 공격 세기

    Vector3 originPos;
    Quaternion originRot;

    public int hp; // 적의 체력
    public int maxhp = 15;
    public Slider hpSlider;
    bool isFirstHit = false;
    
    NavMeshAgent smith;

    Animator anim;

    // 오디오
    AudioSource audioHit;
    public GameObject audioDieobj;
    AudioSource audioDie;
    public GameObject audioAtkobj;
    AudioSource audioAtk;
    public GameObject audioShtobj;
    AudioSource audioSht;


    // Start is called before the first frame update
    void Start()
    {
        m_State = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;
        hp = maxhp;
        anim = transform.GetComponentInChildren<Animator>();
        script = player.GetComponent<PlayerMove>();
        audioHit = GetComponent<AudioSource>();
        audioDie = audioDieobj.GetComponent<AudioSource>();
        audioAtk = audioAtkobj.GetComponent<AudioSource>();
        audioSht = audioShtobj.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {        
        switch(m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.Move:
                Move();
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Damaged:
                //Damaged();
                break;

            case EnemyState.Die:
                break;
        }
        hpSlider.value = (float)hp / (float)maxhp;
    }

    void Idle()
    {
        if(Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            //print("상태 전환 : Idle -> Move");
            anim.SetTrigger("Run");
        }
    }

    void Move()
    {
        if(Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            //navmesh로 플레이어를 따라다님
            transform.GetComponent<NavMeshAgent>().enabled = true; 
            transform.GetComponent<EnemyMove>().enabled = true;
            smith = GetComponent<NavMeshAgent>();
        }
        else
        {
            m_State = EnemyState.Attack;
            //print("상태 전환 : Move -> Attak");
            currentTime = attackDelay;
        }
    }

    void Attack()
    {
        if(Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentTime += Time.deltaTime;
            if(currentTime > attackDelay)
            {
                script.DamageAction(attackPower);
                //print("공격!");
                currentTime = 0;
                audioAtk.Play();
                anim.SetTrigger("Attack");
            }
        }
        else
        {
            m_State = EnemyState.Move;
            //print("상태 전환 : Attack -> Move");
            currentTime = 0;
            anim.SetTrigger("AttackToRun");
        }
    }

    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }

    //코루틴 함수
    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(0.5f);
        if(isFirstHit == true)
        {
            m_State = EnemyState.Move;
            anim.SetTrigger("HitToRun");            
        }
        else
        {
            // 처음 공격 받으면 포효(shout)하고 달려옴
            m_State = EnemyState.Move;
            audioSht.Play();
            anim.SetTrigger("HitToShout");
            isFirstHit = true;
        }
    }

    public void HitEnemy(int hitPower)
    {
        if(m_State == EnemyState.Damaged || m_State == EnemyState.Die)
        {
            return;
        }

        hp -= hitPower;

        if (transform.GetComponent<NavMeshAgent>().enabled == true)
        {
            smith.ResetPath();
        }
        if(hp > 0)
        {
            m_State = EnemyState.Damaged;
            Damaged();
            print("상태 전환 : Any State -> Damagaed");
            anim.SetTrigger("Hit");
            audioHit.Play();
        }
        else{
            transform.GetComponent<EnemyMove>().enabled = false;
            transform.GetComponent<NavMeshAgent>().enabled = false;
            m_State = EnemyState.Die;
            //print("상태 전환 : Any State -> Die");
            anim.SetTrigger("Die");
            audioDie.Play();
            Die();
        }
    }

    void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieProcess());
        script.cnt -= 1;
    }

    IEnumerator DieProcess()
    {
        cc.enabled = false;
        yield return new WaitForSeconds(2f);
        print("소멸 !");
        Destroy(gameObject);
    }
}
