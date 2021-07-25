using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum kidMove
    {
        none,
        down,
        up,
        hide,
    }

    public static GameManager instance = null;

    Transform canvasTrans = null;
    bool isKidMove = false;
    Transform kidTrans = null;
    kidMove currentKidState = kidMove.none;
    bool isPlayGame = false;
    int currentRound = 0;
    bool isShowRabbit = false;
    List<int> remainRabbit = null;
    System.Random rand = new System.Random();
    DateTime lastchecktime = DateTime.Now;
    public int catchRabbitCount = 0;
    bool isMove = false;
    bool isScaleDown = false;
    Transform targerSquareTrans = null;
    string endRound = "";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy() { instance = null; }

    // Start is called before the first frame update
    void Start()
    {
        canvasTrans = Camera.main.transform.Find("Canvas");
        remainRabbit = new List<int>();

        StartTile();
    }

    // Update is called once per frame
    void Update()
    {
        if(isKidMove && kidTrans != null)
            KidMove();

        if (isPlayGame && isShowRabbit == false)
            ShowRabbit();

        if (isShowRabbit && DateTime.Now > lastchecktime)
        {
            if (CheckEndRound(false))
                isShowRabbit = false;

            lastchecktime = DateTime.Now + TimeSpan.FromSeconds(2);
        }

        if(isMove)
        {
            int tempSpeed = 500;
            Vector3 rabbitPos = canvasTrans.Find("game/catchRabbit/rabbit/" + endRound).position;

            canvasTrans.Find("game/catchRabbit/rabbit/" + endRound).position 
                = Vector3.MoveTowards(rabbitPos, targerSquareTrans.position, tempSpeed * Time.deltaTime);

            rabbitPos = canvasTrans.Find("game/catchRabbit/rabbit/" + endRound).position;
            if (rabbitPos.x == targerSquareTrans.position.x && rabbitPos.y == targerSquareTrans.position.y)
                isMove = false;
        }

        if (isScaleDown)
        {
            int tempSpeed = 500;
            Vector2 rabbitSize = canvasTrans.Find("game/catchRabbit/rabbit/" + endRound).GetComponent<RectTransform>().sizeDelta;
            Vector2 targerSquareSize = targerSquareTrans.GetComponent<RectTransform>().sizeDelta;

            float nextMinusXSize = 0;
            float nextMinusYSize = 0;
            if (rabbitSize.x > targerSquareSize.x)
                nextMinusXSize = tempSpeed * Time.deltaTime;

            if (rabbitSize.y > targerSquareSize.y)
                nextMinusYSize = tempSpeed * Time.deltaTime;

            canvasTrans.Find("game/catchRabbit/rabbit/" + endRound).GetComponent<RectTransform>().sizeDelta 
                = new Vector2(rabbitSize.x - nextMinusXSize, rabbitSize.y - nextMinusYSize);

            rabbitSize = canvasTrans.Find("game/catchRabbit/rabbit/" + endRound).GetComponent<RectTransform>().sizeDelta;
            if (rabbitSize.x < targerSquareSize.x && rabbitSize.y < targerSquareSize.x)
                isScaleDown = false;
        }
    }

    void StartTile()
    {
        canvasTrans.Find("title").gameObject.SetActive(true);
    }

    bool isOnClickStartOpening = false;
    public void OnClickStartOpening()
    {
        if (isOnClickStartOpening)
            return;

        isOnClickStartOpening = true;
        SoundManager.instance.PlaySound(SoundType.voice, "title");

        StartCoroutine(DelayStartOpening(1.7f));
    }

    IEnumerator DelayStartOpening(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        canvasTrans.Find("title").gameObject.SetActive(false);
        canvasTrans.Find("opening").gameObject.SetActive(true);
        canvasTrans.Find("opening/Text").GetComponent<Text>().text = GameDataManager.instance.getStringData.opText;
        SoundManager.instance.PlaySound(SoundType.voice, "narr_op");
    }

    public void OnClickOpeningSkip()
    {
        // 사운드 꺼주고
        canvasTrans.Find("opening").gameObject.SetActive(false);

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        canvasTrans.Find("game").gameObject.SetActive(true);
        currentRound = 0;
        remainRabbit.Clear();
        for (int i = 0; i < GameDataManager.rabbitList.Length; ++i)
            remainRabbit.Add(i);

        SoundManager.instance.PlaySound(SoundType.voice, "voice");

        canvasTrans.Find("game/countText").gameObject.SetActive(true);
        canvasTrans.Find("game/countText").GetComponent<Text>().text = "3";
        canvasTrans.Find("game/countText").GetComponent<Text>().color = Color.red;

        yield return new WaitForSeconds(1f);
        canvasTrans.Find("game/countText").GetComponent<Text>().text = "2";
        canvasTrans.Find("game/countText").GetComponent<Text>().color = Color.yellow;

        yield return new WaitForSeconds(1f);
        canvasTrans.Find("game/countText").GetComponent<Text>().text = "1";
        canvasTrans.Find("game/countText").GetComponent<Text>().color = Color.blue;

        yield return new WaitForSeconds(1f);
        canvasTrans.Find("game/countText").GetComponent<Text>().text = "start!";
        canvasTrans.Find("game/countText").GetComponent<Text>().color = Color.green;
        isPlayGame = true;

        yield return new WaitForSeconds(0.5f);
        canvasTrans.Find("game/countText").gameObject.SetActive(false);
    }

    public void OnClickKidMove()
    {
        if (isPlayGame == false)
            return;

        Vector3 mousePos = Input.mousePosition;

        //kidTrans = canvasTrans.Find("game/character").GetComponent<RectTransform>();
        kidTrans = canvasTrans.Find("game/character").transform;
        StopCoroutine(EndKidMove());
        kidTrans.gameObject.SetActive(true);
        kidTrans.position = new Vector3(mousePos.x + 120, mousePos.y + 40);
        kidTrans.rotation = Quaternion.identity;
        currentKidState = kidMove.down;
        isKidMove = true;
        SoundManager.instance.PlaySound(SoundType.click, "game_rabbit_4");

    }

    void KidMove()
    {
        if (isPlayGame == false)
            return;

        float speed = GameDataManager.kidSpeed;
        switch (currentKidState)
        {
            case kidMove.down:
                {
                    float nextZPos = kidTrans.localRotation.z + speed * Time.deltaTime;
                    kidTrans.Rotate(new Vector3(0, 0, nextZPos));
                    if (kidTrans.localEulerAngles.z > 50)
                        currentKidState = kidMove.up;
                }
                break;
            case kidMove.up:
                {
                    float nextZPos = kidTrans.rotation.z - speed * Time.deltaTime;
                    kidTrans.Rotate(new Vector3(0, 0, nextZPos));
                    if (kidTrans.localEulerAngles.z > 300)
                        currentKidState = kidMove.hide;
                }
                break;
            case kidMove.hide:
                {
                    currentKidState = kidMove.none;
                    isKidMove = false;

                    StartCoroutine(EndKidMove());
                }
                break;
        }
    }

    IEnumerator EndKidMove()
    {
        yield return new WaitForSeconds(1f);

        kidTrans.gameObject.SetActive(false);
    }

    void ShowRabbit()
    {
        isShowRabbit = true;
        catchRabbitCount = 0;

        if (currentRound >= GameDataManager.maxCountPerRound.Length)
        {
            isPlayGame = false;
            StartEnding();
            return;
        }

        int maxCount = GameDataManager.maxCountPerRound[currentRound];
        int randomIndex = rand.Next(0, remainRabbit.Count);

        List<int> nextPos = new List<int>();
        while(true)
        {
            int randomPos = rand.Next(0, 9);
            if (nextPos.Contains(randomPos))
                continue;

            nextPos.Add(randomPos);
            if (nextPos.Count == maxCount)
                break;
        }

        for(int i = 0; i < nextPos.Count; ++i)
        {
            int pos = nextPos[i];
            int randomWaitTime = rand.Next(0, 11);

            canvasTrans.Find("game/rabbit" + pos.ToString()).gameObject.SetActive(true);
            canvasTrans.Find("game/rabbit" + pos.ToString()).GetComponent<Rabbit>().ShowRabbit(remainRabbit[randomIndex], (float)randomWaitTime / 2.0f);
        }
    }

    public bool CheckEndRound(bool isAllCatchRabbit)
    {
        if(isAllCatchRabbit)
        {
            if (catchRabbitCount == GameDataManager.maxCountPerRound[currentRound])
                return true;

            return false;
        }
        else
        {
            for (int i = 0; i < 9; ++i)
            {
                if (canvasTrans.Find("game/rabbit" + i.ToString()).gameObject.activeSelf)
                    return false;
            }

            catchRabbitCount = 0;
            return true;
        }
    }

    public void SuccessRound(int rabbitIndex)
    {
        endRound = currentRound.ToString();
        targerSquareTrans = canvasTrans.Find("game/catchRabbit/square/" + endRound);
        Transform rabbitTrans = canvasTrans.Find("game/catchRabbit/rabbit/" + endRound);

        rabbitTrans.gameObject.SetActive(true);
        rabbitTrans.rotation = Quaternion.identity;

        canvasTrans.Find("ending/rabbit/" + endRound).rotation = Quaternion.identity;

        int rabbitImageIndex = GameDataManager.rabbitEndImageIndex[rabbitIndex];
        int rabbitImageRotateZ = GameDataManager.rabbitEndImageRotateZ[rabbitIndex];

        rabbitTrans.GetComponent<Image>().sprite = (Sprite)GameDataManager.instance.rabbitSprites[rabbitImageIndex + 1];
        rabbitTrans.Rotate(new Vector3(0, 0, rabbitImageRotateZ));

        canvasTrans.Find("ending/rabbit/" + endRound).GetComponent<Image>().sprite = (Sprite)GameDataManager.instance.rabbitSprites[rabbitImageIndex + 1];
        canvasTrans.Find("ending/rabbit/" + endRound).Rotate(new Vector3(0, 0, rabbitImageRotateZ));

        remainRabbit.Remove(rabbitIndex);
        ++currentRound;

        SoundManager.instance.PlaySound(SoundType.voice, GameDataManager.rabbitList[rabbitIndex]);

        StartCoroutine(GetRabbit());
    }

    IEnumerator GetRabbit()
    {
        yield return new WaitForSeconds(1f);

        isMove = true;
        isScaleDown = true;

    }

    void StartEnding()
    {
        // 사운드 켜줘야됨
        canvasTrans.Find("game").gameObject.SetActive(false);
        canvasTrans.Find("ending").gameObject.SetActive(true);
        canvasTrans.Find("ending/Text").GetComponent<Text>().text = GameDataManager.instance.getStringData.edText;

        SoundManager.instance.PlaySound(SoundType.voice, "narr_ed");
    }

    public void OnClickResetGame()
    {
        isOnClickStartOpening = false;
        isPlayGame = false;
        StopAllCoroutines();
        canvasTrans.Find("opening").gameObject.SetActive(false);
        canvasTrans.Find("game").gameObject.SetActive(false);
        canvasTrans.Find("ending").gameObject.SetActive(false);

        for(int i = 0; i < 9; ++i)
            canvasTrans.Find("game/rabbit" + i.ToString()).GetComponent<Rabbit>().ResetRabbit();

        for (int i = 0; i < GameDataManager.rabbitList.Length; ++i)
        {
            canvasTrans.Find("game/catchRabbit/rabbit/" + i.ToString()).gameObject.SetActive(false);
            canvasTrans.Find("game/catchRabbit/rabbit/" + i.ToString()).GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0);
            canvasTrans.Find("game/catchRabbit/rabbit/" + i.ToString()).GetComponent<RectTransform>().sizeDelta = new Vector2(413, 369);
        }

        StartTile();
    }

    public void GameQuit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
