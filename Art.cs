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

		public Texture2D CreateSpriteSheetTexture(Point sizeTile_pixels, Point sizeSpriteSheet_tiles)
		{
			Point sizeSpriteTexture = new Point(
				sizeSpriteSheet_tiles.X * sizeTile_pixels.X, 
				sizeSpriteSheet_tiles.Y * sizeTile_pixels.Y);
			int numPixels = sizeSpriteTexture.X * sizeSpriteTexture.Y;
			Color[] colors = new Color[numPixels];

			Random rng = new Random();

			for (int yTile = 0; yTile < sizeSpriteSheet_tiles.Y; yTile++)
				for (int xTile = 0; xTile < sizeSpriteSheet_tiles.X; xTile++)
				{
					Color colorTile = new Color(
						64 + rng.Next(192),
						64 + rng.Next(192),
						64 + rng.Next(192));
					Color colorTile2 = new Color(
						64 + rng.Next(64),
						64 + rng.Next(64),
						64 + rng.Next(64));
					colorTile = new Color(255, 0, 0, 255);
					colorTile2 = new Color(0, 0, 255, 255);
					Point spriteTopLeft = new Point(
						xTile * sizeTile_pixels.X, 
						yTile * sizeTile_pixels.Y);
					for (int yPixel = 0; yPixel < sizeTile_pixels.Y; yPixel++)
						for (int xPixel = 0; xPixel < sizeTile_pixels.X; xPixel++)
						{
							Point pixel = spriteTopLeft + new Point(xPixel, yPixel);
							bool edge = (
								xPixel == 0 || 
								yPixel == 0 || 
								xPixel == sizeTile_pixels.X - 1 || 
								yPixel == sizeTile_pixels.Y - 1);
							bool parity = ((xPixel + yPixel) & 1) == 1;
							int indexPixelColor = (pixel.Y * sizeSpriteTexture.X) + pixel.X;
							colors[indexPixelColor] =
								(edge
									? colorTile
									: (parity
										? colorTile2
										: Color.Black));
						}
				}

			Texture2D texture = new Texture2D(graphicsDevice, sizeSpriteTexture.X, sizeSpriteTexture.Y);
			texture.SetData<Color>(colors);
			return texture;
		}

	}
}
