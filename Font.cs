// Jason Allen Doucette
// November 22, 2021
// Original simple font rendering from MonoGame Tutorial project.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteBatchDemo
{
	class Font
	{
		// ---- constant

		private const int lineSpacing = 1;
		private const int indexCharColorBase = 32768;  // 32768..65535 = 32768 = 15-bit = 5-bits for R,G,B


		// ---- enums

		public enum AlignHoriz
		{
			Left = 0,
			Center = -1,
			Right = -2,
		};

		public enum AlignVert
		{
			Top = 0,
			Center = -1,
			Bottom = -2,
		}


		// --- data members

		readonly Texture2D textFont;
		private Point sizeChar;
		private Point sizeCells;


		// ---- properties

		public int FontHeight { get { return sizeChar.Y; } }
		public int FontWidth { get { return sizeChar.X; } }


		// ---- methods

		public Font(Texture2D texture)
		{
			textFont = texture;
			GetCharArea(texture.Name);  // e.g. "Fonts\\font-jason-5x5-fixed"
			sizeCells = new Point(textFont.Width / sizeChar.X, textFont.Height / sizeChar.Y);
		}

		private void GetCharArea(string fontNameWithDimensions)
		{
			const int numChars = 128 - 32;  // assume all fonts have this
			int areaTotal = textFont.Width * textFont.Height;
			Debug.Assert(areaTotal % numChars == 0);
			int areaChar = areaTotal / numChars;

			// attempt #1 - get size from font name
			// note that font name will be the size of the font, not the cell (1 pixel smaller in X and Y)
			GetTwoNumbersFromString(fontNameWithDimensions, out int x, out int y);
			if ((x + 1) * (y + 1) == areaChar)
				sizeChar = new Point(x + 1, y + 1);
			else if (x * y == areaChar)
				sizeChar = new Point(x, y);
			else
			{
				// attempt #2 - get size from known cell name
				switch (areaChar)
				{
					case 36: sizeChar = new Point(6, 6); break;
					case 42: sizeChar = new Point(6, 7); break;
					//case 42: sizeChar = new Point(7, 6); break; -- cannot have two of the same number
					case 49: sizeChar = new Point(7, 7); break;
					case 64: sizeChar = new Point(8, 8); break;
					case 72: sizeChar = new Point(8, 9); break;
					default: Debug.Assert(false); break;
				}
			}
		}

		// returns 0 if not found
		public void GetTwoNumbersFromString(string str, out int x, out int y)
		{
			char space = ' ';
			// change all non-numeric character in string to spaces
			StringBuilder s = new StringBuilder(str);
			for (int i = 0; i < s.Length; i++)
				if (s[i] < '0' || s[i] > '9')
					s[i] = space;

			// split string into numbers,
			// note that the spaces next to each other creates nulls
			String[] tokens = s.ToString().Split(space);
			int[] numbers = new int[2];

			int indexNumbers = 0;
			for (int i = 0; i < tokens.Length; i++)
				if (Int32.TryParse(tokens[i], out int result))
				{
					numbers[indexNumbers++] = result;
					if (indexNumbers == 2)
						break;
				}
			Debug.Assert(indexNumbers == 2);  // should have found two numbers in the filename

			// set outs
			x = numbers[0];
			y = numbers[1];
		}

		public Vector2 GetSize(StringBuilder str)
		{
			Vector2 size = new Vector2(0f, sizeChar.Y);
			float maxX = 0.0f;
			for (int i = 0; i < str.Length; i++)
			{
				int c = (int)str[i];
				if (c == '\n')
				{
					size.Y += sizeChar.Y + lineSpacing;
					if (size.X > maxX) maxX = size.X;  // store x width
					size.X = 0;
					continue;
				}
				// else if ... color encoding -- not required, as it takes no visible space
				else if (c < 32 || c >= 128)
					continue;
				size.X += sizeChar.X;
			}
			if (size.X > maxX) maxX = size.X;  // store x width

			// reset size to the maximum width of any row
			size.X = maxX;
			return size;
		}

		// shadow:
		// 0 = 1x render = none
		// 1 = 2x render = bottom-right
		// 2 = 4x render = bottom, right, and bottom-right
		public void Draw(SpriteBatch spriteBatch, StringBuilder str, Vector2 position, Color color,
			AlignHoriz alignHoriz = AlignHoriz.Left, AlignVert alignVert = AlignVert.Top, int shadow = 1)
		{
			byte alpha = color.A;
			Debug.Assert(shadow >= 0);
			Debug.Assert(shadow <= 2);
			Color shadowColor = new Color((byte)0, (byte)0, (byte)0, alpha);  // should be full alpha since we overwrite pixels

			// adjust for alignment
			Vector2 size = GetSize(str);
			position.X += ((int)alignHoriz) * size.X * 0.5f;
			position.Y += ((int)alignVert) * size.Y * 0.5f;

			if (shadow >= 1)
				DrawSingleRender(spriteBatch, str, position + Vector2.One, shadowColor, shadow: true);
			if (shadow == 2)
			{
				DrawSingleRender(spriteBatch, str, position + new Vector2(1, 0), shadowColor, shadow: true);
				DrawSingleRender(spriteBatch, str, position + new Vector2(0, 1), shadowColor, shadow: true);
			}
			DrawSingleRender(spriteBatch, str, position, color);
		}

		private void DrawSingleRender(SpriteBatch spriteBatch, StringBuilder str, Vector2 position, Color color, bool shadow = false)
		{
			byte alpha = color.A;
			float startX = position.X;
			for (int i = 0; i < str.Length; i++)
			{
				int c = (int)str[i];
				if (c == '\n')
				{
					position.X = startX;
					position.Y += sizeChar.Y + lineSpacing;
					continue;
				}
				else if ((c >= indexCharColorBase) && (!shadow))
				{
					color = InvChar(c);  // override passed in color
					color.A = alpha;
					continue;
				}
				else if (c < 32 || c >= 128)
					continue;
				c -= 32;
				Point posChar = new Point(c % sizeCells.X, c / sizeCells.X);
				Rectangle sourRect = new Rectangle(
					posChar.X * sizeChar.X,
					posChar.Y * sizeChar.Y,
					sizeChar.X,
					sizeChar.Y);
				spriteBatch.Draw(textFont, position, sourRect, color);
				position.X += sizeChar.X;
			}
		}

		// characters are stored in 16-bit
		// we use: 32..127 for printable characters, some of 0..31 for control characters (basically '\n'),
		// some of 128..255 for printable character, like ASCII #248: degree symbol: ° 
		// so we have 65536 - 256 = 65280, basically have 15 bits for the colors, which is 5-bits for R,G,B.
		// 5-bits are 2^5 = 32 shades
		static public char Char(Color color)
		{
			// EGA: 1 = blue, 2 = green, 4 = red, let's follow the same
			// 0..255 / 8 = 0..31 = 5-bit
			int index = (color.R / 8) * 1024 + (color.G / 8) * 32 + (color.B / 8);  // 2^15 = 0..32767
			Debug.Assert(index >= 0);
			Debug.Assert(index < 32768);
			index += indexCharColorBase;
			return (char)index;
		}

		static public char Char(int r, int g, int b)
		{
			return Char(new Color(r, g, b));
		}

		// index = 0..26
		static public Color InvChar(int index)
		{
			Debug.Assert(index >= indexCharColorBase);
			Debug.Assert(index < indexCharColorBase + 32768);
			index -= indexCharColorBase;
			int b = index % 32;
			int g = index / 32 % 32;
			int r = index / 1024;
			// TODO -- should I make a 0..31 mapping to 0..255?
			return new Color(r * 8, g * 8, b * 8);  // 0..31 * 8 = 248
		}

	}  // class Font
}  // namespace SpriteBatchDemo
