// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameServiceInstaller : MonoInstaller
{
	[Serializable]
	public class GameModeDictionaryElement
	{
		[SerializeField]
		private IGameService.GameModes category;

		public IGameService.GameModes Category
		{
			get => category;
			set => category = value;
		}

		[SerializeField]
		private DeadAliveGameModeParameters parameters;

		public DeadAliveGameModeParameters Parameters
		{
			get => parameters;
			set => parameters = value;
		}
	}

	[SerializeField]
	private List<GameModeDictionaryElement> gameModes;

	private GameService service = null;

	public override void InstallBindings()
	{
		if (service == null)
		{
			Dictionary<IGameService.GameModes, DeadAliveGameMode> gameModesAsDictionary = new();

			foreach (GameModeDictionaryElement gameModeDictionaryElement in gameModes)
			{
				if (!gameModesAsDictionary.ContainsKey(gameModeDictionaryElement.Category))
				{
					DeadAliveGameMode gameMode = new(gameModeDictionaryElement.Parameters);
					gameModesAsDictionary.Add(gameModeDictionaryElement.Category, gameMode);
				}
			}

			GameObject serviceGameObject = new("GameService");

			service = serviceGameObject.AddComponent<GameService>();
			service.Initialize(gameModesAsDictionary);
		}

		Container.BindInterfacesTo<GameService>().FromInstance(service);
		Container.QueueForInject(service);
	}
}