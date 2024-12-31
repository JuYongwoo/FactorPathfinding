using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class InGame_GameManager : MonoBehaviour
{
    static public int[,] mOriginalMap;
    static public int[,] mSelectedMap;
    static public bool mIsLoading = true;

    /// /////////////
    static public MapData mMap;
    /// /////////////////

    //프리팹으로 소환되는 UI

    public JVector2IntXY mJustForCheckPlayer;
    public JVector2IntXY mJustForCheckEnemy;

    JStopWatch mStopWatch = new JStopWatch();

    private GameObject mLeftUI;
    private GameObject mRightUI;
    private GameObject mCamera;



    public void Start()
    {
        //혹시 모르기 때문에 에셋 데이터베이스에 임포트하고 진행
        //AssetDatabase.ImportAsset("Assets/Resources/Maps/" + InGame_GameManager.mSelectedMap + ".csv", ImportAssetOptions.Default);

        mMap = new MapData(mSelectedMap); //읽은 것을 토대로 맵 만들기
                                       //Main_GameManager.mInstance.mGameState = GameState.Stage;

        initializeUI();
        if (mIsLoading)
        {
            mIsLoading = false;
        }

        mStopWatch.start(); //카운트 다운 시작

    }

    // Update is called once per frame
    public void Update()
    {
        if (mIsLoading) return; //맵 만드는 것 기다리기

        /*
        mGV.mGamePlayTime += Time.deltaTime;
        mGV.mLoopTime += Time.deltaTime;
        setGameTimeText(mGV.mGamePlayTime); //시간 최신화
        //if (mGV.mLoopTime > 0.05f) //테스트
        if (mGV.mLoopTime > 10000000000000.05f)
        {
            mGV.mLoopTime = 0.0f;
        }
        */




        /////////////////////////테스트 용
        ///
        //if (Statics.sCSVGenerator.GetType() == typeof(CSVGeneratorforFPSMapTo10)) //현재 제너레이터 형태가 테스트용 To10형태면
        if (mMap.mPlayers.Count >= 2) //현재 플레이어가 2이상이면
        {

            if ((Math.Abs(mMap.mPlayers[0].mNodePositionXY.y - mMap.mPlayers[1].mNodePositionXY.y) < StaticVariables.sGameOverDistance
                && Math.Abs(mMap.mPlayers[0].mNodePositionXY.x - mMap.mPlayers[1].mNodePositionXY.x) < StaticVariables.sGameOverDistance) || mMap.mItems.Count <= 0) // TODO 테스트용  || mMap.mPlayers[0].mGotItems ==6 부분 지울 것/////////////////////////////
            {
                RecordSaver.WriteToCSV(mMap.mPlayers[0].mGotItemsCount, InGame_GameManager.mMap.mStartTime + "_GotItemsDuringPlayTimePlayer" + mMap.mPlayers[0].mPlayerID); //생존 동안 먹은 아이템 개수 CSV에 기록
                RecordSaver.WriteToCSV(mMap.mPlayers[0].mCalculateTimes, InGame_GameManager.mMap.mStartTime + "_Player");
                mMap.mPlayers[0].mCalculateTimes.Clear();
                reGame();
                if (StaticVariables.sReGameCount > StaticVariables.sReGameMaxCount) closeGame();
                StaticVariables.sReGameCount++;

            }
        }
        ////////////////////경로 계산알고리즘
        ///

        for(int i = 0; i < mMap.mPlayers.Count; i++)
        {
            if(mMap.mPlayers[i].mUsingPathAlgorithm != null)
            {
                mMap.mPlayers[i].Update();
            }

        }




        ////////////////////// 플레이어 이동

    }
    public void closeGame()
    {
        Application.Quit();

    }
    public void exitGame()
    {


        MonoBehaviour.Destroy(mLeftUI);
        MonoBehaviour.Destroy(mRightUI);
        MonoBehaviour.Destroy(mCamera);

        for (int i = 0; i < mMap.mMapYsize; i++)
        {
            for (int j = 0; j < mMap.mMapXsize; j++)
            {
                MonoBehaviour.Destroy(mMap.mGrids[i, j].mObject);
            }
        }

        for (int i = 0; i < mMap.mPlayers.Count; i++) 
        {
            Destroy(mMap.mPlayers[i].mObject); //모든 오브젝트 전부 삭제(먹은 것도 inactive라도 메모리 누수 방지, 안먹은것 남은 것 오류 방지)
        }



        for (int i = 0; i < mMap.mItems.Count; i++) //아이템 개수 20개 이하면 다시 실행
        {
            Destroy(mMap.mItems[i].mObject); //모든 오브젝트 전부 삭제(먹은 것도 inactive라도 메모리 누수 방지, 안먹은것 남은 것 오류 방지)
        }
        for (int i = 0; i < mMap.mMapYsize; i++)
        {
            for (int j = 0; j < mMap.mMapXsize; j++)
            {
                Destroy(mMap.mGrids[i,j].mObject); //모든 오브젝트 전부 삭제(먹은 것도 inactive라도 메모리 누수 방지, 안먹은것 남은 것 오류 방지)
            }
        }
    }
    public void reGame() // 테스트 용 임시
    {
        List<PathAlgorithm> lBefore = new List<PathAlgorithm>();
        for (int i = 0; i < mMap.mPlayers.Count; i++) lBefore.Add(mMap.mPlayers[i].mUsingPathAlgorithm); //맵 새로 만들기 전 기존 알고리즘 저장한다
        exitGame();
        mSelectedMap = new MapGenerator().makeItems(mOriginalMap);
        Start();

        for (int i = 0; i < mMap.mPlayers.Count; i++) mMap.mPlayers[i].mUsingPathAlgorithm = lBefore[i]; 

    }








    /*
        void enemypseudocode()
        {
            if (mMap.mEnemyNodePositionXY.mX == mJustForCheckEnemy.mX && mMap.mEnemyNodePositionXY.mY == mJustForCheckEnemy.mY) return; //이전에 계산한 경로의 출발 지점이 같다 = 새로운 경로 계산할 필요 X

            setPathfindingResult(GridState.Enemy, mUsingPathAlgorithm.getFindResult(mMap, GridState.Enemy, GridState.Item));
            mJustForCheckEnemy = mMap.mEnemyNodePositionXY;
        }
    */

    private void backtoMenu_Button()
    {
        //DontDestroyOnLoad덕분에 씬이 바뀌어도 게임오브젝트가 파괴되지 않는다.
        //Destroy로 파괴했을 때 이벤트만 처리하면 깔끔하게 현재 씬이 청소됨
        exitGame();

        SceneManager.LoadScene("Title");  // 씬 이름을 이용하여 씬 로드

        //Main_GameManager.mInstance.mMainMenu.initialize();

    }

    private void initializeUI()
    {



        mCamera = UnityEngine.GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/MainCamera"));
        mCamera.GetComponent<Camera>().orthographicSize = mMap.mMapYsize * 60 / 2;
                

        mLeftUI = UnityEngine.GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/InGame_LeftUI"));
        setGameTimeText(0);
        setCalculateTimeText(0);
        mLeftUI.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { backtoMenu_Button(); }); //리스너 생성 & 콜백 등록



        mRightUI = UnityEngine.GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/InGame_RightUI"));
        mRightUI.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_AStar>(); });
        mRightUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_AStarVision>(); });
        mRightUI.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_BFS>(); });
        mRightUI.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_BFSVision>(); });
        mRightUI.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FAStarC>(); });
        mRightUI.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FAStarU1>(); }); 
        mRightUI.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FAStarU2>(); }); 
        mRightUI.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FAStarU3>(); }); 
        mRightUI.transform.GetChild(8).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FAStarCVision>(); }); 
        mRightUI.transform.GetChild(9).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FAStarU2Vision>(); }); 
        mRightUI.transform.GetChild(10).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FBFSC>(); });
        mRightUI.transform.GetChild(11).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FBFSU2>(); });
        mRightUI.transform.GetChild(12).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FBFSU3>(); });
        mRightUI.transform.GetChild(13).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FBFSCVision>(); });
        mRightUI.transform.GetChild(14).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[0].initialize<PathAlgorithm_FBFSU2Vision>(); });

        mRightUI.transform.GetChild(15).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_AStar>(); });
        mRightUI.transform.GetChild(16).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_AStarVision>(); });
        mRightUI.transform.GetChild(17).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_BFS>(); });
        mRightUI.transform.GetChild(18).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_BFSVision>(); });
        mRightUI.transform.GetChild(19).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_FAStarC>(); });
        mRightUI.transform.GetChild(20).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_FAStarCVision>(); });
        mRightUI.transform.GetChild(21).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_FBFSC>(); });
        mRightUI.transform.GetChild(22).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_FBFSCVision>(); });
        mRightUI.transform.GetChild(23).GetComponent<Button>().onClick.AddListener(() => { mMap.mPlayers[1].initialize<PathAlgorithm_WayPoint>(); }); 
    }



    private void setGameTimeText(float pGameTimeSecond)
    {
        mLeftUI.transform.GetChild(0).gameObject.GetComponent<Text>().text = "게임\n진행 시간\n" + (int)pGameTimeSecond + "초";
    }

    private void setCalculateTimeText(long pCalculateTimeMilliSecond)
    {
        mLeftUI.transform.GetChild(1).gameObject.GetComponent<Text>().text = "알고리즘\n연산 시간\n" + pCalculateTimeMilliSecond + " 마이크로초";
    }

}
