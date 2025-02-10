using System.Collections.Generic;
using UIDocument.Script.Core.Config;
using UIDocument.Script.EventService;
namespace UIDocument.Script.Core.ADT {
    
    // 事件定义走代码生成
    public abstract class EventNameDef {
        public const string N_DumpRound = "dump_round";
        public const string N_DumpRoundInspector = "dump_round_inspector";
        public const string N_OnRoundOption = "on_round_option";
        public const string N_CreateRound = "create_round";
        public const string N_CreateMove = "create_move";
        public const string N_CreateBattle = "create_battle";
        public const string N_StartGame = "start_game";
        public const string N_AnalyticsDmg = "analytics_dmg";
        public const string N_AnalyticsMoveCount = "analytics_moveCount";
        public const string N_OnRoundOver = "on_round_over";
        
        public const int ID_InValid = 0;
        public const int ID_DumpRound = 1;
        public const int ID_DumpRoundInspector = 2;
        public const int ID_OnRoundOption = 3;
        public const int ID_CreateRound = 4;
        public const int ID_CreateMove = 5;
        public const int ID_CreateBattle = 7;
        public const int ID_StartGame = 8;
        public const int ID_AnalyticsDmg = 9;
        public const int ID_AnalyticsMoveCount = 10;
        public const int ID_OnRoundOver = 11;
    }

    public class DefaultEvent : GameEventBase {
        public DefaultEvent() {
            EventId = 0;
        }
    }
    
    public class DumpRoundInspectorEvent : GameEventBase {

        // 用内存流
        public MoveComponentStream result;
        
        public DumpRoundInspectorEvent() {
            EventId = EventNameDef.ID_DumpRoundInspector;
        }
    }

    public class OnRoundOptionEvent : GameEventBase {

        public MoveComponent component;
        
        public OnRoundOptionEvent() {
            EventId = EventNameDef.ID_OnRoundOption;
        }
    }
    
    public class StartGameEvent : GameEventBase {
        public StartGameEvent() {
            EventId = EventNameDef.ID_StartGame;
        }
    }

    public class CreateRoundEvent : GameEventBase {
        public RoundConfig roundConfig;
        
        public CreateRoundEvent() {
            EventId = EventNameDef.ID_CreateRound;
        }
    }

    public class CreateMoveEvent : GameEventBase {

        public MoveComponent[] components;
        
        public CreateMoveEvent() {
            EventId = EventNameDef.ID_CreateMove;
        }
    }

    public class CreateBattleEvent : GameEventBase {
        public BattleComponent[] components;

        public CreateBattleEvent() {
            EventId = EventNameDef.ID_CreateBattle;
        }
    }

    public class AnalyticsDmgEvent : GameEventBase {
        public BattleComponent battleComponent;
        public AnalyticsDmgEvent() {
            EventId = EventNameDef.ID_AnalyticsDmg;
        }
    }

    public class AnalyticsMoveCountEvent : GameEventBase {
        public MoveComponent moveComponent;
        public AnalyticsMoveCountEvent() {
            EventId = EventNameDef.ID_AnalyticsMoveCount;
        }
    }
}
