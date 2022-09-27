using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager:MonoBehaviour
{
    public GameObject player;
    private Controll playerControll;
    private int spawnSpeed = 5;
    public int barriers = 10;
    public GameObject spawnPoint;
    private GameObject deletePoint;
    [SerializeField]
    private Object[] blocks;
    public AudioClip moveSound;
    public AudioClip biteSound;
    public AudioClip painSound;
    public AudioClip confuseSound;
    public AudioClip armorSound;
    public AudioClip haSound;
    public AudioClip winSound;

 
    void Awake()
    {
        AddBlocksToArray();
        MakePoints();
    }

    public void SpawnPlayer()
    {
        player = Resources.Load("Player/Player") as GameObject;
        Vector3 pos = new Vector3(0f, 0f, 0f);
        Quaternion rot = Quaternion.identity;
        player = Instantiate(player, pos, rot);
        player.name = "Player";
        playerControll = player.GetComponent<Controll>();
        playerControll.canInput = true;
    }

    public void RemovePlayer()
    {
        Destroy(GameObject.Find("Player"));
    }

    void MakePoints()
    {
        if (GameObject.Find("Spawner") == null)
        {
            GameObject spawn = new GameObject();
            spawn.transform.position = new Vector3(0f, 0f, 25f);
            spawn.name = "Spawner";
            spawnPoint = spawn;
        }
        if (GameObject.Find("Deleter") == null)
        {
            GameObject deleter = new GameObject();
            deleter.transform.position = new Vector3(0f, 0f, -6f);
            deleter.name = "DeletePoint";
            deletePoint = deleter;
            deletePoint.AddComponent<BoxCollider>();
            BoxCollider delCollider = deletePoint.GetComponent<BoxCollider>();
            delCollider.isTrigger = true;
            delCollider.size = new Vector3(3f, 3f, 1f);
        }
    }
    void AddBlocksToArray()
    {
        blocks = Resources.LoadAll("Blocks", typeof(GameObject));
    }

    public void Spawn()
    {        
        if (barriers > 0)
        {
            StartCoroutine(Timer());
        }
        if (barriers==0)
        {
            CreateFinalBarrier();
        }
    }

    IEnumerator Timer()
    {
        if (GameObject.Find("Spawner")!=null)
        {
            yield return new WaitForSeconds(spawnSpeed);
            CreateBarrier();
        }
        else
        {
            yield return null;
        }
    }

    void CreateBarrier()
    {       
        //создаю барьер
        GameObject barrier = new GameObject();
        barrier.transform.position = spawnPoint.transform.position;
        barrier.name = "Barrier_"+barriers;
        barrier.transform.parent = spawnPoint.transform;
        //наполн€ю барьер
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int chance = Random.Range(1,100);
                int randomBlockId = 0;
                if (chance > 0 && chance <= 5)
                {
                    randomBlockId = 0;
                }
                else if (chance > 6 && chance <= 10)
                {
                    randomBlockId = 4;
                }
                else if (chance > 11 && chance <= 15)
                {
                    randomBlockId = 5;
                }
                else if (chance > 16&& chance <= 20)
                {
                    randomBlockId = 1;
                }
                else if (chance > 21 && chance <= 40)
                {
                    randomBlockId = 3;
                }
                else if (chance > 41 && chance <= 100)
                {
                    randomBlockId = 2;
                }
                GameObject temp = Instantiate(blocks[randomBlockId]) as GameObject;
                temp.name = "Block_(" + i + "," + j + ")";
                temp.transform.position = barrier.transform.position;
                temp.transform.parent = barrier.transform;
                temp.transform.position += new Vector3(i,j,0);
            }
        }
        //добавл€ю скрипт движени€ барьера
        barrier.AddComponent<BarrierMove>();
        //уменьшаю общее количество барьеров
        barriers--;
        //останавливаю текущий счетчик
        StopCoroutine(Timer());
        //создаю по новой
        Spawn(); 
    }

    void CreateFinalBarrier()
    {
        //создаю барьер
        GameObject barrier = new GameObject();
        barrier.transform.position = spawnPoint.transform.position + new Vector3(0f, 0f, 45f);
        barrier.name = "Final";
        barrier.transform.parent = spawnPoint.transform;
        //наполн€ю барьер
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {                
                GameObject temp = Instantiate(blocks[6]) as GameObject;
                temp.name = "Block_(" + i + "," + j + ")";
                temp.transform.position = barrier.transform.position;
                temp.transform.parent = barrier.transform;
                temp.transform.position += new Vector3(i, j, 0);
            }
        }
        //добавл€ю скрипт движени€ барьера
        barrier.AddComponent<BarrierMove>();
        //останавливаю текущий счетчик
        StopCoroutine(Timer());
    }

    public void SpawnPart(int number)
    {
        //body
        if (number==0)
        {
            GameObject body = Resources.Load("Player/Body") as GameObject;
            Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z-0.9f);
            Quaternion rot = Quaternion.identity;
            body = Instantiate(body, pos, rot);
            body.name = "Body";
            body.transform.parent = player.transform;
        }
        //body
        if (number == 1)
        {
            GameObject tail = Resources.Load("Player/Tail") as GameObject;
            Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 1.55f);
            Quaternion rot = Quaternion.identity;
            tail = Instantiate(tail, pos, rot);
            tail.name = "Tail";
            tail.transform.parent = player.transform;
        }
    }

    public void RemovePart(int number)
    {
        //body
        if (number==0)
        {
            Destroy(player.transform.Find("Body").gameObject);
        }
        //tail
        if (number == 1)
        {
            Destroy(player.transform.Find("Tail").gameObject);
        }
    }

    public AudioSource PlaySound(AudioClip clip)
    {
        GameObject go = new GameObject("Sound");
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        GameObject.Destroy(go, clip.length);
        return source;
    }
}
