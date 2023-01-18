using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public GameObject Canv;

    void Update()
    {
        if (Input.GetKey("escape") && SceneManager.GetActiveScene().name == "PlayScene")
        {
            //PlayScene에서 esc를 누르면 옵션 창이 뜨도록 한다.
            Canv.SetActive(true);
            Time.timeScale = 0f; //게임 일시 정지
        }
            
    }
    public void SceneChange() //플레이 화면으로 전환
    {
        SceneManager.LoadScene("PlayScene");
        Time.timeScale = 1f;
    }

    public void Exit() // 프로그램 종료
    {
        Application.Quit();
    }

    public void gohome() // 시작 화면으로 전환
    {
        SceneManager.LoadScene("StartScene");
    }

    public void Menual() //튜토리얼 창 열기
    {
        Canv.SetActive(true);
    }

    public void MenualClose() // 튜토리얼 창 닫기
    {
        Canv.SetActive(false);
    }

    public void Cotinue() // 게임 계속 진행
    {
        Canv.SetActive(false);
        Time.timeScale = 1f;
    }
}
