using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controll : MonoBehaviour
{
    public bool canInput = true;
    private int speed = 4;
    Vector3 currentPosition;
    Vector3 targetPosition;
    bool left = true;
    bool right = true;
    bool up = true;
    bool down = true;
    int shield = 1;
    int teeth = 1;

    Camera camera;
    GameUI gui;
    Manager mngr;

    private LineRenderer lr;

    private void Start()
    {
        camera = Camera.main;
        gui = GameObject.Find("GameManager").GetComponent<GameUI>();
        mngr = GameObject.Find("GameManager").GetComponent<Manager>();
        lr = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        CheckPosition();
    }

    void Update()
    {        
        UserInput();
        CameraWork();
        LineWork();
    }

    void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && up==true && canInput==true)
        {
            canInput = false;
            mngr.PlaySound(mngr.moveSound);
            currentPosition = transform.position;
            targetPosition = currentPosition + new Vector3(0f, 1f, 0f);
            StartCoroutine(Move(currentPosition, targetPosition));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && down == true && canInput == true)
        {
            canInput = false;
            mngr.PlaySound(mngr.moveSound);
            currentPosition = transform.position;
            targetPosition = currentPosition + new Vector3(0f, -1f, 0f);
            StartCoroutine(Move(currentPosition, targetPosition));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && left == true && canInput == true)
        {
            canInput = false;
            mngr.PlaySound(mngr.moveSound);
            currentPosition = transform.position;
            targetPosition = currentPosition + new Vector3(-1f, 0f, 0f);
            StartCoroutine(Move(currentPosition, targetPosition));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && right == true && canInput == true)
        {
            canInput = false;
            mngr.PlaySound(mngr.moveSound);
            currentPosition = transform.position;
            targetPosition = currentPosition + new Vector3(1f, 0f, 0f);
            StartCoroutine(Move(currentPosition, targetPosition));
        }
    }

    IEnumerator Move(Vector3 current, Vector3 pos)
    {
        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime*speed;
            transform.position = Vector3.Lerp(current, pos, timeSinceStarted);
            if (transform.position == pos)
            {
                canInput = true;
                yield break;
            }
            yield return null;
        }
    }

    void CheckPosition()
    {
        if (transform.position.x <= -1f && transform.position.x != 0)
        {
            transform.position = new Vector3(-1f, transform.position.y, transform.position.z);
            left = false;
            right = true;
        }
        if (transform.position.x >= 1f && transform.position.x !=0)
        {
            transform.position = new Vector3(1f, transform.position.y, transform.position.z);
            left = true;
            right = false;
        }
        if (transform.position.x == 0f)
        {
            left = true;
            right = true;
        }
        if (transform.position.y <= -1f && transform.position.y != 0)
        {
            transform.position = new Vector3(transform.position.x, -1f, transform.position.z);
            down = false;
            up = true;
        }
        if (transform.position.y >= 1f && transform.position.y != 0)
        {
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            up = false;
            down = true;
        }
        if (transform.position.x == 0f)
        {
            up = true;
            down = true;
        }
    }

    void CameraWork()
    {
        camera.transform.LookAt(gameObject.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        BlockControll bc = other.GetComponent<BlockControll>();
        int level = gui.GetLevel();
        if (bc.blockType == BlockControll.BlockType.Score)
        {
            //добавл€ю очки и жизни
            int value = bc.Score;
            gui.AddScore(value);
            gui.AddLives(value * teeth);
            mngr.PlaySound(mngr.biteSound);
        }
        if (bc.blockType == BlockControll.BlockType.Enemy)
        {
            //отнимаю жизни
            int value = -1 * bc.Score * shield * teeth;
            gui.AddLives(value);
            mngr.PlaySound(mngr.painSound);
        }
        if (bc.blockType == BlockControll.BlockType.Empty)
        {
            //добавл€ю очки
            int value = 1;
            gui.AddScore(value);
        }
        if (bc.blockType == BlockControll.BlockType.Confuse)
        {
            //отмен€ю управление
            StartCoroutine(GoBonus(1));
            mngr.PlaySound(mngr.confuseSound);
        }
        if (bc.blockType == BlockControll.BlockType.Shield)
        {
            //неу€звимость
            StartCoroutine(GoBonus(2));
            mngr.PlaySound(mngr.armorSound);
        }
        if (bc.blockType == BlockControll.BlockType.Teeth)
        {
            //и еда и урон вдвойне
            StartCoroutine(GoBonus(3));
            mngr.PlaySound(mngr.haSound);
        }
        if (bc.blockType == BlockControll.BlockType.Victory)
        {
            mngr.PlaySound(mngr.winSound);
            gui.Win();
        }
    }

    private IEnumerator GoBonus(int type)
    {
        const float TIME = 5f;
        var timer = TIME;
        while (timer > 0)
        {
            if (type==1)
            {
                canInput = false;
            }
            if (type == 2)
            {
                shield = 0;
            }
            if (type == 3)
            {
                teeth = 2;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        canInput = true;
        shield = 1;
        teeth = 1;
        StopCoroutine(GoBonus(type));
    }

    void LineWork()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
            }
        }
        else
        {
            lr.SetPosition(1, transform.forward * 100);
        }
            
    }
}
