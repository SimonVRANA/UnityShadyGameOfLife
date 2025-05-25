// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using SGOL.GameModes;
using SGOL.Services.Image;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SGOL.Services.Game
{
	public class GameServiceInstaller : MonoInstaller
	{
		[Serializable]
		public class GameModeDictionaryElement
		{
			[SerializeField]
			private string name;

			public string Name
			{
				get => name;
				set => name = value;
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
				Dictionary<string, DeadAliveGameMode> gameModesAsDictionary = new();

				foreach (GameModeDictionaryElement gameModeDictionaryElement in gameModes)
				{
					if (!gameModesAsDictionary.ContainsKey(gameModeDictionaryElement.Name))
					{
						DeadAliveGameMode gameMode = new(gameModeDictionaryElement.Parameters);
						gameModesAsDictionary.Add(gameModeDictionaryElement.Name, gameMode);
					}
				}

				GameObject serviceGameObject = new("GameService");

				service = serviceGameObject.AddComponent<GameService>();
				service.Initialize(gameModesAsDictionary, Container.Resolve<IImageService>());
			}

			Container.BindInterfacesTo<GameService>().FromInstance(service);
			Container.QueueForInject(service);
		}
	}
}