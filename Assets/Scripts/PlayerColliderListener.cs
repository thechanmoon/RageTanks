using UnityEngine;
using System.Collections;

public class PlayerColliderListener : MonoBehaviour
{
	public PlayerStateListener targetStateListener = null;
    
	void OnTriggerEnter2D( Collider2D collidedObject )
    {
		switch(collidedObject.tag)
        {
			case "Platform":
				// 플레이어가 플랫폼에 착지했을 때, landing 상태를 전환한다.
				targetStateListener.onStateChange(PlayerStateController.playerStates.landing);
			break;
		}
	}
	
	void OnTriggerExit2D( Collider2D collidedObject)
	{
		switch(collidedObject.tag)
		{
			case "Platform":
				// 플레이어가 플랫폼을 벗어날 때 falling 상태로 설정한다.
				// 플레이어가 실제로 떨어지고 있지 않으면 PlayerStateListener가 검증할 것이다.
				targetStateListener.onStateChange(PlayerStateController.playerStates.falling);
			break;
			
			case "DeathTrigger":
				// 플레이어가 Death Trigger를 건드린다 - 죽이자!
			targetStateListener.onStateChange(PlayerStateController.playerStates.kill);
			break;
		}         
	}

}
