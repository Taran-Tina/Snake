using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControll : MonoBehaviour
{
    public enum BlockType {Score, Enemy, Teeth, Confuse, Shield, Empty, Victory};
    public BlockType blockType;
    public int Score;

    void Start()
    {        
        SetScore();
    }
    //при столкновении с зоной уничтожени€
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "DeletePoint")
        {
            //удал€ю родительский объект
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    void SetScore()
    {
        int level = GameObject.Find("GameManager").GetComponent<GameUI>().GetLevel();
        if (blockType == BlockType.Score)
        {
            Score = Random.Range(1, 4) * level;
            ShowScore();
        }
        if (blockType == BlockType.Enemy)
        {
            Score = Random.Range(4, 12) * level;
            ShowScore();
        }
        if (blockType == BlockType.Victory)
        {
            Score = 5 * level;
            ShowScore();
        }
    }

    void ShowScore()
    {
        GameObject cgo = new GameObject();
        cgo.AddComponent<Canvas>();        
        cgo.transform.parent = gameObject.transform;
        cgo.name = "Canvas";        
        Canvas cnvs = cgo.GetComponent<Canvas>();        
        cnvs.renderMode = RenderMode.WorldSpace;
        cnvs.worldCamera = Camera.main;
        cnvs.GetComponent<RectTransform>().sizeDelta = new Vector2(1,1);
        cnvs.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);

        GameObject tgo = new GameObject();
        tgo.AddComponent<TMPro.TextMeshPro>();
        tgo.transform.parent = cgo.transform;
        tgo.name = "FaceText";
        TMPro.TextMeshPro text = tgo.GetComponent<TMPro.TextMeshPro>();
        text.GetComponent<RectTransform>().localPosition = new Vector3(0f,0f,-0.5f);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(100,100);
        text.GetComponent<RectTransform>().localScale = new Vector3(0.1f,0.1f,0.1f);
        text.text = Score.ToString();
        text.enableAutoSizing = true;
        text.fontStyle = TMPro.FontStyles.Bold;
        text.fontSizeMin = 32;
        text.fontSizeMax = 64;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.alignment = TMPro.TextAlignmentOptions.Midline;
    }
}
