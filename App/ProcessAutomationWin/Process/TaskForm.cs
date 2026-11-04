using ProcessAutomation.App.Windows.Process;
using ProcessAutomation.BL;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using ProcessAutomationWin.App.Windows;
using ProcessAutomationWin.App.Windows.Process;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ProcessAutomation.Process
{
    public partial class TaskForm : BaseForm
    {
        private StepBC stepBC = null;

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public TaskInfo taskInfo { get; set; }
        private StepInfo stepInfo = null;
        private int stepSelected = -1;

        public TaskForm(int taskNumber)
        {
            InitializeComponent();

            this.taskInfo = GlobalData.processBC.AddTask();
            taskInfo.Number = taskNumber;

            taskInfo.StepList = new List<StepInfo>();
            taskInfo.StepList.Add(new StepInfo());

            stepBC = new StepBC(taskInfo);

            txtNumber.Text = taskNumber.ToString();
            chkEnabled.Checked = true;
            dgvStepList.AutoGenerateColumns = false;
            dgvStepList.DataSource = taskInfo.StepList;
            dgvStepList.DataSource = null;

            taskInfo.StepList.Clear();
            dgvStepList.DataSource = taskInfo.StepList;
            LoadDeclarationType();
            LoadEncapsulatedPartitionType();
            //LoadValueAccess();
            LoadValueType();
            LoadComponent();

            dgvComponentList.AutoGenerateColumns = false;
            dgvComponentList.DataSource = taskInfo.ComponentList;
            dgvComponentList.DataSource = null;
            LoadComponentType();
        }

        public TaskForm(TaskInfo taskInfo)
        {
            InitializeComponent();

            this.taskInfo = taskInfo;
            stepBC = new StepBC(taskInfo);

            if (taskInfo.StepList == null)
            {
                taskInfo.StepList = new List<StepInfo>();
            }

            txtName.Text = taskInfo.Name;
            txtNumber.Text = taskInfo.Number.ToString();
            txtDelay.Text = taskInfo.Delay.ToString();
            chkEnabled.Checked = taskInfo.Enabled;
            dgvStepList.AutoGenerateColumns = false;
            dgvStepList.DataSource = taskInfo.StepList;

            LoadDeclarationType();
            LoadEncapsulatedPartitionType();
            //LoadValueAccess();
            LoadValueType();
            LoadComponent();

            dgvComponentList.AutoGenerateColumns = false;
            dgvComponentList.DataSource = taskInfo.ComponentList;
            LoadComponentType();           
        }

        private void LoadDeclarationType()
        {
            List<ComboBoxInfo> typeValueOptionList = StepBC.RetrieveDeclarationTypeList();

            colValueTypeDeclaration.DisplayMember = "Text";
            colValueTypeDeclaration.ValueMember = "Value";

            colValueTypeDeclaration.DataSource = null;
            colValueTypeDeclaration.DataSource = typeValueOptionList;
        }

        private void LoadEncapsulatedPartitionType()
        {
            List<ComboBoxInfo> typeValueOptionList = StepBC.RetrieveEncapsulatedPartitionType();

            colEncapsulatedType.DisplayMember = "Text";
            colEncapsulatedType.ValueMember = "Value";

            colEncapsulatedType.DataSource = null;
            colEncapsulatedType.DataSource = typeValueOptionList;
        }

        //private void LoadValueAccess()
        //{
        //    List<ComboBoxInfo> typeValueOptionList = StepBC.RetrieveValueAccessList();

        //    colAction.DisplayMember = "Text";
        //    colAction.ValueMember = "Value";

        //    colAction.DataSource = null;
        //    colAction.DataSource = typeValueOptionList;
        //}

        private void LoadValueType()
        {
            {
                List<ComboBoxInfo> typeValueOptionList = StepBC.RetrieveValueTypeList();
                colValueType.DisplayMember = "Text";
                colValueType.ValueMember = "Value";
                colValueType.DataSource = null;
                colValueType.DataSource = typeValueOptionList;
            }

            {
                List<ComboBoxInfo> typeValueOptionList = StepBC.RetrieveValueTypeExtendedList();

                colOutputValueType.DisplayMember = "Text";
                colOutputValueType.ValueMember = "Value";
                colOutputValueType.DataSource = null;
                colOutputValueType.DataSource = typeValueOptionList;
            }

            {
                List<ComboBoxInfo> typeValueOptionList = StepBC.RetrieveValueTypeList();

                colValueTypeComponent.DisplayMember = "Text";
                colValueTypeComponent.ValueMember = "Value";
                colValueTypeComponent.DataSource = null;
                colValueTypeComponent.DataSource = typeValueOptionList;
            }
        }

        private void LoadComponentType()
        {
            List<ComboBoxInfo> componentTypeList = StepBC.RetrieveComponentTypeList(AppDomain.CurrentDomain.BaseDirectory);

            colComponentType.DisplayMember = "Text";
            colComponentType.ValueMember = "Value";
            colComponentType.DataSource = null;
            colComponentType.DataSource = componentTypeList.OrderBy(o => o.Text).ToList();
        }

        private void LoadComponent()
        {
            if (taskInfo.ComponentList != null)
            {
                List<ComboBoxInfo> componentList = taskInfo.ComponentList.Select(s => new ComboBoxInfo(s.Name, s.Name)).ToList();

                componentList.Insert(0, new ComboBoxInfo("", ""));
                if (dgvStepList.DataSource != null)
                {
                    List<StepInfo> stepList = (List<StepInfo>)dgvStepList.DataSource;
                    foreach (StepInfo step in stepList.Where(f => !componentList.Exists(e => e.Value == f.Component)))
                    {
                        step.Component = null;
                    }
                }
                
                colComponent.DisplayMember = "Text";
                colComponent.ValueMember = "Value";
                colComponent.DataSource = componentList;
            }
            else
            {
                colComponent.DataSource = null;
            }
        }

        private void dgvStepList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    stepInfo = (StepInfo)dgvStepList.Rows[e.RowIndex].DataBoundItem;

                    if (e.ColumnIndex == dgvStepList.Columns["colDelete"].Index)
                    {
                        taskInfo.StepList.Remove(stepInfo);
                        dgvStepList.DataSource = null;
                        dgvStepList.DataSource = taskInfo.StepList;
                    }
                    else if (e.ColumnIndex == dgvStepList.Columns["colOutputValueOption"].Index)
                    {
                        btnOutputValue_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvStepList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    stepSelected = e.RowIndex;
                    if (e.ColumnIndex != dgvStepList.Columns["colDelete"].Index)
                    {
                        //stepInfo = (StepInfo)dgvStepList.Rows[e.RowIndex].DataBoundItem;
                        //if (e.ColumnIndex == dgvStepList.Columns["colCopyId"].Index)
                        //{
                        //    Clipboard.SetText(string.Format(FunctionConstants.GET_STEP_VALUE_REGULAR_EXPRESSION_FORMAT
                        //   , taskInfo.Number, stepInfo.Number));
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddNewValueOption_Click(object sender, EventArgs e)
        {
            if (stepInfo != null)
            {
                if (stepInfo.ValueOptionList == null)
                {
                    stepInfo.ValueOptionList = new List<ValueOptionInfo>();
                }
                ValueOptionInfo newValueOptionInfo = new ValueOptionInfo();

                if (stepInfo.ValueOptionList.Count > 0)
                {
                    newValueOptionInfo.Number = stepInfo.ValueOptionList.Max(f => f.Number) + 1;
                }
                else
                {
                    newValueOptionInfo.Number = 1;
                }

                stepInfo.ValueOptionList.Add(newValueOptionInfo);
            }
        }

        private void btnNewStep_Click(object sender, EventArgs e)
        {
            AddStep();
        }

        private void AddStep(int index = -1)
        {
            if (taskInfo != null)
            {
                if (taskInfo.StepList == null)
                {
                    taskInfo.StepList = new List<StepInfo>();
                }
                StepInfo newStepInfo = new StepInfo();

                if (taskInfo.StepList.Count > 0)
                {
                    if (index == -1)
                    {
                        newStepInfo.Number = taskInfo.StepList.Max(f => f.Number) + 1;
                    }
                    else
                    {
                        newStepInfo.Number = index + 1;
                        List<StepInfo> stepList = taskInfo.StepList.Where(f => f.Number >= newStepInfo.Number).ToList();
                        int countMove = newStepInfo.Number;

                        foreach (StepInfo step in stepList)
                        {
                            if (step != stepInfo)
                            {
                                countMove++;
                                step.Number = countMove;
                            }
                        }  
                    }
                }
                else
                {
                    newStepInfo.Number = 1;
                }

                if (index == -1)
                {
                    taskInfo.StepList.Add(newStepInfo); 
                }
                else
                {
                    taskInfo.StepList.Insert(index, newStepInfo);
                }

                dgvStepList.DataSource = null;
                dgvStepList.DataSource = taskInfo.StepList;
                //dgvStepList.ClearSelection();

                //stepSelected = stepInfo.Number - 1;
                //dgvStepList.Rows[stepSelected].Selected = true;
                //dgvStepList.FirstDisplayedScrollingRowIndex = stepSelected;
            }
        }

        private void MoveStep(bool up)
        {
            try
            {
                StepInfo stepToMove = taskInfo.StepList.FirstOrDefault(f => f.Number == stepSelected + 1);
                if (stepInfo != null)
                {
                    List<StepInfo> stepList = null;
                    int countMove = 0;
                    if (up == true && stepSelected > 0)
                    {   
                        stepInfo.Number--;
                        stepList = taskInfo.StepList.Where(f => f.Number >= stepToMove.Number).OrderBy(o => o.Number).ToList();
                        countMove = stepInfo.Number;

                        foreach (StepInfo step in stepList)
                        {
                            if (step != stepInfo)
                            {
                                countMove++;
                                step.Number = countMove;                                
                            }
                        }                       
                    }
                    else if (up == false && stepSelected < dgvStepList.Rows.Count - 1)
                    {
                        stepInfo.Number++;
                        stepList = taskInfo.StepList.Where(f => f.Number <= stepToMove.Number).OrderByDescending(o => o.Number).ToList();
                        countMove = stepInfo.Number;

                        foreach (StepInfo step in stepList)
                        {
                            if (step != stepInfo)
                            {
                                countMove--;
                                step.Number = countMove;                                
                            }
                        }    
                    }

                    dgvStepList.DataSource = null;
                    dgvStepList.DataSource = taskInfo.StepList.OrderBy(o => o.Number).ToList();
                    dgvStepList.ClearSelection();

                    dgvStepList.Rows[stepSelected].Selected = false;
                    stepSelected = stepInfo.Number - 1;
                    dgvStepList.Rows[stepSelected].Selected = true;

                    dgvStepList.FirstDisplayedScrollingRowIndex = stepSelected - 4;
                }
            }
            catch (Exception)
            { 
            }
        }

        private void txtNumber_TextChanged(object sender, EventArgs e)
        {
            if (txtNumber.Text.Trim() != string.Empty)
            {
                try
                {
                    taskInfo.Number = int.Parse(txtNumber.Text);                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    txtNumber.Text = string.Empty;
                }
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            taskInfo.Name = txtName.Text.Trim();
        }

        //private void cmbExplorer_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    taskInfo.Explorer = ((ComboBox)sender).SelectedValue.ToString();
        //}

        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            taskInfo.Enabled = chkEnabled.Checked;
        }

        private void txtPageUrl_TextChanged(object sender, EventArgs e)
        {
            //taskInfo.PageUrl = txtPageUrl.Text.Trim();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (stepSelected != -1)
            {
                AddStep(stepSelected + 1);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            MoveStep(false);
        }

        private void btoMoveUp_Click(object sender, EventArgs e)
        {
            MoveStep(true);
        }

        private void dgvStepList_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboboxCellEnter(sender as DataGridView, e);
        }

        private void dgvValueOptionList_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboboxCellEnter(sender as DataGridView, e);
        }

        private void dgvStepList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e != null && e.Control != null)
            {
                DataGridView gdv = (sender as DataGridView);
                if (gdv.CurrentCell.ColumnIndex == dgvStepList.Columns["colValue"].Index)
                {
                    TextBox txt = (TextBox)e.Control;
                    txt.Multiline = true;
                    //txt.Height = 40;
                }
            }
        }        

        private void TaskForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataGridViewCommitChanges(dgvComponentList);
            DataGridViewCommitChanges(dgvStepList);
        }

        private void btnNewComponent_Click(object sender, EventArgs e)
        {
            if (taskInfo != null)
            {
                if (taskInfo.ComponentList == null)
                {
                    taskInfo.ComponentList = new List<ComponentInfo>();
                }
                ComponentInfo newComponentInfo = new ComponentInfo();

                taskInfo.ComponentList.Add(newComponentInfo);

                dgvComponentList.DataSource = null;
                dgvComponentList.DataSource = taskInfo.ComponentList;
            }
        }

        private void dgvComponentList_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboboxCellEnter(sender as DataGridView, e);
        }

        private void dgvComponentList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewDeleteRow<ComponentInfo>(sender as DataGridView, e, taskInfo.ComponentList);
        }

        private void tabGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            if (tabControl.TabPages[tabControl.SelectedIndex].Name == "tabStepList")
            {
                LoadComponent();
            }
        }

        private void btnConfigFormat_Click(object sender, EventArgs e)
        {
            if (dgvComponentList.CurrentRow.DataBoundItem != null)
            {
                ComponentInfo componentInfo = (ComponentInfo)dgvComponentList.CurrentRow.DataBoundItem;
                InputFormatResultInfo resultInfo = OpenInputDialog(StepBC.BuildInputFormat(componentInfo.Value, ValueFormatEnum.COMPONENT.ToString()
                    , componentInfo.Name, componentInfo.Description
                    , stepComponent: taskInfo.ComponentList.FirstOrDefault(s => s.Name == componentInfo.Description)), false);

                if (resultInfo.Ok)
                {
                    componentInfo.Value = resultInfo.Result;
                }
            }
        }

        private void btnValueFormat_Click(object sender, EventArgs e)
        {   
            if (dgvStepList.CurrentRow.DataBoundItem != null)
            {
                StepInfo stepInfo = (StepInfo)dgvStepList.CurrentRow.DataBoundItem;
                InputFormatResultInfo resultInfo = OpenInputDialog(StepBC.BuildInputFormat(stepInfo.Value, ValueFormatEnum.STEP.ToString(), 
                    stepInfo.Component, stepInfo.Description, GlobalData.processBC.RetrieveStepVariableList(stepInfo.Number, taskInfo.Number),
                    stepComponent: taskInfo.ComponentList.FirstOrDefault(s => s.Name == stepInfo.Component)), true);
                
                if (resultInfo.Ok)
                {
                    stepInfo.Value = resultInfo.Result;
                }
            }
        }

        private void btnResultValue_Click(object sender, EventArgs e)
        {
            if (dgvStepList.CurrentRow.DataBoundItem != null)
            {
                StepInfo stepInfo = (StepInfo)dgvStepList.CurrentRow.DataBoundItem;
                InputFormatResultInfo resultInfo = OpenInputDialog(StepBC.BuildInputFormat(string.Empty, "RESULT", stepInfo.Component, stepInfo.Description
                    , stepComponent: taskInfo.ComponentList.FirstOrDefault(s => s.Name == stepInfo.Component)), false);

                if (resultInfo.Ok)
                {
                    stepInfo.Value = resultInfo.Result;
                }
            }
        }

        private void btnOutputValue_Click(object sender, EventArgs e)
        {
            if (dgvStepList.CurrentRow.DataBoundItem != null)
            {
                StepInfo stepInfo = (StepInfo)dgvStepList.CurrentRow.DataBoundItem;
                string pageText = StepBC.RetrieveFormPageText(CommonConstants.OUTPUT_VALUE_OPTION_FILE);
                string value = null;

                Dictionary<string, object> outputDictionary = null;
                if (stepInfo.OutputResultOption != null)
                {
                    outputDictionary = stepInfo.OutputResultOption.DeserializerToDictionary();
                }

                List<OutputResultOptionInfo> valueOptionList = StepExpressBC.RetrieveOutputValueOptionList(outputDictionary);

                if (valueOptionList != null)
                {
                    value = valueOptionList.SerializerObject();
                    OutputValueForm outputValueForm = new OutputValueForm(stepInfo.Description, stepInfo.Component, pageText, "Token", value);

                    DialogResult dialogResult = outputValueForm.ShowDialog(this);

                    //InputFormatResultInfo resultInfo = new InputFormatResultInfo();
                    //if (outputValueForm.result != null)
                    if (dialogResult == DialogResult.Yes)
                    {
                        stepInfo.OutputResultOption = outputValueForm.result;
                        //resultInfo.Result = outputValueForm.result;
                    }
                    else
                    {
                        //resultInfo.Result = resultInfo.StepValue;
                    }
                }
            }
        }

        private InputFormatResultInfo OpenInputDialog(InputFormatResultInfo resultInfo, bool enabledOptions)
        {
            if (resultInfo.Ok)
            {
                InputFormatForm configForm = new InputFormatForm(resultInfo.Description, resultInfo.Component,
                    resultInfo.PageText, resultInfo.ValueAction, resultInfo.InputFormatText, resultInfo.VariableArray, 
                    enabledOptions);

                configForm.ShowDialog(this);
                resultInfo.Result = StepBC.ProcessResultFormat(configForm.result, resultInfo.StepValue, resultInfo.InputFormatText);
                // LUIS VERA FALTA REFRESCAR EL VALOR DE LA CELDA
                //dgvComponentList.Refresh();
            }
            else
            {
                if (!string.IsNullOrEmpty(resultInfo.ErrorMessage))
                {
                    MessageBox.Show(resultInfo.ErrorMessage);
                }
            }

            return resultInfo;
        }        
    }
}
