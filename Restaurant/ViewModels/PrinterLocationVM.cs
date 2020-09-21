using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedDot
{
    public class PrinterLocationVM:INPCBase
    {
        ReceiptPrinterModel printermodel = new ReceiptPrinterModel();

        public ICommand TestPrintClicked { get; set; }
        public ICommand PrinterClicked { get; set; }
        public ICommand SaveClicked { get; set; }

        public ICommand DeleteClicked { get; set; }

        public ICommand AddNewPrinterClicked { get; set; }

        private int selectedid = 0;
        public PrinterLocationVM()
        {

            AddNewPrinterClicked = new RelayCommand(ExecuteAddNewPrinterClicked, param => true);
            PrinterClicked = new RelayCommand(ExecutePrinterClicked, param => true);
            SaveClicked = new RelayCommand(ExecuteSaveClicked, param => true);
            DeleteClicked = new RelayCommand(ExecuteDeleteClicked, param => true);
            TestPrintClicked = new RelayCommand(ExecuteTestPrintClicked, param => true);

            LoadLocations();
            FillPrinterList();

            PrinterModes = new List<ListPair>();
            PrinterModes.Add(new ListPair() { Description = "Receipt - EPSON", StrValue = "EPSON" });
            PrinterModes.Add(new ListPair() { Description = "Receipt - STAR", StrValue = "STAR" });
            PrinterModes.Add(new ListPair() { Description = "Label Writer", StrValue = "LABEL" });

            ReceiptWidths = new List<ListPair>();
            ReceiptWidths.Add(new ListPair() { Description = "80 mm (3-1/8\")", StrValue = "80" });
            ReceiptWidths.Add(new ListPair() { Description = "58 mm (2-1/4\")", StrValue = "58" });
          
        }

        public List<ListPair> PrinterModes { get; set; }

        public List<ListPair> ReceiptWidths { get; set; }

        DataTable m_locations;
        public DataTable Locations
        {
            get { return m_locations; }
            set
            {
                m_locations = value;
                NotifyPropertyChanged("Locations");
            }
        }


        private List<ListPair> m_printerlist;
        public List<ListPair> PrinterList
        {
            get { return m_printerlist; }
            set
            {
                m_printerlist = value;
                NotifyPropertyChanged("PrinterList");
            }
        }

        private int m_receiptwidth;
        public int ReceiptWidth
        {
            get { return m_receiptwidth; }
            set
            {
                m_receiptwidth = value;
                NotifyPropertyChanged("ReceiptWidth");
            }
        }






        private string m_selectedprinter;
        public string SelectedPrinter
        {
            get { return m_selectedprinter; }   
            set
            {
                m_selectedprinter = value;
                NotifyPropertyChanged("SelectedPrinter");
            
            }
        }

        private string m_assignedprinter;
        public string AssignedPrinter
        {
            get { return m_assignedprinter; }
            set
            {
                m_assignedprinter = value;
                NotifyPropertyChanged("AssignedPrinter");
              
            }
        }


        private string m_printmode;
        public string PrintMode
        {
            get { return m_printmode; }
            set
            {
                m_printmode = value;
                NotifyPropertyChanged("PrintMode");

            }
        }



        private bool m_islabel;
        public bool IsLabel
        {
            get { return m_islabel; }
            set
            {
                m_islabel = value;
                NotifyPropertyChanged("IsLabel");

            }
        }

        private bool m_landscape;
        public bool LandScape
        {
            get { return m_landscape; }
            set
            {
                m_landscape = value;
                NotifyPropertyChanged("LandScape");

            }
        }

        private void FillPrinterList()
        {
            LocalPrintServer printServer = new LocalPrintServer();

            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });


            PrinterList = new List<ListPair>();

          
            foreach (PrintQueue printer in printQueuesOnLocalServer)
            {
                PrinterList.Add(new ListPair() { Description = printer.Name, StrValue = printer.Name });
            }


        }


        private void LoadLocations()
        {
            Locations = printermodel.GetPrinterLocations();
            foreach(DataRow row in Locations.Rows)
            {
                if ((int)row["id"] == selectedid) row["selected"] = true;
            }
        }

        public void ExecuteAddNewPrinterClicked(object m_id)
        {
           
            try
            {

                InputBox inp = new InputBox("Enter Location Description");
                string description = inp.OpenModal();

                printermodel.AddPrinterLocations(description, "");

                LoadLocations();

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Error:" + ex.Message);
            }
        }

        public void ExecutePrinterClicked(object m_id)
        {
            int id;

            try
            {
                id = (int)m_id;

                DataRow selected = printermodel.GetPrinter(id);
                if(selected != null)
                {
                    SelectedPrinter = selected["description"].ToString();
                    AssignedPrinter = selected["assignedprinter"].ToString();
                    selectedid = (int)selected["id"];
                    PrintMode = selected["printermode"].ToString();
                    IsLabel = selected["islabel"].ToString() == "1" ? true : false;
                    LandScape= selected["landscape"].ToString() == "1" ? true : false;
                 

                    if (selected["receiptwidth"].ToString() != "")
                        ReceiptWidth = int.Parse(selected["receiptwidth"].ToString());
                    else ReceiptWidth = 80;
                }



            }catch(Exception ex)
            {
                TouchMessageBox.Show("Error:" + ex.Message);
            }
        }


        public void ExecuteSaveClicked(object obj)
        {

            if (selectedid > 0)
            {
                  printermodel.UpdatePrinterLocation_AssignedPrinter(selectedid, m_assignedprinter, m_selectedprinter, m_printmode, m_receiptwidth, m_islabel,  m_landscape);
              
                LoadLocations();
            }

        }

        public void ExecuteDeleteClicked(object obj)
        {

            if (selectedid > 0)
            {
                ConfirmDelete cnf = new ConfirmDelete("Delete Printer from System??? ") { Topmost = true };
                cnf.ShowDialog();
                if(cnf.Action == "Delete")
                {
                    Confirm again = new Confirm("Are you sure?") { Topmost = true };
                    again.ShowDialog();
                    if(again.Action == Confirm.OK)
                    {
                        printermodel.DeletePrinterLocation(selectedid);
                        selectedid = 0;
                        SelectedPrinter = "";
                        AssignedPrinter = "";
                        LoadLocations();
                    }
                }

              
            }

        }

        public void ExecuteTestPrintClicked(object obj)
        {

            if (selectedid > 0)
            {
                ReceiptPrinterModel.TestPrint(AssignedPrinter, PrintMode, m_receiptwidth, m_landscape);
            }

        }
    }
}
