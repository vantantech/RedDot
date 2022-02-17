using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;

    using RedDot.DataManager;

namespace RedDot
{


    public class SalonLineItem:INPCBase
    {

        public int ID { get; set; }
        public int ProductID { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public decimal Surcharge { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Employee AssignedEmployee { get; set; }
        public string Note { get; set; }
        public string CommissionType { get; set; }
        public string ItemType { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string ReportCategory { get; set; }
        public decimal CommissionPercent { get; set; }
        public decimal CommissionAmount { get; set; }

        public decimal TurnValue { get; set; }
        public decimal SupplyFee { get; set; }



     



        private DBTicket _dbticket;
     

        public SalonLineItem(string description)
        {
            ID = 0;
            Description = description;
            Quantity = 0;
            Price = 0;
            Discount = 0;
     
            AssignedEmployee = null;
            ItemType = "";
            Surcharge = 0;


        }

        private bool _deductdiscount = false;


        public SalonLineItem(DataRow row, decimal commissionpercent, bool deductdiscount)
        {

            try
            {
                _deductdiscount = deductdiscount;
             
                _dbticket = new DBTicket();
                if (row["id"].ToString() != "") ID = int.Parse(row["id"].ToString());
                if (row["productid"].ToString() != "") ProductID = int.Parse(row["productid"].ToString());
                Description = row["description"].ToString().Replace('\r', ' ').Replace('\n', ' '); 
                Note = row["note"].ToString();
                Custom1 = row["custom1"].ToString();
                Custom2 = row["custom2"].ToString();
                Custom3 = row["custom3"].ToString();
                Custom4 = row["custom4"].ToString();
                ReportCategory = row["reportcategory"].ToString();
                CommissionType = row["commissiontype"].ToString();
             

                ItemType = row["type"].ToString().Trim();

                 Quantity = 1;
                if (row["price"].ToString() != "") Price = (decimal)row["price"];
                if (row["turnvalue"].ToString() != "") TurnValue = (decimal)row["turnvalue"];

              
                    CommissionPercent = commissionpercent;


                if (row["commissionamt"].ToString() != "") CommissionAmount = (decimal)row["commissionamt"]; else CommissionAmount = 0;
             



                if (row["surcharge"].ToString() != "") Surcharge = (decimal)row["surcharge"];

                if (row["discount"].ToString() != "") Discount = (decimal)row["discount"];
              
                if (row["supplyfee"].ToString() != "") SupplyFee = (decimal)row["supplyfee"];

   
                if (row["employeeid"].ToString() != "")
                {
                    AssignedEmployee = new Employee(int.Parse(row["employeeid"].ToString()));
                }


            }
            catch (Exception e)
            {

                TouchMessageBox.Show("LineItem constructor: " + e.Message);
            }


        }

        public SalonLineItem()
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




        public string DiscountStr
        {
            get
            {
                if (Discount > 0)
                    return Utility.FormatPrintRow("**Promo Discount**  ", "-" + String.Format("{0:0.00}", Discount), 30);
                else return "";
            }
        }


        public string SupplyFeeStr
        {
            get
            {
                if (SupplyFee > 0)
                    return Utility.FormatPrintRow("Supply Fee: ",   String.Format("{0:0.00}", SupplyFee), 30);
                else return "";

            }

        }
        public string SurchargeStr
        {
            get
            {
                if (Surcharge > 0)
                    return Utility.FormatPrintRow("**Service Upgrade**  ", "+" + String.Format("{0:0.00}", Surcharge), 30);
                else return "";

            }

        }

        public string ReceiptStr
        {
            get
            {
                if (ID > 0) return Utility.FormatPrintRow(" " + Description, " " + PriceStr, 35);
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
                return Price - Discount + Surcharge;
            }
        }

        public ProductType ItemEnumType
        {
            get
            {
                switch (ItemType.Replace(" ", "").ToUpper())
                {
                    case "GIFTCARD": return ProductType.GiftCard;
                    case "GIFTCERTIFICATE": return ProductType.GiftCertificate;
                    case "PRODUCT": return ProductType.Product;
                    case "SERVICE": return ProductType.Service;


                    default: return ProductType.Service;
                }
            }
        }


        public decimal CommissionPrice
        {
            get
            {
                if (CommissionType == "none")
                {
                    return 0;
                }
                else 
                {
                    if(_deductdiscount)
                    {
                        return Price + Surcharge - Discount;
                    }
                    else
                    {
                        return Price + Surcharge;
                    }
                   
                }

                
            }
           
        }

        public decimal Commission
        {
            get
            {
                if(CommissionType == "percent")
                {
                        return (CommissionPrice - SupplyFee) * CommissionPercent / 100;
                }
                else
                {
                   if (CommissionType == "fixed")
                        //fixed commission amount
                        return CommissionAmount - SupplyFee;
                   else
                       return 0;
                }
            }
        }
    }



}
