using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WhoseTurn { WHITE, BLACK }

public class GameHandler : MonoBehaviour
{
    [HideInInspector] public static GameHandler gameHandler;

    public TeamManager whiteTeam = new TeamManager(Team.WHITE);
    public TeamManager blackTeam = new TeamManager(Team.BLACK);

    [SerializeField] private GameObject square;
    [SerializeField] private GameObject stoneW;
    [SerializeField] private GameObject stoneB;

    public GameObject[,] squareMatrix = new GameObject[5, 5];
    public int[,] stonePositionIndexes = new int[4, 2];
    public WhoseTurn whoseTurn = WhoseTurn.WHITE;

    public static Action<WhoseTurn> OnWhoseTurnChanged;

    private void Awake()
    {
        if (gameHandler == null)
            gameHandler = this;
    }

    void Start()
    {
        SpawnSquares();
        SpawnRandomRocks();
    }

    #region SPAWN_SQUARES

    private void SpawnSquares()
    {
        Vector3 spawnPoint = new Vector3(-2.6f, 2.6f, 0f);

        float dist = 0.3f;

        for (int i = 0; i < 5; i++)
        {
            if (i != 0)
                spawnPoint = ResetSpawnPointX(spawnPoint, dist);

            for (int j = 0; j < 5; j++)
            {
                GameObject spawnedSquare = Instantiate(square, spawnPoint, Quaternion.identity);
                squareMatrix[i, j] = spawnedSquare;
                spawnedSquare.TryGetComponent<SquareManager>(out SquareManager squareManager);
                if (squareManager)
                {
                    squareManager.indexX = i;
                    squareManager.indexY = j;
                }
                spawnPoint = AddDistToSpawnPoint(spawnPoint, dist, 0f);
            }
            spawnPoint = AddDistToSpawnPoint(spawnPoint, 0f, dist);
        }
    }

    private Vector3 AddDistToSpawnPoint(Vector3 spawnPoint, float distX, float distY)
    {
        Vector3 temp = spawnPoint;
        return new Vector3(
            temp.x + distX + ((distX > 0f) ? square.transform.lossyScale.x : 0f),
            temp.y - distY - ((distY > 0f) ? square.transform.lossyScale.y : 0f),
            temp.z
            );
    }
    private Vector3 ResetSpawnPointX(Vector3 spawnPoint, float dist)
    {
        Vector3 temp = spawnPoint;
        return new Vector3(temp.x - 5f * (dist + square.transform.lossyScale.x), temp.y, temp.z);
    }
    #endregion

    #region SPAWN_STONES

    private bool CheckForDuplicateStonePoint(int randX, int randY)
    {
        for (int i = 0; i < 4; i++)
        {
            if (stonePositionIndexes[i, 0] == randX && stonePositionIndexes[i, 1] == randY)
                return true;
        }

        return false;
    }

    private (int, int) GenerateUniqueSpawnPoint()
    {
        bool isThereDuplicate = false;
        bool isItCorner = false;
        int randX;
        int randY;
        do
        {
            randX = UnityEngine.Random.Range(0, 5);
            randY = UnityEngine.Random.Range(0, 5);
            isThereDuplicate = CheckForDuplicateStonePoint(randX, randY);
            isItCorner = ((randX == 0 && randY == 0) || (randX == 0 && randY == 4) || (randX == 4 && randY == 0)
                || (randX == 4 && randY == 4)) ? true : false;
        }
        while (isThereDuplicate || isItCorner);

        return (randX, randY);
    }

    private void SpawnRandomRocks()
    {
        Team team = Team.WHITE;


        for (int i = 0; i < 4; i++)
        {

            (int randX, int randY) = GenerateUniqueSpawnPoint();

            GameObject randomSquare = squareMatrix[randX, randY];
            GameObject spawnedStone = Instantiate((team == Team.WHITE) ? stoneW : stoneB,
                randomSquare.transform.position, Quaternion.identity);
            spawnedStone.TryGetComponent<StoneManager>(out StoneManager stoneManager);
            if (stoneManager)
            {
                stoneManager.indexX = randX;
                stoneManager.indexY = randY;
                stoneManager.team = team;
                stoneManager.id = i;
            }
            stonePositionIndexes[i, 0] = randX;
            stonePositionIndexes[i, 1] = randY;

            if (i == 0)
                whiteTeam.stone1 = stoneManager;
            else if (i == 1)
            {
                whiteTeam.stone2 = stoneManager;
                team = Team.BLACK;
            }
            else if (i == 2)
                blackTeam.stone1 = stoneManager;
            else if (i == 3)
                blackTeam.stone2 = stoneManager;

        }
    }

