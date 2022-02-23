using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Drawing;
using System.Text;
using UnityEngine.SceneManagement;

public class Toe : MonoBehaviour
{
    public string[,] tablero = new string [3,3] { { "", "", "" }, { "", "", "" }, { "", "", "" } };
    public static string ia = "X";
    public static string humanoDespreciable = "O";
    private List<string> turno = new List<string>() { ia, humanoDespreciable };
    private string jugadorActual;
    //activar panel de gameOver
    public GameObject gameOverModal;
    public GameObject textoGanador;

    void Start()
    {
        //decidir turno
        System.Random rnd = new System.Random();
        int inedxR = (int) Mathf.Floor(rnd.Next(turno.Count));
        jugadorActual = turno[inedxR];

        if (jugadorActual == ia)
            mejorMovimiento();

    }

    public void jugada()
    {
        //TODO: llamar a crear jugador y comprobar el tablero, tambien llamra a ia cuadno camnie el turno
        if (jugadorActual == humanoDespreciable)
        {
            string boton = EventSystem.current.currentSelectedGameObject.name;
            switch (boton)
            {
                case "Button0":
                    validarJugada(0, 0, boton);
                    break;
                case "Button1":
                    validarJugada(0, 1, boton);
                    break;
                case "Button2":
                    validarJugada(0, 2, boton);
                    break;
                case "Button3":
                    validarJugada(1, 0, boton);
                    break;
                case "Button4":
                    validarJugada(1, 1, boton);
                    break;
                case "Button5":
                    validarJugada(1, 2, boton);
                    break;
                case "Button6":
                    validarJugada(2, 0, boton);
                    break;
                case "Button7":
                    validarJugada(2, 1, boton);
                    break;
                case "Button8":
                    validarJugada(2, 2, boton);
                    break;
            }
        }

        //game over
        string resultado = comprobarGanador(tablero);
        if (resultado != null)
            gameOver(resultado);
        

    }
    public void validarJugada(int i, int j, string buttonref)
    {
        if(tablero[i, j] == "")
        {
            tablero[i, j] = humanoDespreciable;
            
            //dibujar jugador
            GameObject.Find(buttonref).GetComponent<Button>().GetComponentInChildren<Text>().text = humanoDespreciable;
            GameObject.Find(buttonref).GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.blue;
            jugadorActual = ia;
            mejorMovimiento();
            Print2DArray(tablero);
        }
    }
    
