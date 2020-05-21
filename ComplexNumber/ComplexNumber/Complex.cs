using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexNumber
{
    public class Complex
    {
        public float Real { get { return a; } set { a = value; } }
        public float Imaginary { get { return b; } set { b = value; } }
        protected float a;
        protected float b;
        public Complex()
        {
            a = 0f;
            b = 0f;
        }
        public Complex(float Real)
        {
            a = Real;
            b = 0f;
        }
        public Complex(float Real, float Imaginary)
        {
            a = Real;
            b = Imaginary;
        }
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.a + c2.a, c1.b + c2.b);
        }
        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.a - c2.a, c1.b - c2.b);
        }
        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.a * c2.a - c1.b * c2.b, c1.a * c2.b + c1.b * c2.a);
        }
        public static Complex operator ^(Complex c, int n)
        {
            Complex result = c;
            while (n > 1)
            {
                result *= c;
                n--;
            }
            return result;
        }
        public string TrigonometricForm()
        {
            float r = (float)Math.Sqrt(a * a + b * b);
            float teta = (float)Math.Atan(b / a);
            float fa = (float)Math.Cos(teta);
            float fb = (float)Math.Sin(teta);
            string mid = b > 0 ? "+" : (b < 0 ? "-" : "");
            string sfa = fa == 0f ? "0" : fa + "";
            string sfb = fb == 0f ? "0i" : (fb > 0f ? fb + "i" : -fb + "i");
            return $"{r}*({sfa} {mid} {sfb})";
        }
        public override string ToString()
        {
            string mid = b > 0 ? "+" : (b < 0 ? "-" : "");
            string fa = a == 0f ? "" : a + "";
            string fb = b == 0f ? "" : (b > 0f ? b + "i" : -b + "i");
            return $"{fa} {mid} {fb}";
        }
    }
}
