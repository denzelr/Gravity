using UnityEngine;
using System.Collections;

/** @file RobotControllerScript */ 
/// \brief
/// Robot controller sript. Used to Control player and change gravity in scene. Also used to detect collision
/// on player.
/// @param grounded Detects if the player is standing on something
/// @param facingRight Detects which direction the player is standing
/// @param whatIsGround Allows the developer to dictate what is consider ground for grounded variable.
/// @param damping Buffer to tell how much to flip character when gravity shifts
/// @param jumpdir Variable to track gravity orientation
/// @param dead Tracks if character was killed
/// 

public class RobotControllerSript : MonoBehaviour {

	public float maxSpeed = 10f; 
	public bool facingRight = true;
	Animator anim;
	bool grounded = false;
	public Transform groundCheck;
	float groundRadius = 0.2f;
	public LayerMask whatIsGround;
	public float jumpForce = 700f;
	float damping = 64;
	public int jumpdir;
	private Vector2 crushup = new Vector2(0,1);
	private Vector2 crushdown = new Vector2(0,-1);
	private Vector2 crushleft = new Vector2(-1,0);
	private Vector2 crushright = new Vector2(1,0);
	public AudioClip crush;
	public bool dead = false;
	
	// Use this for initialization
	/// <summary>
	/// Start runs code when script is loaded. Finds box colliders, animation states, initializes gravity,
	/// rotates character according to gravity, and sets jumpdir to gravity orientation.
	/// </summary>
	void Start () {
		Collider[] col = (Collider[])Component.FindObjectsOfType<Collider>();
		anim = GetComponent<Animator> ();
		Physics2D.gravity = new Vector2(0f,-9.81f);
		Vector3 targetdown = new Vector3(0, 1, 0);
		transform.up = Vector3.Slerp(transform.up, targetdown, Time.deltaTime * damping);
		jumpdir = 1;
	}

	/// <summary>
	/// OnGUI: This is the in-game GUI. Creates buttons to allow player to return to main menu, restart level or 
	/// toggle sound on and off.
	/// </summary>
	void OnGUI () {
		GUI.Box(new Rect(10,10,290,50), "");
		if(GUI.Button(new Rect(20,25,80,20), "Main Menu")) {
			Application.LoadLevel(0);
		}
		if(GUI.Button(new Rect(110,25,80,20), "Restart")) {
			Application.LoadLevel(Application.loadedLevel);
		}
		if (AudioListener.volume == 1) {
			if (GUI.Button (new Rect (200, 25, 80, 20), "Sound Off")) {
				AudioListener.volume = 0;
			}
		}
		if (AudioListener.volume == 0) {
			if (GUI.Button (new Rect (200, 25, 80, 20), "Sound On")) {
				AudioListener.volume = 1;
			}
		}
	}

