using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;      
    public int pointsPerFood = 10;           
    public int pointsPerSoda = 5;           
    public int wallDamage = 1;         
    public Text foodText;
    public Text sodaText;
    public AudioClip moveSound1;            
    public AudioClip moveSound2;           
    public AudioClip eatSound1;            
    public AudioClip eatSound2;          
    public AudioClip drinkSound1;       
    public AudioClip drinkSound2;         
    public AudioClip gameOverSound;       

    private Animator animator;              
    private int food;
    private int sodas;
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = gameManager.instance.playerFoodPoints;
        sodas = gameManager.instance.playerSodas;
        foodText.text = "Food: " + food;
        sodaText.text = "Soda: " + sodas;
        base.Start();
    }
    private void OnDisable()
    {
        gameManager.instance.playerFoodPoints = food;
        gameManager.instance.playerSodas = sodas;
    }
    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            gameManager.instance.GameOver();
        }
    }

    private void Update()
    {
        if (!gameManager.instance.playersTurn)
        {
            return;
        }
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
        bool drinkSoda;
        drinkSoda = Input.GetKeyDown("x");
        if (drinkSoda && sodas>0)
        {
            sodas--;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            sodaText.text = "Soda : " + sodas;
        }
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if(food > 25) {
            food = food - 2;
        }
        else {
            food--;
        }
        foodText.text = "Food: " + food;
        sodaText.text = "Sodas: " + sodas;

        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        CheckIfGameOver();
        gameManager.instance.playersTurn = false;
    }
    
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
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
            other.gameObject.SetActive(false);
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
        }
        
        else if (other.tag == "Soda")
        {
            sodas++;
            sodaText.text = "+ 1 " + "Sodas: " + sodas;
            other.gameObject.SetActive(false);
        }
    }
    
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        sodas--;
        foodText.text = "-" + loss + " Food: " + food;
        sodaText.text = "-" + 1 + " Food: " + sodas;
        CheckIfGameOver();
    }
}