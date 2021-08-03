using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using System.Collections.Specialized;

namespace Модуль_11_ДЗ
{
    class TreeViewItemModel
    {
        string _name;
        const decimal managerPercent = 0.15M;

        public ObservableCollection<TreeViewItemModel> ChildrenUnits { get; set; }
        public ObservableCollection<Worker> Workers { get; set; }

        private bool _isReCalculated;
        public bool IsReCalculated
        {
            get { return _isReCalculated; }
            set
            {
                if (value != _isReCalculated)
                {
                    _isReCalculated = value;
                    this.OnPropertyChanged(nameof(IsReCalculated));
                }
            }
        }
        
        public Worker Manager { get; set; }
                
        private bool _isSelected;

        public TreeViewItemModel()
        {
            ChildrenUnits = new ObservableCollection<TreeViewItemModel>();
            
            Workers = new ObservableCollection<Worker>();
            Manager = new Worker();
        }


        /// <summary>
        /// Подписаться на изменения в данных работников
        /// </summary>
        public void SubscribeWorkers()
        {
            Workers.CollectionChanged += new NotifyCollectionChangedEventHandler(WorkersChanged);

            if (Workers.Count > 0)
            {
                foreach (var w in Workers)
                    w.SalaryChanged += new PropertyChangedEventHandler(ManagerSalaryEvent);
            }
        }

        /// <summary>
        /// Расчитать заработную плату менеджера
        /// </summary>
        public void ManagerSalary() 
        {
            Manager.Salary = managerPercent * TraverseSalary(this);
            IsReCalculated = false;
        }

        /// <summary>
        /// Действия при изменении заработной платы работника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ManagerSalaryEvent(object sender, PropertyChangedEventArgs e)
        {
            IsReCalculated = true;            
        }

        /// <summary>
        /// Посчитать часть заработной платы менеджера из дочерних элементами
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private decimal TraverseSalary(TreeViewItemModel item)
        {
            decimal temp = 0;
            
            for(int i = 0; i <= item.Workers.Count - 1; i++)
            {
                temp += item.Workers[i].Salary;
            }

            for(int i = 0; i <= item.ChildrenUnits.Count - 1; i++)
            {
                item.ChildrenUnits[i].ManagerSalary();
                temp += item.ChildrenUnits[i].Manager.Salary;
            }
            
            return temp;
        }

        #region.INotifyRealization
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Имя подразделения
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value; OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        
        /// <summary>
        /// Действие при изменения свойства
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion


        /// <summary>
        /// Действия при изменении в коллекции работников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WorkersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                SubscribeWorkers();
            }

            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                IsReCalculated = true;
            }
        }
    }
}

