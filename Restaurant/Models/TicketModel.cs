using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;

namespace RedDot
{
    public class TicketModel
    {

        private static DBTicket m_dbticket = new DBTicket();
        private static DBProducts m_dbproduct = new DBProducts();

        public List<Seat> GetTicketItem(int id)
        {
            List<Seat> seats = new List<Seat>();


            DataTable dt = m_dbticket.GetSeats(id);

            foreach (DataRow row in dt.Rows)
            {
                Seat seat = new Seat();

                seat.SeatNumber = int.Parse(row["seatnumber"].ToString());
                seat.LineItems = GetLineItems(id, seat.SeatNumber);
                seats.Add(seat);
            }

            return seats;
        }


        public ObservableCollection<LineItem> GetLineItems(int salesid, int seatnumber)
        {
            ObservableCollection<LineItem> lineitems;
            DataTable data_receipt;
            LineItem line;


            try
            {

                lineitems = new ObservableCollection<LineItem>();

                //load purchased item that are products
                data_receipt = m_dbticket.GetLineItemPerSeat(salesid, seatnumber);

                //load ticket item from sales item table
                foreach (DataRow row in data_receipt.Rows)
                {

                    line = new LineItem(row);

                    lineitems.Add(line);
                }



                return lineitems;
            }
            catch (Exception ex)
            {

                return null;
            }


        }



    }
}