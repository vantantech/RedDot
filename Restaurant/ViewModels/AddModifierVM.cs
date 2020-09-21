using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class AddModifierVM : INPCBase
    {
        private Ticket m_currentticket;
        private ModGroup _currentmodgroup;

        public ObservableCollection<ModGroup> ModGroups { get; set; }


        public ICommand ModifierAddClicked { get; set; }
        public ICommand ModifierDeleteClicked { get; set; }
        public ICommand DoneClicked { get; set; }
        public ICommand NextClicked { get; set; }
        public ICommand PreviousClicked { get; set; }
        public ICommand CancelClicked { get; set; }

        private int _currentmodgroupcount = 0;
        private LineItem _line;
        private int _lineid;
        private Window _parent;
        private MenuSetupModel m_inventorymodel;
        private int _currentindex = 0;
        private bool _minimum_met = false;
        private Product product;
        private bool m_newticket = false;

        public AddModifierVM(Window parent,Ticket ticket,int lineid, Product prod, bool newticket)
        {
            CurrentTicket = ticket;
            _parent = parent;
            m_inventorymodel = new MenuSetupModel();
            product = prod;
            m_newticket = newticket;
         
            _lineid = lineid;
            CurrentLine = CurrentTicket.GetLineItemLine(_lineid);

            ModifierAddClicked = new RelayCommand(ExecuteModifierAddClicked, param => CanExecuteModifierAddClicked);
            ModifierDeleteClicked = new RelayCommand(ExecuteModifierDeleteClicked, param => true);
            DoneClicked = new RelayCommand(ExecuteDoneClicked, param => CanExecuteDoneClicked);
            NextClicked = new RelayCommand(ExecuteNextClicked, param => CanExecuteNextClicked);
            PreviousClicked = new RelayCommand(ExecutePreviousClicked, param => CanExecutePreviousClicked);

            CancelClicked = new RelayCommand(ExecuteCancelClicked, param => m_newticket);

            ModGroups = m_inventorymodel.GetModGroups(prod.ModProfileID, true);  //include global modifiers also
       

            if (ModGroups.Count == 0) _minimum_met = true;
      
            SelectedIndex = 0; //this will also LoadMods()
        }

        private void LoadMods()
        {
            if (ModGroups == null) return;
            if (ModGroups.Count == 0) return;

            CurrentModGroup = ModGroups[_currentindex];


            _minimum_met = true;  //enable / disable the Done button

            int tempcount = 0;
            _currentmodgroupcount = 0;
           // int totalmin = ModGroups.Sum(x => x.Min);

            //get the count for that found
            foreach(ModGroup modgroup in ModGroups)
            {
                //only count modgroup that are required .. ie .. min > 0
                if(modgroup.Min > 0)
                {
                    tempcount = 0;
                    foreach (Modifier mod in modgroup.Modifiers)
                    {
                        foreach (SalesModifier salemod in CurrentLine.Modifiers)
                        {
                            if (mod.Description == salemod.Description)
                            {
                                if (modgroup == CurrentModGroup)  //for max count
                                {
                                    _currentmodgroupcount++;  //only increment for the current mod group .. this will enabled/disable button
                                }
                                tempcount++;
                            }
                        }

                    }

                    if (tempcount < modgroup.Min) _minimum_met = false;  // if any one of the modgroup minimum is not met


                }else //min = 0 , optional items
                {
                    foreach (Modifier mod in modgroup.Modifiers)
                    {
                        foreach (SalesModifier salemod in CurrentLine.Modifiers)
                        {
                            if (mod.ID == salemod.ModifierID)
                            {
                                if (modgroup == CurrentModGroup)  //for max count
                                {
                                    _currentmodgroupcount++;  //only increment for the current mod group .. this will enabled/disable button
                                }
                              
                            }
                        }

                    }
                }
           

            }


        }

        private int m_selectedindex;
        public int SelectedIndex {

            get { return m_selectedindex; }

            set
            {
                m_selectedindex = value;
                _currentindex = value;
                LoadMods();
                NotifyPropertyChanged("SelectedIndex");
            }
        }

        public Ticket CurrentTicket
        {
            get
            {
                return m_currentticket;
            }

            set
            {
                m_currentticket = value;
                NotifyPropertyChanged("CurrentTicket");
            }
        }

        public LineItem CurrentLine
        {
            get { return _line; }
            set { _line = value; NotifyPropertyChanged("CurrentLine"); }
        }

        public ModGroup CurrentModGroup
        {
            get { return _currentmodgroup; }
            set
            {
                _currentmodgroup = value;
                NotifyPropertyChanged("CurrentModGroup");
            }
        }

        public bool CanExecutePreviousClicked
        {
            get
            {
                if (_currentindex > 0) return true;
                else return false;


            }
        }

        public bool CanExecuteNextClicked
        {
            get
            {
                if (_currentindex < ModGroups.Count - 2 && _currentmodgroupcount >= CurrentModGroup.Min) return true;
                else return false;


            }
        }

        public bool CanExecuteDoneClicked
        {
            get {
                return _minimum_met;
            
            
            }
        }

        public bool CanExecuteModifierAddClicked
        {
            get
            {
                if (_currentmodgroupcount < CurrentModGroup.Max) return true;
                else return false;


            }
        }



        public void ExecuteModifierAddClicked(object modobj)
        {
            int modid=0;
            if (modobj != null) modid = (int)modobj;

            Modifier newmodifier = new Modifier(modid);
            bool found = false;
            int salesmodifierid = 0;
         
            foreach (SalesModifier salemod in CurrentLine.Modifiers)
            {
                if (salemod.ModifierID == newmodifier.ID)
                {
                    //first check to see if allow multiple
                    if (!CurrentModGroup.AllowDuplicate)
                    {
                        TouchMessageBox.Show("No Duplicate Allowed!!");
                        return;
                    }

                    found = true;
                    salesmodifierid = salemod.ID;

                }
            }




            if(found)
            {
                CurrentTicket.AddModifierQuantity(salesmodifierid);
            }
            else
            {
                CurrentTicket.AddModifier(_lineid, modid, CurrentModGroup.SortOrder);
            }
         
            CurrentLine = CurrentTicket.GetLineItemLine(_lineid);

            GlobalSettings.CustomerDisplay.WriteDisplay(CurrentLine.Description, CurrentLine.AdjustedPriceWithModifier, "Total:", CurrentTicket.Total);

            //need to LoadMods() first to get correct count before deciding what to do
            LoadMods();


            //check to see if go next or done or stay
            if(!CanExecuteModifierAddClicked)  //can not add so need to go next or done
            {
                if (CanExecuteNextClicked)
                {
                    ExecuteNextClicked(null);
                }

                else if(GlobalSettings.Instance.AutoCloseModifierWhenFull && _minimum_met) _parent.Close();

            }

           
  

          
        }


        public void ExecuteModifierDeleteClicked(object modobj)
        {
            SalesModifier salesmod = (SalesModifier)modobj;


            if(salesmod.Quantity > 1)
                CurrentTicket.DeleteModifierQuantity(salesmod.ID);
            else
                CurrentTicket.DeleteModifier(salesmod.ID);


            CurrentLine = CurrentTicket.GetLineItemLine(_lineid);
            LoadMods();



        }

        public void ExecutePreviousClicked(object obj)
        {
            _currentindex--;
            SelectedIndex = _currentindex;
            LoadMods();

        }
        public void ExecuteNextClicked(object obj)
        {
            _currentindex++;
            SelectedIndex = _currentindex;
            LoadMods();

        }
        public void ExecuteDoneClicked(object obj)
        {
                _parent.Close();

        }

        public void ExecuteCancelClicked(object obj)
        {

            //delete line item and all modifier using _lineid
            if(m_newticket)
            {
                CurrentTicket.DeleteLineItem(_lineid);
                product = null;
            }
     
            _parent.Close();

        }
    }
}
