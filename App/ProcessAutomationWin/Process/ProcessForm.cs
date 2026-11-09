using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using ProcessAutomationWin.App.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ProcessAutomation.Process
{
    public partial class ProcessForm : Form
    {
        private string fileName = null;

        public ProcessForm()
        {
            InitializeComponent();
            dgvTaskList.AutoGenerateColumns = false;

            GlobalData.processBC = new ProcessBC();
            GlobalData.processBC.ProcessInfo.TaskList = new List<TaskInfo>();
            GlobalData.processBC.ProcessInfo.LanguageSettingList = new List<LanguageSourceSettingInfo>();
        }

        private void btnNewTask_Click(object sender, EventArgs e)
        {
            try
            {
                TaskForm taskForm = new TaskForm(GlobalData.processBC.ProcessInfo.TaskList.Count + 1);
                taskForm.ShowDialog(this);

                if (taskForm.taskInfo != null)
                {
                    if (!GlobalData.processBC.RemoveIncompleteTask(taskForm.taskInfo.Number))
                    {
                        dgvTaskList.DataSource = null;
                        dgvTaskList.DataSource = GlobalData.processBC.ProcessInfo.TaskList;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvTaskList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                TaskInfo taskInfo = (TaskInfo)dgvTaskList.Rows[e.RowIndex].DataBoundItem;

                if (e.ColumnIndex == dgvTaskList.Columns["ColStepList"].Index)
                {
                    try
                    {
                        TaskForm taskForm = new TaskForm(taskInfo);
                        taskForm.ShowDialog(this);

                        if (taskForm.taskInfo != null)
                        {
                            if (taskForm.taskInfo.StepList != null && taskForm.taskInfo.StepList.Count > 0)
                            {
                                TaskInfo taskInfoFound = GlobalData.processBC.ProcessInfo.TaskList.FirstOrDefault(f => f.Name == taskForm.taskInfo.Name);
                                taskInfoFound = taskForm.taskInfo;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                else if (e.ColumnIndex == dgvTaskList.Columns["ColDelete"].Index)
                {
                    if (MessageBox.Show(string.Format("¿Está seguro de borrar la tarea {0}?", taskInfo.Number), "Proceso", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            GlobalData.processBC.ProcessInfo.TaskList.Remove(taskInfo);
                            dgvTaskList.DataSource = null;
                            dgvTaskList.DataSource = GlobalData.processBC.ProcessInfo.TaskList;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private List<ProcessInfo> LoadProcessFileFromJson(string fileName)
        {
            List<ProcessInfo> queryList = new List<ProcessInfo>();
            DirectoryInfo d = new DirectoryInfo(Utilities.CombineWithApplicationPath("ProcessFiles"));
            FileInfo[] Files = d.GetFiles("*.json");
            string json = null;

            try
            {
                foreach (FileInfo file in Files)
                {
                    if (fileName != null && Path.GetFileName(fileName) == file.Name)
                    {
                        using (StreamReader r = new StreamReader(file.FullName))
                        {
                            json = r.ReadToEnd();
                            queryList.Add(JsonConvert.DeserializeObject<ProcessInfo>(json));
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return queryList;
        }

        private void txtProcessNumber_TextChanged(object sender, EventArgs e)
        {
            if (txtProcessNumber.Text.Trim() != string.Empty)
            {
                try
                {
                    GlobalData.processBC.ProcessInfo.Number = int.Parse(txtProcessNumber.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    txtProcessNumber.Text = string.Empty;
                }
            }
        }

        private void txtProcessNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtProcessName_TextChanged(object sender, EventArgs e)
        {
            GlobalData.processBC.ProcessInfo.ProcessName = txtProcessName.Text;
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            GlobalData.processBC.ProcessInfo.Enabled = chkEnabled.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de guardar el proceso?", "Proceso", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                fileName = txtFileName.Text;

                try
                {
                    if ((fileName = ProcessBC.SaveProcessFile(GlobalData.processBC.ProcessInfo, fileName, AppDomain.CurrentDomain.BaseDirectory)) != null)
                    {
                        txtFileName.Text = fileName;
                        MessageBox.Show("Archivo guardado");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo guardar, favor de revisar los datos del proceso completo");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private bool ValidateData()
        {
            bool isValid = false;
            if (GlobalData.processBC.ProcessInfo != null)
            {
                if (string.IsNullOrEmpty(GlobalData.processBC.ProcessInfo.ProcessName))
                {
                    MessageBox.Show("Falta el nombre del proceso");
                    return isValid;
                }

                if (GlobalData.processBC.ProcessInfo.Number == 0)
                {
                    MessageBox.Show("Falta número del proceso");
                    return isValid;
                }

                try
                {
                    List<int> encapsulatedList = new List<int>();
                    int encapsulatedCount = 0;

                    List<TaskInfo> taskInfoList = GlobalData.processBC.ProcessInfo.TaskList.Where(f => !string.IsNullOrEmpty(f.Name)
                                                                    && f.Number > 0
                                                                    && f.StepList != null && f.StepList.Count() > 0).ToList();

                    if (taskInfoList == null || taskInfoList.Count() == 0)
                    {
                        MessageBox.Show("Falta agregar tareas");
                        return isValid;
                    }

                    int taskNumber = 1;
                    int stepNumber = 1;
                    foreach (TaskInfo taskInfo in taskInfoList.OrderBy(o => o.Number))
                    {
                        taskInfo.Number = taskNumber;
                        taskNumber++;

                        taskInfo.StepList = taskInfo.StepList.Where(f =>
                                                f.Number > 0
                            //&& (f.Value != null && f.Value.GetType() != typeof(string) && f.Value.SearchType != null && f.Value.SearchType.ToString().Trim() != string.Empty)
                            //&& (f.Value != null && f.Value.ToString().Trim() != string.Empty)
                            //&& !string.IsNullOrEmpty(f.ObjectType)
                            //&& !string.IsNullOrEmpty(f.Description)
                                                ).ToList();

                        stepNumber = 1;
                        foreach (StepInfo step in taskInfo.StepList.OrderBy(o => o.Number))
                        {
                            step.Number = stepNumber;
                            stepNumber++;

                            step.EncapsulatedId = ProcessBC.EncapsulateTaskStatic(encapsulatedList, ref encapsulatedCount, step.EncapsulatedPartitionType);
                        }

                        taskInfo.StepList = taskInfo.StepList.OrderBy(o => o.Number).ToList();
                    }

                    GlobalData.processBC.ProcessInfo.TaskList = taskInfoList;

                    isValid = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
            }

            return isValid;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "json";
            fileDialog.Filter = "JSON File (*.json)|*.json";

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = txtFileName.Text = fileDialog.FileName;

                List<ProcessInfo> processList = ProcessBC.LoadProcessFileFromJson(AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (processList != null && processList.Count > 0)
                {
                    GlobalData.processBC.ProcessInfo = processList[0];
                    txtProcessName.Text = GlobalData.processBC.ProcessInfo.ProcessName;
                    txtProcessNumber.Text = GlobalData.processBC.ProcessInfo.Number.ToString();
                    chkEnabled.Checked = GlobalData.processBC.ProcessInfo.Enabled;

                    dgvTaskList.DataSource = GlobalData.processBC.ProcessInfo.TaskList;
                }
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                RunProcessForm runProcessForm = new RunProcessForm(GlobalData.processBC.ProcessInfo);
                runProcessForm.ShowDialog(this);
            }
        }

        private void btnProcessBuild_Click(object sender, EventArgs e)
        {
            string exeName = Path.GetFileNameWithoutExtension(GlobalData.processBC.ProcessInfo.ProcessName) + ".exe";

            BuilderExe builderExe = new BuilderExe(AppDomain.CurrentDomain.BaseDirectory, CommonConstants.DEFAULT_USER_FOLDER);

            List<ProcessInfo> processInfoList = ProcessBC.LoadProcessFileFromJson(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (processInfoList != null && processInfoList.Count > 0)
            {
                builderExe.Build(exeName, fileName, processInfoList[0]);
                List<string> fileList = builderExe.GetResultFileList();
            }
            MessageBox.Show("Ejecutable construido");
        }

        private void btnRunEvent_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                RunEventForm runEventForm = new RunEventForm(GlobalData.processBC.ProcessInfo);
                runEventForm.ShowDialog(this);
            }
        }

        private void btnProgrammingLanguage_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalData.processBC.ProcessInfo.LanguageSettingList == null)
                {
                    GlobalData.processBC.ProcessInfo.LanguageSettingList = new List<LanguageSourceSettingInfo>();
                }

                ProgrammingLanguageForm programmingForm = new ProgrammingLanguageForm(GlobalData.processBC.ProcessInfo.LanguageSettingList);
                programmingForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnProcessDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalData.processBC.ProcessInfo.ProcessDatabase == null)
                {
                    GlobalData.processBC.ProcessInfo.ProcessDatabase = new DatabaseInfo();
                }

                DatabaseForm dataBaseForm = new DatabaseForm(GlobalData.processBC.ProcessInfo.ProcessDatabase);
                dataBaseForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
