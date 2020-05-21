using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexNumber
{
    public class ComplexD : Complex
    {
        public ComplexD() : base() { }
        public ComplexD(float Real) : base(Real) { }
        public ComplexD(float Real, float Imaginary) : base(Real, Imaginary) { }
        public string PowerInTrigonometricForm(int n)
        {
            Complex result = new Complex(a, b);
            while (n > 1)
            {
                result.Real = a * a - b * b;
                result.Imaginary = a * b + b * a;
                n--;
            }
            return result.TrigonometricForm();
        }

        public float DistToComplexArray(IEnumerable<Complex> complexes)
        {
            float minDist = float.MaxValue;
            foreach (Complex complex in complexes)
            {
                float dist = (float)Math.Sqrt((a - complex.Real) * (a - complex.Real) + (b - complex.Imaginary) * (b - complex.Imaginary));
                if (dist < minDist) minDist = dist;
            }
            return minDist;
        }

        public float DistToComplexArray(params Complex[] complexes) => DistToComplexArray(complexes.ToList());
    }
}
