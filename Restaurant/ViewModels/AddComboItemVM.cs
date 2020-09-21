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
    public class AddComboItemVM : INPCBase
    {
        private Ticket m_currentticket;
        private ComboSet _currentcomboset;

        public ObservableCollection<ComboSet> ComboSets { get; set; }

        public ICommand ComboAddClicked { get; set; }
        public ICommand ComboDeleteClicked { get; set; }
        public ICommand DoneClicked { get; set; }
        public ICommand NextClicked { get; set; }
        public ICommand PreviousClicked { get; set; }
        public ICommand CancelClicked { get; set; }

        private int _currentcombosetcount = 0;
      
        private int _lineid;
        private Window m_parent;
        private MenuSetupModel m_inventorymodel;
        private int _currentindex = 0;
        private bool _minimum_met = false;
        private bool m_newticket = false;

        public AddComboItemVM(Window parent, Ticket ticket, int lineid, Product prod, bool newticket)
        {
            CurrentTicket = ticket;
            m_parent = parent;
            m_inventorymodel = new MenuSetupModel();
            m_newticket = newticket;

            _lineid = lineid;
            CurrentLine = CurrentTicket.GetLineItemLine(_lineid);

            ComboAddClicked = new RelayCommand(ExecuteComboAddClicked, param => CanExecuteComboAddClicked);
            ComboDeleteClicked = new RelayCommand(ExecuteComboDeleteClicked, param => true);
            DoneClicked = new RelayCommand(ExecuteDoneClicked, param => CanExecuteDoneClicked);
            NextClicked = new RelayCommand(ExecuteNextClicked, param => CanExecuteNextClicked);
            PreviousClicked = new RelayCommand(ExecutePreviousClicked, param => CanExecutePreviousClicked);

            ComboSets = m_inventorymodel.GetComboSets(prod.ComboGroupId);

            CancelClicked = new RelayCommand(ExecuteCancelClicked, param => m_newticket);


            SelectedIndex = 0; //this will also LoadMods()
        }

        private void LoadComboSets()
        {
            if (ComboSets == null) return;
            if (ComboSets.Count == 0) return;

            CurrentComboSet = ComboSets[_currentindex];


            _minimum_met = true;

            int tempcount = 0;
            _currentcombosetcount = 0;
           // int totalmin = ComboSets.Sum(x => x.Min);

            //get the count for that found
            foreach (ComboSet comboset in ComboSets)
            {
                //only count modgroup that are required .. ie .. min > 0
                if (comboset.Min > 0)
                {
                    tempcount = 0;
                    foreach (Product prod in comboset.Products)
                    {
                        if(CurrentLine.LineItems != null)
                        foreach (LineItem line in CurrentLine.LineItems)
                        {
                            if (prod.ID == line.ProductID)
                            {
                                    if (comboset == CurrentComboSet) //for max count
                                    {
                                        _currentcombosetcount++;  //only increment for the current mod group .. this will enabled/disable button
                                    }
                                tempcount++;
                            }
                        }

                    }

                    if (tempcount < comboset.Min) _minimum_met = false;  // if any one of the modgroup minimum is not met

                }else //optional items since min =0
                {
                    foreach (Product prod in comboset.Products)
                    {
                        if (CurrentLine.LineItems != null)
                            foreach (LineItem line in CurrentLine.LineItems)
                            {
                                if (prod.ID == line.ProductID)
                                {
                                    if (comboset == CurrentComboSet) //for max count
                                    {
                                        _currentcombosetcount++;  //only increment for the current mod group .. this will enabled/disable button
                                    }
                                 
                                }
                            }

                    }
                }


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


        private int m_selectedindex;
        public int SelectedIndex
        {

            get { return m_selectedindex; }

            set
            {
                m_selectedindex = value;
                _currentindex = value;
                LoadComboSets();
                NotifyPropertyChanged("SelectedIndex");
            }
        }


        private LineItem _line;
        public LineItem CurrentLine
        {
            get { return _line; }
            set { _line = value; NotifyPropertyChanged("CurrentLine"); }
        }

        public ComboSet CurrentComboSet
        {
            get { return _currentcomboset; }
            set
            {
                _currentcomboset = value;
                NotifyPropertyChanged("CurrentComboSet");
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
                if (_currentindex < ComboSets.Count - 1 && _currentcombosetcount >= CurrentComboSet.Min) return true;
                else return false;


            }
        }

        public bool CanExecuteDoneClicked
        {
            get
            {
                return _minimum_met;


            }
        }

        public bool CanExecuteComboAddClicked
        {
            get
            {
                if (_currentcombosetcount < CurrentComboSet.Max) return true;
                else return false;


            }
        }

        public Product GetProductFromList(int id)
        {
            
                foreach (Product line in CurrentComboSet.Products)
                {
                    if (line.ID == id) return line;
                }
            return null;
        }

        public void ExecuteComboAddClicked(object comboobj)
        {
            int itemid = 0;
            if (comboobj != null) itemid = (int)comboobj;

            Product selected = GetProductFromList(itemid);
            decimal price = selected.AdjustedPrice;

            //first check to see if allow multiple
            if(!CurrentComboSet.AllowDuplicate)
            {
                //does not allow so need to check for existing first
                if (CurrentLine.LineItems != null)
                    foreach (LineItem line in CurrentLine.LineItems)
                    {
                        if (selected.ID == line.ProductID)
                        {
                            TouchMessageBox.Show("No Duplicate Allowed!!");
                            return;
                        }
                    }
            }

            int comboitemid = CurrentTicket.AddComboLineItem(selected, _lineid,CurrentComboSet.MaxPrice, price,1,1,"",CurrentComboSet.SortOrder);

            if (selected.ModProfileID > 0)
            {
                //AddComboView dlg;
                SalesModifierView dlg;

                dlg = new SalesModifierView(m_parent, CurrentTicket, comboitemid, selected,true);
                Utility.OpenModal(m_parent, dlg);
               

            }



            CurrentLine = CurrentTicket.GetLineItemLine(_lineid);

            GlobalSettings.CustomerDisplay.WriteDisplay(CurrentLine.Description, CurrentLine.AdjustedPriceWithModifier, "Total:", CurrentTicket.Total);

            //need to LoadMods() first to get correct count before deciding what to do
            LoadComboSets();


            //check to see if go next or done or stay
            if (!CanExecuteComboAddClicked)  //can not add so need to go next or done
            {
                if (CanExecuteNextClicked)
                {
                    ExecuteNextClicked(null);
                }

                else if (GlobalSettings.Instance.AutoCloseComboWhenFull && _minimum_met) m_parent.Close();

            }





        }


        public void ExecuteComboDeleteClicked(object modobj)
        {
            LineItem salesmod = (LineItem)modobj;

            CurrentTicket.DeleteLineItem(salesmod.ID);
            CurrentLine = CurrentTicket.GetLineItemLine(_lineid);
            LoadComboSets();



        }

        public void ExecutePreviousClicked(object obj)
        {
            _currentindex--;
            SelectedIndex = _currentindex;
            LoadComboSets();

        }
        public void ExecuteNextClicked(object obj)
        {
            _currentindex++;
            SelectedIndex = _currentindex;
            LoadComboSets();

        }
        public void ExecuteDoneClicked(object obj)
        {
            m_parent.Close();

        }

        public void ExecuteCancelClicked(object obj)
        {

            //delete line item and all modifier using _lineid
            if (m_newticket)
            {
                CurrentTicket.DeleteLineItem(_lineid);
                
            }

            m_parent.Close();

        }
    }
}
