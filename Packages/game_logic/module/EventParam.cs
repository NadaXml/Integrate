namespace game_logic.module {
    public interface EventParam {
        
    }

    public class EventParam<T> : EventParam 
        where T : struct {
        public T extra { get; set; }
    }
}