    #endregion

    #region SQUARE_CLICK_CHECK

    private void OnEnable()
    {
        SquareManager.OnStoneClicked += OnStoneClicked;
    }
    private void OnDisable()
    {
        SquareManager.OnStoneClicked -= OnStoneClicked;
    }

    //returns if there is a stone on that square
    private bool CheckForStone(SquareManager squareManager)
    {
        for (int i = 0; i < 4; i++)
        {
            if (stonePositionIndexes[i, 0] == squareManager.indexX &&
                stonePositionIndexes[i, 1] == squareManager.indexY &&
                GetStoneByIndex(squareManager.indexX, squareManager.indexY).team == Team.WHITE)
                return true;

        }

        return false;
    }

    private void OnStoneClicked(SquareManager squareManager)
    {
        if (whoseTurn != WhoseTurn.WHITE) return;

        if (CheckForStone(squareManager))
            ShowPossibleMove(squareManager);
        else if (!CheckForStone(squareManager) && squareManager.canMove)
            MoveStone(squareManager);
    }

    #endregion

    #region STONE_FOCUS

    private void ShowPossibleMove(SquareManager squareManager)
    {
        ResetAllSquares();

        Color newColor = new Color(188, 188, 188);

        try
        {
            HighlightSquare(squareMatrix[squareManager.indexX - 1, squareManager.indexY - 1], newColor);
        }
        catch (IndexOutOfRangeException e) { }

        try
        {
            HighlightSquare(squareMatrix[squareManager.indexX - 1, squareManager.indexY], newColor);
        }
        catch (IndexOutOfRangeException e) { }

        try
        {
            HighlightSquare(squareMatrix[squareManager.indexX - 1, squareManager.indexY + 1], newColor);
        }
        catch (IndexOutOfRangeException e) { }

        try
        {
            HighlightSquare(squareMatrix[squareManager.indexX, squareManager.indexY - 1], newColor);
        }
        catch (IndexOutOfRangeException e) { }

        try
        {
            HighlightSquare(squareMatrix[squareManager.indexX, squareManager.indexY + 1], newColor);
        }
        catch (IndexOutOfRangeException e) { }

        try
        {
            HighlightSquare(squareMatrix[squareManager.indexX + 1, squareManager.indexY - 1], newColor);
        }
        catch (IndexOutOfRangeException e) { }

        try
        {
            HighlightSquare(squareMatrix[squareManager.indexX + 1, squareManager.indexY], newColor);
        }
        catch (IndexOutOfRangeException e) { }

        try
        {
            HighlightSquare(squareMatrix[squareManager.indexX + 1, squareManager.indexY + 1], newColor);
        }
        catch (IndexOutOfRangeException e) { }

        StoneManager stoneManager = GetStoneByIndex(squareManager.indexX, squareManager.indexY);

        if (stoneManager == null) return;

        stoneManager.isFocused = true;
    }

    private StoneManager GetStoneByIndex(int indexX, int indexY)
    {
        StoneManager[] stoneManagers = FindObjectsOfType<StoneManager>();
        foreach (StoneManager stoneManager in stoneManagers)
        {
            if (stoneManager.indexX == indexX && stoneManager.indexY == indexY)
                return stoneManager;
        }
        return null;
    }

    private bool ObstacleCheck(SquareManager squareManager)
    {
        for (int i = 0; i < 4; i++)
        {
            if (stonePositionIndexes[i, 0] == squareManager.indexX &&
                stonePositionIndexes[i, 1] == squareManager.indexY)
                return true;
        }
        return false;
    }

    private void HighlightSquare(GameObject square, Color newColor)
    {
        square.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr);
        square.TryGetComponent<SquareManager>(out SquareManager squareManager);

