namespace UIDocument.Script.Core.ADT {
    public enum RoundStatus {
        None        = 0,    // 初始状态
        Running     = 1,    // 运行中
        Animation   = 2,    // 回合事件表现阶段
        Option      = 3,    // 回合选择中
        Pause       = 4,    // 暂停表现
    }
}
