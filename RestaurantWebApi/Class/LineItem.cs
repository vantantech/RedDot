using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;

namespace RedDot.OrderService.Class
{
    public class LineItem : INPCBase
    {
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Unit { get; set; }

        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }

        public string ReportCategory { get; set; }
        public string MenuPrefix { get; set; }

        public int SeatNumber { get; set; }
        public int KitchenPrinter { get; set; }

        public decimal Quantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Price { get; set; }
        public decimal ComboMaxPrice { get; set; }
        public int ID { get; set; }
        public int ProductID { get; set; }
        public decimal Discount { get; set; }
        public string DiscountName { get; set; }
        public string DiscountType { get; set; }
        public string Note { get; set; }
        public string ItemType { get; set; }

        public bool Taxable { get; set; }
        public bool Sent { get; set; }
        public bool Selected { get; set; }
        public bool Voided { get; set; }
        public bool AllowPartial { get; set; }
        public DateTime VoidDate { get; set; }

        public bool IsCombo { get; set; }
        public bool SpecialPricing { get; set; }
        public bool Weighted { get; set; }

        public bool NeedToSend
        {
            get
            {
                return !Sent;

            }
        }





        private DBTicket _dbticket;
        private decimal _modifiertotal;


        public LineItem(DataRow row)
        {

            try
            {

                _dbticket = new DBTicket();
                if (row["id"].ToString() != "") ID = int.Parse(row["id"].ToString());
                if (row["seatnumber"].ToString() != "") SeatNumber = int.Parse(row["seatnumber"].ToString());
                if (row["productid"].ToString() != "") ProductID = int.Parse(row["productid"].ToString());
                if (row["kitchenprinter"].ToString() != "") KitchenPrinter = int.Parse(row["kitchenprinter"].ToString());
                Description = row["description"].ToString().Replace('\r', ' ').Replace('\n', ' ');
                Description2 = row["description2"].ToString().Replace('\r', ' ').Replace('\n', ' ');
                Description3 = row["description3"].ToString().Replace('\r', ' ').Replace('\n', ' ');
                Unit = row["unit"].ToString().Replace('\r', ' ').Replace('\n', ' ');

                Note = row["note"].ToString();
                DiscountName = row["discountname"].ToString();
                DiscountType = row["discounttype"].ToString();
                Custom1 = row["custom1"].ToString();
                Custom2 = row["custom2"].ToString();
                Custom3 = row["custom3"].ToString();
                Custom4 = row["custom4"].ToString();
                ReportCategory = row["reportcategory"].ToString();
                MenuPrefix = row["menuprefix"].ToString();



                ItemType = row["type"].ToString().Trim();

                if (row["quantity"].ToString() != "") Quantity = decimal.Parse(row["quantity"].ToString());
                if (row["weight"].ToString() != "") Weight = decimal.Parse(row["weight"].ToString());
                if (row["price"].ToString() != "") Price = (decimal)row["price"];
                if (row["combomaxprice"].ToString() != "") ComboMaxPrice = (decimal)row["combomaxprice"];
                if (row["discount"].ToString() != "") Discount = (decimal)row["discount"];

                if (row["taxable"].ToString() != "") Taxable = row["taxable"].ToString().Equals("1");
                if (row["allowpartial"].ToString() != "") AllowPartial = row["allowpartial"].ToString().Equals("1");

                //booleans
                Sent = Convert.ToBoolean(row["sent"]);
                Voided = Convert.ToBoolean(row["void"]);
                SpecialPricing = Convert.ToBoolean(row["specialpricing"]);
                Weighted = Convert.ToBoolean(row["weighted"]);

                if (row["voiddate"].ToString() != "") VoidDate = (DateTime)row["voiddate"];


                Modifiers = GetItemModifiers();
                LineItems = GetComboLineItems();


            }
            catch (Exception e)
            {

              
            }


        }

        public LineItem()
        {
            _dbticket = new DBTicket();

        }




        public string PriceStr
        {
            get
            {
                if (Voided) return "(VOIDED)";

                return String.Format("{0:0.00}", Quantity * PriceWithModifiers);
            }

        }


        public string DiscountStr
        {
            get
            {
                if (DiscountName != "" && !Voided)
                {
                    string amtstr = Discount > 0 ? "-" + string.Format("{0:0.00}", Quantity * Discount) : ""; // multiple by quantity .. ignore weight
                    return Utility.FormatPrintRow("**" + DiscountName + "**", amtstr, 34);
                }

                else return "";

            }

        }

        public string DoFormat(decimal myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        }


        public string Status
        {
            get
            {
                if (Voided) return "Voided";
                if (NeedToSend) return "NeedToSend";


                return "Normal";

            }
        }

        public string WeightStr
        {
            get
            {
                return (Weighted ? "(" + DoFormat(Weight) + " " + Unit + ")" : "");
            }
        }


