using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodtext;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;

    private int food;
    private Vector2 touchOrigin = -Vector2.one;


	// Use this for initialization
	protected override void Start () {
        animator = GetComponent<Animator>();

        food = GM.instance.playerFoodPoints;
        foodtext.text = "Food: " + food;
        base.Start();
	}

    private void OnDisable()
    {
        GM.instance.playerFoodPoints = food;
    }
    // Update is called once per frame
    void Update() {
        if (!GM.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

       
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }

        
        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    
       


    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodtext.text = "Food: " + food;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.Randomnizesfx(moveSound1, moveSound2);
        }
        CheckIfGameOver();
        GM.instance.playersTurn = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;

        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodtext.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.Randomnizesfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodtext.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.Randomnizesfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");

    }
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodtext.text = "-" +  loss +" Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GM.instance.GameOver();
        }
    }
}
