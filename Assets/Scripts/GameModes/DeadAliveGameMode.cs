// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using UnityEngine;

namespace SGOL.GameModes
{
	public class DeadAliveGameMode
	{
		[SerializeField]
		private DeadAliveGameModeParameters parameters;

		public DeadAliveGameMode(DeadAliveGameModeParameters parameters)
		{
			this.parameters = parameters;
		}

		public Color AliveColor => parameters.AliveColor;

		public Color DeadColor => parameters.DeadColor;

		public float AliveRatio => parameters.AliveRatio;

		public Material GetClearShader()
		{
			parameters.ClearShader.SetColor("_Color", DeadColor);
			return parameters.ClearShader;
		}

		public Material GetGameShader()
		{
			parameters.GameShader.SetColor("_DeadColor", DeadColor);
			parameters.GameShader.SetColor("_AliveColor", AliveColor);
			return parameters.GameShader;
		}
	}
}