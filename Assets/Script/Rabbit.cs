using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rabbit : MonoBehaviour
{
    enum state
    {
        none,
        wait,
        up,
        play,
        down,
        hit,
        end,
    }

    bool isShow = false;
    state currentState = state.none;
    int currentRabbitIndex = 0;
    float startPosY = 0;
    bool isPlay = false;

    // Update is called once per frame
    void Update()
    {
        if(isShow)
        {
            float speed = GameDataManager.rabbitSpeed;

            switch (currentState)
            {
                case state.up:
                    {
                        float nextYPos = transform.position.y + speed * Time.deltaTime;
                        transform.position = new Vector3(transform.position.x, nextYPos);
                        if (nextYPos - startPosY > 100)
                        {
                            SoundManager.instance.PlaySound(SoundType.effect, "game_rabbit_1");
                            currentState = state.play;
                        }
                    }
                    break;
                case state.play:
                    {
                        if (isPlay == false)
                            StartCoroutine(DelayRabbitPlay());

                        isPlay = true;
                    }
                    break;
                case state.down:
                    {
                        float nextYPos = transform.position.y - speed * Time.deltaTime;
                        transform.position = new Vector3(transform.position.x, nextYPos);
                        if (nextYPos - startPosY < -50)
                            currentState = state.end;
                    }
                    break;
                case state.hit:
                    {
                        SetImage(false);
                        GameManager.instance.catchRabbitCount++;
                        transform.Find("effect").gameObject.SetActive(true);
                            
                        if (GameManager.instance.CheckEndRound(true))
                            GameManager.instance.SuccessRound(currentRabbitIndex);

                        StartCoroutine(DelayRabbitHit());
                    }
                    break;
                case state.end:
                    {
                        currentState = state.none;
                        isShow = false;
                        transform.position = new Vector3(transform.position.x, startPosY);
                        transform.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }

    IEnumerator DelayRabbitHit()
    {
        currentState = state.none;

        yield return new WaitForSeconds(0.5f);

        transform.Find("effect").gameObject.SetActive(false);
        currentState = state.down;

    }

    IEnumerator DelayRabbitPlay()
    {
        yield return new WaitForSeconds(4f);
        currentState = state.down;
        isPlay = false;
    }

    IEnumerator DelayRabbitWait(float delayWaitTime)
    {
        yield return new WaitForSeconds(delayWaitTime);

        currentState = state.up;
    }

    public void ShowRabbit(int rabbitIndex, float delayWaitTime)
    {
        transform.gameObject.SetActive(true);
        currentRabbitIndex = rabbitIndex;
        isPlay = false;
        isShow = true;
        startPosY = transform.position.y;
        currentState = state.wait;
        SetImage();
        StartCoroutine(DelayRabbitWait(delayWaitTime));
    }

    void SetImage(bool isNormal = true)
    {
        transform.rotation = Quaternion.identity;

        int rabbitImageIndex = 0;
        int rabbitImageRotateZ = 0;

        if (isNormal)
        {
            rabbitImageIndex = GameDataManager.rabbitNormalImageIndex[currentRabbitIndex];
            rabbitImageRotateZ = GameDataManager.rabbitNormalImageRotateZ[currentRabbitIndex];
        }
        else
        {
            rabbitImageIndex = GameDataManager.rabbitHitImageIndex[currentRabbitIndex];
            rabbitImageRotateZ = GameDataManager.rabbitHitImageRotateZ[currentRabbitIndex];
        }

        transform.GetComponent<Image>().sprite = (Sprite)GameDataManager.instance.rabbitSprites[rabbitImageIndex + 1];
        transform.Rotate(new Vector3(0, 0, rabbitImageRotateZ));
    }

    public void OnClickRabbitHit()
    {
        GameManager.instance.OnClickKidMove();

        if (currentState == state.play)
        {
            SoundManager.instance.PlaySound(SoundType.click, "game_rabbit_3");
            currentState = state.hit;
        }
    }

    public void ResetRabbit()
    {
        transform.gameObject.SetActive(false);

        StopAllCoroutines();

        if (currentState != state.none)
            transform.position = new Vector3(transform.position.x, startPosY);

        isPlay = false;
        isShow = false;
        currentState = state.none;
    }
}
