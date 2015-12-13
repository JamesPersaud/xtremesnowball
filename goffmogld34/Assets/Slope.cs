using UnityEngine;
using System.Collections;

public class Slope : MonoBehaviour {

    public static float MoveSpeed = 20;
    public static float moveAmount = 20;

    public bool IsRamp = false;
    public bool IsMadRamp = false;

    public float startx;
    private bool goingup = true;

	// Use this for initialization
	void Start () {
        startx = transform.position.x;
        System.Random r = new System.Random();
        if(r.Next(0,100) >50)
        {
            goingup = false;
        }
	}

    public void resetPosition()
    {
        this.transform.position = new Vector3(startx, transform.position.y, transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
	
        if(IsRamp && IsMadRamp && (Game.Instance.Difficulty == DifficultyState.MAYHEM || Game.Instance.Difficulty == DifficultyState.ARCHMASTER) && Game.Instance.State == GameState.PLAY)
        {
            if(goingup)
            {
                transform.position = new Vector3(transform.position.x + Time.deltaTime * MoveSpeed, this.transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x - Time.deltaTime * MoveSpeed, this.transform.position.y, transform.position.z);
            }
            if(transform.position.x > startx +moveAmount)
            {
                goingup = false;
            }
            else if(transform.position.x < startx - moveAmount)
            {
                goingup = true;
            }
        }
	}
}
