// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using UnityEngine;
using Zenject;

namespace SGOL.Services.Image
{
	public class ImageServiceInstaller : MonoInstaller
	{
		[Header("Links")]
		[SerializeField]
		private UnityEngine.UI.Image gameImage;

		public override void InstallBindings()
		{
			Container.BindInterfacesTo<ImageService>()
					 .AsSingle()
					 .WithArguments(gameImage);
		}
	}
}