using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour {

    [SerializeField]
    private PlayerImages playerImages;
    private Sprite player1Image;
    private Sprite player2Image;

    [SerializeField]
    private Text turnText;
    [SerializeField]
    private GameObject player1;
    [SerializeField]
    private GameObject player2;
    [SerializeField]
    private GameObject board;
    [SerializeField]
    private GameObject backBtn;

    public int whosTurn;
    private int turnCount;
    private int grid;
    [SerializeField]
    private int[] markedCells; //ID's which cell marked by which player

    Dictionary<int, int> turnHistory;

	void Awake ()
    {
        //Observer Pattern for grid cell selection
        CellInfo.CellClicked += CellInfoOverride;
    }

    void OnDisable()
    {
        CellInfo.CellClicked -= CellInfoOverride;
    }

    void Start()
    {
        LoadTurnHistory();
        GameSetup();
    }

    void GameSetup()
    {
        backBtn.SetActive(false);
        whosTurn = 0;
        turnCount = 0;
        turnHistory = new Dictionary<int, int>();
        turnText.text = "Player " + (whosTurn + 1) + "'s Turn";

        //Get Image and grid info
        player1Image = playerImages.sprites[PlayerPrefs.GetInt ("Player1ImageIndex")];
        player2Image = playerImages.sprites[PlayerPrefs.GetInt ("Player2ImageIndex")];
        grid = PlayerPrefs.GetInt("Grid");

        //Set selected player images
        player1.GetComponent<Image>().sprite = player1Image;
        player2.GetComponent<Image>().sprite = player2Image;

        if (grid == 0)
        {
            PlayerPrefs.SetInt("Grid", 3);
            grid = 3;
        }
        
        //Initialized array to store cell value
        markedCells = new int[grid * grid];

        for (int i = 0; i < markedCells.Length; i++)
        {
            markedCells[i] = -100; //dummy data
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void CellInfoOverride(CellInfo cell)
    {
        markedCells[cell.cellNumber] = whosTurn + 1;
        turnCount++;

        //Storing the turn number with selected cell number as the history of every turn
        //For odd number turns count as Player 1's and even number as Player 2's turn 
        turnHistory.Add(turnCount, cell.cellNumber);

        if (whosTurn == 0)
        {
            cell.gameObject.GetComponent<SpriteRenderer>().sprite = player1Image;

            if (GetWinnerCheck())
            {
                DisplayWinner();
            }
            else
            {
                if (turnCount == grid * grid)
                    DisplayDraw();
                else
                {
                    whosTurn = 1;
                    turnText.text = "Player " + (whosTurn + 1) + "'s Turn";
                }
            }
        }
        else
        {
            cell.gameObject.GetComponent<SpriteRenderer>().sprite = player2Image;
            if (GetWinnerCheck())
            {
                DisplayWinner();
            }
            else
            {
                if (turnCount == grid * grid)
                    DisplayDraw();
                else
                {
                    whosTurn = 0;
                    turnText.text = "Player " + (whosTurn + 1) + "'s Turn";
                }
            }
        }
    }

    bool GetWinnerCheck()
    {
        int[] solutions;
        if (grid == 3)
            solutions = ThirdGrid();
        else
            solutions = ForthGrid();

        for (int i = 0; i < solutions.Length; i++)
        {
            if (solutions[i] == grid * (whosTurn + 1))
            {
                return true;
            }
        }

        return false;
    }

    private int[] ThirdGrid()
    {
        //Row
        int s1 = markedCells[0] + markedCells[1] + markedCells[2];
        int s2 = markedCells[3] + markedCells[4] + markedCells[5];
        int s3 = markedCells[6] + markedCells[7] + markedCells[8];
        //Col
        int s4 = markedCells[0] + markedCells[3] + markedCells[6];
        int s5 = markedCells[1] + markedCells[4] + markedCells[7];
        int s6 = markedCells[2] + markedCells[5] + markedCells[8];
        //Diagonal
        int s7 = markedCells[0] + markedCells[4] + markedCells[8];
        int s8 = markedCells[2] + markedCells[4] + markedCells[6];

        var solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };
        return solutions;
    }

    private int[] ForthGrid()
    {
        //Row
        int s1 = markedCells[0] + markedCells[1] + markedCells[2] + markedCells[3];
        int s2 = markedCells[4] + markedCells[5] + markedCells[6] + markedCells[7];
        int s3 = markedCells[8] + markedCells[9] + markedCells[10] + markedCells[11];
        int s4 = markedCells[12] + markedCells[13] + markedCells[14] + markedCells[15];
        //Col
        int s5 = markedCells[0] + markedCells[4] + markedCells[8] + markedCells[12];
        int s6 = markedCells[1] + markedCells[5] + markedCells[9] + markedCells[13];
        int s7 = markedCells[2] + markedCells[6] + markedCells[10] + markedCells[14];
        int s8 = markedCells[3] + markedCells[7] + markedCells[11] + markedCells[15];
        //Diagonal
        int s9 = markedCells[0] + markedCells[5] + markedCells[10] + markedCells[15];
        int s10 = markedCells[3] + markedCells[6] + markedCells[9] + markedCells[12];

        var solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8, s9, s10 };
        return solutions;
    }

    private void DisplayWinner()
    {
        turnText.text = "Player " + (whosTurn + 1) + " Won!";
        GameOver();
    }

    private void DisplayDraw()
    {
        turnText.text = "Game Draw!";
        GameOver();
    }

    private void GameOver()
    {
        //Blocking further cell selection
        BoxCollider2D[] colliders = board.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D col in colliders)
            col.enabled = false;

        //Enable Back to MainMenu option
        backBtn.SetActive(true);

        //Save current game turn history for future uses
        SaveTurnHistory();
    }

    private void SaveTurnHistory()
    {
        BinaryFormatter TurnHistoryBF = new BinaryFormatter();
        FileStream TurnHistoryFS = File.Create(Application.persistentDataPath + "/TurnHistoryData.dat");

        TurnHistoryBF.Serialize(TurnHistoryFS, turnHistory);
        TurnHistoryFS.Close();
    }

    void LoadTurnHistory()
    {
        if (File.Exists(Application.persistentDataPath + "/TurnHistoryData.dat"))
        {
            BinaryFormatter TurnHistoryBF = new BinaryFormatter();
            FileStream TurnHistoryFS = File.Open(Application.persistentDataPath + "/TurnHistoryData.dat", FileMode.Open);

            turnHistory = (Dictionary<int, int>)TurnHistoryBF.Deserialize(TurnHistoryFS);
            TurnHistoryFS.Close();

            Debug.Log(turnHistory.Count + " in Total turn happen the Last game");
            Debug.Log(turnHistory[turnHistory.Count] + " is the last cell selected by the Winner");

        }
    }

}
