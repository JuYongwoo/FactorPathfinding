using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Title_GameManager : MonoBehaviour //MonoBehaviour 이므로 Scene에 있는 GameObject에 적용해야함(SceneChanger)
{
    public GameObject TitleCenterUI;

    public GameObject mRightUI;

    public GameObject mLeftUI;

    public GameObject mMainCameraObject;



    public void Start()
    {

        mMainCameraObject = UnityEngine.GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/MainCamera"));
        mLeftUI = Instantiate(Resources.Load<GameObject>("Prefabs/TitleLeftUI"));

        mLeftUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "맵 크기";
        mLeftUI.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "최대 아이템 개수";
        mLeftUI.transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text = "게임오버 범위";
        mLeftUI.transform.GetChild(3).transform.GetChild(1).GetComponent<Text>().text = "폴더 명";


        //Main_GameManager.mInstance.mGameState = GameState.MainMenu;

        //////////////////////코드로 UI를 만든다.
        mRightUI = UnityEngine.GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/TitleRightUI"));


        ////
        ///

        refreshDropdown(mRightUI.transform.GetChild(0).gameObject);

        addDropDown(mRightUI.transform.GetChild(0).gameObject, "RandomFPSMap");
        ////


        TitleCenterUI = Instantiate(Resources.Load<GameObject>("Prefabs/TitleCenterUI"));
        TitleCenterUI.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        { //리스너에 아래 행동 적용 //////////////////////////////시작 버튼
          //InGame_GameManager.mSelectedMap = mMapSelectDropDownObject.GetComponent<Dropdown>().options[mMapSelectDropDownObject.GetComponent<Dropdown>().value].text;

            if(mLeftUI.transform.GetChild(1).transform.GetChild(2).GetComponent<Text>().text != "") StaticVariables.sItemMaxCount = int.Parse(mLeftUI.transform.GetChild(1).transform.GetChild(2).GetComponent<Text>().text);
            if(mLeftUI.transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text != "") StaticVariables.sGameOverDistance = int.Parse(mLeftUI.transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text);
            if(mLeftUI.transform.GetChild(3).transform.GetChild(2).GetComponent<Text>().text != "") StaticVariables.sTag = mLeftUI.transform.GetChild(3).transform.GetChild(2).GetComponent<Text>().text;
            StaticVariables.sIsMapStick = mLeftUI.transform.GetChild(4).GetComponent<Toggle>().isOn;
            if (mLeftUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text != "") // Mapsize를 작성
            {
                if(mLeftUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text != "")
                    StaticVariables.sMapSize = int.Parse(mLeftUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text);
                InGame_GameManager.mOriginalMap = new MapGenerator().makeNewMap(StaticVariables.sMapSize);
                InGame_GameManager.mSelectedMap = new MapGenerator().makeItems(InGame_GameManager.mOriginalMap);

            }
            else
            { //MapSize를 작성X
                InGame_GameManager.mOriginalMap = CSVLoader.LoadGrid(Application.streamingAssetsPath + "/Maps/" + mRightUI.transform.GetChild(0).gameObject.GetComponent<Dropdown>().options[mRightUI.transform.GetChild(0).gameObject.GetComponent<Dropdown>().value].text);
                InGame_GameManager.mSelectedMap = new MapGenerator().makeItems(InGame_GameManager.mOriginalMap);
            }
            SceneManager.LoadScene("InGame");  // 씬 이름을 이용하여 씬 로드

        }); //리스너 생성 & 콜백 등록

        /*
        mMakeMapButton.GetComponent<Button>().onClick.AddListener(() =>
        { //리스너에 아래 행동 적용 //////////////////////////////시작 버튼

            Statics.sCSVGenerator = new CSVGeneratorforMaze();
            InGame_GameManager.mSelectedMap = Statics.sCSVGenerator.Start();//맵만들고 저장
            //목록 새로고침
            //AssetDatabase.Refresh();
            mMapSelectDropDownObject.GetComponent<Dropdown>().options.Clear();
            var maps2 = Resources.LoadAll("Maps");
            foreach (var map in maps2) // 로드된 각 파일의 이름을 드롭다운 옵션으로 추가
            {
                mMapSelectDropDownObject.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(map.name));
            }
            mMapSelectDropDownObject.GetComponent<Dropdown>().RefreshShownValue();

        }); //리스너 생성 & 콜백 등록
        */
    }

    void refreshDropdown(GameObject pGameObject)
    {
        pGameObject.GetComponent<Dropdown>().options.Clear();

        string directoryPath = Application.streamingAssetsPath + "/Maps"; // /StreamingAssets/Maps를 가리키도록 한다.
        List<string> fileList = new List<string>();
        if (Directory.Exists(directoryPath))
        {
            string[] files = Directory.GetFiles(directoryPath);

            foreach (string file in files)
            {
                pGameObject.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(Path.GetFileName(file)));
            }
        }
    }
    void addDropDown(GameObject pGameObject, string pString)
    {
        mRightUI.transform.GetChild(0).GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(pString));
    }
}
