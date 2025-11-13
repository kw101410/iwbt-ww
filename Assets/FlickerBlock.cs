using System.Collections;
using UnityEngine;

public class FlickerBlock : MonoBehaviour
{
    // --- 인스펙터 창에서 설정할 변수 ---
    [Header("타이밍 설정")]
    public float onTime = 2.0f;  // 블럭이 켜져있는 시간 (2초)
    public float offTime = 1.5f; // 블럭이 꺼져있는 시간 (1.5초)

    [Header("시작 상태 설정")]
    public bool startOn = true; // true로 시작하면 켜진 상태로, false면 꺼진 상태로 시작

    // --- 내부에서 사용할 변수 ---
    private SpriteRenderer blockSprite; // 블럭의 '그림'
    private Collider2D blockCollider; // 블럭의 '충돌'

    void Start()
    {
        // 1. 스크립트가 붙은 오브젝트의 컴포넌트들을 찾아서 변수에 저장
        blockSprite = GetComponent<SpriteRenderer>();
        blockCollider = GetComponent<Collider2D>(); // BoxCollider2D, CapsuleCollider2D 등 모두 OK

        // 2. 타이머 코루틴을 시작시킴
        StartCoroutine(FlickerRoutine());
    }

    // 3. 타이머 기능을 하는 코루틴 함수
    private IEnumerator FlickerRoutine()
    {
        // 시작 상태 설정
        if (startOn)
        {
            // 켜진 상태로 시작
            SetBlockState(true);
            yield return new WaitForSeconds(onTime);
        }
        else
        {
            // 꺼진 상태로 시작
            SetBlockState(false);
            yield return new WaitForSeconds(offTime);
        }

        // 4. 게임이 끝날 때까지 무한 반복
        while (true)
        {
            // 블럭 끄기 (그림과 충돌 모두)
            SetBlockState(false);
            yield return new WaitForSeconds(offTime); // offTime(1.5초) 만큼 기다리기

            // 블럭 켜기 (그림과 충돌 모두)
            SetBlockState(true);
            yield return new WaitForSeconds(onTime);  // onTime(2초) 만큼 기다리기
        }
    }

    // 블럭 켜고 끄는 것을 한 번에 처리하는 함수
    private void SetBlockState(bool state)
    {
        // state가 true면 켜고, false면 끈다
        blockSprite.enabled = state;
        blockCollider.enabled = state;
    }
}