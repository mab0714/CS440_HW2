﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Breakthrough
{
    public class Node 
    {
        // Piece location
        private int _x;
        private int _y;

        private int[,] _chessBoard; // input the initial chess board
        private Node _parentNode;  // reference to parent node
        private List<Node> _childNodes; // list of child nodes
        private Node _OtherNnode;
        private int _depth; // indicate to which level this Node belongs

        private float _evalFunctionValue; //the difference between # offenders and # deffenders

        private int _turn; // claim which turn this node belongs, define "offender" as "true" & "deffender" as "false"
                           // Player1's home is located up
                           // Player2's home is located down

        // public bool _isInitialized;
        // private char _role; // "o" or "x"

        public Node()
        {
            //this._isInitialized = true;
        }

        public Node(int x, int y, int[,] chessBoard)
        {
            this._x = x;
            this._y = y;
            this._chessBoard = chessBoard;
            //this._isInitialized = true;
        }

        public Node(int x, int y, int[,] chessBoard, int turn)
        {
            this._x = x;
            this._y = y;
            this._chessBoard = chessBoard;
            this._turn = turn;
            //this._isInitialized = true;
        }

        public Node(int x, int y, int[,] chessBoard, int turn, Node parentNode)
        {
            this._x = x;
            this._y = y;
            this._chessBoard = chessBoard;
            this._turn = turn;
            this._parentNode = parentNode;
            //this._isInitialized = true;
        }

        public int x
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public int y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public int[,] chessBoard
        {
            get { return this._chessBoard; }
            set { this._chessBoard = value; }
        }

        public int turn
        {
            get { return this._turn; }
            set { this._turn = value; }
        }

        public Node parentNode
        {
            get { return this._parentNode; }
            set { this._parentNode = value; }
        }

        public List<Node> childNodes
        {
            get { return this._childNodes; }
            set { this._childNodes = value; }
        }

        public float evalFunctionValue
        {
            get { return this._evalFunctionValue; }
            set { this._evalFunctionValue = value; }
        }

        public int depth
        {
            get { return this._depth; }
            set { this._depth = value; }
        }

        //private int calcNumOfPlayer1(int[,] chessBoard)
        //{
        //    // calculateing number of player1 remaining on the board
        //    int numOfPlayer1 = 0;
        //    foreach (int item in chessBoard)
        //        if (item == -1)
        //    {
        //        {
        //            numOfPlayer1++;
        //        }
        //    }
        //    return numOfPlayer1;
        //}

        //private int calcNumOfPlayer2(int[,] chessBoard)
        //{
        //    // calculateing number of pieces remaining on the board
        //    int numOfPlayer2 = 0;
        //    foreach (int item in chessBoard)
        //    {
        //        if (item == +1)
        //        {
        //            numOfPlayer2++;
        //        }
        //    }
        //    return numOfPlayer2;
        //}

        private bool isWalkable()
        {
            // checking whether piece at [x, y] is walkable
            try
            {
                if (((this._chessBoard[_x - _turn, _y - 1] != _turn && _y - 1 >= 0 && _y -1 < 8) || 
                    (this._chessBoard[_x - _turn, _y] != _turn && _y >= 0 && _y <= 8) || 
                    (this._chessBoard[_x - turn, _y + 1] != _turn && _y + 1 >= 0 && _y + 1 < 8)) && 
                    (_x - _turn < 8 && _x - _turn >= 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        //private int calcNumOfThreats(Node currNode, int turn)
        //{
        //    // calculating number of threats
        //    int numOfThreats = 0;
        //    for (int i=0;  i<7; i++)
        //    {
        //        for (int j=0; j<7; j++)
        //        {
        //            if (this._chessBoard[i, j] == -turn && (i - currNode._x) * turn < 0)
        //            {
        //                if (isWalkable(x, y, turn))
        //                {
        //                    numOfThreats++;
        //                }
        //            }
        //        }
        //    }
        //    return numOfThreats;
        //}



        private List<int []> findThreats()
        {
            // finding all threats for piece at [x, y] and saving them in a list
            // var threat = new Tuple<int, int>;
            int[] threat;
            threat = new int[2];
            List <int []> threatsList = new List<int []>();
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (this._chessBoard[i, j] == - _turn && (i - _x) * _turn < 0)
                    {
                        if (isWalkable() && Math.Abs(_x - i) >= Math.Abs(j - _y))
                        {
                            threat[0] = i;
                            threat[1] = j;
                            threatsList.Add(threat);
                        }
                    }
                }
            }
            return threatsList;
        }

        private float calcMeanManhattanDistance()
        {
            // calculating mean Manhattan distance used in the evaluation function
            int i = 0;
            int j = 0;
            int sumManhattanDistance = 0;
            //List<int> threats = new List<int>();
            List<int []> threatsList = findThreats();
            foreach (int[] position in threatsList)
            {
                i = position[0];
                j = position[1];
                sumManhattanDistance += Math.Abs(_x - i);
            }
            return sumManhattanDistance / threatsList.Count;
        }
        
        private int calcManhattanDistance(int x, int y, int m, int n)
        {
            // calculating Manhattan distance
            int z = 0;
            int xdelta = 0;
            int ydelta = 0;

            xdelta = Math.Abs(x - m);
            ydelta = Math.Abs(y - n);

            z = xdelta + ydelta;
            return z;
        }

        private int numOfPieces()
        {
            // calculate number of pieces
            int numOfPieces = 0;
            foreach (int item in this._chessBoard)
            {
                if (item == _turn)
                {
                    numOfPieces++;
                }
            }
            return numOfPieces;
        }

        private int numOfThreats()
        {
            List<int[]> listOfThreats = new List<int[]>();
            listOfThreats = this.findThreats();
            return listOfThreats.Count;
        }

        private int maxManhattanDistToHomeBase()
        {
            // calculating the Manhattan distance of the furthest piece of PlayerX
            int temp = 0;
            if (_turn == -1)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (this._chessBoard[i, j] == _turn)
                        {
                            if (i > temp)
                            {
                                temp = i;
                            }
                        }
                    }
                }
            }
            if (turn == 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (this._chessBoard[i, j] == turn)
                        {
                            if (i < temp)
                            {
                                temp = i;
                            }
                        }
                    }
                }
            }
            return temp;
        }

        private int numOfEnemyToCapture()
        {
            // calculating number of enemy's pieces to capture
            int numOfEnemyToCapture = 0;
            if (this._chessBoard[_x - _turn, _y - 1] == _turn)
            {
                numOfEnemyToCapture++;
            }
            //if (this._chessBoard[currNode._x - turn, currNode._y] == turn)
            //{
            //    numOfEnemyToCapture++;
            //}
            if (this._chessBoard[_x - _turn, _y + 1] == _turn)
            {
                numOfEnemyToCapture++;
            }
            return numOfEnemyToCapture;
        }

        public float calcEvalFunctionValue(Node tempNode)
        {
            // calculating heuristic value of one of the three collums, which respectively shifted by +1/0/-1
            return tempNode.numOfPieces() + tempNode.numOfThreats() + 
                tempNode.maxManhattanDistToHomeBase() + tempNode.numOfEnemyToCapture();
        }

        public Node findNodesOfValue(float value, List<Node> bottomNodes, int maxDepth)
        {
            List<Node> theNodes = new List<Node>();
            foreach (Node tempNode in bottomNodes)
            {
                if (tempNode.evalFunctionValue == value)
                {
                    theNodes.Add(tempNode);
                }
            }
            Node theNode = theNodes[0]; // Having a tie here, just pick the option stored as the first element in the list without tie breaker.

            while (theNode.depth != 1)
            {
                theNode = theNode.parentNode;
            }
            return theNode;
        }

        //public void addChild(Node tmpNode)
        //{
        //    try
        //    {
        //        if (_childNodes == null)
        //        {
        //            _childNodes = new List<Node>();
        //        }
        //        _childNodes.Add(tmpNode);  //Null reference

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error adding Child for x: " + tmpNode._x + ", y: " + tmpNode._y + " due to: " + e.InnerException + " " + e.Message);
        //    }
        //}



        //public bool terminalCheck(Node tempNode)
        //{
        //    // checking if the game has been terminated
        //    if tempNode
        //}

        public List<Node> getSuccessor()
        {
            // finding children nodes of tempNode
            int[,] tempBoard;
            Node tempChildNode;
            List<Node> successors = new List<Node>();
            for (int i = 1; i < 7; i++)
            {
                for (int j = 1; j < 7; j++)
                {
                    if (_chessBoard[i, j] == _turn)
                    {
                        Node tempNode = new Node(i, j, _chessBoard, _turn);
                        if (/*numOfThreats(tempNode, turn) == 0 && */isWalkable())
                        {
                            // moving piece at [i, j] to [i - turn, j - 1] (moving to its 10 o'clock for player2 or 4 o'clock for player1 position)
                            tempBoard = _chessBoard;
                            tempBoard[i - _turn, j - 1] = _chessBoard[i, j];
                            tempBoard[i, j] = 0;
                            tempChildNode = new Node(i - _turn, j - 1, tempBoard, _turn);
                            if (!successors.Contains(tempChildNode))
                            {
                                successors.Add(tempChildNode);
                            }

                            // moving piece at [i, j] to [i - turn, j] (moving to its 12 o'clock for player2 or 6 o'clock for player1 position)
                            tempBoard = _chessBoard;
                            tempBoard[i - _turn, j] = _chessBoard[i, j];
                            tempBoard[i, j] = 0;
                            tempChildNode = new Node(i - _turn, j, tempBoard, _turn);
                            if (!successors.Contains(tempChildNode))
                            {
                                successors.Add(tempChildNode);
                            }

                            // moving piece at [i, j] to [i - turn, j + 1] (moving to its 2 o'clock for player2 or 8 o'clock postion)
                            tempBoard = _chessBoard;
                            tempBoard[i - _turn, j + 1] = _chessBoard[i, j];
                            tempBoard[i, j] = 0;
                            tempChildNode = new Node(i - _turn, j + 1, tempBoard, _turn);
                            if (!successors.Contains(tempChildNode))
                            {
                                successors.Add(tempChildNode);
                            }
                        }
                    }
                }
            }
            return successors;
        }

        public void showNodeInfo()
        {
            Console.WriteLine("******************");
            Console.WriteLine("Current Node ");
            Console.WriteLine("Variable to assign: ");
            Console.WriteLine(" X Coordinate: " + this._x);
            Console.WriteLine(" Y Coordinate: " + this._y);

            Console.WriteLine("Breakthrough Board:");
            bool colorFlip = true;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (x % 2 == 0)
                    {
                        if (colorFlip)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        colorFlip = !colorFlip;
                        if (this._chessBoard[x, y] == -1)
                        {
                            Console.Write(this._chessBoard[x, y]);
                        }
                        else if (this._chessBoard[x, y] == 0)
                        {
                            Console.Write("  ");
                        }
                        else
                        {
                            Console.Write("+" + this._chessBoard[x, y]);
                        }
                    }
                    else
                    {
                        if (colorFlip)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                        }
                        colorFlip = !colorFlip;
                        if (this._chessBoard[x, y] == -1)
                        {
                            Console.Write(this._chessBoard[x, y]);
                        }
                        else if (this._chessBoard[x, y] == 0)
                        {
                            Console.Write("  ");
                        }
                        else
                        {
                            Console.Write("+" + this._chessBoard[x, y]);
                        }
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine(this.Assignment);
            Console.WriteLine("******************");
        }

        //public bool equals(node other)
        //{
        //    return ((iequatable<node>)_parentnode).equals(other);
        //}

    }



}
