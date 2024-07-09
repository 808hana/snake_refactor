using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Snake.Drawing;
using static System.Console;

namespace Snake
{
    class Program
    {
        static void Main()
        {
            WindowHeight = 16;
            WindowWidth = 32;
            int score = 5;
            bool gameover = false;

            Drawing drawing = new Drawing();
            Pixel head = drawing.DrawSnakeHead();
            List<Pixel> body = drawing.DrawSnakeBody();
            Pixel berry = drawing.DrawBerry();
            Move.Direction currentMovement = Move.Direction.Right;

            GameLogic logic = new GameLogic();
            Move move = new Move();

            while (!gameover)
            {
                Clear();
                drawing.DrawBorder();
                drawing.DrawPixel(berry);

                gameover = logic.GameOver(head);
                if (gameover) break;

                if (logic.SnakeAteBerry(berry, head))
                {
                    score++;
                    berry = drawing.DrawBerry();
                }

                foreach (Pixel segment in body)
                {
                    drawing.DrawPixel(segment);
                    if (logic.GameOver(head, segment))
                    {
                        gameover = true;
                        break;
                    }
                }

                if (gameover) break;

                drawing.DrawPixel(head);

                move.UpdateMove(ref currentMovement);
                body.Add(new Pixel(head.XPos, head.YPos, ConsoleColor.Green));

                head = drawing.UpdatePosition(head, currentMovement);

                if (body.Count > score)
                {
                    body.RemoveAt(0);
                }
            }

            GameLogic.EndGame(score);
        }
    }

    class GameLogic
    {
        public bool GameOver(Pixel head) => head.XPos == 0 || head.XPos == WindowWidth - 1 || head.YPos == 0 || head.YPos == WindowHeight - 1;

        public bool GameOver(Pixel head, Pixel body) => head.XPos == body.XPos && head.YPos == body.YPos;

        public bool SnakeAteBerry(Pixel berry, Pixel head) => berry.XPos == head.XPos && berry.YPos == head.YPos;

        public static void EndGame(int score)
        {
            SetCursorPosition(WindowWidth / 5, WindowHeight / 2);

            WriteLine($"Game over, Score: {score - 5}");
            ReadKey();
        }
    }

    class Move
    {
        public enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }

        public Direction ReadMovement(Direction movement)
        {
            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow when movement != Direction.Down:
                        movement = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow when movement != Direction.Up:
                        movement = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow when movement != Direction.Right:
                        movement = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow when movement != Direction.Left:
                        movement = Direction.Right;
                        break;
                }
            }
            return movement;
        }

        public void UpdateMove(ref Direction currentMovement)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds <= 500)
            {
                currentMovement = ReadMovement(currentMovement);
            }
        }
    }

    class Drawing
    {

        public class Pixel
        {
            public int XPos { get; set; }
            public int YPos { get; set; }
            public ConsoleColor ScreenColor { get; set; }

            public Pixel(int xPos, int yPos, ConsoleColor color)
            {
                XPos = xPos;
                YPos = yPos;
                ScreenColor = color;
            }
        }

        public void DrawPixel(Pixel pixel)
        {
            SetCursorPosition(pixel.XPos, pixel.YPos);
            ForegroundColor = pixel.ScreenColor;
            Write("■");
            SetCursorPosition(0, 0);
        }

        public void DrawBorder()
        {
            ForegroundColor = ConsoleColor.Cyan;

            for (int i = 0; i < WindowWidth; i++)
            {
                SetCursorPosition(i, 0);
                Write("■");
                SetCursorPosition(i, WindowHeight - 1);
                Write("■");
            }

            for (int i = 0; i < WindowHeight; i++)
            {
                SetCursorPosition(0, i);
                Write("■");
                SetCursorPosition(WindowWidth - 1, i);
                Write("■");
            }
        }

        public Pixel DrawSnakeHead() => new Pixel(WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);

        public List<Pixel> DrawSnakeBody() => new List<Pixel>();

        public Pixel DrawBerry()
        {
            var rand = new Random();
            return new Pixel(rand.Next(1, WindowWidth - 2), rand.Next(1, WindowHeight - 2), ConsoleColor.Magenta);
        }

        public Pixel UpdatePosition(Pixel head, Move.Direction currentMovement)
        {
            switch (currentMovement)
            {
                case Move.Direction.Up:
                    head.YPos--;
                    break;
                case Move.Direction.Down:
                    head.YPos++;
                    break;
                case Move.Direction.Left:
                    head.XPos--;
                    break;
                case Move.Direction.Right:
                    head.XPos++;
                    break;
            }
            return head;
        }
    }
}
