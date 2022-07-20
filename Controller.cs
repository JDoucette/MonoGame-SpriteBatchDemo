// Jason Allen Doucette
// July 19, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpriteBatchDemo
{
	public class Controller
	{
		// ---- constants

		// speed
		private readonly double zoomSpeed = 0.4534828;
		private readonly double rotateSpeed = 0.0;  // 0.2746593;


		// ---- properties

		public Vector2 GetPos { get { return pos; } }
		public float GetZoom { get { return zoom; } }
		public float GetRotate { get { return rotate; } }
		public bool GetUseSpriteSheet { get { return bUseSpriteSheet; } }
		public SamplerState GetSamplerState { get { return samplerState; } }


		// ---- data members

		private Game1 game;

		// controls
		private Vector2 pos = new Vector2(0, 0);
		private float zoom = 1.0f;
		private float rotate = 0.0f;
		private bool bUseSpriteSheet = true;
		private SamplerState samplerState = SamplerState.PointClamp;


		// ---- methods

		public Controller(Game1 game)
		{
			this.game = game;
		}

		public void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
				Keyboard.GetState().IsKeyDown(Keys.Escape))
				game.Exit();

			double timeTotal = gameTime.TotalGameTime.TotalSeconds;

			rotate = (float)(timeTotal * rotateSpeed);
			zoom = (float)(2.5 + 1.5 * Math.Sin(timeTotal * zoomSpeed));

			/*
			public static readonly SamplerState AnisotropicClamp;
			public static readonly SamplerState AnisotropicWrap;
			public static readonly SamplerState LinearClamp;
			public static readonly SamplerState LinearWrap;
			public static readonly SamplerState PointClamp;
			public static readonly SamplerState PointWrap;
			*/

		}

	}  // public class Controller
}  // namespace SpriteBatchDemo

