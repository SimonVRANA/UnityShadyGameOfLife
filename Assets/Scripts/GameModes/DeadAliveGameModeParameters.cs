// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using UnityEngine;

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

	[Header("Shaders")]
	[SerializeField]
	private Material clearShader;

	public Material ClearShader => clearShader;

	[SerializeField]
	private Material gameShader;

	public Material GameShader => gameShader;
}