using System;
namespace UIDocument.Script.RoundSystem.ADT {
    [Serializable]
    public struct Speed {
        /// <summary>
        /// 速度：放大100倍的频率
        /// </summary>
        public int value;
        
        public static Speed FromValue(int value) {
            return new Speed {
                value = value
            };
        }
    }
}
