using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;

namespace WebAccess.Helper
{
    public class SelectListHelper
    {
       
  

      



        public static SelectList GetAllUsers()
        {
            storeEntities context = new storeEntities();
            var customer = context.AspNetUsers.Select(x => new { Id = x.Id, Name = x.FirstName + " " + x.LastName });
            return new SelectList(customer, "Id", "Name");
        }


        public static SelectList GetAllStoresList( object selectedValue=null)
        {
            webaccessEntities context = new webaccessEntities();

            var locations = context.stores.OrderBy(x => x.storename).ToList().Select(s => new
            {
                Id = s.id,
                Name = s.storename
            });
            return new SelectList(locations, "Id", "Name",selectedValue);
        }

  


        public static SelectList GetLastTwoYears(object selectedValue = null)
        {
           
            int start= DateTime.Now.Year - 2;



            var yearlist = Enumerable.Range(start, 3).Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToString()
                }).OrderByDescending(x => x.Value);



            var years = new SelectList(yearlist, "Value", "Text", selectedValue);

            return years;
        }

        public static SelectList GetReportTypes( object selectedValue = null)
        {
            var quarters = new SelectList( new List<SelectListItem>
                {
                    new SelectListItem {Text="Daily",Value="1"},
                    new SelectListItem {Text="Weekly",Value="2"},
                    new SelectListItem {Text="Monthly",Value="3"},
                    new SelectListItem {Text="Yearly",Value="4"}
                
                
                },"Value","Text",selectedValue);

            return quarters;
        }
    }

    public class SelectListPairs
    {
        public int Id { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Combined {
            get{

                if(Field2 == null || Field2 == "")
                {
                    return Field1;
                }else
                {

                    return Field1  + " [" + Field2 + "]";
                }
            }
        }
    }
}