using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBProducts
    {

        DBConnect m_dbconnect;

        public DBProducts()
        {

            m_dbconnect = new DBConnect();
        }



        //------------------------------------------------------------------- PRODUCTS ------------------------------------------------------------------------
        #region PRODUCTS

        public DataTable GetProductsByType(string type)
        {
            string query;

            query = "select * from  product where type='" + type + "' order by id " ;


            return m_dbconnect.GetData(query, "Product");
        }


        public int GetProductIDByMenuPrefix(string menuprefix)
        {
            string query;
            DataTable dt;

            query = "select id from product where menuprefix='" + menuprefix + "'";


            dt =  m_dbconnect.GetData(query, "Menu");

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["id"].ToString() != "") return int.Parse(dt.Rows[0]["id"].ToString());
                else return 0;

            }
            else return 0;
        }


        public DataTable GetMenuPrefix()
        {
            string query;

            query = "select distinct substr(menuprefix,1,1) as menuprefix from product where menuprefix is not null order by menuprefix asc ";


            return m_dbconnect.GetData(query, "Menu");
        }


        public DataTable GetMenuPrefix2(string menuprefix)
        {
            string query;

            query = "select distinct substr(menuprefix,2) as menuprefix from product where substr(menuprefix,1,1) ='" + menuprefix + "' order by cast(substr(menuprefix,2) as unsigned)  ";


            return m_dbconnect.GetData(query, "Menu");
        }
        public DataTable GetAllProducts()
        {
            string query;

           // query = "select * from  product order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.modelnumber, product.id ";
             query = "select 'all items' as category, product.*, concat('pack://siteoforigin:,,,/',product.imagesrc) as imgurl from  product order by SUBSTRING(product.menuprefix, 1, 1), cast(SUBSTRING(product.menuprefix, 2) as unsigned), product.description";


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetRecentProducts()
        {
            string query;

            query = "select   product.* from  product inner join (select distinct productid from salesitem order by id desc limit 10) as list on product.id = list.productid  order by SUBSTRING(product.menuprefix, 1, 1), cast(SUBSTRING(product.menuprefix, 2) as unsigned), product.description";


            return m_dbconnect.GetData(query, "Product");
        }
        public DataTable GetProductsByTypeDescription(string type, string description)
        {
            string query;

            query = "select * from  product where type='" + type + "' and (description like '%" + description.Trim() + "%' or modelnumber like '%" + description.Trim() + "%') order by  product.modelnumber, product.id";


            return m_dbconnect.GetData(query, "Product");
        }




        public DataTable GetProductsByCat(int catid)
        {
            string query = "select printers.description as printer, modprofile.description as modifier, category.description as category, category.colorcode as catcolorcode, product.*,concat('pack://siteoforigin:,,,/',product.imagesrc) as imgurl from category " + 
                " inner join cat2prod on category.id = cat2prod.catid inner join product on cat2prod.prodid = product.id " + 
                " left join printers on product.kitchenprinter = printers.id " + 
                " left join modprofile on product.modprofileid = modprofile.id " +
                " where category.id =" + catid + " order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.description";
           

            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetProductsNOTByCat(string type,int catid)
        {
            string query;

            if (type == "") query = "select * from product where id not in ( select prodid from cat2prod where catid=" + catid + ")  order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned),product.id";
            else query = "select * from product where id not in ( select prodid from cat2prod where catid=" + catid + ") and type='" + type + "'  order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.description";


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetProductByID(int productID)
        {
            string query;

            query = "select product.*, concat('pack://siteoforigin:,,,/',product.imagesrc) as imgurl from product where id=" + productID;


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetAvailableComboSetProducts(int combosetid)
        {
            string query = "select distinct printers.description as printer, category.description as category, category.colorcode as catcolorcode, product.*,concat('pack://siteoforigin:,,,/',product.imagesrc) as imgurl from category " +
                " inner join cat2prod on category.id = cat2prod.catid inner join product on cat2prod.prodid = product.id " +
                " left join printers on product.kitchenprinter = printers.id " +
                " where product.id not in ( select productid from comboset2product where combosetid=" + combosetid + ") and product.type = 'product' " +
                 " order by SUBSTRING(product.menuprefix,1,1), cast(SUBSTRING(product.menuprefix,2) as unsigned), product.id";


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable AddNewProduct()
        {
            string query;

            query = "insert into product (description,colorcode) values ('new item','AliceBlue')";
            m_dbconnect.Command(query);

            query = "select * from product where id = LAST_INSERT_ID()";
            return m_dbconnect.GetData(query);

        }

        public DataTable CloneProduct(int id)
        {
            string query;



            query = "insert into product (menuprefix,combogroupid,description,description2,description3,modprofileid,price,priceadj1,priceadj2,priceadj3,priceadj4,imagesrc,type,kitchenprinter,taxable,agerestricted,allowpartial,weighted,colorcode,reportcategory,qoh,barcode,cost,note) " +
      " select  menuprefix,combogroupid,concat(description,'(clone)'),description2,description3,modprofileid,price,priceadj1,priceadj2,priceadj3,priceadj4,imagesrc,type,kitchenprinter,taxable,agerestricted,allowpartial,weighted,colorcode,reportcategory,qoh,barcode,cost,note from product where id=" + id;
            m_dbconnect.Command(query);

            query = "select * from product where id = LAST_INSERT_ID()";
            return m_dbconnect.GetData(query);

        }
 
        public bool UpdateProductString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update product set " + field + " ='" + fieldvalue.Replace("'","''") + "' where id=" + id;
            return m_dbconnect.Command(query);

        }

        public bool CopyColorToProducts(int catid, string colorcode)
        {
            string query;

            query = "update product  inner join cat2prod on product.id = cat2prod.prodid set colorcode ='" + colorcode + "' where catid=" + catid;
            return m_dbconnect.Command(query);
        }

        public bool DeleteProduct(int productid)
        {
            string query;

            query = "delete from product where id=" + productid;
             m_dbconnect.Command(query);

            query = "delete from cat2prod where prodid=" + productid;
            return m_dbconnect.Command(query);

        }


        public bool UpdateNumeric(int id, string field, int fieldvalue)
        {
            string query;

            query = "update product set " + field + " =" + fieldvalue + " where id=" + id;
            return m_dbconnect.Command(query);
        }

        public bool UpdateNumeric(int id, string field, decimal fieldvalue)
        {
            string query;

            query = "update product set " + field + " =" + fieldvalue + " where id=" + id;
            return m_dbconnect.Command(query);
        }

        #endregion

        //------------------------------------------------------------------  Category ---------------------------------------------------------------
        #region CATEGORIES
        public DataTable GetCategoryByID(int catID)
        {
            string query;

            query = "select * from category where id=" + catID;


            return m_dbconnect.GetData(query, "Product");
        }

        public DataTable GetCategorybyType(string cattype,bool onmenu)
        {
            string query;

            if(cattype == "")
            {
                //gets all .. doesn't care if on menu or not .. used by promotion setting
                 query = "select category.*, concat('pack://siteoforigin:,,,/',category.imagesrc) as imgurl from category   order by sortorder ";

            }
            else
            {
                if (onmenu)
                    query = "select category.*, concat('pack://siteoforigin:,,,/',category.imagesrc) as imgurl from category where onmenu=1 and cattype='" + cattype + "'  order by sortorder ";
                else query = "select category.*, concat('pack://siteoforigin:,,,/',category.imagesrc) as imgurl from category where  cattype='" + cattype + "'  order by sortorder ";


            }

            return m_dbconnect.GetData(query, "Category");
        }

        public string GetCategoryName(int catid)
        {
            DataTable dt;
            string query = "select * from category where id=" + catid;
            dt = m_dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["description"].ToString();
            }
            else return "";

        }
        public bool AddItemToCategory(int catid, int prodid)
        {
            string query;

            query = "insert into cat2prod (catid,prodid) values ( " + catid + "," + prodid + ")";
            return m_dbconnect.Command(query);
        }

        public bool RemoveItemFromCategory(int catid, int prodid)
        {
            string query;

            query = "delete from cat2prod where catid=" + catid + " and prodid=" + prodid;
            return m_dbconnect.Command(query);
        }

        public bool AddCategory(string type, string catname)
        {
            string query;

            query = "insert into category (cattype,description,colorcode,sortorder) values ('" + type + "','" + catname + "','gray',(select coalesce(max(sortorder),0)  from category  as max where cattype='" + type + "') + 1 )";
            return m_dbconnect.Command(query);
        }


    
        public bool UpdateCategoryString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update category set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }

        public bool UpdateCategoryInt(int id, string field, int fieldvalue)
        {
            string query;

            query = "update category set " + field + " =" + fieldvalue + " where id=" + id;
            return m_dbconnect.Command(query);

        }
        public bool DeleteCategory(int catid)
        {
            string query;

            query = "delete from  category where id=" + catid;
            return m_dbconnect.Command(query);
        }

        #endregion


        //----------------------------------------------------------------- MODIFIERS -----------------------------------------------------------
        #region MODIFIERS

        public DataTable AddNewModifier(string description)
        {
            string query;

            string desc = description;

            if (desc == "") desc = "new modifier";

            query = "insert into modifiers (description,price,colorcode,isglobal) values ('" + desc + "',0,'Black',1)";
            m_dbconnect.Command(query);

            query = "select * from modifiers where id = LAST_INSERT_ID()";
            return m_dbconnect.GetData(query);

        }

        public DataTable CloneModifier(int id)
        {
            string query;

            query = "insert into modifiers (modgroupid,description,price) select  modgroupid,concat(description,'(clone)'),price from modifiers where id=" + id;
            m_dbconnect.Command(query);

            query = "select * from modifiers where id = LAST_INSERT_ID()";
            return m_dbconnect.GetData(query);

        }
        public DataTable GetProfileByID(int id)
        {
            string query;
            query = "select * from modprofile where id=" + id;
            return m_dbconnect.GetData(query);
        }
        public DataTable GetModProfiles()
        {
            string query;
            query = "select * from modprofile order by description ";
            return  m_dbconnect.GetData(query);

        }
        public DataTable GetModGroupsByProfileID(int profileid)
        {
            string query;
            query = "select * from modgroup inner join profile2group on modgroup.id = profile2group.modgroupid where profile2group.modprofileid =" + profileid + " order by sortorder asc";
            return m_dbconnect.GetData(query);
        }

        public DataTable GetForcedModGroupsByProfileID(int profileid)
        {
            string query;
            query = "select * from modgroup inner join profile2group on modgroup.id = profile2group.modgroupid where min > 0 and profile2group.modprofileid =" + profileid + " order by sortorder asc";
            return m_dbconnect.GetData(query);
        }


        public DataTable GetAvailableModGroups(int profileid)
        {
            string query;
            query = "select * from modgroup where id not in (select modgroupid from profile2group where profile2group.modprofileid =" + profileid + ")";
       
            return m_dbconnect.GetData(query);
        }
        public DataTable GetModGroup(int modgroupid)
        {
            string query;
            query = "select * from modgroup where id =" + modgroupid;
            return m_dbconnect.GetData(query);
        }
        public DataTable GetAllModGroups()
        {
            string query;
            query = "select id as modgroupid,1000 as modprofileid, 0 as min, 0 as max, 0 as allowduplicate, 0 as sortorder, modgroup.* from modgroup";
            return m_dbconnect.GetData(query);
        }
        public DataTable GetModGroupModifiers(int modgroupid)
        {

            string query;
            query = "select * from modifiers inner join modgroup2modifier on modifiers.id = modgroup2modifier.modifierid where modgroup2modifier.modgroupid = " + modgroupid + " order by sortorder ";
            return m_dbconnect.GetData(query);

        }

        public DataTable GetModifierByID(int modid)
        {

            string query;
            query = "select * from modifiers where id = " + modid;
            return m_dbconnect.GetData(query);

        }
        public DataTable GetAvailableModifiers(int modgroupid)
        {
            string query;
            query = "select * from modifiers where id not in (select modifierid from modgroup2modifier where modgroup2modifier.modgroupid =" + modgroupid + ") order by description";

            return m_dbconnect.GetData(query);
        }

        public DataTable GetAllModifiers()
        {
            string query;
            query = "select * from modifiers order by description";

            return m_dbconnect.GetData(query);
        }

        public DataTable GetGlobalModifiers()
        {
            string query;
            query = "select * from modifiers where isglobal=1 order by description";

            return m_dbconnect.GetData(query);
        }

        public bool AddProfile( string profilename)
        {
            string query;

            query = "insert into modprofile (description) values ('" + profilename + "')";
            return m_dbconnect.Command(query);
        }
        public bool DeleteProfile(int modprofileid)
        {
            string query;

            query = "delete from modprofile where id=" + modprofileid;
            return m_dbconnect.Command(query);
        }

        public bool UpdateProfileString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update modprofile set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }

        public int AddNewModGroup( string modgroupname)
        {
            string query;

            query = "insert into modgroup (description) values ('" + modgroupname + "')";
              m_dbconnect.Command(query);

            //get last item
            query = "select id from product where id = LAST_INSERT_ID()";
           DataTable dt =  m_dbconnect.GetData(query);
            string id = dt.Rows[0]["id"].ToString();
            return int.Parse(id);

        }
        public bool AddModGroupToProfile(int modprofileid, int modgroupid)
        {
            string query;

            query = "insert into profile2group (modprofileid,modgroupid,min,max,sortorder) values (" + modprofileid + "," + modgroupid + ",0,0,(select coalesce(max(sortorder),0)  from profile2group  as max where modprofileid=" + modprofileid + ") + 1)";
            return m_dbconnect.Command(query);
        }


        public bool AddModifierToModGroup(int modgroupid, int modifierid)
        {
            string query;

            query = "insert into modgroup2modifier (modgroupid,modifierid,sortorder) values (" + modgroupid + "," + modifierid + ",(select coalesce(max(sortorder),0)  from modgroup2modifier  as max where modgroupid=" + modgroupid + ") + 1)";
            return m_dbconnect.Command(query);
        }



        public bool DeleteModGroup(int modgroupid)
        {
            string query;

            query = "delete from modgroup where id=" + modgroupid;
            return m_dbconnect.Command(query);
        }

        public bool IsModGroupUsed(int modgroupid)
        {
            string query;
            query = "select count(*) as total from profile2group where modgroupid=" + modgroupid;

            var dt = m_dbconnect.GetData(query);
            if (dt.Rows[0]["total"].ToString() == "0") return false; else return true;
        }

        public bool RemoveModGroupFromProfile(int modprofileid,int modgroupid)
        {
            string query;

            query = "delete from profile2group where modprofileid=" + modprofileid  + " and modgroupid=" + modgroupid;
            return m_dbconnect.Command(query);
        }

        public bool RemoveModifier(int modgroupid,int modid)
        {
            string query;

            query = "delete from modgroup2modifier where modgroupid=" + modgroupid + " and  modifierid=" + modid;
            return m_dbconnect.Command(query);
        }


        public bool DeleteModifier(int modid)
        {
            string query;

            query = "delete from modifiers where id=" + modid;
            m_dbconnect.Command(query);

            query = "delete from modgroup2modifier where  modifierid=" + modid;
            return m_dbconnect.Command(query);
        }
        public bool UpdateModGroupString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update modgroup set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }

        public bool UpdateModGroupInt(int modprofileid, int modgroupid, string field, int fieldvalue)
        {
            string query;

            query = "update profile2group set " + field + " =" + fieldvalue + " where modprofileid =" + modprofileid + " and modgroupid=" + modgroupid;
            return m_dbconnect.Command(query);

        }


        public bool UpdateModifierString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update modifiers set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }

        public bool UpdateModifierInt(int id, string field, int fieldvalue)
        {
            string query;

            query = "update modifiers set " + field + " =" + fieldvalue + " where id=" + id;
            return m_dbconnect.Command(query);

        }

        public bool UpdateModifierPrice(int id, decimal price)
        {
            string query;

            query = "update modifiers set price =" + price + " where id=" + id;
            return m_dbconnect.Command(query);

        }
        #endregion


        //-----------------------------------------------------------COMBO --------------------------------------------------------
        public bool AddComboGroup(string groupname)
        {
            string query;

            query = "insert into combogroup (description) values ('" + groupname + "')";
            return m_dbconnect.Command(query);
        }

        public bool DeleteComboGroup(int comboid)
        {
            string query;

            query = "delete from combogroup where id=" + comboid;
            return m_dbconnect.Command(query);
        }

        public bool UpdateComboGroupString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update combogroup set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }

        public DataTable GetAllComboGroups()
        {
            string query;
            query = "select * from combogroup";
            return m_dbconnect.GetData(query);
        }

        public DataTable GetComboGroup(int combogroupid)
        {
            string query;
            query = "select * from combogroup where id =" + combogroupid;
            return m_dbconnect.GetData(query);
        }

        public DataTable GetComboGroups()
        {
            string query;
            query = "select * from combogroup" ;
            return m_dbconnect.GetData(query);
        }

        public DataTable GetReportRevenue()
        {
            string query;
            query = "select distinct reportcategory from reportgroup inner join  reportsetup on reportgroup.id = reportsetup.reportgroupid where grouptype ='RevenueCategory' order by sortorder";
            return m_dbconnect.GetData(query);
        }

        public DataTable GetComboSetByID(int id)
        {

            string query;
            query = "select * from comboset where id = " + id;
            return m_dbconnect.GetData(query);

        }

        public DataTable GetComboSets(int groupid)
        {

            string query;
            query = "select * from comboset inner join combogroup2comboset on comboset.id = combogroup2comboset.combosetid  where combogroupid=" + groupid + " order by combogroup2comboset.sortorder asc";
            return m_dbconnect.GetData(query);

        }
        public DataTable GetAllComboSets()
        {

            string query;
            query = "select 1000 as combogroupid, comboset.id as combosetid,0 as maxprice, 0 as min, 0 as max, 0 as allowduplicate, comboset.* from comboset";
            return m_dbconnect.GetData(query);

        }

        public DataTable GetComboProducts(int combosetid)
        {

            string query;
            query = "select product.* from product inner join comboset2product on product.id = comboset2product.productid where combosetid = " + combosetid + " order by description ";
            return m_dbconnect.GetData(query);

        }

        public bool UpdateComboSetString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update comboset set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }

        public bool UpdateComboSetPrice(int combogroupid,int combosetid, decimal price)
        {
            string query;

            query = "update combogroup2comboset set maxprice =" + price + " where combogroupid=" + combogroupid + " and  combosetid=" + combosetid;
            return m_dbconnect.Command(query);

        }

        public bool UpdateComboSetInt(int combogroupid, int combosetid, string field, int fieldvalue)
        {
            string query;

            query = "update combogroup2comboset set " + field + " =" + fieldvalue + " where combogroupid=" + combogroupid + " and  combosetid=" + combosetid;
            return m_dbconnect.Command(query);

        }


        public bool AddNewComboSet( string combosetname)
        {
            string query;

            query = "insert into comboset (description) values ('" + combosetname + "')";
           return  m_dbconnect.Command(query);

        }
        public bool AddComboSetToComboGroup(int combogroupid, int combosetid)
        {
            string query;


            query = "insert into combogroup2comboset (combogroupid,combosetid,maxprice,min,max,sortorder) values (" + combogroupid + "," + combosetid + ",0.0,0,1,(select coalesce(max(sortorder),0) from combogroup2comboset  as max where combogroupid=" + combogroupid + ") + 1)";
            return m_dbconnect.Command(query);
        }
        public bool DeleteComboSet(int combosetid)
        {
            string query;

            query = "delete from comboset where id=" + combosetid;
            return m_dbconnect.Command(query);
        }

        public bool IsComboSetUsed(int combosetid)
        {
            string query;
            query = "select count(*) as total from combogroup2comboset where combosetid=" + combosetid;

            var dt =  m_dbconnect.GetData(query);
            if (dt.Rows[0]["total"].ToString() == "0") return false; else return true;
        }

        public bool RemoveComboSetFromComboGroup(int combogroupid, int combosetid)
        {
            string query;

            query = "delete from combogroup2comboset where combogroupid=" + combogroupid + " and combosetid=" + combosetid;
            return m_dbconnect.Command(query);
        }

        public DataTable GetAvailableComboSets(int combogroupid)
        {
            string query;
            query = "select * from comboset where id not in (select combosetid from combogroup2comboset where combogroup2comboset.combogroupid =" + combogroupid + ")";

            return m_dbconnect.GetData(query);
        }


        public bool AddToComboSet(int combosetid,int productid)
        {
            string query;
            query = "insert into comboset2product (combosetid,productid) values (" + combosetid + "," + productid + ")";

            return m_dbconnect.Command(query);
        }

        public bool RemoveFromComboSet(int combosetid, int productid)
        {
            string query;
            query = "delete from  comboset2product where combosetid=" + combosetid + " and productid=" + productid ;

            return m_dbconnect.Command(query);
        }

    }
}
