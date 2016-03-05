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
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix<float> AddColumn(this Matrix<float> m, Vector<float> v)
        {
            if (m.RowCount != v.Count) throw new Exception("SubtractColumn Mismatch");

            var mat = m.Clone();

            int rows = m.RowCount;
            int cols = m.ColumnCount;

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    mat[r, c] -= v[r];
                }
            }

            return mat;
        }

        /// <summary>
        /// Return a new matrix where each row of the original matrix is summed with v
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix<float> AddRow(this Matrix<float> m, Vector<float> v)
        {
            if (m.ColumnCount != v.Count) throw new Exception("SubtractRow Mismatch");
            var mat = m.Clone();

            int rows = m.RowCount;
            int cols = m.ColumnCount;

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    mat[r, c] += v[c];
                }
            }

            return mat;
        }


        /// <summary>
        /// Return a new matrix where each row of the original matrix is multiplied with v
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix<float> MultiplyRow(this Matrix<float> m, Vector<float> v)
        {
            if (m.ColumnCount != v.Count) throw new Exception("SubtractRow Mismatch");
            var mat = m.Clone();

            int rows = m.RowCount;
            int cols = m.ColumnCount;

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    mat[r, c] *= v[c];
                }
            }

            return mat;
        }



        /// <summary>
        /// Eeach row of the original matrix is multiplied with v
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix<float> MultiplyRowT(this Matrix<float> mat, Vector<float> v)
        {
            if (mat.ColumnCount != v.Count) throw new Exception("SubtractRow Mismatch");

            int rows = mat.RowCount;
            int cols = mat.ColumnCount;

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    mat[r, c] *= v[c];
                }
            }

            return mat;
        }



        /// <summary>
        /// Calculate the standard deviation of the rows.  This returns one value for each column!
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector<float> StandardDeviationRow(this Matrix<float> m, bool zero_mean = false)
        {
            int cols = m.ColumnCount;

            var stds = Vector<float>.Build.Dense(cols);

            if (zero_mean)
            {
                for (int c = 0; c < cols; c++)
                    stds[c] = StandardDeviationZeroMean(m.Column(c));
            }
            else
            {
                for (int c = 0; c < cols; c++)
                    stds[c] = StandardDeviation(m.Column(c));
            }

            return stds;
        }

        private static float StandardDeviation(IEnumerable<float> vector)
        {
            float mean = 0;
            int count = 0;

            foreach (var v in vector)
            {
                count++;
                mean += v;
            }

            if (count < 2) return float.NaN;

            mean /= count;

            float std = 0;

            foreach (var v in vector)
            {
                float diff = v - mean;
                std += diff * diff;
            }

            std /= count - 1;

            return (float)Math.Sqrt(std);
        }

        private static float StandardDeviationZeroMean(IEnumerable<float> vector)
        {
            int count = 0;

            float std = 0;

            foreach (var v in vector)
            {
                count++;
                std += v * v;
            }

            if (count < 2) return float.NaN;

            std /= count - 1;

            return (float)Math.Sqrt(std);
        }
    }
}
