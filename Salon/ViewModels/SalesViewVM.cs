using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedDot
{
    public class SalesViewVM
    {

        public Ticket CurrentTicket { get; set; }
        public ICommand PrintReceiptClicked { get; set; }

        public SalesViewVM(int salesid)
        {
           
            CurrentTicket = new Ticket(salesid);
            PrintReceiptClicked = new RelayCommand(ExecutePrintReceiptClicked, param => CanExecutePrintReceipt);
        }

        public bool CanExecutePrintReceipt
        {
            get
            {
                if (CurrentTicket == null)
                {
                    return false;
                }
                else
                {
                    if (CurrentTicket.ItemCount > 0) return true;
                    else return false;

                }

            }
        }

        public void ExecutePrintReceiptClicked(object button)
        {
            ReceiptPrinterModel.PrintReceipt(CurrentTicket);
        }

    }
}
