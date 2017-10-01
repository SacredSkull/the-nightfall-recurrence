using System;
using System.Collections.Generic;
using System.Threading;
using App.Presenters;
using Presenters.Layouts;
using Karma;
using Level;
using Models;
using ModestTree;
using UnityEngine;
using Utility.Collections.Grid;
using Presenters;
using UniRx;
using UnityEngine.UI;
using Zenject;
using Logger = UnityUtilities.Logger;

namespace Controllers {
    public sealed class MessageEvent {
        public static readonly string GRID_PIECE_CHANGED = "GRID_PIECE_CHANGED";
        public static readonly string GLOBAL_DATA_RELOAD = "GLOBAL_DATA_RELOAD";
    }

    public interface IMainApp {}

    //[ExecuteInEditMode]
    public class ActiveLevel : Karma.App, IMainApp {
        public bool dumpLevelXML;
        public bool dumpProgramXML;
        public bool dumpEnemyXML;
        public bool dumpFeatureXML;
        public bool dumpLoadedMapItems;
        public bool dumpProgramAbilities;
        public bool dumpGeometry;
        public bool dumpEntities;
        public bool dumpTileSets;
        public bool dumpLevelEntities;
	    public LevelModel level;
	    public EntityModel entities;
        
        private UniRx.IObservable<Unit> loadingTask;

        public override void Configure(IApplication app, DiContainer container) {
            app.UseLayout(true);
            container.Bind<IMainApp>().FromInstance(this);
            if (Application.isEditor && !Application.isPlaying) {
                // Use a fake database to speed up "live" tests
                container.Bind<IDatabase>().To<FakeDatabase>().AsSingle();
            } else {
                // Use the _real_ database
                container.Bind<IDatabase>().To<XMLDatabase>().AsSingle();
            }

            container.Bind<LevelModel>().AsSingle();
            container.Bind<EntityModel>().AsSingle();
            container.Bind<Utility.ILogger>().To<Logger>().AsSingle();

            level = container.Resolve<LevelModel>();
            entities = container.Resolve<EntityModel>();

            loadingTask = Observable.Start(() => {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                level.Load();
                watch.Stop();
                Debug.Log($"Took {watch.ElapsedMilliseconds} ms to load all content");
            });
        }

        public override void Init(IRouter router, DiContainer container) {
            level.LevelLoaded += () => {
                container.Bind<IGridGraph>().FromInstance(level.graph).AsSingle();
                container.Bind<IDictionary<string, Sprite>>().WithId("Sprites").FromInstance(level.Sprites).AsSingle();
                container.Bind<ILayeredGrid<MapItem>>().FromInstance(level.LayeredGrid).AsSingle();
                router.GoTo(LevelPresenter.Path);
            };
            Observable.WhenAll(loadingTask)
                .ObserveOnMainThread()
                .Subscribe(x => {

                });
        }

        private void ReadData() {
	        EntityModel.dumpEnemyXML = dumpEnemyXML;
	        EntityModel.dumpFeatureXML = dumpFeatureXML;
	        EntityModel.dumpProgramAbilities = dumpProgramAbilities;
	        EntityModel.dumpProgramXML = dumpProgramXML;

	        LevelModel.dumpEntities = dumpEntities;
	        LevelModel.dumpLevelEntities = dumpLevelEntities;
	        LevelModel.dumpGeometry = dumpGeometry;
	        LevelModel.dumpLevelXML = dumpLevelXML;
	        LevelModel.dumpLoadedMapItems = dumpLoadedMapItems;
            LevelModel.dumpTileSets = dumpTileSets;
        }

        public override void OnPresenterDestroy() {

        }
    }
}
