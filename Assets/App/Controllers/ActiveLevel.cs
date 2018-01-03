using System.Collections.Generic;
using App.Presenters;
using Karma;
using Level;
using Models;
using UniRx;
using UnityEngine;
using UnityUtilities.Collections.Grid;
using Zenject;
using ILogger = UnityUtilities.Management.ILogger;

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
        
        private UniRx.IObservable<Unit> loadingTask;

        public override void Configure(IApplication app, DiContainer container) {
            container.Bind<IMainApp>().FromInstance(this);
            ILogger logger = container.Resolve<ILogger>();

            level = container.Resolve<LevelModel>();
            //container.Bind<Func<ILayeredGrid<MapItem>>>().FromInstance(() => level.LayeredGrid);
            
            loadingTask = Observable.Start(() => {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                level.Load();
                watch.Stop();
                logger.Log($"Took {watch.ElapsedMilliseconds} ms to load all content");
            });
            
            app.UseLayout(true);
        }

        public override void Init(IRouter router, DiContainer container) {
            level.LoadedEvent.Subscribe(() => {
                container.Bind<GridGraph<MapItem>>().FromInstance(level.graph).AsSingle();
                container.Bind<IDictionary<string, Sprite>>().WithId("Sprites").FromInstance(level.Sprites).AsSingle();
                container.Bind<ILayeredGrid<MapItem>>().FromInstance(level.LayeredGrid).AsSingle();
                router.GoTo(LevelPresenter.Path);
            });
            Observable.WhenAll(loadingTask)
                .ObserveOnMainThread()
                .Subscribe(x => { });
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
