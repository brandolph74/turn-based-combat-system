using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;

    Vector3 moveVector;   //vector used to generate movement based on the axis that are being pressed
    Rigidbody2D rb;       //rigidbody of the player
    SpriteRenderer astroSprite;  //sprite renderer of the player
    Animator astroAnimator;  //animator of the player

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  //get rigidbody of the player
        astroSprite = GetComponent<SpriteRenderer>();  //get the sprite renderer of the player
        astroAnimator = GetComponent<Animator>();     //get animator component of the player
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Movement logic here for consistent movement regardless of FPS
    /// </summary>
    private void FixedUpdate()
    {

        float xMovement = 0f;   //initialize movement variables
        float yMovement = 0f;
       
        #region
        if (Input.GetKey(KeyCode.W))
        {
            yMovement = +1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xMovement = -1f;
        }
        if (Input.GetKey(KeyCode.S))    //movement
        {
            yMovement = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            xMovement = +1f;
        }
        #endregion


        moveVector = new Vector3(xMovement, yMovement).normalized;  //create the vector based on the movement, normalized means diagnals are equidistance as horizontal and diagnal (always magnitude of 1)

        rb.velocity = moveVector * speed;  //add the movement, the magnitude can be multiplied afterwards for more speed after normalizing

        if (xMovement != 0 || yMovement != 0)
        {
            astroAnimator.SetBool("isWalking", true);

            if (xMovement ==  -1)    //if he is moving left
            {
                astroSprite.flipX = true;  //flip the sprite so it shows that he is walking left
            }
            if (xMovement == 1)
            {
                astroSprite.flipX = false; //flip to right
            }
        }
        else
        {
            astroAnimator.SetBool("isWalking", false);  //not moving, transition back to idle
        }

    }
}
