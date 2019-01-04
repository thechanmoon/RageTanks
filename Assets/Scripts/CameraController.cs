using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public PlayerStateController.playerStates currentPlayerState = PlayerStateController.playerStates.idle;
	public GameObject playerObject = null;
	public float cameraTrackingSpeed = 0.2f;
	private Vector3 lastTargetPosition = Vector3.zero;
	private Vector3 currTargetPosition = Vector3.zero;
	private float currLerpDistance = 0.0f;
	
	void Start()
	{
		// 이상하게 움직이는 일이 없도록 초기 카메라 위치를 지정한다.
		Vector3 playerPos = playerObject.transform.position;
		Vector3 cameraPos = transform.position;
		Vector3 startTargPos = playerPos;
		
		// Z값을 똑같이 설정해서 이 축으로는 움직이지 않도록 한다.
		startTargPos.z = cameraPos.z;
		lastTargetPosition = startTargPos;
		currTargetPosition = startTargPos;
		currLerpDistance = 1.0f;
	}
	
	void OnEnable()
	{
		PlayerStateController.onStateChange += onPlayerStateChange;
	}
	
	void OnDisable()
	{
		PlayerStateController.onStateChange -= onPlayerStateChange;
	}
	
	void onPlayerStateChange(PlayerStateController.playerStates newState)
	{
		currentPlayerState = newState;
	}
	
	void LateUpdate()
	{
		// 현재 상태를 기반으로 업데이트함
		onStateCycle();
		
		// 현재의 타겟 위치를 향해 계속 이동한다
		currLerpDistance += cameraTrackingSpeed;
		transform.position = Vector3.Lerp(lastTargetPosition, currTargetPosition, currLerpDistance);
	}
	
	// 엔진의 매 사이클마다 현재 상태를 업데이트한다
	void onStateCycle()
	{
		// 카메라가 취해야 하는 현재 동작을 정하기 위해 플레이어 상태를 이용한다.
		// 대부분의 경우에 우리는 플레이어의 상태를 추적하지만, 플레이어가 죽거나 부활하는 경우엔 그러지 않길 원한다.
		switch(currentPlayerState)
		{
			case PlayerStateController.playerStates.idle:
				trackPlayer();
			break;
			
			case PlayerStateController.playerStates.left:
				trackPlayer();
			break;
			
			case PlayerStateController.playerStates.right:
				trackPlayer();
			break;
			
			case PlayerStateController.playerStates.jump:
				trackPlayer();
			break;
			
			case PlayerStateController.playerStates.firingWeapon:
				trackPlayer();
			break;
		}
	}
	
	void trackPlayer()
	{
		// 현재의 카메라와 플레이어의 월드 좌표를 얻어서 저장해둔다.
		Vector3 currCamPos = transform.position;
		Vector3 currPlayerPos = playerObject.transform.position;
		
		if(currCamPos.x == currPlayerPos.x && currCamPos.y == currPlayerPos.y)
		{
			// 위치가 동일할 때에는 카메라에게 움직이지 말고 멈추도록 알려준다.
			currLerpDistance = 1.0f;
			lastTargetPosition = currCamPos;
			currTargetPosition = currCamPos;
			return;
		}
		
		// lerp할 이동 거리를 초기화한다.
		currLerpDistance = 0.0f;
		
		// lerp를 시작할 기준점이 될 현재 타겟 위치를 지정한다.
		lastTargetPosition = currCamPos;
		
		// 새로운 타겟 위치를 지정한다.
		currTargetPosition = currPlayerPos;
		
		// 타겟의 Z값을 현재의 값과 동일하게 변경한다.         //Z값이 바뀌는 걸 원하지 않는다.
		currTargetPosition.z = currCamPos.z;
	}
	
	void stopTrackingPlayer()
	{
		// 타겟 위치를 카메라의 현재 위치로 지정하여 움직임을 멈춘다.
		Vector3 currCamPos = transform.position;
		currTargetPosition = currCamPos;
		lastTargetPosition = currCamPos;
		
		// lerp될 거리를 1.0으로 설정하여 lerp가 끝났음을 알려 준다.
		// 타겟 위치를 카메라의 현재 위치로 지정하였기 때문에,
		// 카메라는 단지 현재의 위치로 lerp한 후 거기서 멈추게 된다.
		currLerpDistance = 1.0f;
	}
}
