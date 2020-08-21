using Infrastructure.Core.Data;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class MessageTextConfiguration : EntityConfigurationBase<MessageText, int>
    {
        public MessageTextConfiguration()
        {
            ToTable("MessageText");
        }
    }
}
