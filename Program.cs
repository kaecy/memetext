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
        static void Main(string[] args)
        {
			if (args.Length == 3) {
				string picture = args[0];
				string text = args[1];
				string text2 = args[2];
				
				int errors = makeMeme(picture, text, text2);
				if (errors == 0)
					Console.WriteLine("Completed Successfully");
			}
		}
		
		static int makeMeme(string picture, string text, string text2) {	
			String program_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			SKFontManager fontManager = SKFontManager.CreateDefault();
			SKTypeface pressuru = fontManager.CreateTypeface(program_dir + @"\res\pressuru.otf");
			SKBitmap bitmap = SKBitmap.Decode(picture);
			SKImage image = SKImage.FromBitmap(bitmap);
			SKCanvas canvas = new SKCanvas(bitmap);
			SKPaint paint = new SKPaint();
			SKRect rect = new SKRect();
			
			if (pressuru == null) {
				Console.WriteLine("Couldn't find pressuru font\nHalting program");
				return 1;
			}
			
			text = text.ToUpper();
			text2 = text2.ToUpper();
			
			paint.TextSize = 48f;
			paint.Typeface = pressuru;
			paint.IsAntialias = true;
			float y = 10;
			float sp = 0;
			
			
			List<string> top_lines = wrapLines(text, paint, image.Width);
			foreach (string line in top_lines) {
				paint.Color = new SKColor(0xff, 0xff, 0xff);
				paint.IsStroke = false;
				
				paint.MeasureText(line, ref rect);
				
				y += rect.Height;
				
				canvas.DrawText(line, image.Width/2.0f - rect.Width/2.0f, y + sp, paint);
				
				paint.IsStroke = true;
				paint.StrokeWidth = 2;
				paint.Color = new SKColor(0, 0, 0);
				
				canvas.DrawText(line, image.Width/2.0f - rect.Width/2.0f, y + sp, paint);
				sp += 5;
			}


			List<string> bottom_lines =  wrapLines(text2, paint, image.Width);
			int bottomMargin = 10;
			float textHeight;
			int linesCount = bottom_lines.Count;
			int spaceBetweenLinesSize = ((linesCount - 1) * 5);
			paint.MeasureText("A", ref rect);
			textHeight = rect.Height;
			y = image.Height - textHeight * linesCount - spaceBetweenLinesSize - bottomMargin;
			sp = 0;
			
			// bottom text
			foreach (string line in bottom_lines) {
				paint.Color = new SKColor(0xff, 0xff, 0xff);
				paint.IsStroke = false;
				
				paint.MeasureText(line, ref rect);
				
				y += rect.Height;
				
				canvas.DrawText(line, image.Width/2f - rect.Width/2f, y + sp, paint);
				
				paint.IsStroke = true;
				paint.StrokeWidth = 2;
				paint.Color = new SKColor(0, 0, 0);
				
				canvas.DrawText(line, image.Width/2f - rect.Width/2f, y + sp, paint);
				sp += 5;
			}
			
			image = SKImage.FromBitmap(bitmap);
			SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 80);
			
			using (Stream stream = File.OpenWrite("meme.jpg")) {
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
