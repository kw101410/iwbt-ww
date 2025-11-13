using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    // --- 기본 변수 (이전과 동일) ---
    public float moveSpeed = 5f;
    private float moveInput;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    // --- 바닥 체크 변수 (이전과 동일) ---
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool isGrounded;

    // --- IWBTG 스타일 점프 변수 ---
    public float jumpForce = 10f;       // 점프 힘 (풀 점프 높이)
    public int extraJumpsValue = 1;     // 추가 점프 횟수 (1 = 2단 점프)
    private int jumpCount;              // 남은 점프 횟수

    // --- IWBTG 조작감 핵심 변수 (이전과 동일) ---
    public float jumpCutMultiplier = 0.5f;

    // ★★★ 리스폰 위치를 저장할 변수 ★★★
    private Vector3 currentSpawnPoint;

    [Header("Shooting")] // 인스펙터 창에서 구분선 생김
    public GameObject bulletPrefab; // "Bullet.prefab"을 여기 연결
    public Transform firePoint;     // 총알 나갈 위치 (플레이어 자식 오브젝트)
    public float fireRate = 0.1f;   // 연사 속도 (0.1초에 한 발)
    private float nextFireTime = 0f; // 다음 발사 시간 계산용

    public AudioClip shootSound; // "퓽.wav" 같은 총알 소리 파일
    private AudioSource myAudioSource; // 플레이어에 붙인 오디오 소스 컴포넌트

    public AudioClip jumpSound; // "점프.wav" 같은 파일 연결할 곳
    public BossAI bossAI;
    [Header("Respawn Camera Control")]
    public GameObject room4VCam; // 4번 방 VCam (리스폰할 방)
    public GameObject room5VCam; // 5번 방 VCam (보스방)
   

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpCount = extraJumpsValue;

        // ★★★ 처음 스폰 위치를 현재 위치로 저장 ★★★
        currentSpawnPoint = transform.position;

        myAudioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // 1. 바닥 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // 2. 입력 (★딱 한 번만 해라★)
        moveInput = Input.GetAxisRaw("Horizontal");

        // 3. 물리 이동
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 4. 뒤집기
        if (isFacingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (isFacingRight == true && moveInput < 0)
        {
            Flip();
        }
    }

    void Update()
    {
        // --- 1. 바닥 감지 및 점프 횟수 리셋 (이전과 동일) ---

        if (isGrounded == true)
        {
            jumpCount = extraJumpsValue;
        }

        // --- 2. 점프 (이전과 동일) ---
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded == true)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                myAudioSource.PlayOneShot(jumpSound);
            }
            else if (jumpCount > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpCount--;

                myAudioSource.PlayOneShot(jumpSound);
            }
        }

        // --- 3. 가변 점프 (이전과 동일) ---
        if (Input.GetButtonUp("Jump"))
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
        }

        // ★★★ 4. 리스폰 키 입력 감지 ★★★
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
        if (Input.GetButton("Fire1"))
        {
            // ★★★ 디버깅 2: 쿨타임이 도는지 확인 ★★★

            // 현재 시간이 다음 발사 시간(쿨타임)보다 커졌으면
            if (Time.time >= nextFireTime)
            {

                // 1. 다음 발사 시간 갱신 (쿨타임 시작)
                nextFireTime = Time.time + fireRate;

                // 2. 총알(prefab)을 firePoint 위치에 '찍어내기'
                GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

                // ★★★ (새로 추가) '이건 내 총알이다' 표시 ★★★
                newBullet.GetComponent<Bullet>().isPlayerBullet = true;

                // 3. 총알 방향 설정 (isFacingRight는 니 Flip() 함수에 이미 있는 변수)
                Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;

                // 4. 방금 찍어낸 총알의 Bullet.cs 스크립트를 가져와서 Setup 함수 호출
                newBullet.GetComponent<Bullet>().Setup(shootDirection);

                // ★★★ 사운드 재생 ★★★
                myAudioSource.PlayOneShot(shootSound);
            }
        }
    }
    // 뒤집기 함수 (이전과 동일)
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // ★★★ 리스폰 실행 함수 ★★★
    public void Respawn()
    {
        // 1. 보스 리셋 꼰지르기 (기존 코드)
        if (bossAI != null)
        {
            bossAI.ResetBoss();
        }

        // 2. 스폰 위치로 이동 (기존 코드)
        transform.position = currentSpawnPoint;
        rb.linearVelocity = Vector2.zero;

        // 3. (★핵심★) 카메라 강제 전환
        // (currentSpawnPoint가 4번 방(Boss_Checkpoint)이라는 전제 하에)
        if (room4VCam != null && room5VCam != null)
        {
            Debug.Log("카메라 강제 전환: 5번 방(보스방) 끄고, 4번 방(리스폰) 켠다.");
            room5VCam.SetActive(false);
            room4VCam.SetActive(true);
        }
        else
        {
            Debug.LogError("PlayerMovement 스크립트에 VCam 연결 삑사리났다 이기!");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 닿은 물체의 태그가 "Death"인지 확인
        if (other.CompareTag("Death"))
        {
            // "Death" 태그가 맞다면, Respawn() 함수를 즉시 호출
            Respawn();
        }
    }
    public void UpdateSpawnPoint(Vector3 newPosition)
    {
        currentSpawnPoint = newPosition;
    }
}