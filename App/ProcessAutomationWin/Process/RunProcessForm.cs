using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessAutomation.Process
{
    public partial class RunProcessForm : Form
    {
        private List<RunProcessInfo> runProcessList = new List<RunProcessInfo>();
        ProcessBC processBC = new ProcessBC();

        private ProcessInfo processInfo = null;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public RunProcessForm(ProcessInfo processInfo)
        {
            InitializeComponent();
            this.processInfo = processInfo.JsonClone();
            
            runProcessList = processBC.InitProcess(this.processInfo);
            txtProcessName.Text = this.processInfo.ProcessName;
            progressProcess.DisplayStyle = App.Windows.Controls.ProgressBarDisplayText.CustomText;

            dgvStepList.AutoGenerateColumns = false;
            dgvStepList.DataSource = null;
            dgvStepList.DataSource = runProcessList;

            dgvStepGetList.AutoGenerateColumns = false;
            dgvStepGetList.DataSource = null;
            //dgvStepGetList.DataSource = runProcessList.Where(f => f.ValueAction == ValueActionEnum.GET.ToString()).ToList();

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
                if (MessageBox.Show("¿Está seguro de ejecutar el proceso?", "Proceso", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    Task.Run(() =>
                    {
                        if (ValidateData())
                        {
                            processBC.RunProcess(runProcessList);
                        }
                    });

                    progressProcess.Value = 0;
                    progressProcess.CustomText = null;
                    backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("El proceso esta en ejecución", "Proceso", MessageBoxButtons.OK);
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
    }
}