	// Update is called once per frame
	/// <summary>
	/// Fixed Update moves character and sets animations states according to idling and walking. Also flips character
	/// </summary>
	void FixedUpdate () {
				if (!dead) {
						grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
						anim.SetBool ("Ground", grounded);

						float move = Input.GetAxis ("Horizontal");
						float movev = Input.GetAxis ("Vertical");

						if (jumpdir == 0 || jumpdir == 1) {
								anim.SetFloat ("Speed", Mathf.Abs (move));
								anim.SetFloat ("Vspeed", Mathf.Abs (0));
								GetComponent<Rigidbody2D>().velocity = new Vector2 (move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
						} else if (jumpdir == 2 || jumpdir == 3) {
								anim.SetFloat ("Speed", Mathf.Abs (0));
								anim.SetFloat ("Vspeed", Mathf.Abs (movev));
								GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, movev * maxSpeed);
						}

						if (move > 0 && !facingRight || (jumpdir == 2 && movev > 0 && facingRight) || (jumpdir == 3 && movev > 0 && !facingRight))
								Flip ();
						else if (move < 0 && facingRight || (jumpdir == 2 && movev < 0 && !facingRight) || (jumpdir == 3 && movev < 0 && facingRight))
								Flip ();

				}
		}
	 /// <summary>
	 /// Update changes gravity according to key presses. In charge of flipping characer according to gravity.
	 /// </summary>
	void Update(){
		if (!dead) {
						if (Input.GetKeyDown (KeyCode.I)) {
								if (grounded == true) {
										Physics2D.gravity = new Vector2 (0f, 9.81f);
										Vector3 targetUp = new Vector3 (0, -1, 0);
										transform.up = Vector3.Slerp (transform.up, targetUp, Time.deltaTime * damping);
										jumpdir = 0;
								}
						}
						if (Input.GetKeyDown (KeyCode.K)) {
								if (grounded == true) {
										Physics2D.gravity = new Vector2 (0f, -9.81f);
										Vector3 targetdown = new Vector3 (0, 1, 0);
										transform.up = Vector3.Slerp (transform.up, targetdown, Time.deltaTime * damping);
										jumpdir = 1;
								}
						}
						if (Input.GetKeyDown (KeyCode.J)) {
								if (grounded == true) {
										Physics2D.gravity = new Vector2 (-9.81f, 0f);
										Vector3 targetleft = new Vector3 (1, 0, 0);
										transform.up = Vector3.Slerp (transform.up, targetleft, Time.deltaTime * damping);
										jumpdir = 2;
								}
						}
						if (Input.GetKeyDown (KeyCode.L)) {
								if (grounded == true) {
										Physics2D.gravity = new Vector2 (9.81f, 0f);
										Vector3 targetright = new Vector3 (-1, 0, 0);
										transform.up = Vector3.Slerp (transform.up, targetright, Time.deltaTime * damping);
										jumpdir = 3;
								}
						}
				}
	}
	 /// <summary>
	 /// Flip: Changes which direction the character is facing
	 /// </summary>
	void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	 /// <summary>
	 /// OnCollitionEnter2D: detects collisions when a player hits spikes or falls of the map.
	 /// </summary>
	void OnCollisionEnter2D(Collision2D hit) {
		if (hit.gameObject.tag == "Death" || hit.gameObject.tag == "space") {
			dead = true;
			anim.SetBool ("Dead", true);
			StartCoroutine(waitForDeath());
		}
		if (hit.gameObject.tag == "Respawn") {
			dead = true;
			anim.SetBool ("Dead", true);
			StartCoroutine(waitForDeath());
		}
	}
	 /// <summary>
	 /// OnCollisionStay2D: detects when the player is crushed by a box. Calls coroutine that allows death
	/// animation to finish befor reloading the level.
	 /// </summary>
	void OnCollisionStay2D(Collision2D hit) {
		if (hit.gameObject.tag == "Finish" && jumpdir == 0 && hit.contacts[0].normal == crushup && grounded == true) {
			GetComponent<AudioSource>().PlayOneShot(crush);
			dead = true;
			anim.SetBool ("Dead", true);
			Destroy (gameObject.GetComponent<BoxCollider2D>()); 
			StartCoroutine(waitForDeath());
		}  
		if (hit.gameObject.tag == "Finish" && jumpdir == 1 && hit.contacts[0].normal == crushdown && grounded == true) {
			GetComponent<AudioSource>().PlayOneShot(crush);
			dead = true;
			anim.SetBool ("Dead", true);
			Destroy (gameObject.GetComponent<BoxCollider2D>()); 
			StartCoroutine(waitForDeath());
		} 
		if (hit.gameObject.tag == "Finish" && jumpdir == 2 && hit.contacts[0].normal == crushleft && grounded == true) {
			GetComponent<AudioSource>().PlayOneShot(crush);
			dead = true;
			anim.SetBool ("Dead", true);
			Destroy (gameObject.GetComponent<BoxCollider2D>()); 
			StartCoroutine(waitForDeath());
		} 
		if (hit.gameObject.tag == "Finish" && jumpdir == 3 && hit.contacts[0].normal == crushright && grounded == true) {
			GetComponent<AudioSource>().PlayOneShot(crush);
			dead = true;
			anim.SetBool ("Dead", true);
			Destroy (gameObject.GetComponent<BoxCollider2D>()); 
			StartCoroutine(waitForDeath());
		} 
	}

	IEnumerator waitForDeath() {
		yield return new WaitForSeconds(1);
		Application.LoadLevel (Application.loadedLevel);
	}
	
}
