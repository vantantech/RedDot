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
    public class ReportSetupVM:INPCBase
    {
        public List<ListPair> CatTypes { get; set; }
        private QSReports m_reports = new QSReports();


        private ObservableCollection<ReportGroup> m_revenuegroups;
        public ObservableCollection<ReportGroup> RevenueGroups
        {
            get { return m_revenuegroups; }
            set
            {
                m_revenuegroups = value;
                NotifyPropertyChanged("RevenueGroups");
            }
        }


        private ObservableCollection<ReportGroup> m_settlementgroups;
        public ObservableCollection<ReportGroup> SettlementGroups
        {
            get { return m_settlementgroups; }
            set
            {
                m_settlementgroups = value;
                NotifyPropertyChanged("SettlementGroups");
            }
        }

        private ReportGroup m_currentgroup;
        public ReportGroup CurrentGroup
        {
            get { return m_currentgroup; }
            set
            {
                m_currentgroup = value;
                NotifyPropertyChanged("CurrentGroup");
            }
        }

        public ICommand RevenueGroupClicked { get; set; }
        public ICommand SettlementGroupClicked { get; set; }
        public ICommand AddNewRevenueGroupClicked { get; set; }
        public ICommand AddNewSettlementGroupClicked { get; set; }
        public ICommand AddNewCategoryClicked { get; set; }
        public ICommand DeleteCategoryClicked { get; set; }
        public ICommand DeleteGroupClicked { get; set; }
        public ICommand CatMoveUpClicked { get; set; }
        public ICommand CatMoveDownClicked { get; set; }

        private Window m_parent;
        private int m_lastgroupid;

        public ReportSetupVM(Window parent)
        {
            m_parent = parent;


            RevenueGroupClicked = new RelayCommand(ExecuteRevenueGroupClicked, param => true);
            SettlementGroupClicked = new RelayCommand(ExecuteSettlementGroupClicked, param => true);
            AddNewRevenueGroupClicked = new RelayCommand(ExecuteAddNewRevenueGroupClicked, param => true);
            AddNewSettlementGroupClicked = new RelayCommand(ExecuteAddNewSettlementGroupClicked, param => true);
            DeleteGroupClicked = new RelayCommand(ExecuteDeleteGroupClicked, param => this.CanDeleteGroup);
            DeleteCategoryClicked = new RelayCommand(ExecuteDeleteCategoryClicked, param => true);
            AddNewCategoryClicked = new RelayCommand(ExecuteAddNewCategoryClicked, param => this.CurrentGroupSelected);
            CatMoveUpClicked = new RelayCommand(ExecuteGroupMoveUpClicked, param => this.CurrentGroupSelected);
            CatMoveDownClicked = new RelayCommand(ExecuteGroupMoveDownClicked, param => this.CurrentGroupSelected);

            LoadGroups();
        }


        public bool CurrentGroupSelected
        {
            get
            {
                if (CurrentGroup != null) return true;
                else return false;
            }
        }


        //group must be selected and empty
        public bool CanDeleteGroup
        {
            get
            {
                if (CurrentGroupSelected)
                {
                    if (CurrentGroup.ReportCategory.Rows.Count == 0) return true;
                    else return false;
                }
                else return false;
            }
        }

        private void LoadGroups()
        {
            RevenueGroups = m_reports.GetReportGroupList("RevenueCategory");
            SettlementGroups = m_reports.GetReportGroupList("SettlementCategory");


            foreach (ReportGroup group in RevenueGroups)
            {
                if (group.ID == m_lastgroupid)
                {
                    CurrentGroup = group;
                    CurrentGroup.Selected = true;
                    return;
                }
                group.Selected = false;
            }


            foreach (ReportGroup group in SettlementGroups)
            {
                if (group.ID == m_lastgroupid)
                {
                    CurrentGroup = group;
                    CurrentGroup.Selected = true;
                    return;
                }
                group.Selected = false;
            }
        }

        public void ExecuteRevenueGroupClicked(object id)
        {
            int ID = int.Parse(id.ToString());

            foreach(ReportGroup rp in RevenueGroups)
            {
                if(rp.ID == ID)
                {
                    CurrentGroup = rp;
                    m_lastgroupid = rp.ID;
                    return;
                }
            }
        }

        public void ExecuteSettlementGroupClicked(object id)
        {
            int ID = int.Parse(id.ToString());

            foreach (ReportGroup rp in SettlementGroups)
            {
                if (rp.ID == ID)
                {
                    CurrentGroup = rp;
                    m_lastgroupid = rp.ID;
                    return;
                }
            }
        }

        public void ExecuteAddNewRevenueGroupClicked(object obj)
        {
            TextPad tp = new TextPad("Enter Report Group Name", "");
            Utility.OpenModal(m_parent, tp);

            if (tp.ReturnText == "") return;

            m_reports.AddNewGroupList("RevenueCategory", tp.ReturnText);
            LoadGroups();

        }

        public void ExecuteAddNewSettlementGroupClicked(object obj)
        {

            TextPad tp = new TextPad("Enter Report Group Name", "");
            Utility.OpenModal(m_parent, tp);

            if (tp.ReturnText == "") return;

            m_reports.AddNewGroupList("SettlementCategory", tp.ReturnText);
            LoadGroups();

        }


        public void ExecuteDeleteGroupClicked(object obj)
        {
            if (CurrentGroup == null) return;

            m_reports.DeleteGroupList(CurrentGroup.ID);
            LoadGroups();
            CurrentGroup = null;
        }

        public void ExecuteDeleteCategoryClicked(object obj)
        {
            if (obj.ToString() == "") return;

            int id = int.Parse(obj.ToString());

            m_reports.DeleteCatList(id);
            CurrentGroup.LoadCategories();
        }

        public void ExecuteAddNewCategoryClicked(object obj)
        {
            if (CurrentGroup == null) return;

            TextPad tp = new TextPad("Enter Report Category Description", "");
            Utility.OpenModal(m_parent, tp);

            if (tp.ReturnText == "") return;

            m_reports.AddNewCatList(CurrentGroup.ID, tp.ReturnText);

            CurrentGroup.LoadCategories();
        }


        public void ExecuteGroupMoveUpClicked(object obj)
        {
            CurrentGroupMoveUp();
            LoadGroups();
        }

        public void ExecuteGroupMoveDownClicked(object obj)
        {
            CurrentGroupMoveDown();
            LoadGroups();
        }


        private void CurrentGroupMoveUp()
        {
            int last = 0;
            int current = 0;
            ReportGroup LastCat = null;
            foreach (ReportGroup cat in SettlementGroups)
            {
                current = cat.SortOrder;
                if (cat == CurrentGroup)
                    if (LastCat != null)
                    {
                        LastCat.SortOrder = current;
                        CurrentGroup.SortOrder = last;
                        return;
                    }

                last = cat.SortOrder;
                LastCat = cat;
            }

            last = 0;
            current = 0;
            LastCat = null;
            foreach (ReportGroup cat in RevenueGroups)
            {
                current = cat.SortOrder;
                if (cat == CurrentGroup)
                    if (LastCat != null)
                    {
                        LastCat.SortOrder = current;
                        CurrentGroup.SortOrder = last;
                        return;
                    }

                last = cat.SortOrder;
                LastCat = cat;
            }

        }

        private void CurrentGroupMoveDown()
        {

            int current = 0;
            bool found = false;

            foreach (ReportGroup cat in SettlementGroups)
            {
                if (found)
                {
                    if (cat.SortOrder == 0) return; //last one is not a valid category and should have sort order of 0
                    current = cat.SortOrder; //save first
                    cat.SortOrder = CurrentGroup.SortOrder; //replace
                    CurrentGroup.SortOrder = current;
                    found = false;
                    return;
                }
                if (cat == CurrentGroup) found = true;
            }

            current = 0;
            found = false;

            foreach (ReportGroup cat in RevenueGroups)
            {
                if (found)
                {
                    if (cat.SortOrder == 0) return; //last one is not a valid category and should have sort order of 0
                    current = cat.SortOrder; //save first
                    cat.SortOrder = CurrentGroup.SortOrder; //replace
                    CurrentGroup.SortOrder = current;
                    found = false;
                    return;
                }
                if (cat == CurrentGroup) found = true;
            }




        }
    }


 
}
