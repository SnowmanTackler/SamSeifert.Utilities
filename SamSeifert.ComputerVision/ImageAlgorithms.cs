using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace SamSeifert.ComputerVision
{
    public enum ToolboxReturn { Good, NullInput, ImageSizeMismatch, SpecialError };

    public static partial class SingleImage
    {






        






        /*














        public static ImageData erode(ImageData indata, Boolean[, ] grid, int cycles)
        {
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.erode(indata, grid);

            return indata;
        }

        public static ImageData erode(ImageData indata, Boolean[, ] grid)
        {
            int w = indata.Width;
            int h = indata.Height;

            ImageData _SpecialBitmap = new ImageData();
            _SpecialBitmap.CopyTraits(indata);
            _SpecialBitmap.CreateNonNullArrays(indata);

            var sourceList = indata.getNonNullArrays();
            var targetList = _SpecialBitmap.getNonNullArrays();
           
            int cols = grid.Length;
            int rows = grid[0].Length;

            int c2 = cols / 2;
            int r2 = rows / 2;

            int x, y, i, j;
            int tx, ty;
            bool contains;

            for (int index = 0; index < sourceList.Count; index++)
            {
                var inpArray = sourceList[index];
                var outArray = targetList[index];

                for (y = 0; y < h; y++)
                {
                    for (x = 0, x = 0; x < w; x++)
                    {
                        contains = true;

                        for (i = 0; contains && i < rows; i++)
                        {
                            ty = y + i - r2;
                            if (ty >= 0 && ty < h)
                            {
                                for (j = 0; contains && j < cols; j++)
                                {
                                    tx = x + j - c2;
                                    if (tx >= 0 && tx < w)
                                    {
                                        if (grid[j, i])
                                        {
                                            if (inpArray[ty, tx] < 128)
                                            {
                                                contains = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (contains) outArray[y, x] = 255;
                        else outArray[y, x] = 0;
                    }
                }
            }

            return _SpecialBitmap;
        }

        public static ImageData dilate(ImageData indata, Boolean[, ] grid, int cycles)
        {
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.dilate(indata, grid);

            return indata;
        }
        
        public static ImageData dilate(ImageData indata, Boolean[, ] grid)
        {
            int w = indata.Width;
            int h = indata.Height;

            ImageData _SpecialBitmap = new ImageData();
            _SpecialBitmap.CopyTraits(indata);
            _SpecialBitmap.CreateNonNullArrays(indata);

            var sourceList = indata.getNonNullArrays();
            var targetList = _SpecialBitmap.getNonNullArrays();

            int cols = grid.Length;
            int rows = grid[0].Length;

            int c2 = cols / 2;
            int r2 = rows / 2;

            int x, y, i, j;
            int tx, ty;
            bool contains;

            for (int index = 0; index < sourceList.Count; index++)
            {
                var inpArray = sourceList[index];
                var outArray = targetList[index];

                for (y = 0; y < h; y++)
                {
                    for (x = 0, x = 0; x < w; x++)
                    {
                        contains = true;

                        for (i = 0; contains && i < rows; i++)
                        {
                            ty = y + i - r2;
                            if (ty >= 0 && ty < h)
                            {
                                for (j = 0; contains && j < cols; j++)
                                {
                                    tx = x + j - c2;
                                    if (tx >= 0 && tx < w)
                                    {
                                        if (grid[j, i])
                                        {
                                            if (inpArray[ty, tx] > 127)
                                            {
                                                contains = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!contains) outArray[y, x] = 255;
                        else outArray[y, x] = 0;
                    }
                }
            }

            return _SpecialBitmap; 
        }

        public static ImageData open(ImageData indata, Boolean[, ] grid, int cycles)
        {
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.erode(indata, grid);
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.dilate(indata, grid);
            return indata;
        }

        public static ImageData close(ImageData indata, Boolean[, ] grid, int cycles)
        {
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.dilate(indata, grid);
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.erode(indata, grid);
            return indata;
        }
        */























    }
}
