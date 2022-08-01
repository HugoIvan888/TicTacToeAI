using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    //Properity
    private int [,] Board = new int [3,3];
    public GameObject PChess;
    public GameObject Canvas;
    private GameObject [,] oBoard = new GameObject[3,3];
    public GameObject Result;
    public GameObject PlayerFirstBtn;
    public GameObject CPUFirstBtn;

    protected int curPlayer = 1;
    private int MainPlayer;
    private int CPUPlayer;
    private int BestPos; // record the best position for CPU
    //Board Properity
    private bool isGameOver = false ;

    // Start is called before the first frame update
    void Start()
    {
        // bind chess
        for(int i = 0 ; i < 3 ; i++)
            for (int j = 0 ; j < 3 ; j++)
            {
                oBoard[i,j] = Canvas.transform.GetChild((i*3+j)).gameObject;
                string  name =  oBoard[i,j].name;
                oBoard[i,j].GetComponent<Button>().onClick.AddListener(()=>ClickButton(name));
            }
        InitBoard();

    }

    void InitBoard()
    {
        for(int i = 0 ; i < 3 ; i++)
        {
            for (int j = 0 ; j < 3 ; j++)
            {
                MakeChess(i,j,0);
            }
        }
        curPlayer = 1;
        ShowResult(-1);
    }

    public void StartToPlay(int player)
    {
        isGameOver = false;
        InitBoard();
        MainPlayer = player;
        CPUPlayer = MainPlayer == 1 ? 2:1;
        PlayerFirstBtn.active = false;
        CPUFirstBtn.active = false;
        if(CPUPlayer==curPlayer) CPUTurn();
    }

    void MakeChess(int i, int j, int input)
    {
        if(isGameOver) return;
        oBoard[i,j].transform.GetChild(0).gameObject.active = false;
        oBoard[i,j].transform.GetChild(1).gameObject.active = false;
        if(input!=0)
        oBoard[i,j].transform.GetChild(input-1).gameObject.active = true;  
        Board[i,j] = input;   

    }


    public void ClickButton(string ButtonName)
    {
        int i = (int.Parse(ButtonName)-1)/3;
        int j = (int.Parse(ButtonName)-1)%3;
        if(Board[i,j] == 0  && curPlayer == MainPlayer)
        {
            MakeChess(i,j,curPlayer);
            ShowResult(CheckState());
            curPlayer = curPlayer == 1 ? 2: 1; 
            CPUTurn();
        }
        
    }

    void CPUTurn()
    {
        AI(1,int.MaxValue,int.MinValue);
        MakeChess(BestPos/3,BestPos%3,curPlayer);
        ShowResult(CheckState());
        curPlayer = curPlayer == 1 ? 2: 1; 
    }

    void ShowResult(int result)
    {
        // 0 unfinished; 1 player1; 2 player2; 3 tie
        for(int i = 0 ; i < 3 ; i++)
        {
            Result.transform.GetChild(i).gameObject.active = false;
        }
        if(result >= 0 )
        {
            Result.transform.GetChild(result).gameObject.active = true;
            isGameOver = true;
            PlayerFirstBtn.active = true;
            CPUFirstBtn.active = true;
        }
        else  isGameOver = false;
            
    }

    int CheckState()
    {
        // -1 unfinished; 1 player1; 2 player2; 0 tie
        int num = 0 ; 
        //check coloum & row
        for(int i = 0 ; i < 3 ; i++)
        {
            if(Board[i,0] != 0 && Board[i,0] == Board[i,1] && Board[i,1] == Board[i,2]) return Board[i,0];
            if(Board[0,i] != 0 && Board[0,i] == Board[1,i] && Board[1,i] == Board[2,i]) return Board[0,i];
            for(int j = 0 ; j < 3 ; j++)
                if (Board[i,j] != 0)
                    num++;
        }
        //check slash
        if(Board[0,0] != 0 && Board[1,1] == Board[0,0] && Board[2,2] == Board[1,1]) return Board[0,0];

        if(Board[0,2] != 0 && Board[1,1] == Board[0,2] && Board[2,0] == Board[1,1]) return Board[0,2];

        if(num == 9) return 0;

        return -1;
        
    }

    int Evaluate()
    {
        int value = CheckState();
        if(value==MainPlayer) return int.MaxValue;
        if(value==CPUPlayer) return int.MinValue;
        return value;
    }

    int AI(int depth,int a , int b)
    {
        if(depth%2 == 1) a = int.MaxValue;
        else b = int.MinValue;

        if(CheckState()>0) //There is result
            return Evaluate();
        
        List<int> positions = new List<int>(); 
        for(int i = 0 ; i < 3; i++)
            for(int j = 0 ; j < 3 ; j++)
                if(Board[i,j] == 0) positions.Add(i*3 + j );

        if(positions.Count==0 ) return Evaluate(); // Tie
        for(int i = 0 ; i < positions.Count ; i ++)
        {
            int x = positions[i];
            Board[x/3,x%3] = (depth % 2 ==1) ? CPUPlayer : MainPlayer;
            int SonValue = AI(depth + 1,a,b);
            Board[x/3,x%3] = 0;

            if(depth%2==1)
            {
                if(a > SonValue)
                {
                    a = SonValue;
                    if(depth == 1) BestPos = positions[i];
                    if(a<=b)    break;
                }
            }
            else
            {
                if(b < SonValue)
                    b = SonValue;
                    if(a<=b) break;
            }
        }
        if(depth%2 ==1 ) return a;
        else return b;
    }

}
