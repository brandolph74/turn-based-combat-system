using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] GameObject player;


    Vector3 moveVector;   //vector used to generate movement based on the axis that are being pressed
    Rigidbody2D rb;       //rigidbody of the player
    SpriteRenderer astroSprite;  //sprite renderer of the player
    Animator astroAnimator;  //animator of the player
    
    PlayerControls controls;  //input system for a controller

    

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("X"))
        {
            var savedPosition = new Vector2(PlayerPrefs.GetFloat("X"), PlayerPrefs.GetFloat("Y"));
            player.transform.position = (savedPosition);
        }
        rb = GetComponent<Rigidbody2D>();  //get rigidbody of the player
        astroSprite = GetComponent<SpriteRenderer>();  //get the sprite renderer of the player
        astroAnimator = GetComponent<Animator>();     //get animator component of the player

        controls = new PlayerControls();  //instantiate the new input system script
        controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = controls.TopDown.Move.ReadValue<Vector2>().normalized; //create the vector based on the movement, normalized means diagnals are equidistance as horizontal and diagnal (always magnitude of 1)
        
        //put rigidbody velocity in fixedupdate for consistent movement
        
    }

    /// <summary>
    /// Movement logic here for consistent movement regardless of FPS
    /// </summary>
    private void FixedUpdate()
    {

        float xMovement = 0f;   //initialize movement variables
        float yMovement = 0f;
       
        //old input system code used before the switch
        
        /*
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
        */
  
        rb.velocity = moveVector * speed;  //add the movement, the magnitude can be multiplied afterwards for more speed after normalizing

        xMovement = moveVector.x;
        yMovement = moveVector.y;

        if (xMovement != 0 || yMovement != 0)
        {
            astroAnimator.SetBool("isWalking", true);

            if (xMovement <  0)    //if he is moving left
            {
                astroSprite.flipX = true;  //flip the sprite so it shows that he is walking left
            }
            if (xMovement > 0)
            {
                astroSprite.flipX = false; //flip to right
            }
        }
        else
        {
            astroAnimator.SetBool("isWalking", false);  //not moving, transition back to idle
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Battle-able")
        {
            if (controls.TopDown.Interact.IsPressed())
            {
                PlayerPrefs.SetFloat("X", rb.position.x);
                PlayerPrefs.SetFloat("Y", rb.position.y);
                SceneManager.LoadScene("Battleground");
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("X");
        PlayerPrefs.DeleteKey("Y");
    }

    private void OnApplicationPause(bool pause)
    {
        PlayerPrefs.DeleteKey("X");
        PlayerPrefs.DeleteKey("Y");
    }


}
