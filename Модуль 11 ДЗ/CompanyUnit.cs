using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Модуль_11_ДЗ
{
    class CompanyUnit 
    {
        public ObservableCollection<CompanyUnit> Units { get; set; }
        public ObservableCollection<Worker> Workers { get; set; }

        public CompanyUnit()
        {

        }

        public CompanyUnit(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        
    }

    
}
