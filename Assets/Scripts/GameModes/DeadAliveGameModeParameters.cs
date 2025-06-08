// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using UnityEngine;

namespace SGOL.GameModes
{
	[CreateAssetMenu(fileName = "DeadAliveGameModeParameters", menuName = "ScriptableObjects/DeadAliveGameModeParameters", order = 1)]
	public class DeadAliveGameModeParameters : ScriptableObject
	{
		[Header("Colors")]
		[SerializeField]
		private Color deadColor;

		public Color DeadColor => deadColor;

		[SerializeField]
		private Color aliveColor;

		public Color AliveColor => aliveColor;

		[Header("Ratios")]
		[SerializeField, Range(0.0f, 1.0f), Tooltip("The ratio of alive cells when randomizing the world")]
		private float aliveRatio = 0.5f;

		public float AliveRatio => aliveRatio;

		[Header("Shaders")]
		[SerializeField]
		private Material clearShader;

		public Material ClearShader => clearShader;

		[SerializeField]
		private Material gameShader;

		public Material GameShader => gameShader;
	}
}