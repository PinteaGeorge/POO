using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBallGame
{
    public enum BallType
    {
        RegularBall,
        MonsterBall,
        RepelantBall
    }

    public class Ball
    {
        public BallType ballType;
        public Pixel color;
        public float radius;
        public Vector position;
        public Vector velocity;
        public float speed;
        public bool dead;
        public List<Ball> interactions;

        public Ball(BallType ballType, float radius, Vector pos, Vector vel, float speed)
        {
            this.ballType = ballType;
            this.radius = radius;
            position = pos.Clone();
            velocity = vel.Clone();
            this.speed = speed;
            color = Pixel.Random();
            dead = false;
            interactions = new List<Ball>();
        }

        public void Update(float fElapsedTime, Vector screen, int speedMultiplier)
        {
            position += velocity * speed * speedMultiplier * fElapsedTime;

            if (position.x - radius < 0)
            {
                position.x = radius;
                velocity.x *= -1;
            }
            if (position.x + radius >= screen.x)
            {
                position.x = screen.x - 1 - radius;
                velocity.x *= -1;
            }
            if (position.y - radius < 0)
            {
                position.y = radius;
                velocity.y *= -1;
            }
            if (position.y + radius >= screen.y)
            {
                position.y = screen.y - 1 - radius;
                velocity.y *= -1;
            }
        }

        public void Collision(Ball ball)
        {
            float dist = (position - ball.position).Magnitude();
            bool noInteractions = true;
            if (dist <= radius + ball.radius)
            {
                foreach (Ball b in interactions)
                {
                    if (b == ball)
                    {
                        noInteractions = false;
                        break;
                    }
                }
                if (noInteractions)
                {
                    interactions.Add(ball);
                    ball.interactions.Add(this);

                    if (ballType == BallType.RegularBall)
                    {
                        if (ball.ballType == BallType.RegularBall) RegularWithRegular(this, ball);
                        if (ball.ballType == BallType.MonsterBall) RegularWithMonster(this, ball);
                        if (ball.ballType == BallType.RepelantBall) RegularWithRepelant(this, ball);
                    }
                    if (ballType == BallType.MonsterBall)
                    {
                        if (ball.ballType == BallType.RegularBall) RegularWithMonster(ball, this);
                        if (ball.ballType == BallType.RepelantBall) RepelantWithMonster(ball, this);
                    }
                    if (ballType == BallType.RepelantBall)
                    {
                        if (ball.ballType == BallType.RegularBall) RegularWithRepelant(ball, this);
                        if (ball.ballType == BallType.MonsterBall) RepelantWithMonster(this, ball);
                        if (ball.ballType == BallType.RepelantBall) RepelantWithRepelant(this, ball);
                    }
                }
            }
            else
            {
                interactions.Remove(ball);
                ball.interactions.Remove(this);
            }
        }

        private void RegularWithRegular(Ball ball1, Ball ball2)
        {
            float rr = ball1.radius + ball2.radius;

            int r1 = (int)(ball1.radius / rr * ball1.color.r);
            int g1 = (int)(ball1.radius / rr * ball1.color.g);
            int b1 = (int)(ball1.radius / rr * ball1.color.b);

            int r2 = (int)(ball2.radius / rr * ball2.color.r);
            int g2 = (int)(ball2.radius / rr * ball2.color.g);
            int b2 = (int)(ball2.radius / rr * ball2.color.b);

            if (ball1.radius > ball2.radius)
            {
                ball1.radius += ball2.radius;
                ball2.dead = true;
                ball1.color = Pixel.RGB(r1 + r2, g1 + g2, b1 + b2);
            }
            else
            {
                ball2.radius += ball1.radius;
                ball1.dead = true;
                ball2.color = Pixel.RGB(r1 + r2, g1 + g2, b1 + b2);
            }
        }

        private void RegularWithMonster(Ball ball1, Ball ball2)
        {
            ball2.radius += ball1.radius;
            ball1.dead = true;
        }

        private void RegularWithRepelant(Ball ball1, Ball ball2)
        {
            ball2.color = ball1.color;
            ball1.velocity *= -1;
        }

        private void RepelantWithRepelant(Ball ball1, Ball ball2)
        {
            Utility.Swap(ref ball1.color, ref ball2.color);
        }

        private void RepelantWithMonster(Ball ball1, Ball ball2)
        {
            ball1.radius /= 2f;
            if (ball1.radius <= 1f) ball1.dead = true;
        }

        public void Render(FormGameEngine fge)
        {
            fge.FillCircle(position, radius, color);
        }
    }
}
