namespace AppFrame {
    public interface IComponent {
        ulong actorSequenceId { get; set; }
        ulong sequenceId { get; set; }
    }

    public static class ComponentExtension {
        public static bool IsSameActor<T1>(this IComponent component1, T1 component2) where T1 : IComponent {
            return component1.actorSequenceId == component2.actorSequenceId;
        }
    }
}
