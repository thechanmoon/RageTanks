using UnityEngine;
using System.Collections;

public class PlayerStateController : MonoBehaviour 
{
	public enum playerStates
	{
		idle = 0,
		left,
		right,
		jump,
		landing,
		falling,
		kill,
		resurrect
	}
		
	public delegate void playerStateHandler(PlayerStateController.playerStates newState);
	public static event playerStateHandler onStateChange;
	
	void LateUpdate () 
	{
		// 가로축의 현재 입력 상태를 알아내서 필요에 따라 플레이어 상태 변경을 전파한다.
		// 현재 유저 입력이 정확히 반영되도록 매 프레임마다 수행한다.
		float horizontal = Input.GetAxis("Horizontal");
		if(horizontal != 0.0f)
		{
			if(horizontal < 0.0f)
			{
				if(onStateChange != null)
					onStateChange(PlayerStateController.playerStates.left);
			}
			else
			{
				if(onStateChange != null)
					onStateChange(PlayerStateController.playerStates.right);
			}
		}
		else
		{
			if(onStateChange != null)
				onStateChange(PlayerStateController.playerStates.idle);
		}
	}
}
