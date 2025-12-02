using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ToolExplorerWPF.Helpers.Extensions
{
    public static class BitmapExtension
    {
        public static void SetPixels(this Bitmap bmp, int x, int y, int size, Color pixelColor)
        {
			int width = bmp.Width;
			int height = bmp.Height;
			for (int px = 0; px < size; px++)
			{
				for (int py = 0; py < size; py++)
				{
					int finalX = x + px;
					int finalY = y + py;

					if (finalX < width && finalY < height)
					{
						bmp.SetPixel(finalX, finalY, pixelColor);
					}
				}
			}
		}
    }
}
