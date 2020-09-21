using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class WorkOrderVM:INPCBase
    {

        public ICommand OKClicked { get; set; }

  
        public ICommand PrintPDFClicked { get; set; }

        public Ticket CurrentTicket { get; set; }

        public string SalesCustomName1
        {
            get { return GlobalSettings.Instance.SalesCustomName1; }
        }

        public string SalesCustomName2
        {
            get { return GlobalSettings.Instance.SalesCustomName2; }
        }
        public string SalesCustomName3
        {
            get { return GlobalSettings.Instance.SalesCustomName3; }
        }
        public string SalesCustomName4
        {
            get { return GlobalSettings.Instance.SalesCustomName4; }
        }

        public List<DropDownListType> List1 { get; set; }

        public List<DropDownListType> List2 { get; set; }
        public List<DropDownListType> List3 { get; set; }
        public List<DropDownListType> List4 { get; set; }
        public List<DropDownListType> List5 { get; set; }



        private bool test;

        public bool IsEnabled
        {
            get { return test; }
            set
            {
                test = value;
                NotifyPropertyChanged("IsEnabled");
            }
        }

        Window m_parent;
        public WorkOrderVM(Window parent,Ticket ticket,bool canedit)
        {
            IsEnabled = canedit;
            CurrentTicket = ticket;
            m_parent = parent;
            OKClicked = new RelayCommand(ExecuteOKClicked, param => true);

            PrintPDFClicked = new RelayCommand(ExecutePrintPDFClicked, param => true);


            List1 = new List<DropDownListType>();
            List1.Add(new DropDownListType() { Name = "Stage 1" });
            List1.Add(new DropDownListType() { Name = "Stage 2" });
            List1.Add(new DropDownListType() { Name = "Stage 3" });
            List1.Add(new DropDownListType() { Name = "Stage 4" });
            List1.Add(new DropDownListType() { Name = "Custom" });
          

            List2 = new List<DropDownListType>();
            List2.Add(new DropDownListType() { Name = "YES" });
            List2.Add(new DropDownListType() { Name = "NO" });
            List2.Add(new DropDownListType() { Name = "RGB" });


            List3 = new List<DropDownListType>();
            List3.Add(new DropDownListType() { Name = "YES" });
            List3.Add(new DropDownListType() { Name = "NO" });

            List4 = new List<DropDownListType>();
            List4.Add(new DropDownListType() { Name = "CUSTOMER OWNED" });
            List4.Add(new DropDownListType() { Name = "WE PROVIDE" });

            List5 = new List<DropDownListType>();
            List5.Add(new DropDownListType() { Name = "INTEGRATED" });
            List5.Add(new DropDownListType() { Name = "BEHIND SEAT" });


        }

        public WorkOrderVM(List<DropDownListType> list1)
        {
            List1 = list1;
        }

        public void ExecuteOKClicked(object obj)
        {
            if (IsEnabled) CurrentTicket.WorkOrder.Save();
            m_parent.Close();
          
        }




        public void ExecutePrintPDFClicked(object obj)
        {

          if(IsEnabled)  CurrentTicket.WorkOrder.Save();

          

             CurrentTicket.WorkOrder.PrintPDF(true);


        }

    }
}