        public string ReceiptStr
        {
            get
            {


                string rtn = Utility.FormatPrintRow(" " + DoFormat(Quantity) + "x " + Description + WeightStr, " " + PriceStr, 38);



                if (Description2 != "") rtn = rtn + (char)13 + (char)10 + Description2;
                return rtn;
            }

        }

        public decimal PriceWithModifiers
        {
            get
            {
                return Price * Weight + ComboSurcharge + ModifierTotal;
            }
        }



        public decimal ComboPrice
        {
            get
            {
                decimal price = Price * Weight + ModifierTotal;

                if (price > ComboMaxPrice) price = price - ComboMaxPrice; //adjust for difference
                else price = 0;
                return price;
            }
        }


        public decimal ComboSurcharge
        {
            get
            {
                decimal combototal = 0;
                foreach (LineItem comboline in LineItems)
                {
                    combototal += comboline.ComboPrice;
                }
                return combototal;
            }
        }


        public string ComboStr
        {
            get
            {
                string rtn = "";


                if (ComboPrice > 0)
                    rtn = Utility.FormatPrintRow(" + " + Description, " " + String.Format("({0:0.00})", ComboPrice), 30);
                else
                    rtn = " + " + Description;




                if (Description2 != "") rtn = rtn + (char)13 + (char)10 + "  " + Description2;
                return rtn;
            }

        }


        public string IDItemType
        {
            get
            {
                return ID.ToString() + "," + ItemType;
            }
        }


        public decimal AdjustedPrice
        {
            get
            {
                return Price * Weight + ComboSurcharge - Discount;
            }
        }

        public decimal TotalAdjustedPrice
        {
            get
            {
                return AdjustedPrice * Quantity;
            }
        }

        public decimal AdjustedPriceWithModifier
        {
            get
            {
                return PriceWithModifiers - Discount;
            }
        }

        public decimal TotalAdjustedPriceWithModifier
        {
            get
            {
                return AdjustedPriceWithModifier * Quantity;

            }
        }

        public decimal ModifierTotal
        {
            get
            {
                return _modifiertotal;
            }

            set
            {
                _modifiertotal = value;
                NotifyPropertyChanged("ModifierTotal");
            }
        }






        private ObservableCollection<SalesModifier> _modifiers;
        public ObservableCollection<SalesModifier> Modifiers
        {
            get { return _modifiers; }
            set
            {
                _modifiers = value;
                NotifyPropertyChanged("Modifiers");
            }

        }



        public ObservableCollection<SalesModifier> GetItemModifiers()
        {
            ObservableCollection<SalesModifier> modifiers = new ObservableCollection<SalesModifier>();
            DataTable dt;
            SalesModifier modifier;
            decimal total = 0;
            //bool showmodprice = GlobalSettings.Instance.DisplayShowModifierPrice;
            bool showmodprice = false;

            if (_dbticket == null) return modifiers; //returns empty list

            dt = _dbticket.GetSalesItemModifiers(ID);
            foreach (DataRow row in dt.Rows)
            {
                modifier = new SalesModifier(row);

                modifier.ShowModPrice = showmodprice;
                total = total + modifier.TotalPrice;
                modifiers.Add(modifier);
            }
            ModifierTotal = total;

            return modifiers;
        }


        private ObservableCollection<LineItem> _lineitems;
        public ObservableCollection<LineItem> LineItems
        {
            get { return _lineitems; }
            set
            {
                _lineitems = value;
                NotifyPropertyChanged("LineItems");
            }

        }


        public ObservableCollection<LineItem> GetComboLineItems()
        {
            ObservableCollection<LineItem> lineitems = new ObservableCollection<LineItem>();
            DataTable dt;
            LineItem line;


            if (_dbticket == null) return lineitems; //returns empty list

            dt = _dbticket.GetComboSalesItem(ID);
            foreach (DataRow row in dt.Rows)
            {
                line = new LineItem(row);

                lineitems.Add(line);
            }


            return lineitems;
        }


        public bool UpdatePrice(decimal amount, bool specialpricing)
        {
            bool result;


            try
            {

                DBTicket m_dbTicket;
                m_dbTicket = new DBTicket();

                if (Status == "Closed") return false;

                Price = amount;

                result = m_dbTicket.DBUpdateSalesItemPrice(ID, amount, specialpricing);


                return result;
            }
            catch (Exception e)
            {
              
                return false;
            }
        }

        public bool UpdatePrice(decimal amount)
        {
            bool result;


            try
            {

                DBTicket m_dbTicket;
                m_dbTicket = new DBTicket();

                if (Status == "Closed") return false;

                Price = amount;

                result = m_dbTicket.DBUpdateSalesItemPrice(ID, amount, SpecialPricing);


                return result;
            }
            catch (Exception e)
            {
           
                return false;
            }
        }
    }
}