using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BigBallGame
{
    class BBG : FormGameEngine
    {
        List<Ball> balls;
        int ballCount;
        Ball grab;
        bool debugMode;
        bool endGame;
        string endMessage;
        int speedMultiplier;
        ButtonUI info;
        ButtonUI restart;
        bool infoScreen;
        string infoMessage;
        public override void OnUserCreate()
        {
            grab = null;
            debugMode = false;
            endGame = false;
            endMessage = "";
            speedMultiplier = 1;
            infoScreen = false;
            infoMessage = "- Tab to enter debug mode\n\n" +
                          "- Once in debug mode you can drag\n\n  the balls with LeftClick\n\n" +
                          "- Once you grabbed a ball press Right and\n\n  Left keys to change the type of the ball\n\n" +
                          "- Keys 1 to 9 to change the speed\n\n" +
                          "- Click R button (top left) to restart\n\n" +
                          "- To exit info screen click LeftClick";

            info = new ButtonUI(ScreenWidth() - 30, 10, 20, 20, "?");
            info.idleColor = Pixel.Blank;
            info.hoverColor = Pixel.Blank;
            info.activeColor = Pixel.Blank;
            info.textColor = Pixel.White;
            info.textSize = 2;

            restart = new ButtonUI(10, 10, 20, 20, "R");
            restart.idleColor = Pixel.Blank;
            restart.hoverColor = Pixel.Blank;
            restart.activeColor = Pixel.Blank;
            restart.textColor = Pixel.White;
            restart.textSize = 2;

            ballCount = 10;
            balls = new List<Ball>();
            for (int i = 0; i < ballCount; i++)
            {
                int type = rnd.Next(3);
                BallType bType;
                if (type == 0) bType = BallType.RegularBall;
                else if (type == 1) bType = BallType.MonsterBall;
                else bType = BallType.RepelantBall;

                int radius = rnd.Next(10, 30);
                Vector pos;

                bool goodPos;
                do
                {
                    goodPos = true;
                    pos = new Vector(rnd.Next(radius, ScreenWidth() - radius), rnd.Next(radius, ScreenHeight() - radius));

                    foreach (Ball ball in balls)
                        if ((pos - ball.position).Magnitude() < radius + ball.radius)
                        {
                            goodPos = false;
                            break;
                        }

                } while (!goodPos);

                Vector vel = bType == BallType.MonsterBall ? Vector.Zero : Vector.RandomVector();
                int speed = rnd.Next(50, 200);

                balls.Add(new Ball(bType, radius, pos, vel, speed));
            }
        }
        public override void OnUserUpdate(float fElapsedTime)
        {
            Clear(Pixel.Black);

            if (infoScreen)
            {
                DrawString(Middle(), infoMessage, Pixel.White, 2, PositionMode.Center);
                if (Input.KeyReleased(MouseButtons.Left)) infoScreen = false;
                return;
            }

            if (Input.KeyPressed(Keys.Tab)) debugMode = !debugMode;

            if (Input.KeyPressed(Keys.D1)) speedMultiplier = 1;
            if (Input.KeyPressed(Keys.D2)) speedMultiplier = 2;
            if (Input.KeyPressed(Keys.D3)) speedMultiplier = 3;
            if (Input.KeyPressed(Keys.D4)) speedMultiplier = 4;
            if (Input.KeyPressed(Keys.D5)) speedMultiplier = 5;
            if (Input.KeyPressed(Keys.D6)) speedMultiplier = 6;
            if (Input.KeyPressed(Keys.D7)) speedMultiplier = 7;
            if (Input.KeyPressed(Keys.D8)) speedMultiplier = 8;
            if (Input.KeyPressed(Keys.D9)) speedMultiplier = 9;

            if (debugMode)
            {
                if (Input.KeyPressed(MouseButtons.Left))
                {
                    Vector mouse = ScreenMousePos();
                    foreach (Ball ball in balls)
                    {
                        if ((ball.position - mouse).Magnitude() < ball.radius)
                        {
                            grab = ball;
                            break;
                        }
                    }
                }
                if (Input.KeyHeld(MouseButtons.Left))
                {
                    if (grab != null)
                    {
                        if (Input.KeyPressed(Keys.Right)) grab.ballType = (BallType)(((int)grab.ballType + 1) % 3);
                        if (Input.KeyPressed(Keys.Left)) grab.ballType = (BallType)(((int)grab.ballType - 1 + 3) % 3);
                        grab.position.x += ChangeInScreenMouse().x;
                        grab.position.y += ChangeInScreenMouse().y;
                    }
                }
                if (Input.KeyReleased(MouseButtons.Left))
                {
                    grab = null;
                }
            }

            for (int i = 0; i < balls.Count; i++)
            {
                Ball ball = balls[i];
                if (!debugMode)
                {
                    ball.Update(fElapsedTime, ScreenSize(), speedMultiplier);

                    for (int j = i + 1; j < balls.Count; j++)
                    {
                        Ball colBall = balls[j];
                        if (j != i)
                            ball.Collision(colBall);
                    }
                }

                ball.Render(this);

                if (debugMode)
                {
                    DrawString(ball.position, balls.IndexOf(ball) + ": " + ball.ballType, Pixel.White, 1, PositionMode.Center);
                    string interText = "";
                    foreach (Ball b in ball.interactions)
                        interText += balls.IndexOf(b) + " ";
                    DrawString(ball.position + new Vector(0, 25), interText, Pixel.White, 1, PositionMode.Center);
                }

                ball.interactions.RemoveAll(b => b.dead);
            }

            balls.RemoveAll(b => b.dead);

            if (!endGame)
            {
                int repelants = 0;
                int regulars = 0;
                int monsters = 0;
                foreach (Ball ball in balls)
                {
                    switch (ball.ballType)
                    {
                        case BallType.RegularBall: regulars++; break;
                        case BallType.MonsterBall: monsters++; break;
                        case BallType.RepelantBall: repelants++; break;
                    }
                }

                if (regulars == 0)
                {
                    endGame = true;
                    endMessage = "All regular balls are gone.";
                }
                else
                {
                    if (monsters == 0)
                    {
                        endGame = true;
                        endMessage = "Regular balls have no way of dying.";
                    }
                }
            }
            else
            {
                DrawString(Middle() - new Vector(0, 20), "Game ended", Pixel.White, 3, PositionMode.Center);
                DrawString(Middle() + new Vector(0, 20), endMessage, Pixel.White, 2, PositionMode.Center);
            }

            SetPixelMode(PixelMode.Alpha);
            info.Render(this);
            restart.Render(this);
            SetPixelMode(PixelMode.Normal);

            if (info.clicked) infoScreen = true;
            if (restart.clicked) OnUserCreate();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            BBG bbg = new BBG();
            bbg.Construct(800, 800, 1, 1);
        }
    }
}
