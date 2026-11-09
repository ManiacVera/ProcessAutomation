using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessAutomation.Process
{
    public partial class RunEventForm : Form
    {
        private List<RunProcessInfo> runProcessList = new List<RunProcessInfo>();
        ProcessBC processBC = new ProcessBC();

        private ProcessInfo processInfo = null;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public RunEventForm(ProcessInfo processInfo)
        {
            InitializeComponent();
            this.processInfo = processInfo.JsonClone();
            
            //runProcessList = processBC.InitProcess(this.processInfo);
            txtProcessName.Text = this.processInfo.ProcessName;
            progressProcess.DisplayStyle = App.Windows.Controls.ProgressBarDisplayText.CustomText;

            cmbEventList.DataSource = null;

            cmbTaskList.DataSource = null;
            List<ComboBoxInfo> taskCombo = ProcessBC.RetrieveTaskListFromProcessInfo(this.processInfo);
            if (taskCombo != null)
            {
                taskCombo.Insert(0, new ComboBoxInfo(string.Empty, string.Empty));
                cmbTaskList.DataSource = taskCombo;
            }

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (processBC != null)
            {
                const int sleepTime = 100;
                Thread.Sleep(sleepTime);
                while (processBC.IsRunning())
                {
                    if (backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    backgroundWorker.ReportProgress(processBC.Progress, processBC.MaximumProgress);                    
                    Thread.Sleep(sleepTime);
                }

                backgroundWorker.ReportProgress(processBC.Progress, processBC.MaximumProgress);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (processBC != null && processBC.Progress > 0)
            {
                progressProcess.Maximum = processBC.MaximumProgress;
                progressProcess.Value = processBC.Progress;

                if (processBC.MainProcess != null)
                {   
                    progressProcess.CustomText = string.Format("{0}: {1}" , processBC.MainProcess.StepNumber, processBC.MainProcess.StepDescription);
                }

            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (!processBC.IsRunning())
            {
                if (MessageBox.Show("¿Está seguro de ejecutar el evento?", "Proceso", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    object[] parameters = null;
                    if (!string.IsNullOrWhiteSpace(txtParameters.Text))
                    {
                        string parametersFormat = string.Format("[{0}]", txtParameters.Text.Trim());
                        parameters = JsonConvert.DeserializeObject<object[]>(parametersFormat);
                    }

                    ProcessInfo processInfoFiltered = ProcessBC.FilterFunctionProcess(processInfo, cmbTaskList.Text, cmbEventList.Text, parameters);
                    runProcessList = processBC.InitProcess(processInfoFiltered);

                    Task.Run(() =>
                    {
                        if (ValidateData())
                        {
                            processBC.RunProcess(runProcessList, true);
                        }
                    });

                    progressProcess.Value = 0;
                    progressProcess.CustomText = null;
                    backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("El evento esta en ejecución", "Proceso", MessageBoxButtons.OK);
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            if (processBC.IsRunning())
            {
                if (MessageBox.Show("¿Está seguro que quiere abortar la ejecución?", "Proceso", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    var asyncTask = Task.Run(() =>
                    {
                        processBC.ProcessAbort();
                    });
                }
                backgroundWorker.CancelAsync();
            }
            else
            {
                MessageBox.Show("El proceso no esta en ejecución", "Proceso", MessageBoxButtons.OK);
            }
        }

        private void btnWebApi_Click(object sender, EventArgs e)
        {
            //ComboBox cmbTask = (ComboBox)cmbTaskList;
            //if (cmbTask.SelectedIndex >= 0)
            //{
            //    int taskIndex = string.IsNullOrEmpty(cmbTask.SelectedValue.ToString()) ? -1 : Convert.ToInt32(cmbTask.SelectedValue);
            //    TaskInfo task = processInfo.TaskList.FirstOrDefault(f => f.Number == taskIndex);
            //    if (task != null)
            //    {
            //        WebApiBuilder.BuildWebApi(@"C:\Proyectos\TestProcess\Publishing\INGLUISVERA561\Publish", new List<TaskInfo> { task });
            //    }
            //}
        }

        private bool ValidateData()
        {
            bool isValid = false;

            isValid = true;
            //isValid = StepList.Where(f => string.IsNullOrEmpty(f.Value)).Count() > 0;

            return isValid;
        }

        private void dgvStepList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView gridView = (DataGridView)sender;

            if (gridView.Rows != null && gridView.Rows.Count > 0)
            {
                RunProcessInfo runProcess = null;
                gridView.CurrentCell = null;
                foreach (DataGridViewRow row in gridView.Rows)
                {
                    runProcess = (RunProcessInfo)row.DataBoundItem;
                    if (runProcess.ValueTypeDeclaration != "EDITABLE")
                    {
                        row.Visible = false;
                    }
                }
            }
        }

        private void dgvStepGetList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    //RunProcessInfo processInfo = (RunProcessInfo)dgvStepGetList.Rows[e.RowIndex].DataBoundItem;
                    //if (e.ColumnIndex == dgvStepGetList.Columns["colGetStepValue"].Index)
                    //{
                    //    Clipboard.SetText(string.Format(FunctionConstants.GET_STEP_VALUE_REGULAR_EXPRESSION_FORMAT
                    //        , processInfo.TaskLegacyNumber, processInfo.StepLegacyNumber));
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbTaskList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmbTask = (ComboBox)sender;
            if (cmbTask.SelectedIndex >= 0)
            {
                cmbEventList.SelectedIndex = -1;
                cmbEventList.DataSource = null;

                int taskIndex = string.IsNullOrEmpty(cmbTask.SelectedValue.ToString()) ? -1 : Convert.ToInt32(cmbTask.SelectedValue);
                TaskInfo task = processInfo.TaskList.FirstOrDefault(f => f.Number == taskIndex);
                if (task != null)
                {
                    List<ComboBoxInfo> eventCombo = ProcessBC.RetrieveFunctionListFromTaskInfo(task);
                    if (eventCombo != null)
                    {
                        eventCombo.Insert(0, new ComboBoxInfo(string.Empty, string.Empty));
                        cmbEventList.DataSource = eventCombo;
                    }
                }
            }
        }
    }
}
