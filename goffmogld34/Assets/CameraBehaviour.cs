using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

    public float FollowAbove;
    public float FollowBehind;   
    public float RotationSpeed;

    public PlayerBehaviour Player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Game.Instance.State == GameState.PLAY)
        {
            //follow the snowball

            float t = Time.deltaTime;

            float targetX = Player.transform.position.x;
            float targetZ = Player.transform.position.z - FollowBehind;
            float targetY = Player.transform.position.y + FollowAbove;

            Rigidbody playerbody = Player.GetComponent<Rigidbody>();

            float newx = Mathf.MoveTowards(this.transform.position.x, targetX, t * playerbody.velocity.magnitude);
            float newz = targetZ;
            float newy = Mathf.MoveTowards(this.transform.position.y, targetY, t * playerbody.velocity.magnitude);

            this.transform.position = new Vector3(newx, newy, newz);

            Quaternion q = Quaternion.LookRotation(Player.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, RotationSpeed * Time.deltaTime);
        }
    }
}
