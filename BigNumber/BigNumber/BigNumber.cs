using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNumber
{
    public class BigNumber
    {
        public List<int> digits;
        public BigNumber()
        {
            digits = new List<int>();
        }
        public BigNumber(params int[] digits)
        {
            this.digits = new List<int>();
            for (int i = 0; i < digits.Length; i++)
            {
                if (digits[i] > 9)
                {
                    List<int> ndig = new List<int>();
                    while (digits[i] > 0)
                    {
                        ndig.Add(digits[i] % 10);
                        digits[i] /= 10;
                    }
                    for (int d = ndig.Count - 1; d >= 0; d--)
                    {
                        this.digits.Add(ndig[d]);
                    }
                }
                else
                {
                    this.digits.Add(digits[i]);
                }
            }
        }

        public BigNumber Clone()
        {
            BigNumber clone = new BigNumber();
            for (int i = 0; i < digits.Count; i++)
                clone.digits.Add(digits[i]);
            return clone;
        }

        public BigNumber Invert()
        {
            BigNumber bgi = new BigNumber();
            for (int i = digits.Count - 1; i >= 0; i--)
                bgi.digits.Add(digits[i]);
            return bgi;
        }

        public static BigNumber operator +(BigNumber bn1, BigNumber bn2)
        {
            BigNumber sum = new BigNumber();
            int maxLen = bn1.digits.Count > bn2.digits.Count ? bn1.digits.Count : bn2.digits.Count;
            int carry = 0;
            int i = 0;
            while (i < maxLen)
            {
                int a = i < bn1.digits.Count ? bn1.digits[bn1.digits.Count - i - 1] : 0;
                int b = i < bn2.digits.Count ? bn2.digits[bn2.digits.Count - i - 1] : 0;
                sum.digits.Add((a + b + carry) % 10);
                carry = (a + b + carry) / 10;
                i++;
            }
            if (carry > 0)
                sum.digits.Add(carry);
            return sum.Invert();
        }

        public static BigNumber operator *(BigNumber bn1, BigNumber bn2)
        {
            BigNumber prod = new BigNumber();
            for (int i = 0; i < bn1.digits.Count + bn2.digits.Count; i++)
                prod.digits.Add(0);
            int k = prod.digits.Count - 1;
            int shift = 0;
            for (int i = bn1.digits.Count - 1; i >= 0; i--)
            {
                int a = bn1.digits[i];
                for (int j = bn2.digits.Count - 1; j >= 0; j--)
                {
                    int b = bn2.digits[j];
                    prod.digits[k - shift] += a * b;
                    prod.digits[k - shift - 1] += prod.digits[k - shift] / 10;
                    prod.digits[k - shift] %= 10;
                    k--;
                }
                shift++;
                k = prod.digits.Count - 1;
            }
            while (prod.digits[0] == 0)
                prod.digits.RemoveAt(0);
            return prod;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < digits.Count; i++)
                result += digits[i];
            return result;
        }
    }
}
