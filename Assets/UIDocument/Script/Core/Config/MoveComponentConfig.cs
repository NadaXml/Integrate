using System;
namespace UIDocument.Script.Core.Config {
    [Serializable]
    public class MoveComponentConfig {
        /// <summary>
        /// 速度
        /// </summary>
        public int speed;
        /// <summary>
        /// 行动位置
        /// </summary>
        public int position;
        /// <summary>
        /// 不应该在这边
        /// </summary>
        public int dmg;
    }
}
