using System;
namespace UIDocument.Script.Core.Config {
    [Serializable]
    public class RoundConfig {
        /// <summary>
        /// 回合行动值最大值
        /// </summary>
        public ADT.ActionValue perActionValue;
        public ADT.ActionValue firstActionValue;

        /// <summary>
        /// 目标轮次
        /// </summary>
        public int needTurn;

        /// <summary>
        /// 最大轮次
        /// </summary>
        public int maxTurn;

        /// <summary>
        /// 回合运行帧率
        /// </summary>
        public int logicFrameRate;
    }
}
