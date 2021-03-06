﻿namespace Figlut.Aplex.Desktop.AuxilaryUI
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Figlut.Desktop.BaseForms;
    using Figlut.Aplex.Desktop.Data.WebService;
    using Figlut.Desktop.Utilities;
    using Figlut.Aplex.Desktop.Data;

    #endregion //Using Directives

    public partial class ViewServerActionsForm : BorderlessForm
    {
        #region Constructors

        public ViewServerActionsForm(DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            _serverActionCache = new ServerActionCache();
            InitHiddenColumns();
            _startDate = startDate;
            _endDate = endDate;
        }

        #endregion //Constructors

        #region Fields

        private ServerActionCache _serverActionCache;
        private bool _unsavedChanges;
        private List<string> _hiddenColumns;
        private DateTime _startDate;
        private DateTime _endDate;

        #endregion //Fields

        #region Methods

        private void InitHiddenColumns()
        {
            _hiddenColumns = new List<string>();
            _hiddenColumns.Add(EntityReader<ServerAction>.GetPropertyName(p => p.ServerActionCacheId, true));
        }

        private void Refresh(bool fromServer)
        {
            string originalStatus = Status;
            try
            {
                if ((_unsavedChanges) && (fromServer) &&
                    (UIHelper.AskQuestion("There are unsaved changes. Are you sure you want to refresh from server. All unsaved changes will be lost.") !=
                    DialogResult.Yes))
                {
                    return;
                }
                using (WaitCursor w = new WaitCursor())
                {
                    if (fromServer)
                    {
                        Status = "Refreshing from server ...";
                        if (_serverActionCache.RefreshFromServerByFilters(
                            _startDate,
                            _endDate))
                        {
                            return;
                        }
                        _unsavedChanges = false;
                    }
                    int index = UIHelper.GetSelectedGridRowIndex(grdServerActions);
                    Dictionary<string, object> properties = new Dictionary<string, object>();
                    properties.Add(EntityReader<ServerAction>.GetPropertyName(p => p.FieldChanged, false), txtFieldChanged.Text);
                    grdServerActions.DataSource = _serverActionCache.GetDataTable(properties, false, true);
                    _hiddenColumns.ForEach(p => grdServerActions.Columns[p].Visible = false);
                    UIHelper.SelectGridRow(grdServerActions, index);
                }
            }
            finally
            {
                if (Status != originalStatus)
                {
                    Status = originalStatus;
                }
            }
        }

        #endregion //Methods

        #region Event Handlers

        private void ViewServerActionsForm_Load(object sender, EventArgs e)
        {
            Refresh(true);
        }

        private void txtDeviceId_TextChanged(object sender, EventArgs e)
        {
            Refresh(false);
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ViewServerActionsForm_MouseDown(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseDown(sender, e);
        }

        private void ViewServerActionsForm_MouseMove(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseMove(sender, e);
        }

        private void ViewServerActionsForm_MouseUp(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseUp(sender, e);
        }

        private void picResizeWindow_MouseMove(object sender, MouseEventArgs e)
        {
            base.BorderLessFormResize(sender, e);
        }

        private void ViewServerActionsForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                mnuCancel_Click(sender, e);
            }
        }

        private void picMaximize_Click(object sender, EventArgs e)
        {
            base.BorderlessForm_Maximize(sender, e);
        }

        #endregion //Event Handlers
    }
}