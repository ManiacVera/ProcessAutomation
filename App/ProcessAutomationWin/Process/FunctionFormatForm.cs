using ProcessAutomation.BL;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProcessAutomationWin.App.Windows.Process
{
    public partial class FunctionFormatForm : BaseForm
    {
        //private List<FunctionParameterTypeInfo> parameterTypeInfoList;

        public FunctionFormatForm()
        {
            InitializeComponent();
        }        

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            List<string> parameterList = null;

            if (dgvParameterList.Rows != null && dgvParameterList.Rows.Count > 0)
            {
                parameterList = new List<string>();
                foreach (DataGridViewRow row in dgvParameterList.Rows)
                {
                    if (row.Cells["colType"] != null && row.Cells["colType"].Value != null)
                    {
                        parameterList.Add(row.Cells["colType"].Value.ToString());
                    }
                }
            }

            txtResult.Text = StepBC.GenerateFuntionFormat(txtMethod.Text, parameterList);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtResult.Text);
        }

        private void StepFunctionForm_Load(object sender, EventArgs e)
        {
            //parameterTypeInfoList = new List<FunctionParameterTypeInfo>();
            //dgvParameterList.DataSource = parameterTypeInfoList;
            LoadMethodType();
        }

        private void LoadMethodType()
        {
            List<ComboBoxInfo> typeValueOptionList = StepBC.RetrieveMethodTypeList();

            colType.DisplayMember = "Text";
            colType.ValueMember = "Value";

            colType.DataSource = null;
            colType.DataSource = typeValueOptionList;
        }

        private void dgvParameterList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvParameterList_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboboxCellEnter(sender as DataGridView, e);
        }
    }
}