    //------------------------------implementacion de ia--------------------------------------------------------------------
    public void mejorMovimiento()
    {
        float mejorJugada = - 1 / 0f; // -infinito
        
        var movimiento = new Point(-1, -1);
        for(int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if(tablero[i, j] == "")
                {
                    tablero[i, j] = ia;
                    
                    float jugada = minimax(tablero, 0, false);
                    
                    tablero[i, j] = "";
                    
                    if(jugada > mejorJugada)
                    {
                        mejorJugada = jugada;
                        movimiento = new Point(i, j);
                        //print("x: " + i + " y: "+j);
                    }
                }
            }
        }
        if (movimiento.X != -1 && movimiento.Y != -1)
        {
            tablero[movimiento.X, movimiento.Y] = ia;
            dibujarUI(movimiento);
            jugadorActual = humanoDespreciable;
            Print2DArray(tablero);
        }

    }
    
    private float movimientos(string mov)
    {
       float valor;
       switch(mov)
        {
            case "X":
                valor = 1;
                break;
            case "O":
                valor = -1;
                break;
            case "empate":
                valor = 0;
                break;
            default:
                valor = -1 / 0f;
                break;
        }
        return valor;
    }

    private float minimax(string[,] tablero, int heuristica, bool estaMaximizando)
    {
        string resultado = comprobarGanador(tablero);
        /*
         * en este caso la heuristiac hace poco, ya que se trata de un tres en raya y los recursos que consume 
         * el ordenador para ver todas las jugadas posibles pues no son muchos
         * y en mi caso si limito la heuristica a tres por ejemplo no podria devolver mas que tres mejores jugadas
         * ya que limitaria su alcance, esto es util para juegos como el ajedrez 
         */
        if(resultado != null )
        {
            return movimientos(resultado);
        }
        if(estaMaximizando)
        {
            float mejorJugada = -1 / 0f; // -intinito 
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tablero[i, j] == "")
                    {
                        tablero[i, j] = ia;
                        
                        float jugada = minimax(tablero, heuristica + 1, false);
                        
                        tablero[i, j] = "";
                        
                        mejorJugada = Mathf.Max(jugada, mejorJugada);
                    }
                }
            }
            return mejorJugada;
        }
        else
        {
            float mejorJugada =  1 / 0f; // infinito
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tablero[i, j] == "")
                    {
                        tablero[i, j] = humanoDespreciable;
                        
                        float jugada = minimax(tablero, heuristica + 1, true);
                        
                        tablero[i, j] = "";
                        
                        mejorJugada = Mathf.Min(jugada, mejorJugada);
                    }
                }
            }
            return mejorJugada;
        }

    }
    //---------------------------------------------------------------------------------------------------------------------------

    //recursos
    void dibujarUI(Point point)
    {
        var p = point;
        switch (p)
        {
            case { X: 0, Y: 0 }:
                GameObject.Find("Button0").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button0").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
            case { X: 0, Y: 1 }:
                GameObject.Find("Button1").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button1").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
            case { X: 0, Y: 2 }:
                GameObject.Find("Button2").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button2").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
            case { X: 1, Y: 0 }:
                GameObject.Find("Button3").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button3").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
            case { X: 1, Y: 1 }:
                GameObject.Find("Button4").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button4").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
            case { X: 1, Y: 2 }:
                GameObject.Find("Button5").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button5").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
            case { X: 2, Y: 0 }:
                GameObject.Find("Button6").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button6").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
            case { X: 2, Y: 1 }:
                GameObject.Find("Button7").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button7").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
            case { X: 2, Y: 2 }:
                GameObject.Find("Button8").GetComponent<Button>().GetComponentInChildren<Text>().text = ia;
                GameObject.Find("Button8").GetComponent<Button>().GetComponentInChildren<Text>().color = UnityEngine.Color.red;
                break;
        }
    }

    public string comprobarGanador(string [,] tablero)
    {
        string ganador = null;

        //horizontal
        for (int i = 0; i < 3; i++)
        {
            if (igualTres(tablero[i, 0], tablero[i, 1], tablero[i, 2]))  
                ganador = tablero[i,0];
        }
        //vertical
        for (int i = 0; i < 3; i++)
        {
            if (igualTres(tablero[0, i], tablero[1, i], tablero[2, i]))  
                ganador = tablero[0, i];
        }
        //diagonales
        if (igualTres(tablero[0, 0], tablero[1, 1], tablero[2, 2])) 
            ganador = tablero[0, 0];
        
        if (igualTres(tablero[0, 2], tablero[1, 1], tablero[2, 0])) 
            ganador = tablero[0, 2];

        //comprobar el empate
        int casillasVacias = 0;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tablero[i, j] == "")
                    casillasVacias++;
            }
        }

        string resultado = (ganador == null && casillasVacias == 0) ? "empate" : ganador;

        return resultado;
    }

    private bool igualTres(string a, string b, string c)
    {
        return a == b && b == c && a != "";
    }

    void Print2DArray(string[,] matrix)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                sb.Append(matrix[i, j]);
                sb.Append(" ");
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    public void gameOver(string resultado)
    {
        /*
         * no vale la pena comprobar el caso en el que el jugador pueda ganar 
         * ya que eso es imposible el jugador es demasiado tonto
         */
        string res = (resultado == ia) ? "GANADOR: PODEROSA IA." : "EMPATE, pff...";

        textoGanador.GetComponent<Text>().text = res;
        gameOverModal.SetActive(true);

        //desactivar todos los botones
        for(int i = 0; i < 9; i++)
            GameObject.Find("Button" + i).GetComponent<Button>().interactable = false;
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
