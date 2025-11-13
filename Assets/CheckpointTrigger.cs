using UnityEngine;

// ★★★ 찐 최종 CheckpointTrigger.cs ★★★
public class CheckpointTrigger : MonoBehaviour
{
    // (니가 쳐만든 변수 2개)
    public Transform newSpawnPoint;
    public BossAI bossAI; // (니가 인스펙터에 연결한 놈)

    private bool hasBeenTriggered = false;

    // ★★★ (수정) 이 함수를 '통째로' 덮어씌워라 ★★★
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 닿은 놈이 'Player'인지 '먼저' 체크
        if (other.CompareTag("Player"))
        {
            // 1. (수정) '스폰 갱신'만 '1회용'으로 쳐막음
            if (hasBeenTriggered == false)
            {
                other.GetComponent<PlayerMovement>().UpdateSpawnPoint(newSpawnPoint.position);
                hasBeenTriggered = true;
                Debug.Log("★★★ 체크포인트 갱신!");
            }

            // 2. (수정) '보스 깨우기'는 '매번' 실행함 (if문 밖으로 뺌)
            if (bossAI != null)
            {
                bossAI.ActivateBoss(); // "일어나라 HP바 켜라" 함수 호출
            }
        }
    }
}