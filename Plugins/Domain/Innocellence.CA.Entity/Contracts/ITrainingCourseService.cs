using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.CA.Entity;
using Innocellence.CA.ModelsView;

namespace Innocellence.CA.Contracts
{
    public interface ITrainingCourseService : IDependency, IBaseService<TrainingCourse>
    {
        //List<CourseDateRange> GetDateListByCode<T>(string coursecode);
        //List<T> GetListByCode<T>(string coursecode) where T : IViewModel, new();
        //TrainingCourse MapTrainingCourse(TrainingCourseView objModal, TrainingCourse obj, bool IsEnglish);

        //List<CourseDateCourseView> GetListDate(DateTime dtStart, DateTime dtEnd);

        //List<Innocellence.CA.ViewModelFront.TrainingCourseView> GetListByCondition(
        //    DateTime dtStart, List<int> lstIDs, int? role, int? skill,
        //    string strLanguage, string searchStr, string strUserID, bool myCourse);
    }
}
