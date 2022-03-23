using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Settings
{
    public class SettingNode
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public SettingViewModel Me { get; set; }

        public List<SettingNode> Childrens { get; set; }
    }
}
