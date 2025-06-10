using adt;
using System.Collections.Generic;
namespace game_logic.system {
    public class Round : GameSystem {
        protected override void OnUpdate() {
            base.OnUpdate();

            SimulationData simulationData = gameContext.dataModule.simulationData;
            if (simulationData.gameStatus != GameStatus.Running) {
                return;
            }
            Step(simulationData);
        }

        void RoundOver() {
            var simulationData = gameContext.dataModule.simulationData;
            simulationData.gameStatus = GameStatus.End;
            gameContext.logService.logger.Info(simulationData.Dump());
        }

        unsafe void HandleMove(IMove move, SimulationData simulationData) {
            ActionComponent actionComponent = move.action;
            ActionComponent* p = &actionComponent;
            p->roundActionValue.Forward();
            if (p->roundActionValue.IsPass()) {
                simulationData.actionQueue.Enqueue(move);
            }
            move.action = actionComponent;
        }

        unsafe void SettleMove(SimulationData simulationData) {
            while (simulationData.actionQueue.Count > 0) { 
                IMove move = simulationData.actionQueue.Dequeue();
                gameContext.logService.logger.Info("action");
                ActionComponent action = move.action;
                ActionComponent* p = &action;
                p->roundActionValue.Reset();
                move.action = action;
            }
        }

        unsafe void HandleTurn(SimulationData simulationData) {
            List<TurnComponent> turns = simulationData.battleField.turns;
            TurnComponent turn = turns[simulationData.battleField.nowTurnIndex];
            TurnComponent* p = &turn;
            p->roundActionValue.Forward();
            turns[simulationData.battleField.nowTurnIndex] = turn;
            
            // 推进轮次，校验轮次是否结束
            if (turn.roundActionValue.IsPass()) {
                simulationData.battleField.nowTurnIndex += 1;
            }
        }
        
        void Step(SimulationData simulationData) {
            IMove[] moves = simulationData.GetMoves();
            
            // 行动者消耗行动值
            RoundActionValue roundActionValue;
            foreach (IMove move in moves) {
                HandleMove(move, simulationData);
            }
            
            // 结算行动
            SettleMove(simulationData);
            
            // 轮次消耗行动值
            HandleTurn(simulationData);

            if (simulationData.battleField.CheckNeedTurn()) {
                RoundOver();
            }
            else if (simulationData.battleField.CheckMaxTurn()) {
                RoundOver();
            }
        }
    }
}
