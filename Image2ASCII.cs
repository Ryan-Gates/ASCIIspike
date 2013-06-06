using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


public class Image2ASCII
{
	static void Main(string[] args)
	{
		Console.WriteLine("Enter File name: ");
		string fname = Console.ReadLine();
		Console.WriteLine("Enter the size: ");
		int size = Convert.ToInt32(Console.ReadLine());
		Console.WriteLine("Color?");
		string col = Console.ReadLine();
		if (col == "y")
		{
			Image2ASCII con = new Image2ASCII(fname, size, true);
		}
		else
		{
			Image2ASCII con = new Image2ASCII(fname, size);
		}
		Console.WriteLine("Done");
		Console.ReadLine();
	}

	Bitmap source,ASCII;
	int Size;
	int xs, ys;
	string FilePath,fileName;
	char[,] data;
	bool[,] outline;
	bool col;
	public Image2ASCII(String file, int size, bool color = false)
	{
		try
		{
			fileName = file.Split('.')[0];
			FilePath = Directory.GetCurrentDirectory()+"\\"+ file;
			Console.WriteLine(FilePath);
			Size = size;
			xs = size * 3;
			ys = size * 5;
			source = new Bitmap(file, true);
			data = new char[source.Width,source.Height];
			outline = new bool[source.Width, source.Height];
			col = color;
			convert();
			outLineImage();
			createASCII();
			writeImageASCII();
		}
		catch(Exception e)
		{
			Console.WriteLine("Error: " + e.Message);
		}
	}
	public void writeImageASCII()
	{
		Bitmap img = new Bitmap(source.Width*Size, source.Height*Size);

		using (Graphics graphics = Graphics.FromImage(img))
		{
			graphics.FillRectangle(Brushes.White, 0, 0, source.Width*Size, source.Height*Size);

			using (Font arialFont = new Font("Arial", Size))
			{
				for (int y = 0; y < source.Height; y++)
				{
					for (int x = 0; x < source.Width; x++)
					{
						Brush b = (col) ? new SolidBrush(source.GetPixel(x, y)): Brushes.Black;
						Point p = new Point(x*Size , y * Size );
						char c = (col) ? data[x, y] : (!outline[x,y]) ? data[x, y] : ' ';
						graphics.DrawString("" + c, arialFont,b, p);
					}
				}
			}
		}
		img.Save(fileName+"convertASCII.jpg", ImageFormat.Jpeg);
	}
	public void createASCII()
	{
		using (System.IO.StreamWriter file = new System.IO.StreamWriter(Directory.GetCurrentDirectory()+'\\'+fileName+".txt"))
		{
			for (int y = 0; y < ASCII.Height; y++)
			{
				for (int x = 0; x < ASCII.Width; x++)
				{
					double GC = ASCII.GetPixel(x, y).G;
					GC /= 255;
					GC *= 94;
					int c = (int)GC+32; //126 range
					char ch = (char)c;
					data[x, y] = ch;
					file.Write(ch);
				}
				file.WriteLine();
			}

		}
	}
	public void outLineImage()
	{
		int value = 200;
		// image size
		int width = source.Width;
		int height = source.Height;
		// create output bitmap

		// create a mutable empty bitmap
		Bitmap newBitmap = new Bitmap(width, height);
		Bitmap bmOut = new Bitmap(width, height);

		// color information
		int A, R, G, B;
		Color pixel;
		// get contrast value
		double contrast = Math.Pow((100 + value) / 100, 2);

		// scan through all pixels
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				// get pixel color
				pixel = source.GetPixel(x, y);
				A = pixel.A;
				// apply filter contrast for every channel R, G, B
				R = pixel.R;
				R = (int)(((((R / 255.0) - 0.5) * contrast) + 0.5) * 255.0);
				if (R < 0) { R = 0; }
				else if (R > 255) { R = 255; }

				G = pixel.G;
				G = (int)(((((G / 255.0) - 0.5) * contrast) + 0.5) * 255.0);
				if (G < 0) { G = 0; }
				else if (G > 255) { G = 255; }

				B = pixel.B;
				B = (int)(((((B / 255.0) - 0.5) * contrast) + 0.5) * 255.0);
				if (B < 0) { B = 0; }
				else if (B > 255) { B = 255; }

				// set new pixel color to output bitmap
				bmOut.SetPixel(x, y, Color.FromArgb(A, R, G, B));
			}
		}
		for (int r = 0; r < width; r++)
		{
			for (int c = 0; c < height; c++)
			{
				bool tran = false;
				//get the pixel from the original image
				Color originalColor = bmOut.GetPixel(r, c);

				if (r - 1 > 0 && r + 1 < source.Width)
				{
					Color lcolor = bmOut.GetPixel(r - 1, c);
					Color rcolor = source.GetPixel(r + 1, c);
					if ((Math.Abs(lcolor.ToArgb() - originalColor.ToArgb()) > 20 ) ^ (Math.Abs(rcolor.ToArgb() - originalColor.ToArgb()) > 20))
					{
						tran = true;
					}
					
				}

				outline[r, c] = tran;
				newBitmap.SetPixel(r, c, (tran)?Color.Black:Color.White);
			}
		}
		bmOut.Save(fileName + "hcon.jpg");
		newBitmap.Save(fileName + "outLine.jpg");
	}
	public void convert()
	{
		Bitmap newBitmap = new Bitmap(source.Width, source.Height);

		for (int i = 0; i < source.Width; i++)
		{
			for (int j = 0; j < source.Height; j++)
			{
				//get the pixel from the original image
				Color originalColor = source.GetPixel(i, j);

				//create the grayscale version of the pixel
				int grayScale = (int)((originalColor.R * .3) + (originalColor.G * .59)
					+ (originalColor.B * .11));
				if (!col)
				{
					if (grayScale > 230)
					{
						grayScale = 94;
					}
					if (grayScale < 40)
					{
						grayScale = 0;
					}
				}
				//create the color object
				Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);

				//set the new image's pixel to the grayscale version
				newBitmap.SetPixel(i, j, newColor);
			}
		   
		}
		newBitmap.Save(fileName+"convert.jpg");
		ASCII = newBitmap;
	}
}

