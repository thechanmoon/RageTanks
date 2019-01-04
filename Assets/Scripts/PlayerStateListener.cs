using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour
{         
	public float playerWalkSpeed = 3f;
	public float playerJumpForceVertical = 500f;
	public float playerJumpForceHorizontal = 250f;
	public GameObject playerRespawnPoint = null;
	public GameObject bulletPrefab = null; 
	public Transform bulletSpawnTransform;

	private Animator playerAnimator = null;
	private PlayerStateController.playerStates previousState = PlayerStateController.playerStates.idle;
	private PlayerStateController.playerStates currentState = PlayerStateController.playerStates.idle;
    private bool playerHasLanded = true;
	
	void OnEnable()
    {
         PlayerStateController.onStateChange += onStateChange;
    }
	
    void OnDisable()
    {
         PlayerStateController.onStateChange -= onStateChange;
    }
	
	void Start()
	{
		playerAnimator = GetComponent<Animator>();
		
		// 모든 특정 초기값들을 여기서 설정한다.
		PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.jump] = 1.0f;
		PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.firingWeapon] = 1.0f;
	}
    
    void LateUpdate()
    {
         onStateCycle();
    }
    
	public void hitDeathTrigger()
	{
		onStateChange(PlayerStateController.playerStates.kill);
	}
	
    // 매 주기마다 현재의 상태를 처리한다.
    void onStateCycle()
    {
		// 아래의 코드에서 접근할 수 있게 현재의 localScale을 저장해 둔다
		Vector3 localScale = transform.localScale;
		
		switch(currentState)
		{
			case PlayerStateController.playerStates.idle:
			break;
        
			case PlayerStateController.playerStates.left:
				transform.Translate(new Vector3((playerWalkSpeed * -1.0f) * Time.deltaTime, 0.0f, 0.0f));
			
				if(localScale.x > 0.0f)
				{
					localScale.x *= -1.0f;
					transform.localScale  = localScale;
				}
			
			break;
             
			case PlayerStateController.playerStates.right:
				transform.Translate(new Vector3(playerWalkSpeed * Time.deltaTime, 0.0f, 0.0f));
			
				if(localScale.x < 0.0f)
				{
					localScale.x *= -1.0f;
					transform.localScale = localScale;              
				}

			break;
             
			case PlayerStateController.playerStates.jump:
			break;
             
			case PlayerStateController.playerStates.landing:
			break;
             
			case PlayerStateController.playerStates.falling:
			break;              

			case PlayerStateController.playerStates.kill:
				onStateChange(PlayerStateController.playerStates.resurrect);
			break;         

			case PlayerStateController.playerStates.resurrect:
				onStateChange(PlayerStateController.playerStates.idle);
			break;
			
			case PlayerStateController.playerStates.firingWeapon:
			break;
		}
	}
    
    // onStateChange는 게임 코드의 어디서든 플레이어의 상태를 변경하면 호출된다
	public void onStateChange(PlayerStateController.playerStates newState)
	{
		// 현재 상태와 새로운 상태가 동일하면 중단 - 이미 지정된 상태를 바꿀 필요가 없다.
		if(newState == currentState)
			return;
		
		// 새로운 상태를 중단시킬 특별한 조건이 없는지 검증한다.
		if(checkIfAbortOnStateCondition(newState))
			return;

         
		// 현재의 상태가 새로운 상태로 전환될 수 있는지 확인한다. 아니면 중단.
		if(!checkForValidStatePair(newState))
			return;
         
		// 여기까지 도달했다면 이제 상태 변경이 허용된다는 것을 알 수 있다.
		// 새로운 상태가 무엇인지에 따라 필요한 동작을 하게 하자.
		switch(newState)
		{
			case PlayerStateController.playerStates.idle:
				playerAnimator.SetBool("Walking", false);
			break;
         
			case PlayerStateController.playerStates.left:
				playerAnimator.SetBool("Walking", true);
			break;
              
			case PlayerStateController.playerStates.right:
				playerAnimator.SetBool("Walking", true);
			break;
              
			case PlayerStateController.playerStates.jump:                   
				if(playerHasLanded)
				{
					// jumpDirection 변수를 이용하여 플레이어가 왼쪽/오른쪽/위쪽으로 점프할지를 지정한다
					float jumpDirection = 0.0f;
					if(currentState == PlayerStateController.playerStates.left)
						jumpDirection = -1.0f;
					else if(currentState == PlayerStateController.playerStates.right)
						jumpDirection = 1.0f;
					else
						jumpDirection = 0.0f;
					             
					// 실제로 점프하는 힘을 적용한다
					GetComponent<Rigidbody2D>().AddForce(new Vector2(jumpDirection * playerJumpForceHorizontal, playerJumpForceVertical));
									
					playerHasLanded = false;
    				PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.jump] = 0f;
				}
			break;

              
			case PlayerStateController.playerStates.landing:
				playerHasLanded = true;
				PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.jump]= Time.time + 0.1f;
			break;
              
			case PlayerStateController.playerStates.falling:
				PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.jump] = 0.0f;
			break;              
              
			case PlayerStateController.playerStates.kill:
			break;         

			case PlayerStateController.playerStates.resurrect:
				transform.position = playerRespawnPoint.transform.position;
				transform.rotation = Quaternion.identity;
			break;
			
			case PlayerStateController.playerStates.firingWeapon:
				// 총알 오브젝트를 만든다
				GameObject newBullet = (GameObject)Instantiate(bulletPrefab);
				              
				// 총알의 시작 위치를 설정한다
				newBullet.transform.position = bulletSpawnTransform.position;
				
				// 새 오브젝트의 PlayerBulletController 컴포넌트를 할당해서 몇 가지 데이터를 지정할 수 있다
				PlayerBulletController bullCon = newBullet.GetComponent<PlayerBulletController>();
				
				// 플레이어 오브젝트를 지정한다
				bullCon.playerObject = gameObject;
				              
				// 총알 발사!
				bullCon.launchBullet();    
				              
				// 총알이 발사되고 나면 플레이어의 상태를 이전 상태로 되돌린다
				onStateChange(currentState);
			
				PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.firingWeapon] = Time.time + 0.25f;
			break;
		}
         
		// 현재의 상태를 이전 상태로 저장해 둔다
		previousState = currentState;
		
		// 최종적으로 새로운 상태를 플레이어 오브젝트에 할당한다.
		currentState = newState;
	}    
    
	// 변경을 원하는 상태를 현재의 상태와 비교하여     // 새로운 상태로의 변경을 허용할지 확인한다.
	// 이것이 일어나길 원하는 일만 확실히 일어나도록 하는 강력한 시스템이다.
	bool checkForValidStatePair(PlayerStateController.playerStates newState)
	{
		bool returnVal = false;

		// 현재의 상태를 바꾸길 원하는 상태와 비교
		switch(currentState)
		{
			case PlayerStateController.playerStates.idle:
				// idle에서 어떤 상태로든 넘어갈 수 있음
				returnVal = true;
			break;
         
			case PlayerStateController.playerStates.left:
				// 좌측 이동에서 어떤 상태로든 넘어갈 수 있음
				returnVal = true;
			break;
              
			case PlayerStateController.playerStates.right:         
				// 우측 이동에서 어떤 상태로든 넘어갈 수 있음
				returnVal = true;              
			break;
              
			case PlayerStateController.playerStates.jump:
				// 점프 상태에서 넘아갈 수 있는 상태는 landing(착지)이나 kill뿐이다.
				if(
					newState == PlayerStateController.playerStates.landing
					|| newState == PlayerStateController.playerStates.kill
					|| newState == PlayerStateController.playerStates.firingWeapon
				  )
						returnVal = true;
				  else
						returnVal = false;
			break;
              
			case PlayerStateController.playerStates.landing:
				// 착지 상태에서 넘어갈 수 있는 상태는 idle, left, right 상태이다.
				if(
					newState == PlayerStateController.playerStates.left
					|| newState == PlayerStateController.playerStates.right
					|| newState == PlayerStateController.playerStates.idle
					|| newState == PlayerStateController.playerStates.firingWeapon
				  )
					returnVal = true;
				else
					returnVal = false;
			break;              
              
			case PlayerStateController.playerStates.falling:    
				// 추락 상태에서 넘어갈 수 있는 상태는 landing이나 kill뿐이다.
				if(
					newState == PlayerStateController.playerStates.landing
					|| newState == PlayerStateController.playerStates.kill
					|| newState == PlayerStateController.playerStates.firingWeapon
				  )
					returnVal = true;
				else
					returnVal = false;
				break;              
              
			case PlayerStateController.playerStates.kill:         
				// kill 상태에서 넘어갈 수 있는 상태는 부활이다.
				if(newState == PlayerStateController.playerStates.resurrect)
					returnVal = true;
				else
					returnVal = false;
			break;              
              
			case PlayerStateController.playerStates. resurrect :
				// 부활 상태에서 넘어갈 수 있는 상태는 idle 상태이다.
				if(newState == PlayerStateController.playerStates.idle)
					returnVal = true;
				else
					returnVal = false;                          
			break;
			
			case PlayerStateController.playerStates.firingWeapon:
				returnVal = true;
			break;
		}          
		return returnVal;
	}
	
	// checkIfAbortOnStateCondition은 상태가 시작되지 말아야 하는 이유가 있는지 확인할 수 있도록 추가적인 상태 검증을 수행한다.
	bool checkIfAbortOnStateCondition(PlayerStateController.playerStates newState)
	{
		bool returnVal = false;
		
		switch(newState)
		{
			case PlayerStateController.playerStates.idle:
			break;
			
			case PlayerStateController.playerStates.left:
			break;
			
			case PlayerStateController.playerStates.right:
			break;
			
			case PlayerStateController.playerStates.jump:
				float nextAllowedJumpTime = PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.jump ];
				
				if(nextAllowedJumpTime == 0.0f || nextAllowedJumpTime > Time.time)
					returnVal = true;
			break;
			
			case PlayerStateController.playerStates.landing:
			break;
			
			case PlayerStateController.playerStates.falling:
			break;
			
			case PlayerStateController.playerStates.kill:
			break;
			
			case PlayerStateController.playerStates.resurrect:
			break;
			
			case PlayerStateController.playerStates.firingWeapon:		
				if(PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.firingWeapon] > Time.time)
					returnVal = true;
			
			break;
		}
		
		// true 값은 ‘중지’, false 값은 ‘계속’을 의미한다.
		return returnVal;
	}

}
