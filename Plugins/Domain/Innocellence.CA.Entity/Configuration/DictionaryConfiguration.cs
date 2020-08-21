using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class DictionaryConfiguration : EntityConfigurationBase<DictionaryEntity, int>
    {
        public DictionaryConfiguration()
        {
            ToTable("Dictionary");
        }
    }
}
