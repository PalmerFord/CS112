using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mankalah
{
    // rename me
    public class pjf5Player : Player // class must be public
    {
        public pjf5Player(Position pos, int maxTimePerMove) // constructor must match this signature
            : base(pos, "BigBagelBoy69", maxTimePerMove) // choose a string other than "MyPlayer"
        {
        }
        Stopwatch timer = new Stopwatch();

        public override string gloat()
        {
            return "get Kringled";
        }

        public Result minimaxVal(Board b, int d)
        {
            int bestMove;
            int bestVal;
            if (b.gameOver() || d == 0 || timer.ElapsedMilliseconds >= getTimePerMove() - 5)
                return new Result(-5, evaluate(b));

            if (b.whoseMove() == Position.Top)     //TOP is MAX 
            {
                bestMove = 7;
                bestVal = -2000000000;
                for (int move = 7; move <= 12; move++)
                {
                    if (b.legalMove(move))  //[and time not expired]
                    {
                        Board b1 = new Board(b);  //duplicate board
                        b1.makeMove(move, false);        //make the move
                        int val = minimaxVal(b1, d - 1).val;   //find its value
                        if (val > bestVal)         //remember if best
                            bestVal = val; bestMove = move;
                    }
                }
            }
            else  // similarly for BOTTOM’s move
            {
                bestMove = 0;
                bestVal = 2000000000;
                for (int move = 0; move <= 4; move++)
                {
                    if (b.legalMove(move))  //[and time not expired]
                    {
                        Board b2 = new Board(b);  //duplicate board
                        b2.makeMove(move, false);        //make the move
                        int val = minimaxVal(b2, d - 1).val;   //find its value
                        if (val < bestVal)         //remember if best
                            bestVal = val; bestMove = move;
                    }
                }
            }
            return new Result(bestMove, bestVal);
        }                   // this can't happen unless game is over

        public int evaluate(Board b)
        {
            int score = b.stonesAt(13) - b.stonesAt(6);
            int goAgain = 0;
            int sum = 0;
            if (b.whoseMove() == Position.Top)
            {
                for (int i = 12; i >= 7; i--)
                {
                    if (b.stonesAt(i) == 13 - i) goAgain++;
                    sum += b.board[i];
                }
            }
            else
            {
                for (int i = 5; i >= 0; i--)
                {
                    if (b.stonesAt(i) == 6 - i) goAgain--;
                    sum += b.board[i];
                }
            }
            return score * (50) + goAgain * (60) + sum * (40);
        }

        public override int chooseMove(Board b)
        {
            timer.Reset();
            timer.Start();
            Result move = new Result(0, 0);
            Result oldMove = new Result(0, 0);
            int d = 1;
            while (timer.ElapsedMilliseconds <= getTimePerMove() - 6)
            {
                oldMove = move;
                move = minimaxVal(b, d);
                Console.WriteLine("Searching to depth: " + d
                    + ",   Current Best move: " + move.move
                    + ",   " + "Heuristic of Move: " + evaluate(b)
                    + ",   " + timer.ElapsedMilliseconds + "ms ellapsed");
                d++;
            }
            return oldMove.move;
        }
        // adapt all code from your player class into this
        public String getImage() { return "https://i.kym-cdn.com/photos/images/newsfeed/000/744/400/8d2.jpg"; }

    }


    public class Result
    {
        public int move;
        public int val;
        public Result(int m, int v)
        {
            move = m;
            val = v;
        }
    }
}