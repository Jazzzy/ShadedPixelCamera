using UnityEngine;
using System.Collections;

public class moviiiingAllThePeopleMoving : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    bool movingUp = true;

    // Update is called once per frame
    void FixedUpdate () {
        float moveSpeed = 300;

        Vector2 position = GetComponent<Rigidbody2D>().position;
        

        if (position.y > 100f)
        {
            movingUp = false;
        }
        else if (position.y < -100f) {
            movingUp = true;
        }




            if (movingUp)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, moveSpeed);
        }
        else { GetComponent<Rigidbody2D>().velocity = new Vector2(0, -moveSpeed); }

    }

    
 
void OnTriggerEnter(Collider other)
{

    if (other.tag == "top") { movingUp = false; }

    if (other.tag == "bottom") { movingUp = true; }

}


}
