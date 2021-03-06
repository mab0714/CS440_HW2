﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WordSudoku
{
    public class Node : IEquatable<Node>
    {
        // Node state
        private char[,] _sudokuBoardData;
        private Dictionary<string, List<string>> _possibleValuesDict = new Dictionary<string, List<string>>();
        private List<string> _sudokuWordBankList = new List<string>();
        private List<string> _sudokuUsedWordList = new List<String>();
        private List<string> _givenHints = new List<string>();
        private Dictionary<string, int> _currentVariablePriority = new Dictionary<string, int>();        

        private Node _parentNode;  // reference to parent node
        private List<Node> _childNodes; // list of child nodes

        private int _x;
        private int _y;

        private string _assignment;

        public bool _isInitialized;

        public Node(char[,] boardData, List<string> sudokuWordBankList, List<string> sudokuUsedWordList, List<string> givenHints, int x, int y, Dictionary<string, int> currentVariablePriority, Node parentNode)
        {
            this._sudokuBoardData = boardData;
            this._sudokuWordBankList = sudokuWordBankList;
            this._sudokuUsedWordList = sudokuUsedWordList;
            this._givenHints = givenHints;
            this._x = x;
            this._y = y;
            this._currentVariablePriority = currentVariablePriority;
            this._parentNode = parentNode;
            this._isInitialized = true;
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

        public string Assignment
        {
            get { return this._assignment; }
            set { this._assignment = value; }
        }

        public char[,] SudokuBoardData
        {
            get { return this._sudokuBoardData; }
            set { this._sudokuBoardData = value; }
        }

        public Dictionary<string, List<string>> PossibleValuesDict
        {
            get { return this._possibleValuesDict; }
            set { this._possibleValuesDict = value; }
        }

        public Dictionary<string, int> CurrentVariablePriority
        {
            get { return this._currentVariablePriority; }
            set { this._currentVariablePriority = value; }
        }

        public List<string> SudokuWordBankList
        {
            get { return this._sudokuWordBankList; }
            set { this._sudokuWordBankList = value; }
        }

        public List<string> SudokuUsedWordList
        {
            get { return this._sudokuUsedWordList; }
            set { this._sudokuUsedWordList = value; }
        }

        public List<string> GivenHints
        {
            get { return this._givenHints; }
            set { this._givenHints = value; }
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
        public bool Equals(Node n)
        {

            for (int y = 8; y > 0; y--)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (!this._sudokuBoardData[x, y].Equals(n.SudokuBoardData[x, y]))
                    {
                        return false;
                    }
                }
                
            }

                return true;

        }

        public List<Node> findEligibleAssignments()
        {
            this._possibleValuesDict = calcPossibleValues(this._sudokuBoardData, this._sudokuWordBankList, this._sudokuUsedWordList, this._givenHints);

            foreach (string option in this._possibleValuesDict[this._x + "_" + this._y])
            {
                char[,] newBoardData = new char[this._sudokuBoardData.GetLength(0), this._sudokuBoardData.GetLength(1)];
                Array.Copy(this._sudokuBoardData, newBoardData, this._sudokuBoardData.Length);
                List<string> newWordBankList = new List<string>(this._sudokuWordBankList);
                List<string> newUsedWordList = new List<string>(this._sudokuUsedWordList);
                Dictionary<string, int> newVariablePriority = new Dictionary<string, int>(this._currentVariablePriority);
                Node tmpNode;

                // Need to assign option to x,y option: H_WORD1, V_WORD2
                // Handle offset
                int offset = Int32.Parse(option.Split('_')[2].ToString());
                string direction = option.Split('_')[0].ToString().ToUpper();
                string word = option.Split('_')[1].ToString().ToUpper();

                if (direction.Equals("H"))
                {
                    newBoardData = updateBoard(newBoardData, direction, this._x - offset, this._y, word, newVariablePriority);
                }
                else
                {
                    newBoardData = updateBoard(newBoardData, direction, this._x, this._y - offset, word, newVariablePriority);
                }

                newUsedWordList.Add(option.Split('_')[1].ToUpper());
                newWordBankList.Remove(option.Split('_')[1].ToUpper());
                
                tmpNode = new Node(newBoardData, newWordBankList, newUsedWordList, this._givenHints, this._x, this._y, newVariablePriority, this);
                tmpNode.Assignment = option.Split('_')[0].ToUpper() + "," + this._x + "," + this.y + ": " + option;
                AddChild(tmpNode);

            }
 
            return _childNodes;
        }

        //public List<Node> findEligibleChildrenA(List<List<char>> mazeBoard)
        //{
        //    // See N   check if parentNode is not null, then check i'm not going to revisit a parent
        //    Node tmpNode = new Node(this._x, this._y - 1, this);

        //    if (isWalkable(this._x, this._y - 1, mazeBoard))
        //    {
        //        tmpNode.g = tmpNode.parentNode.g + 1;
        //        tmpNode.h = calcManhattanDistance(this._x, this.y - 1);
        //        tmpNode.f = tmpNode.g + tmpNode.h;
        //        tmpNode.goalStateNode = this.goalStateNode;
        //        AddChild(tmpNode);
        //    }



        //    //// See NE
        //    //tmpNode = new Node(this._x + 1, this._y - 1, this);
        //    //if (isWalkable(this._x + 1, this._y - 1, mazeBoard))
        //    //{
        //    //    tmpNode.g = tmpNode.parentNode.g + 14;
        //    //    tmpNode.h = calcManhattanDistance(this._x + 1, this.y - 1);
        //    //    tmpNode.f = tmpNode.g + tmpNode.h;
        //    //    tmpNode.goalStateNode = this.goalStateNode;
        //    //    AddChild(tmpNode);
        //    //}

        //    // See E
        //    tmpNode = new Node(this._x + 1, this._y, this);
        //    if (isWalkable(this._x + 1, this._y, mazeBoard))
        //    {
        //        tmpNode.g = tmpNode.parentNode.g + 1;
        //        tmpNode.h = calcManhattanDistance(this._x + 1, this._y);
        //        tmpNode.f = tmpNode.g + tmpNode.h;
        //        tmpNode.goalStateNode = this.goalStateNode;
        //        AddChild(tmpNode);
        //    }

        //    //// See SE
        //    //tmpNode = new Node(this._x + 1, this._y + 1, this);
        //    //if (isWalkable(this._x + 1, this._y + 1, mazeBoard))
        //    //{
        //    //    tmpNode.g = tmpNode.parentNode.g + 14;
        //    //    tmpNode.h = calcManhattanDistance(this._x + 1, this._y + 1);
        //    //    tmpNode.f = tmpNode.g + tmpNode.h;
        //    //    tmpNode.goalStateNode = this.goalStateNode;
        //    //    AddChild(tmpNode);
        //    //}

        //    // See S
        //    tmpNode = new Node(this._x, this._y + 1, this);
        //    if (isWalkable(this._x, this._y + 1, mazeBoard))
        //    {
        //        tmpNode.g = tmpNode.parentNode.g + 1;
        //        tmpNode.h = calcManhattanDistance(this._x, this._y + 1);
        //        tmpNode.f = tmpNode.g + tmpNode.h;
        //        tmpNode.goalStateNode = this.goalStateNode;
        //        AddChild(tmpNode);
        //    }

        //    //// See SW
        //    //tmpNode = new Node(this._x - 1, this._y + 1, this);
        //    //if (isWalkable(this._x - 1, this._y + 1, mazeBoard))
        //    //{
        //    //    tmpNode.g = tmpNode.parentNode.g + 14;
        //    //    tmpNode.h = calcManhattanDistance(this._x - 1, this.y + 1);
        //    //    tmpNode.f = tmpNode.g + tmpNode.h;
        //    //    tmpNode.goalStateNode = this.goalStateNode;
        //    //    AddChild(tmpNode);
        //    //}

        //    // See W
        //    tmpNode = new Node(this._x - 1, this._y, this);
        //    if (isWalkable(this._x - 1, this._y, mazeBoard))
        //    {
        //        tmpNode.g = tmpNode.parentNode.g + 1;
        //        tmpNode.h = calcManhattanDistance(this._x - 1, this._y);
        //        tmpNode.f = tmpNode.g + tmpNode.h;
        //        tmpNode.goalStateNode = this.goalStateNode;
        //        AddChild(tmpNode);
        //    }

        //    //// See NW
        //    //tmpNode = new Node(this._x - 1, this._y - 1, this);
        //    //if (isWalkable(this._x - 1, this._y - 1, mazeBoard))
        //    //{
        //    //    tmpNode.g = tmpNode.parentNode.g + 14;
        //    //    tmpNode.h = calcManhattanDistance(this._x - 1, this._y - 1);
        //    //    tmpNode.f = tmpNode.g + tmpNode.h;
        //    //    tmpNode.goalStateNode = this.goalStateNode;
        //    //    AddChild(tmpNode);
        //    //}
        //    if (this._parentNode != null && this._childNodes != null)
        //    {
        //        _childNodes.Remove(this._parentNode);
        //    }
        //    return _childNodes;
        //}
        //private bool isWalkable(int x, int y, List<List<char>> mazeBoard)
        //{
        //    try
        //    {
        //        if (mazeBoard[y][x].Equals(' ') || mazeBoard[y][x].Equals('.'))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public char[,] updateBoard(char[,] board, string direction, int x, int y, string word, Dictionary<string, int> newVariablePriority)
        {
            char[] charWord = word.ToCharArray();
            int charWordX = 0;
            int charWordY = 0;
            if (direction.ToUpper().Equals("H"))
            {
                for (int tmpX = x; tmpX < charWord.Length; tmpX++)
                {
                    board[tmpX, y] = charWord[charWordX];
                    newVariablePriority.Remove(tmpX + "_" + y);
                    charWordX++;
                }           
            }
            else
            {
                for (int tmpY = y; tmpY > y-charWord.Length; tmpY--)
                {
                    board[x, tmpY] = charWord[charWordY];
                    newVariablePriority.Remove(x + "_" + tmpY);
                    charWordY++;
                }
            }

            //// recalculate variablePriority
            //for (int tmpX = 0; tmpX <= 8; tmpX++)
            //{
            //    for (int tmpY = 8; tmpX >= 0; tmpY--)
            //    {
            //        if (c.Equals('_'))
            //        {
            //            newVariablePriority.Add(tmpX + "_" + tmpY, (8 - tmpX + tmpY));
            //        }
            //    }
            //}

            return board;
        }

        public void AddChild(Node tmpNode)
        {
            try
            {
                if (_childNodes == null)
                {
                    _childNodes = new List<Node>();
                }
                _childNodes.Add(tmpNode);  //Null reference

            }
            catch (Exception e)
            {
                Console.WriteLine("Error adding Child for x: " + tmpNode._x + ", y: " + tmpNode._y + " due to: " + e.InnerException + " " + e.Message);
            }
        }

        public static Dictionary<string, List<string>> calcPossibleValues(char[,] tmpboard, List<string> wordList, List<string> usedWordList, List<string> givenHints)
        {
            char[,] board = (char[,])tmpboard.Clone();
            Dictionary<string, List<string>> newDict = new Dictionary<string, List<string>>();
            // Loop through the board to get the variables
            for (int y = 8; y >= 0; y--)
            {
                for (int x = 0; x < 9; x++)
                {
                    //if (board[x, y].Equals('_'))
                    //{
                    List<string> tmpList = getPossibleWords(board, x, y, wordList, usedWordList, givenHints);
                    newDict.Add(x + "_" + y, tmpList);
                    //}
                }
                Console.WriteLine();
            }

            return newDict;
        }

        static List<string> getPossibleWords(char[,] tmpboard, int x, int y, List<string> wordList, List<string> usedWordList, List<string> givenHints)
        {
            char[,] board = (char[,])tmpboard.Clone();
            List<string> tmpList = new List<string>();
            //int tmpX = x;
            // bool constraints that must be satisfied
            bool uniqueInRow = false;
            bool uniqueInCol = false;
            bool uniqueInCell = false; //3x3 cell
            bool safeHints = false;
            bool safeBoard = false;

            foreach (string line in wordList)
            {                                

                // Horizontal
                // Store all offsets as a possible word
                // IE: (2,8), word is DOG
                // __X______ DOG______ _DOG_____ __DOG____
                // _________ _________ _________ _________
                // _________ _________ _________ _________
                // _________ _________ _________ _________
                // _________ _________ _________ _________
                // _________ _________ _________ _________
                // _________ _________ _________ _________
                // _________ _________ _________ _________
                // _________ _________ _________ _________

                // Dog fills (2,8) variable 3 different ways.
                // Dictionary should say "2_8" = {H_DOG_2, H_DOG_1, H_DOG_0}
                // Format: H_DOG_1 = <orientation>_<word>_<offset with respect to given coordinate>
                // When assigning horizontally, shift the word by using X - 2, to store 'D' in "0_8"
                //                              shift the word by using X - 1, to store 'D' in "1_8"
                //                              shift the word by using X - 0, to store 'D' in "2_8"
                // Stop shifting when beginning of the word is reached or end of word exceeds the board.
                // Similar logic for vertically

                // if it is NOT used yet
                if (!usedWordList.Contains(line))
                {
                    // word too long for remaining x positions
                    //if (satisfyRowLength)
                    //{
                        // Qualifies horizontally

                    for (int tmpX = 0; tmpX <= x && (line.Length - 1 + tmpX) <= 8; tmpX++)
                    {
                        //H_CONFUSE_2
                        if (line.Equals("CONFUSE") && x == 4 && tmpX == 2 && y == 7) {
                            ;
                        }
                        uniqueInCol = isUniqueInCol(board, tmpX, y, line);
                        uniqueInCell = isUniqueInCell(board, tmpX, y, line, 'h');
                        safeHints = isHintsSafe(board, tmpX, y, line, givenHints, 'h');
                        safeBoard = isBoardSafe(board, tmpX, y, line, givenHints, 'h') && isSatisfyRowLength(tmpX, line);

                        if (uniqueInCol && uniqueInCell && safeHints && safeBoard)
                        {

                            tmpList.Add("H_" + line + "_" + (x - tmpX));
                        }
                    }

                    //}





                    // word too long for remaining y positions
                    //if (satisfyColLength)
                    //{
                    // Qualifies vertically      
                    for (int tmpY = 8; tmpY >= y && (tmpY - line.Length + 1 >= 0); tmpY--)
                    {
                        uniqueInRow = isUniqueInRow(board, x, tmpY, line);
                        uniqueInCell = isUniqueInCell(board, x, tmpY, line, 'v');
                        safeHints = isHintsSafe(board, x, tmpY, line, givenHints, 'v');
                        safeBoard = isBoardSafe(board, x, tmpY, line, givenHints, 'v') && isSatisfyColLength(tmpY, line);
                        if (uniqueInRow && uniqueInCell && safeHints && safeBoard)
                        {                            
                            tmpList.Add("V_" + line + "_" + (tmpY - y));
                        }
                    }
                    //}




                }
            }


            return tmpList;
        }

        static bool isBoardSafe(char[,] tmpboard, int x, int y, string word, List<string> givenHints, char direction)
        {
            char[,] board = (char[,])tmpboard.Clone();

            // populate board
            if (direction.ToString().ToUpper().Equals("H"))
            {
                // update board
                for (int tmpX = x; tmpX < x + word.Length; tmpX++)
                {
                    if (board[tmpX, y] != '_')
                    {
                        if (board[tmpX,y] != word[tmpX - x])
                        { 
                            return false;
                        }
                    }
                }

            }
            else
            {
                int revY = 0;
                for (int tmpY = y; (tmpY > y - word.Length) && (y + 1 - word.Length >= 0); tmpY--)
                {
                    if (board[x, tmpY] != '_')
                    {
                        if (board[x, tmpY] != word[revY])
                        {
                            return false;
                        }
                    }
                    revY++;
                }
            }

            return true;
        }
        static bool isHintsSafe(char[,] tmpboard, int x, int y, string word, List<string> givenHints, char direction)
        {
            char[,] board = (char[,])tmpboard.Clone();

            // populate board
            if (direction.ToString().ToUpper().Equals("H"))
            {
                // update board
                for (int tmpX = x; tmpX < x + word.Length; tmpX++)
                {
                    board[tmpX, y] = word[tmpX - x];
                }

            }
            else
            {
                int revY = 0;
                for (int tmpY = y; (tmpY > y - word.Length) && (y + 1 - word.Length >= 0); tmpY--)
                {
                    board[x, tmpY] = word[revY];
                    revY++;
                }
            }

            foreach (string hint in givenHints)
            {
                // check violations within the board
                // hint is in for X_Y_<char>
                int hintX = Int32.Parse(hint.Split('_')[0].ToString());
                int hintY = Int32.Parse(hint.Split('_')[1].ToString());
                char c = char.Parse(hint.Split('_')[2].ToString());

                // if board doesn't match any hint
                if (!board[hintX, hintY].Equals(c))
                {
                    return false;
                }

            }

            return true;
        }
        static bool isSatisfyRowLength(int x, string word)
        {
            if (word.Length <= (9 - x))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool isSatisfyColLength(int y, string word)
        {
            if (word.Length <= (y + 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool isUniqueInRow(char[,] tmpboard, int x, int y, string word)
        {
            char[,] board = (char[,])tmpboard.Clone();

            // build a string consisting of all the letters in the row
            string line = "";
            // Y is in reverse due to orientation, so this reverts it back 
            int revY = 0;
            //EX: We are assigning the word CATS in a 9x9 table at y position 1, y doesn't matter since we check all x, 
            //    we want to investigate the uniqueness in the rows marked in with X
            //    [_________]
            //    [X________]
            //    [X________]
            //    [X________]
            //    [X________]
            //    [_________]
            //    [_________]
            //    [_________]
            //    [_________]

            // start at the proper y, and continue until the we exceed the length of the word
            for (int tmpY = y; tmpY >= y + 1 - word.Length ; tmpY--)
            {
                board[x, tmpY] = word[revY];
                // loop through all the columnns and build a string
                for (int tmpX = 0; tmpX < 9; tmpX++)
                {
                    line = line + board[tmpX, tmpY];
                }
                revY++;
                // check for uniqueness in the string by filtering out distinct values of the linue using Linq
                // if the string returns to anything less than 9, then there was some duplication
                // and there is a violation of distinct letters in a row
                // remove '_'
                string tmpLine = "";
                if (line.Contains("_"))
                {
                    tmpLine = line.Replace("_", "");
                }

                int tempLineLength = tmpLine.Length;
                if (tmpLine.ToCharArray().Distinct().ToArray().Length < tempLineLength)
                {
                    return false;
                }
                line = "";

            }
            // if we checked all columns without being false, we return true
            return true;
        }

        static bool isUniqueInCol(char[,] tmpboard, int x, int y, string word)
        {
            char[,] board = (char[,])tmpboard.Clone();
            // build a string consisting of all the letters in the column
            string line = "";

            //EX: We are assigning the word CATS in a 9x9 table at x position 1, y doesn't matter since we check all y, 
            //    we want to investigate the uniqueness in the columns marked in with X
            //    [_RXXX____]
            //    [_________]
            //    [_________]
            //    [_________]
            //    [_________]
            //    [_R_______]
            //    [_________]
            //    [_________]
            //    [_________]

            // start at the proper x, and continue until the we exceed the length of the word or end of the board
            int wordX = 0;
            for (int tmpX = x; tmpX < x + word.Length - 1; tmpX++)
            {
                board[tmpX, y] = word[wordX];
                wordX++;
                // loop through all the columnns and build a string
                for (int tmpY = 8; tmpY >= 0; tmpY--)
                {
                    line = line + board[tmpX, tmpY];
                }
                // check for uniqueness in the string by filtering out distinct values of the linue using Linq
                // if the string returns to anything less than 9, then there was some duplication
                // and there is a violation of distinct letters in a row
                // remove '_'
                string tmpLine = "";
                if (line.Contains("_"))
                {
                    tmpLine = line.Replace("_", "");
                }

                int tempLineLength = tmpLine.Length;
                if (tmpLine.ToCharArray().Distinct().ToArray().Length < tempLineLength)
                {
                    return false;
                }
                line = "";

            }
            // if we checked all columns without being false, we return true
            return true;
        }

        static bool isUniqueInCell(char[,] tmpboard, int x, int y, string word, char direction)
        {
            char[,] board = (char[,])tmpboard.Clone();
            // build a string consisting of all the letters in the cell
            string line = "";
            if (direction.ToString().ToUpper().Equals("H"))
            {
                // populate our board horizontally
                for (int w = x; w < word.Length; w++)
                {
                    board[w, y] = word[w];
                }

                // check all variables impacted horizontally
                for (int tmpX = x; tmpX < x + word.Length; tmpX++)
                {
                    // Consider X coodordinate range only til 9
                    // EX: 0 1 2 3 4 5 6 7 8 
                    // We investigate x = 2
                    // If divide by 3 (3 spots per cell), we can calculate cell 1-3, then add one, so we aren't dealing with cell 0-2, but cell 1-3 instead
                    int cellX = (int)Math.Floor((tmpX / 3.0)) + 1;

                    // Now, since we don't have a 0, we can multiply by 3 to get our upper bound, then subract 1
                    int maxX = cellX * 3 - 1;

                    // Finally, we can subtract 2 to get the lower bound
                    int minX = maxX - 2;

                    // Same logic as above
                    int cellY = (int)Math.Floor((y / 3.0)) + 1;
                    int maxY = cellY * 3 - 1;
                    int minY = maxY - 2;


                    // loop through all the cell and build a string
                    for (int tmpY = minY; tmpY <= maxY; tmpY++)
                    {
                        for (int tmpX2 = minX; tmpX2 <= maxX; tmpX2++)
                        {
                            line = line + board[tmpX2, tmpY];
                        }
                    }

                    // check for uniqueness in the string by filtering out distinct values of the linue using Linq
                    // if the string returns to anything less than 9, then there was some duplication
                    // and there is a violation of distinct letters in a row               
                    // remove '_'
                    string tmpLine = "";
                    if (line.Contains("_"))
                    {
                        tmpLine = line.Replace("_", "");
                    }

                    int tempLineLength = tmpLine.Length;
                    if (tmpLine.ToCharArray().Distinct().ToArray().Length < tempLineLength)
                    {
                        return false;
                    }
                    line = "";
                }
            }
            else
            {
                // populate our board vertically
                int revY = 0;
                for (int w = y; (w >= y + 1 - word.Length) && (y + 1 - word.Length > 0); w--)
                {
                    board[x, w] = word[revY];
                    revY++;
                }

                // check all variables impacted vertically
                for (int tmpY = y; (tmpY > y + 1 - word.Length) && (tmpY + 1 - word.Length >= 0); tmpY--)
                {
                    // Consider X coodordinate range only til 9
                    // EX: 0 1 2 3 4 5 6 7 8 
                    // We investigate x = 2
                    // If divide by 3 (3 spots per cell), we can calculate cell 1-3, then add one, so we aren't dealing with cell 0-2, but cell 1-3 instead
                    int cellY = (int)Math.Floor((tmpY / 3.0)) + 1;

                    // Now, since we don't have a 0, we can multiply by 3 to get our upper bound, then subract 1
                    int maxY = cellY * 3 - 1;

                    // Finally, we can subtract 2 to get the lower bound
                    int minY = maxY - 2;

                    // Same logic as above
                    int cellX = (int)Math.Floor((x / 3.0)) + 1;
                    int maxX = cellX * 3 - 1;
                    int minX = maxX - 2;


                    // loop through all the cell and build a string
                    for (int tmpY2 = minY; tmpY2 <= maxY; tmpY2++)
                    {
                        for (int tmpX = minX; tmpX <= maxX; tmpX++)
                        {
                            line = line + board[tmpX, tmpY2];
                        }
                    }

                    // check for uniqueness in the string by filtering out distinct values of the linue using Linq
                    // if the string returns to anything less than 9, then there was some duplication
                    // and there is a violation of distinct letters in a row               
                    // remove '_'
                    string tmpLine = "";
                    if (line.Contains("_"))
                    {
                        tmpLine = line.Replace("_", "");
                    }

                    int tempLineLength = tmpLine.Length;
                    if (tmpLine.ToCharArray().Distinct().ToArray().Length < tempLineLength)
                    {
                        return false;
                    }
                    line = "";
                }
            }

            return true;
        }


        public void showNodeInfo()
        {
            Console.WriteLine("******************");
            Console.WriteLine("Current Node ");
            Console.WriteLine("Variable to assign: ");
            Console.WriteLine(" X Coordinate: " + this._x);
            Console.WriteLine(" Y Coordinate: " + this._y);

            Console.WriteLine("Word Sudoku Board:");

            for (int y = 8; y > 0; y--)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (x == this._x && y == this._y) { Console.BackgroundColor = ConsoleColor.Green; }
                    Console.Write(this._sudokuBoardData[x, y]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();                
            }
            Console.WriteLine(this.Assignment);
            Console.WriteLine("******************");
        }
        
    }

   

}
