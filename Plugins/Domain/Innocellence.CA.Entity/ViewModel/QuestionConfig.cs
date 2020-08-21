using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Contracts.ViewModel
{
    public class QuestionConfig
    {
        public string menuKey { get; set; }
        public string title { get; set; }
        public string mainPictureUrl { get; set; }
        public string subPictureUrl { get; set; }
        public string bottomContent { get; set; }
        public string bottomContent_tel1 { get; set; }
        public string bottomContent_tel2 { get; set; }
        public string bottomContent_time { get; set; }
        public string bottomContent_Email { get; set; }
        public string questionContent { get; set; }
        public string questionPara { get; set; }
        public string successHint { get; set; }
        public string emailConfigCode { get; set; }
    }
}
