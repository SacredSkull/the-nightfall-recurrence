using System;
using Controllers;
using Level;
using Presenters;
using UnityEngine;
using Zenject;

namespace Installers {
	public class AssetInstaller : MonoInstaller {
		public OverlayAssets OverlaySpriteAssetSettings;
		public SoundAssets SoundAssetsSettings;
		
		public override void InstallBindings() {
			Container.Bind<OverlayController>().AsSingle();
			Container.BindInstance(OverlaySpriteAssetSettings);
			Container.BindInstance(SoundAssetsSettings);
			Container.Bind<SoundPresenter>().FromInstance(FindObjectOfType<SoundPresenter>()).AsSingle();
		}
		
		[Serializable]
		public class OverlayAssets {
			public Sprite AttackSprite;
			public Sprite AugmentSprite;
			public Sprite SelectionSprite;
			public Sprite MovementSprite;
			public Sprite UpDirectionalSprite;
			public Sprite DownDirectionalSprite;
			public Sprite LeftDirectionalSprite;
			public Sprite RightDirectionalSprite;
			public Sprite MissingSprite;
			public Sprite EmptySprite = MapItem.BlankTile.sprite;
		}

		[Serializable]
		public class SoundAssets {
			public AudioClip Hit;
			public AudioClip Movement;
		}
	}
}