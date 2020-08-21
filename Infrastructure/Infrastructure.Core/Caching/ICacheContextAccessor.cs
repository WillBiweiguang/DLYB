namespace Infrastructure.Core.Caching {
    public interface ICacheContextAccessor {
        IAcquireContext Current { get; set; }
    }
}