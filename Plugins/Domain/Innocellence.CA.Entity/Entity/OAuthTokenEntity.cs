using System;
using Infrastructure.Core;


namespace DLYB.CA.Contracts.Entity
{
    public class OAuthClientsEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public string ClientId { get; set; }

        public string Status { get; set; }

        public string ClientName { get; set; }

        public string ClientCallBackUrl { get; set; }
    }

    public class OAuthTokenEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public string Token { get; set; }

        public DateTime ExpiredDateUtc { get; set; }

        public DateTime CreatedDateUtc { get; set; }

        public string Code { get; set; }

        public string UserId { get; set; }

        public string ClientId { get; set; }

        public bool IsUsed { get; set; }
    }
}
