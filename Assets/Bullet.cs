using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;

    // ★★★ (새로 추가) 총알 주인 (true: 플레이어 총알, false: 보스 총알) ★★★
    public bool isPlayerBullet = false;

    private Rigidbody2D rb;

    public void Setup(Vector2 shootDirection)
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb.sharedMaterial == null)
        {
            rb.sharedMaterial = new PhysicsMaterial2D { name = "RuntimeNoFriction", friction = 0, bounciness = 0 };
        }

        rb.linearVelocity = shootDirection.normalized * speed;
        Destroy(gameObject, lifeTime);
    }

    // ★★★ (수정) Is Trigger 로직 싹 다 덮어쓰기 ★★★
    private void OnTriggerEnter2D(Collider2D other)
    {
        // --- 이 총알이 '플레이어' 총알일 경우 ---
        if (isPlayerBullet)
        {
            // 1. 보스 때리기
            if (other.CompareTag("Boss"))
            {
                other.GetComponent<BossAI>().TakeDamage(1);
                Destroy(gameObject); // 총알 삭제
                return;
            }

            // 2. 플레이어 빼고 나머지 (벽, 가시 등)
            if (!other.CompareTag("Player"))
            {
                string layerName = LayerMask.LayerToName(other.gameObject.layer);
                if (layerName == "Platforms" || layerName == "Ground" || other.CompareTag("Death"))
                {
                    Destroy(gameObject); // 총알 삭제
                }
            }
        }
        // --- 이 총알이 '보스' 총알일 경우 ---
        else
        {
            // 1. 플레이어 때리기
            if (other.CompareTag("Player"))
            {
                Debug.Log("플레이어 쳐맞음!");
                // 걍 리스폰 시킴 (니 PlayerMovement.cs에 Respawn() 함수 이미 있잖노)
                other.GetComponent<PlayerMovement>().Respawn();
                Destroy(gameObject); // 총알 삭제
                return;
            }

            // 2. 보스 빼고 나머지 (벽, 가시 등)
            // (보스 태그에 닿으면 걍 무시하고 통과함)
            if (!other.CompareTag("Boss"))
            {
                string layerName = LayerMask.LayerToName(other.gameObject.layer);
                if (layerName == "Platforms" || layerName == "Ground" || other.CompareTag("Death"))
                {
                    Destroy(gameObject); // 총알 삭제
                }
            }
        }
    }
}