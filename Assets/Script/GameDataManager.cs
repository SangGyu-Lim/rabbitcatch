using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance = null;

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

        SetStringData();
        SetResources();
    }
    void OnDestroy() { instance = null; }

    StringData stringData;
    public StringData getStringData
    {
        get { return stringData; }
    }

    public Object[] rabbitSprites = null;

    [System.Serializable]
    public class StringData
    {
        public int[] opVal;
        public int[] edVal;
        public string opText;
        public string edText;
    }

    public static float kidSpeed = 500;
    public static float rabbitSpeed = 300;
    public static int[] maxCountPerRound = { 3, 3, 4, 5, 5, 6, 7, 7, 8, 9 };
    public static string[] rabbitList = { "yellowGreen", "orange", "blue", "navy", "purple", "yellow", "skyBlue", "green", "pink", "red" };
    public static int[] rabbitNormalImageIndex = { 15, 17, 18, 21, 26, 19, 10, 28, 29, 24 };
    public static int[] rabbitNormalImageRotateZ = { 0, 90, 90, 90, 90, 0, 0, 90, 90, 0 };
    public static int[] rabbitHitImageIndex = { 14, 16, 12, 20, 25, 13, 9, 22, 27, 23 };
    public static int[] rabbitHitImageRotateZ = { 0, 90, 90, 90, 90, 0, 0, 90, 90, 0 };
    public static int[] rabbitEndImageIndex = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 11 };
    public static int[] rabbitEndImageRotateZ = { 0, 90, 0, 90, 90, 0, 90, 90, 0, 90 };

    public void SetStringData()
    {
        if (stringData == null)
        {
            TextAsset textData = Resources.Load("etc/strings") as TextAsset;

            stringData = JsonUtility.FromJson<StringData>(textData.ToString());

        }
    }

    public void SetResources()
    {
        rabbitSprites = Resources.LoadAll("Image/data_item");

    }

}
