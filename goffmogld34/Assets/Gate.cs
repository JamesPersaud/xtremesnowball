using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour {

    public static float MoveSpeed = 10;
    public static float moveAmount = 20;

    private float startx;
    private bool goingup = true;

    public Pole Pole1;
    public Pole Pole2;
    public PlayerBehaviour ThePlayer;

    public bool playerPast = false;
    public bool playerScored = false;

	// Use this for initialization
	void Start ()
    {
        startx = transform.position.x;
        System.Random r = new System.Random();
        if (r.Next(0, 100) > 50)
        {
            goingup = false;
        }
    }
	
	// Update is called once per frame
	void Update () {

	    if(ThePlayer != null && !playerScored)
        {
            bool playerNowPast = (ThePlayer.transform.position.z >= this.transform.position.z);
            if(playerNowPast && !playerPast) //player has now passed and wasn't before
            {
                //the player just went past the gate, did he do it in between the two poles?
                if(ThePlayer.transform.position.x > Pole1.transform.position.x && ThePlayer.transform.position.x < Pole2.transform.position.x)
                {
                    playerScored = true;
                    Game.Instance.GatesHit++;
                    Game.Instance.Score += ThePlayer.GetComponent<Rigidbody>().velocity.magnitude;
                }
                else
                {
                    playerScored = false;
                    Game.Instance.GatesMissed++;
                }
                playerPast = true;
            }            
        }

        if (Game.Instance.Difficulty == DifficultyState.ARCHMASTER && Game.Instance.State == GameState.PLAY)
        {
            if (goingup)
            {
                transform.position = new Vector3(transform.position.x + Time.deltaTime * MoveSpeed, this.transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x - Time.deltaTime * MoveSpeed, this.transform.position.y, transform.position.z);
            }
            if (transform.position.x > startx + moveAmount)
            {
                goingup = false;
            }
            else if (transform.position.x < startx - moveAmount)
            {
                goingup = true;
            }
        }
    }
}
