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
            Step(ref simulationData);
        }

        void RoundOver() {
            var simulationData = gameContext.dataModule.simulationData;
            simulationData.gameStatus = GameStatus.End;
            gameContext.logService.logger.Info(simulationData.Dump());
        }
        
        void Step(ref SimulationData simulationData) {
            IMove[] moves = simulationData.GetMoves();
            
            // 行动者消耗行动值
            foreach (IMove move in moves) {
                ActionComponent actionComponent = move.action;
                actionComponent.roundActionValue.Forward();
                if (actionComponent.roundActionValue.IsPass()) {
                    simulationData.actionQueue.Enqueue(move);
                }
            }
            
            // 结算行动
            while (simulationData.actionQueue.Count > 0) { 
                IMove move = simulationData.actionQueue.Dequeue();
                gameContext.logService.logger.Info("action");
                move.action.roundActionValue.Reset();
            }
            
            // 轮次消耗行动值
            List<TurnComponent> turns = simulationData.battleField.turns;
            TurnComponent turn = turns[simulationData.battleField.nowTurnIndex];
            turn.roundActionValue.Forward();

            // 推进轮次，校验轮次是否结束
            if (turn.roundActionValue.IsPass()) {
                simulationData.battleField.nowTurnIndex += 1;
            }

            if (simulationData.battleField.CheckNeedTurn()) {
                RoundOver();
            }
            else if (simulationData.battleField.CheckMaxTurn()) {
                RoundOver();
            }
        }
    }
}
