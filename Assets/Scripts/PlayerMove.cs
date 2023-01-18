using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 7f; // 움직임 속도
    private Rigidbody rb;
    CharacterController cc;
    float gravity = -20f; // 중력변수
    float yVelocity = 0; // 수직 속력 변수
    public float jumpPower = 7f; // 점프력 변수
    public bool isJumping = false;
    public int hp = 100;// 플레이어 체력 변수
    int maxHP = 100;
    public Slider hpSlider;
    public GameObject hitEffect; // 공격 받았을 때의 효과
    public Text hpText;

    public GameObject[] Canvs;
    public Image fadeImage;

    // 공격 받고 나서 fade in 처리하기하기 위한 함수
    public Image hitImg; 
    public float duration;
    public float fadeSpeed;
    private float durationTimer;

    public int cnt = 5; //없애야 하는 몬스터의 수 
    AudioSource audioSoure;
    public GameObject[] audioObj;
    private bool isPlay = false;
    AudioSource audioEnd;
    AudioSource audioItem;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = transform.GetComponentInChildren<Animator>();
        hitEffect.SetActive(true);
        hitImg.color = new Color(hitImg.color.r, hitImg.color.g, hitImg.color.b, 0);
        Canvs[2].SetActive(true);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        rb = GetComponent<Rigidbody>();
        audioSoure = GetComponent<AudioSource>();
        audioEnd = audioObj[0].GetComponent<AudioSource>();
        audioItem = audioObj[1].GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if(h != 0 || v != 0)
        {
            anim.SetBool("Run", true);
            audioSoure.Play();
        }
        else
        {
            anim.SetBool("Run", false);
        }
        h = h * moveSpeed * Time.deltaTime;
        v = v * moveSpeed * Time.deltaTime;

        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;

        // 메인 카메라를 기준으로 방향 변경
        dir = Camera.main.transform.TransformDirection(dir);

        transform.position += dir * moveSpeed * Time.deltaTime;

        if(isJumping && cc.collisionFlags == CollisionFlags.Below)
        {
            isJumping = false;
            yVelocity = 0;
        }

        if(Input.GetButtonDown("Jump") && !isJumping)
        {
            yVelocity = jumpPower;
            isJumping = true;
        }
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        cc.Move(dir * moveSpeed * Time.deltaTime);

        //hp bar 실시간 변경
        hpSlider.value = (float)hp / (float)maxHP;
        hpText.text = hp.ToString();

        if (hitImg.color.a > 0)
        {
            if(hp >= 30) //hp가 30 이상이면 공격 받은 효과가 점차 사라짐
            {
                durationTimer += Time.deltaTime;
                if(durationTimer > duration)
                {
                    float tempAlpha = hitImg.color.a;
                    tempAlpha -= Time.deltaTime * fadeSpeed;
                    hitImg.color = new Color(hitImg.color.r, hitImg.color.g, hitImg.color.b, tempAlpha);
                }
            }
        }

        if (cnt <= 0) //모든 몬스터를 없앤 경우
        {
            Canvs[3].SetActive(false); //게임 클리어 화면을 나타난다
            if(isPlay == false)
            {
                audioEnd.Play();
                isPlay = true;
            }
            if (fadeImage.color.a < 1)
            {
                durationTimer += Time.deltaTime;
                if(durationTimer > duration)
                {
                    float tempAlpha = fadeImage.color.a;
                    tempAlpha += Time.deltaTime * fadeSpeed;
                    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, tempAlpha);
                }
            }
            else
            {
                Canvs[1].SetActive(true);
            }
        }
    }

    public void DamageAction(int damage)
    {
        hp -= damage;
        if(hp > 0)
        {
            durationTimer = 0;
            hitImg.color = new Color(hitImg.color.r, hitImg.color.g, hitImg.color.b, 1);
        }
        if(hp <= 0)
        {
            hp = 0;
            anim.SetTrigger("Die");
            Canvs[0].SetActive(true); //게임 오버 화면을 띄운다
        }
        print(hp);
    }

    void OnTriggerEnter(Collider other) //아이템을 먹었을 경우
    {
		if (other.gameObject.CompareTag("Item"))
        {
            audioItem.Play();
			other.gameObject.SetActive(false);
            if(hp + 15 > 100)
            {
                hp = 100;
            }
            else
            {
                hp += 15;
            }
        }
		
    }
}

