using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace RedDot
{
    public class MenuSetupModel:INPCBase
    {

        private DBProducts m_dbproducts;
   
        public MenuSetupModel()
        {
            m_dbproducts = new DBProducts();
        }

        public DataTable GetCategoryList(string menu, bool onmenu)
        {
            DataTable data_category = m_dbproducts.GetCategorybyType(menu, onmenu);
            return data_category;
        }

        public ObservableCollection<Category> FillCategorybyType(string cattype)
        {
            ObservableCollection<Category> data = new ObservableCollection<Category>();

            Category category;

            DataTable data_category = m_dbproducts.GetCategorybyType(cattype,false);

            DataRow newrow = data_category.NewRow();
            newrow["id"] = 1000;
            newrow["colorcode"] = "Gray";
            newrow["description"] = "All Menu Items";
            newrow["cattype"] = cattype;

            data_category.Rows.InsertAt(newrow, data_category.Rows.Count);

               

            foreach (DataRow cat in data_category.Rows)
            {
               // category = new Category((int)cat["id"], cat["description"].ToString().Trim(), cat["imagesrc"].ToString(), cat["colorcode"].ToString(), cat["cattype"].ToString());
                category = new Category(cat);
                data.Add(category);
            }

            return data;
        }

        public ObservableCollection<Product> FillProductbyCatID(int catid, OrderType m_ordertype)
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>();

            DataTable data_category;
            Product newitem;

            m_dbproducts = new DBProducts();

            if (catid == 1000)
            {
                data_category = m_dbproducts.GetAllProducts();
            }
            else if (catid == 999)
            {
                data_category = m_dbproducts.GetRecentProducts( );
            }
            else
                data_category = m_dbproducts.GetProductsByCat(catid);

            foreach (DataRow row in data_category.Rows)
            {

                newitem = new Product(row,m_ordertype);


                products.Add(newitem);

            }
            return products;

        }

        public Product GetProductbyID(int id,OrderType m_ordertype)
        {
            DataTable dt = m_dbproducts.GetProductByID(id);
            if (dt.Rows.Count > 0)
            {
                Product prod = new Product(dt.Rows[0],m_ordertype);
                return prod;
            }
            else return null;


        }

        public int GetProductID(string menuprefix)
        {
            return m_dbproducts.GetProductIDByMenuPrefix(menuprefix);
        }

        public DataTable GetCateogories(string cattype)
        {
            return m_dbproducts.GetCategorybyType(cattype,false);
        }

        public DataTable GetProducts()
        {
            return m_dbproducts.GetProductsByType("product");
        }

        public DataTable GetMenuPrefix()
        {
            return m_dbproducts.GetMenuPrefix();
        }
        public DataTable GetMenuPrefix2(string menuprefix)
        {
            return m_dbproducts.GetMenuPrefix2(menuprefix);
        }
        public DataTable GetAllProducts()
        {
            return m_dbproducts.GetAllProducts();

        }
        public DataTable GetProductsNOTbyCatID(string type, int catid)
        {
            return m_dbproducts.GetProductsNOTByCat(type, catid);

        }

        public DataTable GetAvailableComboSetProducts(int combosetid)
        {
            return m_dbproducts.GetAvailableComboSetProducts( combosetid);
        }

        public DataTable GetProductsbyCatID(int catid)
        {
            return m_dbproducts.GetProductsByCat(catid);
        }

        public bool AddItemToCategory(int catid, int prodid)
        {
            return m_dbproducts.AddItemToCategory(catid, prodid);
        }

        public bool RemoveItemFromCategory(int catid, int prodid)
        {
            return m_dbproducts.RemoveItemFromCategory(catid, prodid);
        }


        public bool AddCategory(string type, string catname)
        {

            return m_dbproducts.AddCategory(type, catname);
        }

        public bool DeleteCategory(int catid)
        {
            return m_dbproducts.DeleteCategory(catid);
        }


        public bool AddProfile(string profilename)
        {
            return m_dbproducts.AddProfile(profilename);
        }
        public bool DeleteProfile(int profileid)
        {
            return m_dbproducts.DeleteProfile(profileid);
        }
        public bool AddModGroupToProfile(int profileid, int modgroupid)
        {
            return m_dbproducts.AddModGroupToProfile(profileid, modgroupid);
        }

        public bool AddModifierToModGroup( int modgroupid, int modifierid)
        {
            return m_dbproducts.AddModifierToModGroup( modgroupid,modifierid);
        }



        public int AddNewModGroup( string modgroupname)
        {
            return m_dbproducts.AddNewModGroup( modgroupname);
        }
        public bool DeleteModGroup(int modgroupid)
        {
            return m_dbproducts.DeleteModGroup(modgroupid);
        }
        public bool IsModGroupUsed(int modgroupid)
        {
            return m_dbproducts.IsModGroupUsed(modgroupid);
        }
        public bool RemoveModGroup(int modprofileid,int modgroupid)
        {
            return m_dbproducts.RemoveModGroupFromProfile(modprofileid, modgroupid);
        }
        public string GetCategoryName(int catid)
        {
            return m_dbproducts.GetCategoryName(catid);
        }

        public bool CopyColorToProducts(int catid, string colorcode)
        {

            return m_dbproducts.CopyColorToProducts(catid, colorcode);
        }
        public Product AddNewProduct()
        {

            DataTable dt = m_dbproducts.AddNewProduct();
            if (dt.Rows.Count > 0)
            {
                return new Product(dt.Rows[0],0);

            }
            else return null;
        }

        public Product CloneProduct(int id)
        {
            DataTable dt = m_dbproducts.CloneProduct(id);
            if (dt.Rows.Count > 0)
            {
                return new Product(dt.Rows[0],0);

            }
            else return null;
        }

        public Modifier AddNewModifier(string description)
        {

            DataTable dt = m_dbproducts.AddNewModifier(description);
            if (dt.Rows.Count > 0)
            {
                return new Modifier(dt.Rows[0]);

            }
            else return null;
        }


        public Modifier CloneModifier(int id)
        {

            DataTable dt = m_dbproducts.CloneModifier(id);
            if (dt.Rows.Count > 0)
            {
                return new Modifier(dt.Rows[0]);

            }
            else return null;
        }
        public bool DeleteModifier(int modid)
        {
            return m_dbproducts.DeleteModifier(modid);

        }

        public bool RemoveModifier(int modgroupid,int modid)
        {
            return m_dbproducts.RemoveModifier(modgroupid,modid);

        }

        public bool DeleteProduct(int productid)
        {
            return m_dbproducts.DeleteProduct(productid);

        }

        public ObservableCollection<ModProfile> GetProfiles(bool insertnone)
        {
            ObservableCollection<ModProfile> profiles = new ObservableCollection<ModProfile>();
            DataTable dt = m_dbproducts.GetModProfiles();
            DataRow newrow;

            if (insertnone)
            {
                //Add empty row on top so user can select "none"
                newrow = dt.NewRow();
                newrow["id"] = 0;
                newrow["Description"] = "None";
                dt.Rows.InsertAt(newrow, 0);
            }



            newrow = dt.NewRow();
            newrow["id"] = 1000;
            newrow["description"] = "All Modifier Groups";
   

            dt.Rows.InsertAt(newrow, dt.Rows.Count);

            foreach(DataRow row in dt.Rows)
            {
                profiles.Add(new ModProfile(row));
            }
            return profiles;
        }

        public DataTable GetAvailableModGroups(int profileid)
        {
            return m_dbproducts.GetAvailableModGroups(profileid);
        }

        public ObservableCollection<ModGroup> GetModGroups(int profileid, bool includeglobal=false)
        {
            DataTable dt;

            if (profileid == 1000) dt = m_dbproducts.GetAllModGroups();
            else  dt= m_dbproducts.GetModGroupsByProfileID(profileid);

            ObservableCollection<ModGroup> modgroups = new ObservableCollection<ModGroup>();
            ModGroup modgroup;

            foreach (DataRow row in dt.Rows)
            {
                modgroup = new ModGroup(row);
                if (profileid == 1000) modgroup.Editable = false;
                //load all modifiers in mod group
                modgroup.Modifiers = GetModGroupModifiers(modgroup.ID);
                modgroups.Add(modgroup);
            }

            if(includeglobal)
            {
                //global modifiers
                modgroup = new ModGroup();
                modgroup.Description = "Global Modifiers";
                //load all modifiers in mod group
                modgroup.Modifiers = GetGlobalModifiers();
                modgroups.Add(modgroup);
            }

  


            return modgroups;
        }


        public ObservableCollection<Modifier> GetModGroupModifiers(int groupid)
        {
            DataTable dt = m_dbproducts.GetModGroupModifiers(groupid);
            ObservableCollection<Modifier> modifiers = new ObservableCollection<Modifier>();
            Modifier modifier;
            foreach (DataRow row in dt.Rows)
            {
                modifier = new Modifier(row);
   

                modifiers.Add(modifier);
            }
            return modifiers;
        }

        public ObservableCollection<Modifier> GetAvailableModifiers(int modgroupid)
        {
           DataTable dt =  m_dbproducts.GetAvailableModifiers(modgroupid);
            ObservableCollection<Modifier> modifiers = new ObservableCollection<Modifier>();
            Modifier modifier;
            foreach (DataRow row in dt.Rows)
            {
                modifier = new Modifier(row);


                modifiers.Add(modifier);
            }
            return modifiers;
        }


        public ObservableCollection<Modifier> GetAllModifiers()
        {
            DataTable dt = m_dbproducts.GetAllModifiers();
            ObservableCollection<Modifier> modifiers = new ObservableCollection<Modifier>();
            Modifier modifier;
            foreach (DataRow row in dt.Rows)
            {
                modifier = new Modifier(row);


                modifiers.Add(modifier);
            }
            return modifiers;
        }


        public ObservableCollection<Modifier> GetGlobalModifiers()
        {
            DataTable dt = m_dbproducts.GetGlobalModifiers();
            ObservableCollection<Modifier> modifiers = new ObservableCollection<Modifier>();
            Modifier modifier;
            foreach (DataRow row in dt.Rows)
            {
                modifier = new Modifier(row);


                modifiers.Add(modifier);
            }
            return modifiers;
        }
        //----------------------------------------------------------COMBO -------------------------------------------------------

        public bool AddComboGroup(string profilename)
        {
            return m_dbproducts.AddComboGroup(profilename);
        }

        public bool DeleteComboGroup(int combogroupid)
        {
            return m_dbproducts.DeleteComboGroup(combogroupid);
        }


        public ObservableCollection<ComboGroup> GetComboGroups(bool insertnone)
        {
            ObservableCollection<ComboGroup> combogroups = new ObservableCollection<ComboGroup>();
            DataTable dt = m_dbproducts.GetComboGroups();
            DataRow newrow;

            if (insertnone)
            {
                //Add empty row on top so user can select "none"
                newrow = dt.NewRow();
                newrow["id"] = 0;
                newrow["Description"] = "None";
                dt.Rows.InsertAt(newrow, 0);
            }else
            {
                newrow = dt.NewRow();
                newrow["id"] = 1000;
                newrow["description"] = "All Combo Groups";
                dt.Rows.InsertAt(newrow, dt.Rows.Count);
            }


            foreach (DataRow row in dt.Rows)
            {
                combogroups.Add(new ComboGroup(row));
            }
            return combogroups;
        }


        public List<ListPair> GetReportCategories()
        {
            List<ListPair> list = new List<ListPair>();
            DataTable dt = m_dbproducts.GetReportRevenue();

            foreach(DataRow row in dt.Rows)
            {
                list.Add(new ListPair() { Description = row["reportcategory"].ToString(), StrValue = row["reportcategory"].ToString() });
            }

            return list;
        }

        public ObservableCollection<ComboSet> GetComboSets(int groupid)
        {
            DataTable dt;

            if (groupid == 1000) dt = m_dbproducts.GetAllComboSets();
            else dt = m_dbproducts.GetComboSets(groupid);


          
            ObservableCollection<ComboSet> combosets = new ObservableCollection<ComboSet>();
            ComboSet comboset;
            foreach (DataRow row in dt.Rows)
            {
                comboset= new ComboSet(row);

                comboset.Products = GetComboProducts(comboset.ID,0);
                combosets.Add(comboset);
            }
            return combosets;
        }


        public ObservableCollection<Product> GetComboProducts(int comboid, OrderType m_ordertype)
        {
            DataTable dt = m_dbproducts.GetComboProducts(comboid);
            ObservableCollection<Product> products = new ObservableCollection<Product>();
            Product product;
            foreach (DataRow row in dt.Rows)
            {
                product = new Product(row,m_ordertype);


                products.Add(product);
            }
            return products;
        }



        public bool AddNewComboSet(string combosetname)
        {
            return m_dbproducts.AddNewComboSet( combosetname);
        }
        public bool DeleteComboSet(int combosetid)
        {
            return m_dbproducts.DeleteComboSet(combosetid);
        }

        public bool IsComboSetUsed(int combosetid)
        {
            return m_dbproducts.IsComboSetUsed(combosetid);
        }

        public bool RemoveComboSet(int combogroupid, int combosetid)
        {
            return m_dbproducts.RemoveComboSetFromComboGroup(combogroupid, combosetid);
        }

        public DataTable GetAvailableComboSets(int combogroupid)
        {
            return m_dbproducts.GetAvailableComboSets(combogroupid);
        }

        public bool AddComboSetToComboGroup(int combogroupid, int combosetid)
        {
            return m_dbproducts.AddComboSetToComboGroup(combogroupid, combosetid);
        }

        public bool AddToComboSet(int combosetid, int productid)
        {
            return m_dbproducts.AddToComboSet(combosetid, productid);
        }

        public bool RemoveFromComboSet(int combosetid, int productid)
        {
            return m_dbproducts.RemoveFromComboSet(combosetid, productid);
        }
    }


}
