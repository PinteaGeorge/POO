using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixClass
{
    public class Matrix
    {
        public float[,] matrix;
        public int rows;
        public int cols;
        public Matrix(int n, int m)
        {
            rows = n;
            cols = m;
            matrix = new float[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = 0f;
        }

        public Matrix(float[,] sample)
        {
            rows = sample.GetLength(0);
            cols = sample.GetLength(1);
            matrix = new float[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = sample[i, j];
        }
        
        public static Matrix RandomMatrix(int rows, int cols, int minval = 0, int maxval = 9)
        {
            Matrix mat = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mat.matrix[i, j] = Program.rnd.Next(minval, maxval + 1);
            return mat;
        }

        public Matrix Transpuse()
        {
            Matrix mat = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mat.matrix[j, i] = matrix[i, j];
            return mat;
        }

        public Matrix RemoveLines(int row, int col)
        {
            Matrix mat = new Matrix(row >= 0 && row < rows ? rows - 1 : rows,
                                    col >= 0 && col < cols ? cols - 1 : cols);
            for (int i = 0, j = 0; i < rows; i++)
            {
                if (i == row) continue;
                for (int k = 0, u = 0; k < cols; k++)
                {
                    if (k == col) continue;
                    mat.matrix[j, u] = matrix[i, k];
                    u++;
                }
                j++;
            }
            return mat;
        }

        public Matrix Multiply(Matrix m)
        {
            if (cols == m.rows)
            {
                Matrix mat = new Matrix(rows, m.cols);
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < m.cols; j++)
                    {
                        float sum = 0f;
                        for (int k = 0; k < cols; k++)
                            sum += matrix[i, k] * m.matrix[k, j];
                        mat.matrix[i, j] = sum;
                    }
                return mat;
            }
            else
                throw new Exception("Can not mulitiply those");
        }

        public float Determinant()
        {
            if (cols == rows)
            {
                int n = rows;
                if (n == 1)
                    return matrix[0, 0];
                if (n == 2)
                    return matrix[0, 0] * matrix[1, 1] - matrix[1, 0] * matrix[0, 1];
                if (n == 3)
                {
                    return matrix[0, 0] * matrix[1, 1] * matrix[2, 2] +
                           matrix[1, 0] * matrix[2, 1] * matrix[0, 2] +
                           matrix[0, 1] * matrix[1, 2] * matrix[2, 0] -
                           matrix[2, 0] * matrix[1, 1] * matrix[0, 2] -
                           matrix[1, 0] * matrix[0, 1] * matrix[2, 2] -
                           matrix[2, 1] * matrix[1, 2] * matrix[0, 0];
                }
                else
                {
                    float sum = 0;
                    for (int i = 0; i < n; i++)
                        sum += (i % 2 == 0 ? matrix[0, i] : -matrix[0, i]) * RemoveLines(0, i).Determinant();
                    return sum;
                }
            }
            else throw new Exception("Matrix needs to be square");
        }

        public Matrix Inverse()
        {
            float det = Determinant();
            if (det != 0f)
            {
                Matrix tr = Transpuse();
                Matrix mat = new Matrix(rows, cols);
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                    {
                        float val = (i + j) % 2 == 0 ? 1 : -1;
                        mat.matrix[i, j] = val * tr.RemoveLines(i, j).Determinant();
                    }
                return mat / det;
            }
            else throw new Exception("Matrix is not inverible");
        }

        public static Matrix operator /(Matrix m, float l)
        {
            Matrix mat = new Matrix(m.rows, m.cols);
            for (int i = 0; i < m.rows; i++)
                for (int j = 0; j < m.cols; j++)
                    mat.matrix[i, j] = m.matrix[i, j] / l;
            return mat;
        }

        public override string ToString()
        {
            string value = "";
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    value += matrix[i, j] + " ";
                value += "\n";
            }
            return value;
        }

        public Matrix Add(Matrix m)
        {
            if (cols == m.cols && rows == m.rows)
            {
                Matrix mat = new Matrix(rows, cols);
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        mat.matrix[i, j] = matrix[i, j] + m.matrix[i, j];
                return mat;
            }
            else
                throw new Exception("The matrices do not have similar sizes");
        }

        public Matrix Subtract(Matrix m)
        {
            if (cols == m.cols && rows == m.rows)
            {
                Matrix mat = new Matrix(rows, cols);
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        mat.matrix[i, j] = matrix[i, j] - m.matrix[i, j];
                return mat;
            }
            else
                throw new Exception("The matrices do not have similar sizes");
        }
    }
}
