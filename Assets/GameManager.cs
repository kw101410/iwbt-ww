using UnityEngine;
using TMPro; // ★★★ TextMeshPro 쓰려면 이거 필수 ★★★

public class GameManager : MonoBehaviour
{
    // 1. 싱글톤 (어디서든 GameManager.instance로 부름)
    public static GameManager instance;

    // 2. 변수
    public int deathCount = 0; // 데스 카운트

    // 3. UI 연결
    public TextMeshProUGUI deathText; // 1단계에서 만든 'DeathCounter_Text' 연결할 빈칸

    void Awake()
    {
        // 4. 싱글톤 설정 (씬에 1개만 있게)
        if (instance == null)
        {
            instance = this; // 대가리는 나다
            DontDestroyOnLoad(gameObject); // 씬(Room) 바뀌어도 이 오브젝트 '안 뒤짐'
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 씬 다시 왔는데 대가리 이미 있으면 난 걍 뒤짐
        }

        // 5. 시작할 때 UI 갱신
        UpdateDeathText();
    }

    // ★★★ 플레이어가 뒤질 때 호출할 함수 ★★★
    public void IncrementDeathCount()
    {
        deathCount++; // 1 올리고
        UpdateDeathText(); // UI 갱신
    }

    // UI 텍스트 갱신하는 함수
    void UpdateDeathText()
    {
        if (deathText != null) // 1. 텍스트 쪼가리 연결됐는지 확인
        {
            // 2. ★★★ 50 넘었는지 체크 ★★★
            if (deathCount >= 50)
            {
                // 3. 50 넘었으면 'ㅋㅋ' 쳐박음
                deathText.text = "DEATH: " + deathCount + " ㅋㅋ";
           if (deathCount>= 100)
               
                deathText.text = "DEATH: " + deathCount + " ㅋㅋㅋㅋㅋ";
            }
            else
            {
                // 4. 50 안 넘었으면 걍 원래대로
                deathText.text = "DEATH: " + deathCount;
            }
        }
    }
}