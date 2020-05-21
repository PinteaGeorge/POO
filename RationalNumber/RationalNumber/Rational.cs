using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RationalNumber
{
    public class Rational
    {
		public int Numerator { get { return a; } set { a = value; } }
		public int Denominator { get { return b; } set { b = value; } }
		private int a;
		private int b;

		public Rational()
		{
			a = 0;
			b = 0;
		}

		public Rational(int Numerator)
		{
			a = Numerator;
			b = 0;
		}

		public Rational(int Numerator, int Denominator)
		{
			a = Numerator;
			b = Denominator;
		}

		public static Rational operator +(Rational r1, Rational r2)
		{
			return new Rational(r1.a * r2.b + r2.a * r1.b, r1.b * r2.b);
		}

		public static Rational operator -(Rational r1, Rational r2)
		{
			return new Rational(r1.a * r2.b - r2.a * r1.b, r1.b * r2.b);
		}

		public static Rational operator *(Rational r1, Rational r2)
		{
			return new Rational(r1.a * r2.a, r1.b * r2.b);
		}

		public static Rational operator /(Rational r1, Rational r2)
		{
			return new Rational(r1.a * r2.b, r1.b * r2.a);
		}

		public static Rational operator ^(Rational r, int n)
		{
			Rational result = r;
			while (n > 1)
			{
				result *= r;
				n--;
			}
			return result;
		}

		public static bool operator <(Rational r1, Rational r2)
		{
			if (r1.b == r2.b) return r1 < r2;
			else return (r1.a * r2.b < r2.a * r1.b);
		}

		public static bool operator >(Rational r1, Rational r2)
		{
			if (r1.b == r2.b) return r1 > r2;
			else return (r1.a * r2.b > r2.a * r1.b);
		}

		public static bool operator <=(Rational r1, Rational r2)
		{
			if (r1.b == r2.b) return r1 <= r2;
			else return (r1.a * r2.b < r2.a * r1.b);
		}

		public static bool operator >=(Rational r1, Rational r2)
		{
			if (r1.b == r2.b) return r1 >= r2;
			else return (r1.a * r2.b > r2.a * r1.b);
		}

		public static bool operator ==(Rational r1, Rational r2)
		{
			return r1.a == r2.a && r1.b == r2.b;
		}

		public static bool operator !=(Rational r1, Rational r2)
		{
			return r1.a == r2.a && r1.b != r2.b;
		}

		private int Cmmdc(int a, int b)
		{
			a = Math.Abs(a);
			b = Math.Abs(b);
			if (a == 0 && b == 0) return 0;
			if (a == 0) return b;
			if (b == 0) return a;
			if (a == b) return a;
			while (a != b)
			{
				if (a > b) a -= b;
				else b -= a;
			}
			return a;
		}

		public Rational SimplestForm()
		{
			int cmmdc = Cmmdc(a, b);
			if (cmmdc != 0)
			{
				a /= cmmdc;
				b /= cmmdc;
			}
			return new Rational(a, b);
		}

		public override string ToString()
		{
			string sign = ((a >= 0 && b >= 0) || (a < 0 && b < 0)) ? "" : "-";
			string fa = a >= 0 ? a + "" : -a + "";
			string fb = b >= 0 ? b + "" : -b + "";
			if (a == 0 && b != 0) return "0";
			if (a > 0 && b == 0) return $"Infinity ({a}/0)";
			if (a < 0 && b == 0) return $"-Infinity ({a}/0)";
			if (a == 0 && b == 0) return "Undefined (0/0)";
			return $"{sign}{fa}/{fb}";
		}
	}
}
