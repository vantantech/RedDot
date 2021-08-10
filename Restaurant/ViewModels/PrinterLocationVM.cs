using RedDot.Class;
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
            PrinterModes.Add(new ListPair() { Description = "Zebra Printer", StrValue = "ZEBRA" });

            ReceiptWidths = new List<ListPair>();
            ReceiptWidths.Add(new ListPair() { Description = "80 mm (3-1/8\")", StrValue = "80" });
            ReceiptWidths.Add(new ListPair() { Description = "58 mm (2-1/4\")", StrValue = "58" });

            SelectedPrinter = new RedDotPrinter();
            SelectedPrinter.Description = "Please select printer ";
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


        private RedDotPrinter m_selectedprinter;
        public RedDotPrinter SelectedPrinter
        {
            get { return m_selectedprinter; }
            set
            {
                m_selectedprinter = value;
                NotifyPropertyChanged("SelectedPrinter");
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

            if (SelectedPrinter == null) return;

            foreach (DataRow row in Locations.Rows)
            {
                if ((int)row["id"] == SelectedPrinter.id) row["selected"] = true;
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

               RedDotPrinter sel = ReceiptPrinterModel.GetPrinterObject(id);



                SelectedPrinter = sel;

            }catch(Exception ex)
            {
                TouchMessageBox.Show("Error:" + ex.Message);
            }
        }


        public void ExecuteSaveClicked(object obj)
        {

            if (SelectedPrinter.id > 0)
            {
                  printermodel.UpdatePrinterLocation_AssignedPrinter(SelectedPrinter);
              
                LoadLocations();
            }

        }

        public void ExecuteDeleteClicked(object obj)
        {

            if (SelectedPrinter.id > 0)
            {
                ConfirmDelete cnf = new ConfirmDelete("Delete Printer from System??? ") { Topmost = true };
                cnf.ShowDialog();
                if(cnf.Action == "Delete")
                {
                    Confirm again = new Confirm("Are you sure?") { Topmost = true };
                    again.ShowDialog();
                    if(again.Action == Confirm.OK)
                    {
                        printermodel.DeletePrinterLocation(SelectedPrinter.id);
                        SelectedPrinter.id = 0;
                        SelectedPrinter = null;
                      
                        LoadLocations();
                    }
                }

              
            }

        }

        public void ExecuteTestPrintClicked(object obj)
        {

            if (SelectedPrinter.id > 0)
            {
                ReceiptPrinterModel.TestPrint(SelectedPrinter);
            }

        }
    }
}
