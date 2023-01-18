using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    //무기 상태
    enum WeaponMode
    {
        Normal,
        Sniper
    }
    WeaponMode wMode;
    bool ZoomMode = false;
    public GameObject eff_Flash; // 총알 발사 시 총 헤드 부분의 효과
    public Text wModeText;
    public Text bulletCntText;
    public GameObject[] weapons;
    public GameObject crosshair02_zoom;
    public GameObject[] bulletEffects;
    public SceneLoad script;
    ParticleSystem ps;
    ParticleSystem es;
    public int weaponPower = 5;

    public GameObject[] audioObj;
    AudioSource shot;
    AudioSource load;
    public int currentBulletCnt;

    private bool isReload = false;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        ps = bulletEffects[0].GetComponent<ParticleSystem>();
        es = bulletEffects[1].GetComponent<ParticleSystem>();
        anim = transform.GetComponentInChildren<Animator>();
        wMode = WeaponMode.Normal;
        shot = audioObj[0].GetComponent<AudioSource>();
        load = audioObj[1].GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(script.Canv.activeSelf == false) //옵션이 켜져있지 않을 때
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)) //1을 누르면 노멀 모드로 총 전환
            {
                wMode = WeaponMode.Normal;
                if(wModeText.text != "Normal Mode")
                {
                    load.Play();
                    Camera.main.fieldOfView = 60f;
                    wModeText.text = "Normal Mode";
                    weapons[0].SetActive(true);
                    weapons[1].SetActive(false);
                    weapons[2].SetActive(true);
                    weapons[3].SetActive(false);
                    crosshair02_zoom.SetActive(false);
                    Camera.main.fieldOfView = 60f;
                    ZoomMode = false;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2)) //2를 누르면 스나이퍼 모드로 전환
            {

                wMode = WeaponMode.Sniper;
                if(wModeText.text != "Sniper Mode")
                {   
                    load.Play();
                    wModeText.text = "Sniper Mode";
                    weapons[0].SetActive(false);
                    weapons[1].SetActive(true);
                    weapons[2].SetActive(false);
                    weapons[3].SetActive(true);
                }

            }

            if(Input.GetMouseButtonDown(1)) //마우스 우 클릭(스나이퍼 줌 모드 설정)
            {
                switch(wMode)
                {
                    case WeaponMode.Normal:
                        break;
                    
                    case WeaponMode.Sniper:
                        if(!ZoomMode)
                        {
                            Camera.main.fieldOfView = 15f;
                            ZoomMode = true;
                            crosshair02_zoom.SetActive(true);
                            weapons[3].SetActive(false);
                        }
                        else
                        {
                            Camera.main.fieldOfView = 60f;
                            ZoomMode = false;
                            crosshair02_zoom.SetActive(false);
                            weapons[3].SetActive(true);
                        }
                        break;
                }
                
            }
            else if(Input.GetMouseButtonDown(0) && currentBulletCnt > 0){
                // 총알이 있는 상태에서 마우스 왼클릭 시 총 발사
                currentBulletCnt -= 1;
                bulletCntText.text = "10 / " + currentBulletCnt.ToString();
                anim.SetTrigger("GuardToShoot");
                shot.Play();
                StartCoroutine(ShootEffectOn(0.05f));
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hitInfo = new RaycastHit();
                anim.SetTrigger("ShootToGuard");
                if(Physics.Raycast(ray, out hitInfo))
                {
                    if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        // enemy일 때에만 빨간색 효과를 사용하고 데미지를 입힘
                        EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                        if(wMode == WeaponMode.Sniper)
                        {
                            eFSM.HitEnemy(5);
                        }
                        else
                        {
                            eFSM.HitEnemy(weaponPower);
                        }
                        bulletEffects[1].transform.position = hitInfo.point;
                        es.Play();
                    }
                    else
                    {
                    bulletEffects[0].transform.position = hitInfo.point;
                    ps.Play();
                    }
                }
            }
            TryReload();
        }
    }

    IEnumerator ShootEffectOn(float duration) //총알 발사 효과
    {
        eff_Flash.SetActive(true);
        yield return new WaitForSeconds(duration);
        eff_Flash.SetActive(false);
    }

    private void TryReload() //재장전 시도
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentBulletCnt < 10)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
            isReload = true;
            load.Play();
            bulletCntText.text = "10 / 10";
            yield return new WaitForSeconds(0.01f);
            currentBulletCnt = 10;
            isReload = false;
    }
}
