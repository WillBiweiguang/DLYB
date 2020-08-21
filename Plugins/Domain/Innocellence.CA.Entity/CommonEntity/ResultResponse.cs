namespace DLYB.CA.Contracts.CommonEntity
{
    public class ResultResponse<TEntity, TStatus>
    {
        public TEntity Entity { get; set; }

        public TStatus Status { get; set; }
    }
}
