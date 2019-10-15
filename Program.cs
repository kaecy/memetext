using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection; // for assembly stuff
using System.IO;
using SkiaSharp;

namespace mememaker
{
    class Program
    {
		static String program_dir;
		
		static void Init() {
			program_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}
		
        static void Main(string[] args)
        {
			Init();
			
			if (args.Length == 1) {
				if (args[0] == "/version")
					Console.WriteLine("v0.0.2");
			}
			
			if (args.Length == 3) {
				string picture = args[0];
				string text = args[1];
				string text2 = args[2];
				
				if (picture.StartsWith("/res:")) {
					string name = picture.Split(":")[1];
					
					picture = (program_dir + @"\res\img\") + name;
					if (! File.Exists(picture)) {
						Console.WriteLine("File: /res:" + name + ", doesn't exists");
						return;
					}
				}
				else {
					if (! File.Exists(picture)) {
						Console.WriteLine("File: " + picture + ", doesn't exists");
						return;
					}
				}
				int errors = makeMeme(picture, text, text2);
				if (errors == 0)
					Console.WriteLine("Completed Successfully");
			}
		}
		
		static int makeMeme(string picture, string text, string text2) {	
			SKFontManager fontManager = SKFontManager.CreateDefault();
			SKTypeface pressuru = fontManager.CreateTypeface(program_dir + @"\res\pressuru.otf");
			SKBitmap bitmap = SKBitmap.Decode(picture);
			SKCanvas canvas = new SKCanvas(bitmap);
			SKPaint paint = new SKPaint();
			SKRect rect = new SKRect();
			
			SKColor white = new SKColor(255, 255, 255);
			SKColor black = new SKColor(0, 0, 0);
			
			if (pressuru == null) {
				Console.WriteLine("Couldn't find pressuru font\nHalting program");
				return 1;
			}
			
			text = text.ToUpper();
			text2 = text2.ToUpper();
			
			paint.TextSize = 48f;
			paint.Typeface = pressuru;
			paint.IsAntialias = true;
			paint.StrokeWidth = 2;
			
			
			float y = 10;
			float sp = 0;
			float textHeight;
			
			paint.MeasureText("A", ref rect);
			textHeight = rect.Height;
			
			
			
			List<string> top_lines = wrapLines(text, paint, bitmap.Width);
			foreach (string line in top_lines) {
				paint.Color = white;
				paint.IsStroke = false;
				
				paint.MeasureText(line, ref rect);
				
				y += textHeight;
				
				canvas.DrawText(line, bitmap.Width/2f - rect.Width/2f, y + sp, paint);
				
				paint.IsStroke = true;
				paint.Color = black;
				
				canvas.DrawText(line, bitmap.Width/2f - rect.Width/2f, y + sp, paint);
				sp += 5;
			}

			List<string> bottom_lines =  wrapLines(text2, paint, bitmap.Width);
			int bottomMargin = 10; // space added to the bottom. the same for the top text.
			int linesCount = bottom_lines.Count;
			// no space is needed between 1 line
			int spaceBetweenLinesTotal = ((linesCount - 1) * 5);
			
			// raise the y high enough to print down
			y = bitmap.Height - textHeight * linesCount - spaceBetweenLinesTotal - bottomMargin;
			sp = 0;
			
			// bottom text
			foreach (string line in bottom_lines) {
				paint.Color = white;
				paint.IsStroke = false;
				
				paint.MeasureText(line, ref rect);
				
				y += textHeight;
				
				canvas.DrawText(line, bitmap.Width/2f - rect.Width/2f, y + sp, paint);
				
				paint.IsStroke = true;
				paint.Color = black;
				
				canvas.DrawText(line, bitmap.Width/2f - rect.Width/2f, y + sp, paint);
				sp += 5;
			}
			
			SKImage image = SKImage.FromBitmap(bitmap);
			SKData data = image.Encode(SKEncodedImageFormat.Png, 80);
			
			using (Stream stream = File.OpenWrite("meme.png")) {
				data.SaveTo(stream);
			}
			return 0;
        }
		
		static List<string> wrapLines(string text, SKPaint paint, float maxLength) {
			List<string> lines = new List<string>();
			StringBuilder line = new StringBuilder();
			SKRect rect = new SKRect();
			float spaceUsed;
			string[] words = text.Split(" ");
			
			foreach (string word in words) {
				paint.MeasureText(line.ToString() + word + " ", ref rect);
				spaceUsed = rect.Width;
				if (spaceUsed < maxLength) {
					line.Append(word + " ");		
				}
				else {
					lines.Add(line.ToString());
					line.Clear();
					line.Append(word + " ");
 				}
			}
			if (line.Length != 0) {
				lines.Add(line.ToString());
			}
			return lines;
		}
    }
}
