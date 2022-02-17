using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace RedDot
{
    public class LineItem
    {

        public int ID { get; set; }
        public int ProductID { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public decimal Surcharge { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
   
        public string Note { get; set; }

        public string ItemType { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }

        public bool Taxable { get; set; }


        public decimal SupplyFee { get; set; }
        public bool Selected { get; set; }


        public LineItem(string description)
        {
            ID = 0;
            Description = description;
            Quantity = 0;
            Price = 0;
            Discount = 0;
            Surcharge = 0;
        
       
            ItemType = "";

            Selected = false;
           
        }


          public LineItem(DataRow row)
        {

            try
            {
              
          
                if (row["id"].ToString() != "") ID = int.Parse(row["id"].ToString());
                if (row["productid"].ToString() != "") ProductID = int.Parse(row["productid"].ToString());
                Description = row["description"].ToString(); 
                Note = row["note"].ToString();
                Custom1 = row["custom1"].ToString();
                Custom2 = row["custom2"].ToString();
                Custom3 = row["custom3"].ToString();
                Custom4 = row["custom4"].ToString();
              
             

                ItemType = row["type"].ToString().Trim();

                 Quantity = 1;
                if (row["price"].ToString() != "") Price = (decimal)row["price"];


                DiscountName = row["discountname"].ToString();
                DiscountType = row["discounttype"].ToString();
                if (row["discount"].ToString() != "") Discount = (decimal)row["discount"];
                if (row["surcharge"].ToString() != "") Surcharge = (decimal)row["surcharge"];

                if (row["supplyfee"].ToString() != "") SupplyFee = (decimal)row["supplyfee"];

                Taxable = Convert.ToBoolean(row["taxable"]);
           

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("LineItem constructor: " + e.Message);
            }


        }




        public string DiscountName { get; set; }
        public string DiscountType { get; set; }


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
                {
                    string disc = DiscountName;
                    if (disc.Length > 20) disc = DiscountName.Substring(0, 20);

                    return Utility.FormatPrintRow("**" + disc + "** ", "-" + String.Format("{0:0.00}", Discount), 30);
                }
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

        public string SupplyFeeStr
          {
              get
              {
                  if (SupplyFee > 0)
                      return Utility.FormatPrintRow("Supply Fee: ", String.Format("{0:0.00}", SupplyFee), 30);
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

    }
}
