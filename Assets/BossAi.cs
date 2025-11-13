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

        currentState = BossState.Idle;
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
        currentHp -= damage;
        Debug.Log("보스 HP: " + currentHp);

        // (나중에 쳐맞는 이펙트, 무적 시간, 상태 변경 등 추가할 곳)

        if (hpSlider != null)
        {
            hpSlider.value = currentHp;
        }

        if (currentHp <= 0)
        {
            currentState = BossState.Dead;
            isBusy = true; // 뒤졌으니 아무것도 못하게 막음
            Debug.Log("보스: 으악 뒤짐");
            // (폭발 이펙트, 오브젝트 파괴 로직 추가)
            Destroy(gameObject, 2f); // 2초 뒤에 시체 치우기
        }
    }
    public void ResetBoss()
    {
        Debug.Log("보스: ㅋ 쫄? 리셋함.");

        // 1. HP 꽉 채우기
        currentHp = maxHp;

        // 2. HP바 (슬라이더) 꽉 채우기
        if (hpSlider != null)
        {
            hpSlider.value = currentHp;
        }

        // 3. (중요) 보스 패턴 멈추기
        // 걍 멍 때리는 상태로 강제 변경하고 isBusy 풀어버림
        StopAllCoroutines(); // 쏘던 총알 멈춤
        currentState = BossState.Idle;
        isBusy = false;
    }
}