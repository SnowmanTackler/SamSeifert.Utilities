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

            float[] means = new float[cols];

            for (int c = 0; c < cols; c++)
            {
                means[c] = 0;
                for (int r = 0; r < rows; r++) means[c] += m[r, c];
                means[c] /= rows;
            }


            var of_the_king = Matrix<float>.Build.Dense(cols, cols, 0);

            for (int var1 = 0; var1 < cols; var1++)
            {
                for (int var2 = var1; var2 < cols; var2++)
                {
                    float sum = 0;

                    for (int r = 0; r < rows; r++)
                        sum += (m[r, var1] - means[var1]) * (m[r, var2] - means[var2]);

                    sum /= (rows - 1);

                    of_the_king[var1, var2] = sum;
                    of_the_king[var2, var1] = sum;
                }
            }

            return of_the_king;
        }
    }
}
