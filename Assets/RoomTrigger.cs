using UnityEngine;

// ★★★ 찐 최종: 파일 이름 RoomTrigger.cs / 클래스 이름 RoomTrigger ★★★
public class RoomTrigger : MonoBehaviour
{
    // ★★★ (니가 찾던 거) 스폰 갱신할지 말지 정하는 체크박스 ★★★
    // (보스방 들어가는 문은 '체크 해제' 해라)
    public bool shouldUpdateSpawnPoint = true;

    // (기존 변수들)
    public Transform teleportTarget;     // 순간이동할 위치 (예: Room5_SpawnPoint)
    public GameObject cameraToActivate;   // 켤 카메라 (예: Room5_VCam)
    public GameObject cameraToDeactivate; // 끌 카메라 (예: Room4_VCam)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 닿은 놈이 'Player' 태그인지 확인
        if (other.CompareTag("Player"))
        {
            // 1. 플레이어 위치 순간이동
            // (teleportTarget이 null 아니면)
            if (teleportTarget != null)
            {
                other.transform.position = teleportTarget.position;
            }

            // 2. 다음 방 카메라 켜기
            if (cameraToActivate != null)
                cameraToActivate.SetActive(true);

            // 3. 이전 방 카메라 끄기
            if (cameraToDeactivate != null)
                cameraToDeactivate.SetActive(false);

            // 4. (★수정★) 체크박스 켜져있을 때만 스폰 갱신
            if (shouldUpdateSpawnPoint)
            {
                // PlayerMovement 스크립트 없어도 에러 안 나게 방지 (?)
                // 널 체크 (?.): PlayerMovement 스크립트가 있으면 UpdateSpawnPoint 호출
                other.GetComponent<PlayerMovement>()?.UpdateSpawnPoint(teleportTarget.position);
            }
        }
    }
}