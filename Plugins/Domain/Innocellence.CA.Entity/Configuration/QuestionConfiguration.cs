using Infrastructure.Core.Data;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class QuestionManageConfiguration : EntityConfigurationBase<QuestionManage, int>
    {
        public QuestionManageConfiguration()
        {
            ToTable("QuestionManage");
        }
    }

    public class QuestionImagesConfiguration : EntityConfigurationBase<QuestionImages, int>
    {
        public QuestionImagesConfiguration()
        {
            ToTable("QuestionImages");
        }
    }
    public class QuestionSubConfiguration : EntityConfigurationBase<QuestionSub, int>
    {
        public QuestionSubConfiguration()
        {
            ToTable("QuestionSub");
        }
    }
}
