using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

    public float MinTorque;
    public float MaxTorque;
    public float CurrentTorque;
    public float AccumulatedSnow;

    private float lasty;
    public float Velocity;

	// Use this for initialization
	void Start () {
        CurrentTorque = MaxTorque;
	}
	
	// Update is called once per frame
	void Update () {

        if (Game.Instance.State == GameState.PLAY)
        {
            float torque = CurrentTorque * Input.GetAxis("Horizontal");

            if (torque > float.Epsilon || torque < -float.Epsilon)
            {
                //force is applied at right angles to the current direction of momentum
                Vector3 turnvector = Vector3.zero;
                //get velocity
                Rigidbody rb = GetComponent<Rigidbody>();
                Vector3 velocity = rb.velocity.normalized;

                turnvector = Vector3.Cross(Vector3.up, velocity);

                //apply force
                GetComponent<Rigidbody>().AddForce(turnvector.normalized * torque);

                //imagine we're skiing, there will be more friction the more sideways the skis are
                //the larger (abs) the z component of the turn vector, the more friction
                PhysicMaterial pmat = GetComponent<Collider>().material;
                pmat.dynamicFriction = Mathf.Abs(turnvector.z);
                pmat.staticFriction = Mathf.Abs(turnvector.z);
            }

            AccumulatedSnow += transform.position.y - lasty;
            lasty = transform.position.y;

            Velocity = GetComponent<Rigidbody>().velocity.magnitude;
            //turning should get harder when you're going slower
            CurrentTorque = Mathf.Clamp(Velocity, MinTorque, MaxTorque);
        }
        else
        {
            transform.position = new Vector3(0, 0, 10);
            transform.LookAt(new Vector3(0,0,11));
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();           
        }
    }
}
