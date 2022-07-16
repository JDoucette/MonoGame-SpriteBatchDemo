// Jason Allen Doucette
// July 15, 2022
// SpriteBatch Demo

using System;

namespace SpriteBatchDemo
{
#if WINDOWS || LINUX
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			using (var game = new Game1())
				game.Run();
		}
	}
#endif
}