        if (sr == null || squareManager == null) return;

        if (ObstacleCheck(squareManager)) return;

        squareManager.canMove = true;

        sr.color = newColor;

    }

    #endregion

    #region MOVE_STONE

    public void MoveStone(SquareManager squareManager)
    {
        GetFocusedStone().transform.position = squareManager.transform.position;

        stonePositionIndexes[GetFocusedStone().id, 0] = squareManager.indexX;
        stonePositionIndexes[GetFocusedStone().id, 1] = squareManager.indexY;

        GetFocusedStone().indexX = squareManager.indexX;
        GetFocusedStone().indexY = squareManager.indexY;

        //set corner bool
        SetCorner(GetFocusedStone(), GetFocusedStone().indexX, GetFocusedStone().indexY);

        GetFocusedStone().isFocused = false;
        ResetAllSquares();

        // Check for win condition after move
        CheckForWin();


        whoseTurn = (whoseTurn == WhoseTurn.WHITE) ? WhoseTurn.BLACK : WhoseTurn.WHITE;
        OnWhoseTurnChanged?.Invoke(whoseTurn);
    }

    public int TotalVisitedCornerAmount(Team team)
    {
        StoneManager[] stones = FindObjectsOfType<StoneManager>();

        int total = 0;

        bool[] _visitedCorners = new bool[4];

        TeamManager teamManager = (team == Team.WHITE) ? whiteTeam : blackTeam;

            if (teamManager.visitedCorners[0]) _visitedCorners[0] = true;
            if (teamManager.visitedCorners[1]) _visitedCorners[1] = true;
            if (teamManager.visitedCorners[2]) _visitedCorners[2] = true;
            if (teamManager.visitedCorners[3]) _visitedCorners[3] = true;

        for(int i = 0; i < 4; i++)
        {
            total += (_visitedCorners[i] ? 1 : 0);
        }

        return total;
    }

    public void SetCorner(StoneManager stoneManager, int x, int y)
    {
        if(GetTeamByStone(stoneManager) == Team.WHITE)
        {
            if (x == 0 && y == 0)
                whiteTeam.visitedCorners[0] = true;
            else if(x == 0 && y == 4)
                whiteTeam.visitedCorners[1] = true;
            else if (x == 4 && y == 0)
                whiteTeam.visitedCorners[2] = true;
            else if (x == 4 && y == 4)
                whiteTeam.visitedCorners[3] = true;
        }
        else
        {
            if (x == 0 && y == 0)
                blackTeam.visitedCorners[0] = true;
            else if (x == 0 && y == 4)
                blackTeam.visitedCorners[1] = true;
            else if (x == 4 && y == 0)
                blackTeam.visitedCorners[2] = true;
            else if (x == 4 && y == 4)
                blackTeam.visitedCorners[3] = true;
        }
    }

    private Team GetTeamByStone(StoneManager stoneManager)
    {
        if (whiteTeam.stone1 == stoneManager || whiteTeam.stone2 == stoneManager) return Team.WHITE;
        else return Team.BLACK;
    }

    private void CheckForWin()
    {
        if (TotalVisitedCornerAmount(Team.WHITE) == 4)
        {
            Debug.Log("White team wins!");
            UIManager.uiManager.winner_text.text = "White team wins!";
        }
        else if (TotalVisitedCornerAmount(Team.BLACK) == 4)
        {
            Debug.Log("Black team wins!");
            UIManager.uiManager.winner_text.text = "Black team wins!";
        }
    }

    private StoneManager GetFocusedStone()
    {
        StoneManager[] stoneManagers = FindObjectsOfType<StoneManager>();
        foreach (StoneManager stoneManager in stoneManagers)
        {
            if (stoneManager.isFocused)
                return stoneManager;
        }
        return null;
    }

    private void ResetAllSquares()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                squareMatrix[i, j].TryGetComponent<SpriteRenderer>(out SpriteRenderer sr);
                squareMatrix[i, j].TryGetComponent<SquareManager>(out SquareManager sm);
                if (sr && sm)
                {
                    sr.color = SquareManager.defaultColor;
                    sm.canMove = false;
                }

            }
        }
    }

    #endregion



}