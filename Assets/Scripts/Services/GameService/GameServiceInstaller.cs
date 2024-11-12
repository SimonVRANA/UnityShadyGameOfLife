// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using UnityEngine;
using Zenject;

public class GameServiceInstaller : MonoInstaller
{
	[Header("GameOfLife")]
	[SerializeField]
	private Color deadColor;

	[SerializeField]
	private Color aliveColor;

	[Header("Shaders")]
	[SerializeField]
	private Material clearShader;

	[SerializeField]
	private Material gameOfLifeShader;

	private GameService service = null;

	public override void InstallBindings()
	{
		if (service == null)
		{
			GameObject serviceGameObject = new("GameService");

			service = serviceGameObject.AddComponent<GameService>();
			service.Initialize(deadColor, aliveColor, clearShader, gameOfLifeShader);
		}

		Container.BindInterfacesTo<GameService>().FromInstance(service);
		Container.QueueForInject(service);
	}
}