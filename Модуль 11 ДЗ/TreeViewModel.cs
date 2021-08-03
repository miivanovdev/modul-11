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
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace Модуль_11_ДЗ
{
    class TreeViewModel : ViewModelBase
    {

        PropertyChangedEventHandler _propertyChangedHandler;
        NotifyCollectionChangedEventHandler _collectionChangedhandler;
                
        public ObservableCollection<TreeViewItemModel> ChildrenUnits { get; set; }

        public TreeViewModel()
        {
            _propertyChangedHandler = new PropertyChangedEventHandler(item_PropertyChanged);
            _collectionChangedhandler = new NotifyCollectionChangedEventHandler(items_CollectionChanged);

            InitData();            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Действия при изменении свойства
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Выбранный работник
        /// </summary>
        public Worker SelectedWorker { get; set; }

        /// <summary>
        /// Команда добавления подразделения
        /// </summary>
        private RelayCommand addTreeElementCommand;

        public RelayCommand AddTreeElementCommand
        {
            get
            {
                return addTreeElementCommand ??
                (addTreeElementCommand = new RelayCommand(new Action<object>(AddUnit)
                ));
            }
        }

        /// <summary>
        /// Метод добавление подразделения
        /// </summary>
        /// <param name="args"></param>
        public void AddUnit(object args)
        {
            SelectedItem.ChildrenUnits.Add(new TreeViewItemModel()
            {
                Name = "Новый департамент",
                Manager = new Worker("", "", "", 1, true, WorkerStatus.Сотрудник, 1),
                Workers = new ObservableCollection<Worker>()
            });  
            
        }


        /// <summary>
        /// Команда добавления работника
        /// </summary>
        private RelayCommand addWorkerCommand;

        public RelayCommand AddWorkerCommand
        {
            get
            {
                return addWorkerCommand ??
                (addWorkerCommand = new RelayCommand(new Action<object>(AddWorker)
                ));
            }
        }

        /// <summary>
        /// Метод добавления работника
        /// </summary>
        /// <param name="args"></param>
        public void AddWorker(object args)
        {
            SelectedItem.Workers.Add(new Worker(" ", " ", " ", 1, false));
        }

        /// <summary>
        /// Команда закрытия приложения
        /// </summary>
        private RelayCommand closeCommand;

        public RelayCommand CloseCommand
        {
            get
            {
                return closeCommand ??
                (closeCommand = new RelayCommand(new Action<object>(SaveData)
                ));
            }
        }

        /// <summary>
        /// Команда удаления подразделения
        /// </summary>
        private RelayCommand removeTreeElementCommand;

        public RelayCommand RemoveTreeElementCommand
        {
            get
            {
                return removeTreeElementCommand ??
                (removeTreeElementCommand = new RelayCommand(new Action<object>(RemoveDepartment)
                ));
            }
        }

        /// <summary>
        /// Комадна удаления работника
        /// </summary>
        private RelayCommand removeWorkerCommand;

        public RelayCommand RemoveWorkerCommand
        {
            get
            {
                return removeWorkerCommand ??
                (removeWorkerCommand = new RelayCommand(new Action<object>(RemoveWorker)
                ));
            }
        }

        /// <summary>
        /// Метод удаления работника
        /// </summary>
        /// <param name="nothing"></param>
        private void RemoveWorker(object nothing)
        {
            SelectedItem.Workers.Remove(SelectedWorker);
        }

        /// <summary>
        /// Метод удаления департамента
        /// </summary>
        /// <param name="nothing"></param>
        private void RemoveDepartment(object nothing)
        {
            if (!RemoveItem(ChildrenUnits, SelectedItem))
                MessageBox.Show("Не удалось удалить департамент");

        }

        /// <summary>
        /// Команда пересчета зароботной платы менеджера
        /// </summary>
        private RelayCommand calculateSalaryCommand;

        public RelayCommand CalculateSalaryCommand
        {
            get
            {
                return calculateSalaryCommand ??
                (calculateSalaryCommand = new RelayCommand(new Action<object>(CalculateSalary)
                ));
            }
        }

        /// <summary>
        /// Метод расчета зароботной платы менеджера
        /// </summary>
        /// <param name="args"></param>
        public void CalculateSalary(object args)
        {
            SelectedItem.ManagerSalary();
        }

        /// <summary>
        /// Метод сохранения данных в файл
        /// </summary>
        /// <param name="args"></param>
        public void SaveData(object args)
        {            
            string json = JsonConvert.SerializeObject(ChildrenUnits);
            File.WriteAllText("Organization.json", json);            
        }
        
        /// <summary>
        /// Метод инициализации данных из файла
        /// </summary>
        public void InitData()
        {
            if (!File.Exists("Organization.json"))
                File.Create("Organization.json").Close();

            this.ChildrenUnits = JsonConvert.DeserializeObject<ObservableCollection<TreeViewItemModel>>(File.ReadAllText("Organization.json"));

            if (this.ChildrenUnits == null)
                this.ChildrenUnits = new ObservableCollection<TreeViewItemModel>();

            Subscribe(ChildrenUnits);
        }

        /// <summary>
        /// Метод нахождения выбранного элемента
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public TreeViewItemModel Traverse(ObservableCollection<TreeViewItemModel> items)
        {                             
            for (int i = 0; i <= items.Count - 1; i++)
            {
                if (items[i].IsSelected)
                    return items[i];

                if (items[i].ChildrenUnits.Count > 0)
                {
                    var result = Traverse(items[i].ChildrenUnits);

                    if (result != null)
                        return result;
                }               
            }

            return null;
        }

        /// <summary>
        /// Метод удаления элемента дерева
        /// </summary>
        /// <param name="items"></param>
        /// <param name="selectedItem"></param>
        /// <returns></returns>
        public bool RemoveItem(ObservableCollection<TreeViewItemModel> items, TreeViewItemModel selectedItem)
        {
            if (items.IndexOf(selectedItem) >= 0)
            {
                unsubscribePropertyChanged(selectedItem);
                items.Remove(selectedItem);
                return true;
            }                

            for(int i = 0; i <= items.Count - 1; i++)
            {
                if (RemoveItem(items[i].ChildrenUnits, selectedItem))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Свойство доступа к выбранному элементу дерева
        /// </summary>
        public TreeViewItemModel SelectedItem 
        {
              get
              {
                return Traverse(ChildrenUnits);
              }
        }

        /// <summary>
        /// Метод подписания на события элемента
        /// </summary>
        /// <param name="item"></param>
        void subscribePropertyChanged(TreeViewItemModel item)
        {
            item.PropertyChanged += _propertyChangedHandler;
            item.ChildrenUnits.CollectionChanged += _collectionChangedhandler;

            foreach (var subitem in item.ChildrenUnits)
            {
                subscribePropertyChanged(subitem);
            }
        }
    
        /// <summary>
        /// Метод отписки от событий элемента
        /// </summary>
        /// <param name="item"></param>
       void unsubscribePropertyChanged(TreeViewItemModel item)
       {
           foreach (var subitem in item.ChildrenUnits)
           {
               unsubscribePropertyChanged(subitem);
           }

           item.ChildrenUnits.CollectionChanged -= _collectionChangedhandler;
           item.PropertyChanged -= _propertyChangedHandler;
       }
        
       
        /// <summary>
        /// Метод переподписания на события элементов дерева
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
       {
           if (e.OldItems != null)
           {
               foreach (TreeViewItemModel item in e.OldItems)
               {
                   unsubscribePropertyChanged(item);
               }
           }
           
           if (e.NewItems != null)
           {
               foreach (TreeViewItemModel item in e.NewItems)
               {
                   subscribePropertyChanged(item);
               }
           }
       }

        /// <summary>
        /// Метод запуска события SelectedItem при смене свойства IsSelected 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           if (e.PropertyName == "IsSelected")
           {
               OnPropertyChanged(nameof(SelectedItem));
           }

            if (e.PropertyName == "IsReCalculated")
            {
                foreach (var i in ChildrenUnits)
                    i.ManagerSalary();
            }
        }

        /// <summary>
        /// Подписаться на события всех элементов
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private void Subscribe(ObservableCollection<TreeViewItemModel> items)
        {
            items.CollectionChanged += _collectionChangedhandler;

            for (int i = 0; i <= items.Count - 1; i++)
            {
                items[i].SubscribeWorkers();
                items[i].PropertyChanged += _propertyChangedHandler;

                if (items[i].ChildrenUnits.Count > 0)
                    Subscribe(items[i].ChildrenUnits);                
            }
        }
    }       
    

}
