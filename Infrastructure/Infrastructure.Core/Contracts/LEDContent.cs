using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Contracts
{
    public class LEDContent
    {
        public Int32 ArticleId { get; set; }

        public String ArticleCode { get; set; }

        public String ArticleTitle { get; set; }

        public String MeunKey { get; set; }

        public String MeunName { get; set; }
    }
}
