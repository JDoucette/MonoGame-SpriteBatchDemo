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
	public class Art
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

		private void GetTileColors(out Color colorTileEdge, out Color colorTileChecker1, out Color colorTileChecker2)
		{
			colorTileEdge = new Color(
				64 + MathUtil.rng.Next(192),
				64 + MathUtil.rng.Next(192),
				0 + MathUtil.rng.Next(128));
			colorTileChecker1 = new Color(
				64 + MathUtil.rng.Next(64),
				64 + MathUtil.rng.Next(64),
				0 + MathUtil.rng.Next(64));
			colorTileChecker2 = new Color(0, 0, 0);
		}

		public Texture2D CreateSpriteSheetTexture(Point sizeTile_pixels, Point sizeSpriteSheet_tiles)
		{
			Point sizeSpriteTexture = new Point(
				sizeSpriteSheet_tiles.X * sizeTile_pixels.X, 
				sizeSpriteSheet_tiles.Y * sizeTile_pixels.Y);
			int numPixels = sizeSpriteTexture.X * sizeSpriteTexture.Y;
			Color[] colors = new Color[numPixels];

			for (int yTile = 0; yTile < sizeSpriteSheet_tiles.Y; yTile++)
				for (int xTile = 0; xTile < sizeSpriteSheet_tiles.X; xTile++)
				{
					bool invisibleTile = MathUtil.rng.NextDouble() < 0.5;
					GetTileColors(out Color colorTileEdge, out Color colorTileChecker1, out Color colorTileChecker2);
					if (invisibleTile)
					{
						colorTileEdge = new Color(0, 0, 0, 0);
						colorTileChecker1 = new Color(0, 0, 0, 0);
						colorTileChecker2 = new Color(0, 0, 0, 0);
					}
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
							bool parityPixel = ((xPixel + yPixel) & 1) == 1;
							int indexPixelColor = (pixel.Y * sizeSpriteTexture.X) + pixel.X;
							colors[indexPixelColor] =
								(edge
									? colorTileEdge
									: (parityPixel ? colorTileChecker1 : colorTileChecker2));
						}
				}

			Texture2D texture = new Texture2D(graphicsDevice, sizeSpriteTexture.X, sizeSpriteTexture.Y);
			texture.SetData<Color>(colors);
			return texture;
		}

		internal List<Texture2D> CreateIndividualSprites(int numSprites, Point sizeTile)
		{
			List<Texture2D> textures = new List<Texture2D>(numSprites);
			for (int i = 0; i < numSprites; i++)
				textures.Add(CreateSingleSprite(sizeTile));
			return textures;
		}

		private Texture2D CreateSingleSprite(Point sizeTile)
		{
			GetTileColors(out Color colorTileEdge, out Color colorTileChecker1, out Color colorTileChecker2);

			int numPixels = sizeTile.X * sizeTile.Y;
			Color[] colors = new Color[numPixels];
			for (int yPixel = 0; yPixel < sizeTile.X; yPixel++)
				for (int xPixel = 0; xPixel < sizeTile.Y; xPixel++)
				{
					bool edge = (
						xPixel == 0 ||
						yPixel == 0 ||
						xPixel == sizeTile.X - 1 ||
						yPixel == sizeTile.Y - 1);
					bool parityPixel = ((xPixel + yPixel) & 1) == 1;
					colors[yPixel * sizeTile.X + xPixel] = (edge
							? colorTileEdge
							: (parityPixel ? colorTileChecker1 : colorTileChecker2));
				}
			Texture2D texture = new Texture2D(graphicsDevice, sizeTile.X, sizeTile.Y);
			texture.SetData<Color>(colors);
			return texture;
		}

	}  // 	class Art
}  // namespace SpriteBatchDemo

