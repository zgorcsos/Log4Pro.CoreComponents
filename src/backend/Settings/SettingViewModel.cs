using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Settings
{
    public class SettingViewModel
    {
        public string UniqueId { get; set; }

        public string Instance { get; set; }

        public string DataType { get; set; }

        public object PersistedValue { get; set; }

        public object CachedValue { get; set; }

        public object DefaultValue { get; set; }

        public string TypeId { get; set; }

        public string DefiningTypeId { get; set; }

        public IEnumerable<SettingSelection> Options { get; set; }

        public bool SensitiveData { get; set; }
    }
}
