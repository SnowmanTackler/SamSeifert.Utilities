using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.MathNet.Numerics.Extensions
{
    public static class MatrixUtil
    {
        public static Matrix<float> Covariance(this Matrix<float> m)
        {
            int rows = m.RowCount;
            int cols = m.ColumnCount;

            var means = m.MeanRow();

            var of_the_king = Matrix<float>.Build.Dense(cols, cols, 0);

            for (int c1 = 0; c1 < cols; c1++)
            {
                for (int c2 = c1; c2 < cols; c2++)
                {
                    float sum = 0;

                    for (int r = 0; r < rows; r++)
                        sum += (m[r, c1] - means[c1]) * (m[r, c2] - means[c2]);

                    sum /= (rows - 1);

                    of_the_king[c1, c2] = sum;
                    of_the_king[c2, c1] = sum;
                }
            }

            return of_the_king;
        }

        /// <summary>
        /// Calculate the mean row.  This returns one value for each column!
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector<float> MeanRow(this Matrix<float> m)
        {
            return m.ColumnSums() / m.RowCount;
        }

        /// <summary>
        /// Calculate the mean column.  This returns one value for each row!
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector<float> MeanColumn(this Matrix<float> m)
        {
            return m.RowSums() / m.ColumnCount;
        }

        /// <summary>
        /// Return a new matrix where each column of the original matrix is summed with v
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static void AddColumn(this Matrix<float> mat, Vector<float> v)
        {
            if (mat.RowCount != v.Count) throw new Exception("SubtractColumn Mismatch");

            int rows = mat.RowCount;
            int cols = mat.ColumnCount;

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    mat.At(r, c, mat.At(r, c) + v.At(r));
                }
            }

        }

        /// <summary>
        /// Return a new matrix where each row of the original matrix is summed with v
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static void AddRow(this Matrix<float> mat, Vector<float> v)
        {
            if (mat.ColumnCount != v.Count) throw new Exception("SubtractRow Mismatch");

            int rows = mat.RowCount;
            int cols = mat.ColumnCount;

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    mat.At(r, c, mat.At(r, c) + v.At(c));
                }
            }
        }


        /// <summary>
        /// Eeach row of the original matrix is multiplied with v
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static void MultiplyRow(this Matrix<float> mat, Vector<float> v)
        {
            if (mat.ColumnCount != v.Count) throw new Exception("SubtractRow Mismatch");

            int rows = mat.RowCount;
            int cols = mat.ColumnCount;

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    mat.At(r, c, mat.At(r, c) * v.At(c));
                }
            }
        }



        /// <summary>
        /// Calculate the standard deviation of the rows.  This returns one value for each column!
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector<float> StandardDeviationRow(this Matrix<float> m)
        {
            int cols = m.ColumnCount;

            var stds = Vector<float>.Build.Dense(cols);
            for (int c = 0; c < cols; c++)
                stds[c] = m.Column(c).StandardDeviation();

            return stds;
        }

        public static void setAllValues(this Matrix<float> m, float value = 0)
        {
            for (int i = 0; i < m.RowCount; i++)
            {
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    m.At(i, j, value);
                }
            }
        }
    }
}
