using AppFrame;
using System.Collections;
using UIDocument.Script.Core.ADT;
using UIDocument.Script.EventService;
using Unity.Collections;
namespace UIDocument.Script.BattleSystem {
    public class BattleSystem : ISystem {

        public void Update(float deltaTime) {
            // throw new System.NotImplementedException();
        }
        public void Awake() {
            _context.eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_CreateBattle, OnCreateBattle);
            _context.eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_OnRoundOption, OnRoundOption);
        }
        public void Destroy() {
            _context.eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_CreateBattle, OnCreateBattle);
            _context.eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_OnRoundOption, OnRoundOption);
            _battleComponents.Dispose();
        }
        public IEnumerator Start() {
            yield return null;
        }

        public struct Context {
            public EventService.EventServiceProvider eventServiceProvider;
        }

        Context _context;
        
        NativeArray<BattleComponent> _battleComponents;

        public BattleSystem(in Context context) {
            _context = context;
        }

        void OnCreateBattle(object sender, GameEventBase e) {
            CreateBattleEvent evt = e as CreateBattleEvent;
            _battleComponents = new NativeArray<BattleComponent>(evt.components.Length, Allocator.Persistent);
            _battleComponents.CopyFrom(evt.components);
        }

        void OnRoundOption(object sender, GameEventBase e) {
            OnRoundOptionEvent evt = e as OnRoundOptionEvent;
            foreach (BattleComponent battleComponent in _battleComponents) {
                if (battleComponent.IsSameActor(evt.component)) {
                    var evt2 = new AnalyticsDmgEvent();
                    evt2.battleComponent = battleComponent;
                    _context.eventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_AnalyticsDmg, evt2);
                }
            }
        }
    }
}
