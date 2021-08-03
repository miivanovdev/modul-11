using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Модуль_11_ДЗ
{
    class Worker : INotifyPropertyChanged
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Surname { get; set; }
        public uint Age { get; set; }     
        public bool Manager { get; set; }
        public bool PerHourPayment { get; private set; }
        public bool EditableSalary { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler SalaryChanged;


        public Worker(string firstName, string secondName, string surname, uint age, bool manager, WorkerStatus workerStatus = WorkerStatus.Сотрудник, decimal salary = 0, uint workingHours = 0, double hourlyPay = 0, PropertyChangedEventHandler salaryChanged = null)
        {
            FirstName = firstName;
            SecondName = secondName;
            Surname = surname;
            Age = age;
            WorkerStatus = workerStatus;
            HourlyPay = hourlyPay;
            WorkingHours = workingHours;
            Manager = manager;
            Salary = salary;
            SalaryChanged += salaryChanged;
        }

        public Worker() { }

        private decimal salary;
        private WorkerStatus workerStatus;
        private uint workingHours;
        private double hourlyPay;

        public uint WorkingHours
        {
            get
            {
                return workingHours;
            }
            set
            {
                workingHours = value;

                if (WorkerStatus == WorkerStatus.Сотрудник && !Manager)
                    Salary = (decimal)(HourlyPay * WorkingHours);

                NotifyPropertyChanged(nameof(Salary));
            }
        }

        public double HourlyPay
        {
            get
            {
                return hourlyPay;
            }
            set
            {
                if (value < 0)
                    value = value * (-1);

                hourlyPay = value;

                if (WorkerStatus == WorkerStatus.Сотрудник && !Manager)
                    Salary = (decimal)(HourlyPay * WorkingHours);

                NotifyPropertyChanged(nameof(Salary));
            }
        }

        public WorkerStatus WorkerStatus
        {
            get
            {
                return workerStatus;
            }
            set
            {
                workerStatus = value;
                PerHourPayment = workerStatus == WorkerStatus.Сотрудник ? true : false;
                EditableSalary = !PerHourPayment;

                if (!PerHourPayment)
                {
                    HourlyPay = 0;
                    WorkingHours = 0;

                    NotifyPropertyChanged(nameof(WorkingHours));
                    NotifyPropertyChanged(nameof(HourlyPay));
                }
                    

                NotifyPropertyChanged(nameof(PerHourPayment));
                NotifyPropertyChanged(nameof(EditableSalary));
            }
        }       
        
        /// <summary>
        /// Метод запуска события изменения свойства
        /// </summary>
        /// <param name="propertyName">Изменеяемое свойство</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifySalaryChanged([CallerMemberName] string propertyName = null)
        {
            SalaryChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public decimal Salary
        {
            get
            {                
                return Math.Round(salary, 2);    
            }   
            set
            {
                if (value < 0)
                    value = value * (-1);

                if (WorkerStatus == WorkerStatus.Сотрудник && Manager)
                {
                    if (value < 1300)
                        value = 1300;
                }                

                salary = value;                

                NotifyPropertyChanged(nameof(Salary));
                NotifySalaryChanged();
            }
        }        
    }
}
