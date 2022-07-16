// Jason Allen Doucette
// July 16, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteBatchDemo
{
	class Art
	{
		// ---- data members

		private GraphicsDevice graphicsDevice;


		// ---- methods

		public Art(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
		}

		public Texture2D CreateWhiteTexture(int sizeEdge)
		{
			int numPixels = sizeEdge * sizeEdge;
			Color[] colors = new Color[numPixels];
			for (int y = 0; y < sizeEdge; y++)
				for (int x = 0; x < sizeEdge; x++)
					colors[y * sizeEdge + x] = Color.White;
			Texture2D texture = new Texture2D(graphicsDevice, sizeEdge, sizeEdge);
			texture.SetData<Color>(colors);
			return texture;
		}

		public Texture2D CreateSpriteTexture(Point sizeSpriteTexture)
		{
			int numPixels = sizeSpriteTexture.X * sizeSpriteTexture.Y;
			Color[] colors = new Color[numPixels];
			for (int y = 0; y < sizeSpriteTexture.Y; y++)
				for (int x = 0; x < sizeSpriteTexture.X; x++)
				{
					bool edge = (x == 0 || y == 0 || x == sizeSpriteTexture.X - 1 || y == sizeSpriteTexture.Y - 1);
					int r = (edge ? 255 : 0);
					bool parity = ((x + y) & 1) == 1;
					int g = (parity ? 255 : 0);
					int b = (parity ? 255 : 0);
					colors[y * sizeSpriteTexture.X + x] = new Color(r, g, b);
				}
			Texture2D texture = new Texture2D(graphicsDevice, sizeSpriteTexture.X, sizeSpriteTexture.Y);
			texture.SetData<Color>(colors);
			return texture;
		}


	}
}
