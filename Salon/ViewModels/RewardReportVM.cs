using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RedDotBase;

namespace RedDot
{
    public class RewardReportVM:INPCBase
    {
        Reports m_report;
        private DataTable m_reward;
        private decimal m_totalrewards;
        private decimal m_totalused;
        private decimal m_totalrewardused;
        private decimal m_balance;
        private ReportDate m_currentdate;
        public ICommand CustomClicked { get; set; }



        public RewardReportVM()
        {


            m_report = new Reports();
            CurrentDate = new ReportDate();
            CurrentDate.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            CurrentDate.EndDate = DateTime.Today;
            CustomClicked = new RelayCommand(ExecuteCustomClicked, param => true);

            RunReport();
        }


        public void RunReport()
        {

            decimal totalrewards = 0;
            decimal totalused = 0;
            decimal balance = 0;
            decimal totalrewardused = 0;


            Rewards = m_report.GetRewardSummary(CurrentDate.StartDate, CurrentDate.EndDate);
            if(Rewards != null)
            {
                foreach (DataRow row in Rewards.Rows)
                {
                    totalrewards = totalrewards + (decimal)row["salesreward"];
                    totalused = totalused + (decimal)row["usedreward"];
                    balance = balance + (decimal)row["balance"];
                    totalrewardused = totalrewardused + (decimal)row["totalusedreward"];
                }
            }
    


            TotalRewards = totalrewards;
            TotalUsed = totalused;
            Balance = balance;
            TotalRewardUsed = totalrewardused;

        }

        public ReportDate CurrentDate
        {
            get { return m_currentdate; }
            set
            {
                m_currentdate = value;
                NotifyPropertyChanged("CurrentDate");
            }
        }
        public DataTable Rewards
        {
            get { return m_reward; }
            set
            {
                m_reward = value;
                NotifyPropertyChanged("Rewards");
            }

        }

        public decimal TotalRewards
        {
            get { return m_totalrewards; }
            set
            {
                m_totalrewards = value;
                NotifyPropertyChanged("TotalRewards");
            }
        }


        public decimal TotalUsed
        {
            get { return m_totalused; }
            set
            {
                m_totalused = value;
                NotifyPropertyChanged("TotalUsed");
            }
        }

        public decimal TotalRewardUsed
        {
            get { return m_totalrewardused; }
            set
            {
                m_totalrewardused = value;
                NotifyPropertyChanged("TotalRewardUsed");
            }
        }

        public decimal Balance
        {
            get { return m_balance; }
            set
            {
                m_balance = value;
                NotifyPropertyChanged("Balance");
            }
        }


        public void ExecuteCustomClicked(object tagstr)
        {

            CustomDate cd = new CustomDate(Visibility.Visible);
            cd.Topmost = true;
            cd.ShowDialog();
            //Utility.OpenModal(this, cd);

            CurrentDate.StartDate = cd.StartDate;
            CurrentDate.EndDate = cd.EndDate;
            NotifyPropertyChanged("CurrentDate");

            RunReport();

        }
    }
}
