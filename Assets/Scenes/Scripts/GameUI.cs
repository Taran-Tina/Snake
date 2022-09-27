using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    GameObject gui;
    Dictionary<string, GameObject> _panels = new Dictionary<string, GameObject>();
    private int score = 0;
    private int lives = 1;
    private int level = 1;
    private int continues = 3;
    GameObject currentMenu;
    Manager mngr;
    bool gotBody = false;
    bool gotTail = false;

    private void Awake()
    {
        gui = GameObject.Find("GUI");
        _panels.Add("MainMenu", gui.transform.Find("MainMenu").gameObject);
        _panels.Add("GameHUD", gui.transform.Find("GameHUD").gameObject);
        _panels.Add("Lose", gui.transform.Find("Lose").gameObject);
        _panels.Add("Win", gui.transform.Find("Win").gameObject);
        mngr = gameObject.GetComponent<Manager>();

    }
    void Start()
    {
        ShowPanel("MainMenu");
        Button startButton = currentMenu.gameObject.transform.Find("NewGame").GetComponent<Button>();
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartNewGame);
    }

    void StartNewGame()
    {
        ShowPanel("GameHUD");
        mngr.barriers = Random.Range(10, 20);
        score = 0;
        lives = 1;
        level = 1;
        continues = 3;
        SetText(currentMenu.gameObject.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>(), "Очки: " + score);
        SetText(currentMenu.gameObject.transform.Find("Lives").GetComponent<TMPro.TextMeshProUGUI>(), "Жизни: " + lives);
        SetText(currentMenu.gameObject.transform.Find("Level").GetComponent<TMPro.TextMeshProUGUI>(), level.ToString());
        mngr.SpawnPlayer();
        mngr.Spawn();
    }

    public void Lose()
    {
        mngr.barriers = 0;
        mngr.RemovePlayer();
        mngr.StopAllCoroutines();
        foreach (Transform child in mngr.spawnPoint.transform)
        {
            Destroy(child.gameObject);
        }
        ShowPanel("Lose");
        int totalScore = score + lives;
        SetText(currentMenu.gameObject.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>(), "Набрано очков: " + totalScore + "\n Достигнут уровень: " + level);
        if (continues > 0)
        {
            currentMenu.gameObject.transform.Find("Reload").gameObject.SetActive(true);
            SetText(currentMenu.gameObject.transform.Find("Reload").transform.Find("Cont").GetComponent<TMPro.TextMeshProUGUI>(), "(осталось попыток: "+continues+")");
            Button reload = currentMenu.gameObject.transform.Find("Reload").GetComponent<Button>();
            reload.onClick.RemoveAllListeners();
            reload.onClick.AddListener(delegate
            {
                ReloadLevel(level);
            });
        }
        else
        {
            currentMenu.gameObject.transform.Find("Reload").gameObject.SetActive(false);
        }
        Button restart = currentMenu.gameObject.transform.Find("Restart").GetComponent<Button>();
        restart.onClick.RemoveAllListeners();
        restart.onClick.AddListener(StartNewGame);
    }

    public void Win()
    {
        mngr.barriers = 0;
        mngr.RemovePlayer();
        mngr.StopAllCoroutines();
        foreach (Transform child in mngr.spawnPoint.transform)
        {
            Destroy(child.gameObject);
        }
        ShowPanel("Win");
        score = score + lives;
        level++;
        SetText(currentMenu.gameObject.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>(), "Набрано очков: " + score + "\n Достигнут уровень: " + level);
        Button next = currentMenu.gameObject.transform.Find("Next").GetComponent<Button>();
        next.onClick.RemoveAllListeners();
        next.onClick.AddListener(StartNextLevel);
    }

    void StartNextLevel()
    {
        ShowPanel("GameHUD");
        mngr.barriers = Random.Range(10, 20);
        SetText(currentMenu.gameObject.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>(), "Очки: " + score);
        SetText(currentMenu.gameObject.transform.Find("Lives").GetComponent<TMPro.TextMeshProUGUI>(), "Жизни: " + lives);
        SetText(currentMenu.gameObject.transform.Find("Level").GetComponent<TMPro.TextMeshProUGUI>(), level.ToString());
        mngr.SpawnPlayer();
        gotBody = false;
        gotTail = false;
        AddLives(0);
        mngr.Spawn();
    }

    void ReloadLevel(int lvl)
    {
        continues--;
        score = 0;
        lives = 1;
        level = lvl;
        ShowPanel("GameHUD");
        SetText(currentMenu.gameObject.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>(), "Очки: " + score);
        SetText(currentMenu.gameObject.transform.Find("Lives").GetComponent<TMPro.TextMeshProUGUI>(), "Жизни: " + lives);
        SetText(currentMenu.gameObject.transform.Find("Level").GetComponent<TMPro.TextMeshProUGUI>(), level.ToString());
        mngr.SpawnPlayer();
        mngr.barriers = Random.Range(10, 20);
        mngr.Spawn();
    }

    void ShowPanel(string panelName)
    {
        foreach (KeyValuePair<string, GameObject> panel in _panels)
        {
            panel.Value.gameObject.SetActive(false);
            if (panel.Key==panelName)
            {
                panel.Value.gameObject.SetActive(true);
                currentMenu = panel.Value;
            }
        }
    }

    void SetText(TMPro.TextMeshProUGUI txt, string text)
    {
        txt.text = text;
    }

    public void AddScore(int value)
    {
        score = score + value;
        SetText(currentMenu.gameObject.transform.Find("Score").GetComponent<TMPro.TextMeshProUGUI>(), "Очки: " + score);
    }

    public void AddLives(int value)
    {
        lives = lives + value;
        if (lives>0 )
        {
            SetText(currentMenu.gameObject.transform.Find("Lives").GetComponent<TMPro.TextMeshProUGUI>(), "Жизни: " + lives);
            if (lives>=10 && gotBody == false)
            {
                //добавляю торс
                mngr.GetComponent<Manager>().SpawnPart(0);
                gotBody = true;
            }
            if (lives<10 && gotBody == true)
            {
                //удаляю торс
                mngr.GetComponent<Manager>().RemovePart(0);
                gotBody = false;
            }
            if (lives >= 20 && gotTail == false)
            {
                //добавляю хвост
                mngr.GetComponent<Manager>().SpawnPart(1);
                gotTail = true;
            }
            if (lives <20 && gotTail == true)
            {
                //удаляю хвост
                mngr.GetComponent<Manager>().RemovePart(1);
                gotTail = false;
            }
        }
        if (lives <= 0)
        {
            SetText(currentMenu.gameObject.transform.Find("Lives").GetComponent<TMPro.TextMeshProUGUI>(), "Жизни: " + 0);
            //меню перезагрузки
            Lose();
        }
    }

    public int GetLevel()
    {
        return level;
    }
}
