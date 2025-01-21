using UIDocument.Script.RoundSystem.ADT;
using UnityEngine;
namespace UIDocument.Script.RoundSystem.Config {
    [CreateAssetMenu(fileName = "RoundConfig", menuName = "RoundConfig", order = 0)]
    public class RoundConfig : ScriptableObject {
        /// <summary>
        /// 回合行动值最大值
        /// </summary>
        public ActionValue perActionValue;
        public ActionValue firstActionValue;

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
