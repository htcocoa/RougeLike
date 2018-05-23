using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GM instance = null;
    public BoardManager boardScript;

    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private bool doingSetup;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private int level = 1;
    public int playerFoodPoints = 100;
	// Use this for initialization
	void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}

    public void GameOver()
    {
        levelText.text = "After " + level + "days, you starved.";
        levelImage.SetActive(true);

        enabled = false;
    }

    private void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;

        }
        StartCoroutine(MoveEnemies());

    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }
    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day" + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
	IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i =0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);

        }
        playersTurn = true;
        enemiesMoving = false;

    }



}
