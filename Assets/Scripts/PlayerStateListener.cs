using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour
{         
	public float playerWalkSpeed = 3f;
	
	private Animator playerAnimator = null;
	private PlayerStateController.playerStates currentState = PlayerStateController.playerStates.idle;
    
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
	}
    
    void LateUpdate()
    {
         onStateCycle();
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
			break;         

			case PlayerStateController.playerStates.resurrect:
			break;                   
		}
	}
    
    // onStateChange는 게임 코드의 어디서든 플레이어의 상태를 변경하면 호출된다
	public void onStateChange(PlayerStateController.playerStates newState)
	{
		// 현재 상태와 새로운 상태가 동일하면 중단 - 이미 지정된 상태를 바꿀 필요가 없다.
		if(newState == currentState)
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
			break;
              
			case PlayerStateController.playerStates.landing:
			break;
              
			case PlayerStateController.playerStates.falling:
			break;              
              
			case PlayerStateController.playerStates.kill:
			break;         

			case PlayerStateController.playerStates.resurrect:
			break;                   
		}
         
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
		}          
		return returnVal;
	}
}
