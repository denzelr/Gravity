using UnityEngine;
using System.Collections;

public class HeroFreeMovement : MonoBehaviour {

    //Hero Object for game movement and transformations
    public Rigidbody2D hero;

    //Empty gameobjects used for jump detection
    public Transform groundCheck;
    public Transform wallCheck;

    //Movement speed parameters
    public float moveSpeed = 5.0f;
    public float airSpeed = 1.0f;
    public float jumpSpeed = 6.0f;
    public float jumpPush = 10.0f;

    //Initializing 2d vector for constant movement speed.
    Vector2 moveDirection = Vector2.zero;

    //Ground detection and wall detection booleans
    bool grounded = false;
    bool walled = false;

    //Detects which direction the character is facing
    bool facingRight = true;

    //Radius in which the ground and wall detection objects collide with wall
    float groundRadius = 0.2f;

    //initializing layermask to detect if the object the ground and wall checks is actually a wall.
    public LayerMask whatIsGround;

    float damping = 64;
    public int jumpdir;

    // Use this for initialization
    // What do you want to happen when the scene loads up
    void Start()
    {

    }

    void FixedUpdate ()
    {
        if (grounded)
        {
            float move = Input.GetAxis("Horizontal");
            float movev = Input.GetAxis("Vertical");

            if (jumpdir == 0 || jumpdir == 1)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(move * moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if (jumpdir == 2 || jumpdir == 3)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, movev * moveSpeed);
            }

            if (move > 0 && !facingRight || (jumpdir == 2 && movev > 0 && facingRight) || (jumpdir == 3 && movev > 0 && !facingRight))
                flip();
            else if (move < 0 && facingRight || (jumpdir == 2 && movev < 0 && !facingRight) || (jumpdir == 3 && movev < 0 && facingRight))
                flip();
        }
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (grounded == true)
            {
                Physics2D.gravity = new Vector2(0f, -9.81f);
                Vector3 targetdown = new Vector3(0, 1, 0);
                transform.up = Vector3.Slerp(transform.up, targetdown, Time.deltaTime * damping);
                jumpdir = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (grounded == true)
            {
                Physics2D.gravity = new Vector2(0f, 9.81f);
                Vector3 targetUp = new Vector3(0, -1, 0);
                transform.up = Vector3.Slerp(transform.up, targetUp, Time.deltaTime * damping);
                jumpdir = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (grounded == true)
            {
                Physics2D.gravity = new Vector2(-9.81f, 0f);
                Vector3 targetleft = new Vector3(1, 0, 0);
                transform.up = Vector3.Slerp(transform.up, targetleft, Time.deltaTime * damping);
                jumpdir = 2;
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (grounded == true)
            {
                Physics2D.gravity = new Vector2(9.81f, 0f);
                Vector3 targetright = new Vector3(-1, 0, 0);
                transform.up = Vector3.Slerp(transform.up, targetright, Time.deltaTime * damping);
                jumpdir = 3;
            }
        }

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        walled = Physics2D.OverlapCircle(wallCheck.position, groundRadius, whatIsGround);

        //Change movement direction after character changes direction
        if (facingRight)
        {
            if (jumpdir == 0)
                hero.velocity = new Vector2(moveSpeed, hero.velocity.y);
            else if (jumpdir == 1)
                hero.velocity = new Vector2(moveSpeed, hero.velocity.y);
            else if (jumpdir == 2)
                hero.velocity = new Vector2(hero.velocity.x, -moveSpeed);
            else if (jumpdir == 3)
                hero.velocity = new Vector2(hero.velocity.x, moveSpeed);
        }
        else
        {
            if (jumpdir == 0)
                hero.velocity = new Vector2(-moveSpeed, hero.velocity.y);
            else if (jumpdir == 1)
                hero.velocity = new Vector2(-moveSpeed, hero.velocity.y);
            else if (jumpdir == 2)
                hero.velocity = new Vector2(hero.velocity.x, moveSpeed);
            else if (jumpdir == 3)
                hero.velocity = new Vector2(hero.velocity.x, -moveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                if (jumpdir == 0)
                {
                    hero.velocity = new Vector2(0, jumpSpeed);
                }
                else if (jumpdir == 1)
                {
                    hero.velocity = new Vector2(0, -jumpSpeed);
                }
                else if (jumpdir == 2)
                {
                    hero.velocity = new Vector2(jumpSpeed, 0);
                }
                else if (jumpdir == 3)
                {
                    hero.velocity = new Vector2(-jumpSpeed, 0);
                }

            }
            else if (walled)
            {
                if (facingRight)
                {
                    if (jumpdir == 0)
                    {
                        hero.velocity = new Vector2(-jumpPush, jumpSpeed);
                    }
                    else if (jumpdir == 1)
                    {
                        hero.velocity = new Vector2(-jumpSpeed, -jumpSpeed);
                    }
                }
                else
                {
                    if (jumpdir == 0)
                    {
                        hero.velocity = new Vector2(jumpPush, jumpSpeed);
                    }
                    else if (jumpdir == 1)
                    {
                        hero.velocity = new Vector2(jumpPush, -jumpSpeed);
                    }
                }
                flip();
            }
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
