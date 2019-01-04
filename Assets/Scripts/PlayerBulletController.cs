using UnityEngine;
using System.Collections;

public class PlayerBulletController : MonoBehaviour
{
     public GameObject playerObject = null; // PlayerStateListener에서 총알이 생성된 후 자동으로 지정된다.
     public float bulletSpeed = 15.0f;
	
	 private float selfDestructTimer = 0.0f;
    
	void Update()
	{
		if(selfDestructTimer > 0.0f)
		{
			if(selfDestructTimer < Time.time)
				Destroy(gameObject);
		}
	}
	
     public void launchBullet()
     {
          // 플레이어 오브젝트의 localScale은 플레이어가 어느 방향을 바라보는지 알려준다.
          // 플레이어가 바라보는 방향을 별도의 변수로 만드는 대신 이미 알고 있는 정보로 확인한다.
          float mainXScale = playerObject.transform.localScale.x;

		  Vector2 bulletForce;
		
          if(mainXScale < 0.0f)
          {
               // 왼쪽으로 총알 발사
               bulletForce = new Vector2(bulletSpeed * -1.0f,0.0f);
          }
          else
          {
               // 오른쪽으로 총알 발사
               bulletForce = new Vector2(bulletSpeed,0.0f);
          }
		
		GetComponent<Rigidbody2D>().velocity = bulletForce;
		
		selfDestructTimer = Time.time + 1.0f;
     }
}
