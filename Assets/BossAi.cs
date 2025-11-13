using System.Collections;
using UnityEngine;
using UnityEngine.UI; // ★★★ (새로 추가) UI 쓰려면 이거 필수 


public class BossAI : MonoBehaviour
{
    // --- 보스 상태 정의 ---
    public enum BossState
    {
        Idle,       // 1. 멍 때리기
        Pattern1,   // 2. 1번 패턴 (총 쏘기)
        Hit,        // 3. 쳐맞음
        Dead        // 4. 뒤짐
    }

    [Header("상태")]
    public BossState currentState; // 현재 상태
    private bool isBusy = false; // 지금 뭐 하느라 바쁜지 (패턴 중복 방지)

    [Header("체력")]
    public int maxHp = 100;
    public int currentHp;
    public Slider hpSlider;

    [Header("패턴 1: 총 쏘기")]
    public GameObject bulletPrefab; // 니가 만든 'Bullet.prefab' 연결
    public Transform firePoint;     // 보스 총구
    public float fireRate = 0.2f;   // 플레이어보다 연사 느리게
    public int bulletCount = 10;    // 한 패턴에 10발 쏘기

    [Header("패턴 1: 멍 때리기")]
    public float idleTime = 2.0f;   // 2초 멍 때리기 (쉼표)


    void Start()
    {
        currentHp = maxHp;
        // 시작하자마자 '멍 때리기' 상태로 돌입

        if (hpSlider != null) // (실수로 연결 안 해도 에러 안 나게 방지)
        {
            hpSlider.maxValue = maxHp; // 슬라이더 최대값을 보스 최대 HP로
            hpSlider.value = currentHp;  // 슬라이더 현재값을 보스 현재 HP로
        }

    }

    void Update()
    {
        // 1. 'isBusy' (바쁨) 상태가 아닐 때만 상태를 체크함
        if (isBusy) return;

        // 2. 현재 상태(currentState)에 따라 다른 코드를 실행
        switch (currentState)
        {
            case BossState.Idle:
                // 멍 때리는 코루틴 실행
                StartCoroutine(IdleRoutine());
                break;

            case BossState.Pattern1:
                // 총 쏘는 코루틴 실행
                StartCoroutine(Pattern1Routine());
                break;

            case BossState.Hit:
                // (일단 비워둠)
                break;

            case BossState.Dead:
                // (일단 비워둠)
                break;
        }
    }

    // 1번: 멍 때리기 코루틴
    IEnumerator IdleRoutine()
    {
        // "나 이제 바쁨 (멍 때리는 중)"
        isBusy = true;
        Debug.Log("보스: 멍 때리는 중...");

        // idleTime (2초) 만큼 쉼표
        yield return new WaitForSeconds(idleTime);

        // 멍 때리기 끝났으니, 다음 행동(패턴1)으로 상태 변경
        currentState = BossState.Pattern1;
        isBusy = false; // "이제 안 바쁨"
    }

    // 2번: 총 쏘기 코루틴
    IEnumerator Pattern1Routine()
    {
        // "나 이제 바쁨 (총 쏘는 중)"
        isBusy = true;
        Debug.Log("보스: 패턴 1 (총 쏘기) 시작!");

        // bulletCount (10발) 만큼 총 쏘기 반복
        for (int i = 0; i < bulletCount; i++)
        {
            // 총알 생성 (플레이어 총 쏘기 코드 복붙)
            GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // ★★★ (중요) 보스 총알 방향 설정 ★★★
            // (일단은 무조건 왼쪽으로 쏘게 만듦. 나중에 플레이어 방향으로 쏘게 고쳐야 함)
            Vector2 shootDirection = Vector2.left; // 걍 왼쪽으로 고정

            newBullet.GetComponent<Bullet>().Setup(shootDirection);

            // fireRate (0.2초) 만큼 기다렸다가 다음 총알 쏨
            yield return new WaitForSeconds(fireRate);
        }

        // 총 10발 다 쐈으니, 다음 행동(멍 때리기)으로 상태 변경
        currentState = BossState.Idle;
        isBusy = false; // "이제 안 바쁨"
    }

    // 3번: 피격 함수 (나중에 Bullet.cs가 이걸 호출해야 함)
    public void TakeDamage(int damage)
    {
        // 이미 뒤졌거나 멍때리는 중(리셋 직후)이면 씹음
        if (currentState == BossState.Dead || currentState == BossState.Idle && !isBusy) return;

        currentHp -= damage;
        Debug.Log("보스 HP: " + currentHp);

        if (hpSlider != null) hpSlider.value = currentHp;

        // (HP 0 이하 && 아직 안 뒤졌으면)
        if (currentHp <= 0 && currentState != BossState.Dead)
        {
            currentState = BossState.Dead;
            isBusy = true;
            Debug.Log("보스: 으악 뒤짐");
            StopAllCoroutines(); // 쏘던 총알 멈춤
            StartCoroutine(DeathRoutine()); // ★폭발/시체 치우기 코루틴 호출★
        }

    }
    public void ResetBoss()
    {
        // (총알 청소는 PlayerMovement가 Respawn()에서 싹 다 함)
        Debug.Log("보스 리셋함");
        currentHp = maxHp;

        // ★★★ (수정) HP바 끄기 ★★★
        if (hpSlider != null)
        {
            hpSlider.value = currentHp;
            hpSlider.gameObject.SetActive(false); // (이게 핵심)
        }

        StopAllCoroutines();
        currentState = BossState.Idle;
        isBusy = false; // ★★★ 좆망 원인 (true -> false) ★★★
    
}
    public void ActivateBoss()
    {
        // ★★★ (수정) HP바가 '이미' 켜져있으면 걍 씹어라 ★★★
        if (hpSlider != null && hpSlider.gameObject.activeInHierarchy) return;

        if (hpSlider != null)
            hpSlider.gameObject.SetActive(true); // 1. HP바 켜기

        currentHp = maxHp;
        hpSlider.value = maxHp;

        currentState = BossState.Idle; // 2. AI 시작
        isBusy = false;
        Debug.Log("보스: ㅋ 떴노.");

}
    // ★★★ (새 함수 2) 뒤지는 연출 (TakeDamage가 호출함) ★★★
    IEnumerator DeathRoutine()
    {
        // (여기다 폭발 이펙트 빵빵 쳐넣어라)
        yield return new WaitForSeconds(2f); // 2초 뒤에

        if (hpSlider != null)
            hpSlider.gameObject.SetActive(false); // HP바 끄기

        Destroy(gameObject); // 시체 치우기
    }
}
