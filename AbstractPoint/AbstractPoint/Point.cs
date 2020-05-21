using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractPoint
{
    public class Point : AbstractPoint
    {
        public override float a { get { return _a; } set { _a = value; _r = Get_r(); _A = Get_A(); } }
        public override float b { get { return _b; } set { _b = value; _r = Get_r(); _A = Get_A(); } }
        public override float r { get { return _r; } set { _r = value; _a = Get_a(); _b = Get_b(); } }
        public override float A { get { return _A; } set { _A = value; _a = Get_a(); _b = Get_b(); } }
        private float _a;
        private float _b;
        private float _r;
        private float _A;

        public Point(PointRepresentation pr, float a_r, float b_A)
        {
            if (pr == PointRepresentation.Polar)
            {
                _r = a_r;
                _A = b_A;
                _a = Get_a();
                _b = Get_b();
            }
            if (pr == PointRepresentation.Rectangular)
            {
                _a = a_r;
                _b = b_A;
                _r = Get_r();
                _A = Get_A();
            }
        }

        public override void Move(float x, float y)
        {
            _a += x;
            _b += y;
            _r = Get_r();
            _A = Get_A();
        }

        public override void Rotate(float angle)
        {
            _A += angle;
            _a = Get_a();
            _b = Get_b();
        }
    }
}
