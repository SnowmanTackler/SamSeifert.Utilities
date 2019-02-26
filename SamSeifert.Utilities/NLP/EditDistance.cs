using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.NLP
{
    public static class EditDistance
    {
        public static float Calculate(
            String base_string,
            String proposal_string,
            float edit_weight = 1,
            float insert_weight = 1,
            float delete_weight = 1)
        {

            int columns = proposal_string.Length + 1;
            int rows = base_string.Length + 1;

            var mat = new float[rows, columns];

            for (int c = 0; c < columns; c++)
                mat[0, c] = c * delete_weight;

            for (int r = 1; r < rows; r++)
                mat[r, 0] = r * insert_weight;

            for (int r = 1; r < rows; r++)
            {
                char base_char = base_string[r - 1];
                for (int c = 1; c < columns; c++)
                {
                    char proposal_char = proposal_string[c - 1];

                    if (proposal_char == base_char)
                    {
                        mat[r, c] = mat[r - 1, c - 1];
                    }
                    else
                    {
                        mat[r, c] = MathUtil.Min(
                            mat[r - 1, c - 1] + edit_weight,
                            mat[r - 1, c] + insert_weight,
                            mat[r, c - 1] + delete_weight
                            );
                    }
                }
            }

            return mat[rows - 1, columns - 1];
        }
    }
}
