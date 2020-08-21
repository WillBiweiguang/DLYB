using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class FileManageConfiguration : EntityConfigurationBase<FileManage, int>
    {
        public FileManageConfiguration()
        {
            ToTable("FileManage");
        }
    }
}
