namespace Infrastructure.Core.Caching {
    public interface IVolatileToken {
        bool IsCurrent { get; }
    }
}