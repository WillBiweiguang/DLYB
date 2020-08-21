namespace DLYB.CA.Contracts.CommonEntity
{
    public class Result<T>
    {
        public int Status { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }
    }
}
