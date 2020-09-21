using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;

namespace RedDot
{


    public class LineItem:INPCBase
    {
        public string Description { get; set; }
        public string ModelNumber { get; set; }
        public string PartNumber { get; set; }

        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }


        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int ID { get; set; }
        public int ProductID { get; set; }
        public decimal Discount { get; set; }
        public string Note { get; set; }
        public string InternalNote { get; set; }
        public string ItemType { get; set; }
        public decimal Surcharge { get; set; }
        public decimal Cost { get; set; }
        
        public bool TaxExempt { get; set; }


        public decimal CommissionRate { get; set; }
        public string CommissionType { get; set; }

        private DBTicket _dbticket;


 

        public LineItem(string description)
        {
            ID = 0;
            Description = description;
            Quantity = 0;
            Price = 0;
            Discount = 0;
        
              ItemType = "";
            Surcharge = 0;
            Cost = 0;
           
        }



        public LineItem(DataRow row)
        {

            try
            {

                _dbticket = new DBTicket();
                if (row["id"].ToString() != "") ID = int.Parse(row["id"].ToString());
                if (row["productid"].ToString() != "") ProductID = int.Parse(row["productid"].ToString());
                Description = row["description"].ToString().Replace('\r', ' ').Replace('\n', ' '); 
                Note = row["note"].ToString();
                InternalNote = row["internalnote"].ToString();
                Custom1 = row["custom1"].ToString();
                Custom2 = row["custom1"].ToString();
                Custom3 = row["custom1"].ToString();
                Custom4 = row["custom1"].ToString();
                ModelNumber = row["modelnumber"].ToString();
                PartNumber = row["partnumber"].ToString();
                CommissionType = row["commissiontype"].ToString();

                ItemType = row["type"].ToString().Trim();

                if (row["quantity"].ToString() != "") Quantity = int.Parse(row["quantity"].ToString());
                if (row["price"].ToString() != "") Price = (decimal)row["price"];
                if (row["cost"].ToString() != "") Cost = (decimal)row["cost"];
                if (row["commissionamt"].ToString() != "") CommissionRate = (decimal)row["commissionamt"];
                if (row["discount"].ToString() != "") Discount = (decimal)row["discount"];
                if (row["surcharge"].ToString() != "") Surcharge = (decimal)row["surcharge"];
                if (row["taxexempt"].ToString() != "") TaxExempt = row["taxexempt"].ToString().Equals("1");


        


            }
            catch (Exception e)
            {

                MessageBox.Show("LineItem constructor: " + e.Message);
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
             return String.Format("{0:0.00}", Price);
           
            }

        }

        public decimal Margin { 
            get {
                if (ItemType.ToUpper() == "PRODUCT")
                {
                    if (Cost == 0) return 0;
                    return AdjustedPrice - Cost;
                }
                else return AdjustedPrice;
                
            } 
        }

        public decimal TotalMargin
        {
            get
            {
                if (ItemType.ToUpper() == "PRODUCT")
                {
                    if (Cost == 0) return 0;
                    return TotalAdjustedPrice - TotalCost;
                }
                else return TotalAdjustedPrice;

            } 
        }

        public string CostStr
        {
            get
            {
                if(ItemType.ToUpper() == "PRODUCT")
                {
                    return String.Format("${0:0.00}", Cost);
                }else
                return "--.--";

            }

        }


        public string TotalCostStr
        {
            get
            {
                if (ItemType.ToUpper() == "PRODUCT")
                {
                    return String.Format("{0:0.00}", TotalCost);
                }
                else
                    return "--.--";

            }

        }

        public string CostColor
        {
            get
            {
                if (ItemType.ToUpper() == "PRODUCT")
                {
                    if (Cost == 0) return "Red"; else return "Black";
                }
                else
                    return "Black";

                

            }
        }

        public decimal Commission
        {
            get
            {
                return Math.Round( CommissionRate/100 * Margin,2);
            }
        }

        public decimal TotalCommission
        {
            get
            {
                return Math.Round(CommissionRate / 100 * TotalMargin, 2);
            }
        }

        public string PriceStrExtended
        {
            get
            {
               return String.Format("{0:0.00}",Quantity *  AdjustedPrice);
               
            }

        }


        public string DiscountStr
        {
            get
            {
                if (Discount > 0)
                    return Utility.FormatPrintRow("**Promo Discount**  ", "-" + String.Format("{0:0.00}", Discount), 38);
                else return "";

            }

        }
        public string SurchargeStr
        {
            get
            {
                if (Surcharge > 0)
                    return Utility.FormatPrintRow("**Surcharge**  ", "+" + String.Format("{0:0.00}", Surcharge), 38);
                else return "";

            }

        }

        public string DiscountStrExtended
        {
            get
            {
                if (Discount > 0)
                    return Utility.FormatPrintRow("**Promo Discount**  ", "-" + String.Format("{0:0.00}", Discount), 46);
                else return "";

            }

        }
        public string SurchargeStrExtended
        {
            get
            {
                if (Surcharge > 0)
                    return Utility.FormatPrintRow("**Surcharge**  ", "+" + String.Format("{0:0.00}", Surcharge), 46);
                else return "";

            }

        }

        public string ReceiptStr
        {
            get
            {
                if (ID > 0) return Utility.FormatPrintRow(Quantity + " " + ModelNumber + "--" +  Description , " " + PriceStr, 42);
                else return Description;
            }

        }


        public string ReceiptStrExtended
        {
            get
            {
                if (ID > 0) return Utility.FormatPrintRow(" " + Quantity + "  " + ModelNumber + "--" +  Description," " +   PriceStr, 46) + Utility.FormatPrintRow(" ", PriceStrExtended, 10);
                else return Description;
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
                return Price + Surcharge - Discount;
            }
        }

        public decimal TotalAdjustedPrice
        {
            get
            {
                return (Price + Surcharge - Discount) * Quantity;
            }
        }

        public decimal TotalCost
        {
            get
            {
                return Cost * Quantity;
            }
        }


        public decimal PriceSurcharge
        {
            get
            {
                return Price + Surcharge;
            }
        }


  
     
    }



}
