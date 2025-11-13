using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    // ★★★ 'Boss_Checkpoint' 연결할 곳
    public Transform newSpawnPoint;

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered == false && other.CompareTag("Player"))
        {
            // 1. 리스폰 위치 갱신
            other.GetComponent<PlayerMovement>().UpdateSpawnPoint(newSpawnPoint.position);

            // 2. 두 번 작동 안 하게 막음
            hasBeenTriggered = true;

            Debug.Log("★★★ 체크포인트 갱신! 새 스폰 위치: " + newSpawnPoint.name);
        }
    }
}