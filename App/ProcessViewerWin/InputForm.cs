using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using static ProcessAutomation.Framework.Shared.EncryptionShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace ProcessViewerWin.App.Windows
{
    public partial class InputForm : Form
    {
        private string processName = null;
        private string processFile = null;
        private string processFolder = null;
        private ProcessInfo processInfo = null;
        private ProcessBC processBC = new ProcessBC();
        private StepBC stepBC = null;

        public InputForm(string processName, string processFile, string processFolder)
        {
            InitializeComponent();
            this.processName = processName;
            this.processFile = processFile;
            this.processFolder = processFolder;

            stepBC = new StepBC();
        }

        private void InputForm_Load(object sender, EventArgs e)
        {
            ChangeLanguage(ShellForm.language);
            txtProcessName.Text = processName;
            dgvStepList.AutoGenerateColumns = false;
            LoadInputGrid();
        }

        private void ChangeLanguage(string language = "es-MX")
        {
            ShellForm.language = language;
            ResourceManager languageResource = new ResourceManager(string.Format("ProcessViewerWin.App.Windows.i18n.InputForm_{0}", language), Assembly.GetExecutingAssembly());

            if (languageResource != null)
            {
                lblProcessName.Text = languageResource.GetString("Process");
                btnAccept.Text = languageResource.GetString("Accept");
                btnCancel.Text = languageResource.GetString("Cancel");
                colTaskName.HeaderText = languageResource.GetString("TaskHeader");
                colDescription.HeaderText = languageResource.GetString("DescriptionHeader");
                colValue.HeaderText = languageResource.GetString("ValueHeader");                
                colValueType.HeaderText = languageResource.GetString("OutputTypeHeader");
                //colValueFormat.HeaderText = languageResource.GetString("Process");
            }
        }

        //private static List<ProcessInfo> LoadProcessFileFromJson(string processFile)
        //{
        //    List<ProcessInfo> queryList = new List<ProcessInfo>();

        //    try
        //    {
        //        if (File.Exists(processFile))
        //        {
        //            byte[] dataFile = Encryption.DecryptFile(processFile, EncryptionConstants.ENCRYPTION_KEY);

        //            if (dataFile != null)
        //            {
        //                string json = Encoding.UTF8.GetString(dataFile, 0, dataFile.Length);
        //                queryList.Add(JsonConvert.DeserializeObject<ProcessInfo>(json));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.ToString();
        //    }

        //    return queryList;
        //}

        private void LoadInputGrid()
        {
            List<ProcessInfo> processList = ProcessBC.LoadProcessFileFromJsonAndDecrypt(processFile);
            if (processList != null && processList.Count > 0)
            {
                processInfo = processList[0];
                List<RunProcessInfo> runProcessList = processBC.InitProcess(processInfo, false);

                dgvStepList.DataSource = null;
                dgvStepList.DataSource = runProcessList;
                dgvStepList.Refresh();
            }
        }

        private void SaveConfig()
        {   
            if (dgvStepList.Rows != null && dgvStepList.Rows.Count > 0)
            {
                List<RunProcessInfo> runProcessList = (List<RunProcessInfo>)dgvStepList.DataSource;

                List<TaskInfo> taskList = (from rp in runProcessList
                         from s in processInfo.TaskList.Where(t => t.Number == rp.TaskLegacyNumber)
                         from step in s.StepList.Where(s => s.Number == rp.StepLegacyNumber)
                         .Select(step =>
                         {
                             if (step.ValueTypeDeclaration == "EDITABLE")
                             {
                                 dynamic dynamicValue = null;

                                 if (step.InputValueType == ValueTypeEnum.JSON.ToString())
                                 {
                                     try
                                     {
                                         dynamicValue = JsonConvert.DeserializeObject<dynamic>(step.Value);

                                         if (dynamicValue["Value"] == null)
                                         {
                                             step.Value = rp.InputValue;
                                         }
                                         else
                                         {
                                             dynamicValue.Value = rp.InputValue;
                                             step.Value = dynamicValue.ToString();
                                         }
                                     }
                                     catch (Exception)
                                     {
                                         step.Value = rp.InputValue;
                                     }
                                 }
                                 else
                                 {
                                     step.Value = rp.InputValue;
                                 }
                             }
                             return step;
                         })
                         select s).Distinct().ToList();

                processInfo.TaskList = taskList;

                string processFileBackup = Path.ChangeExtension(processFile, "bak");
                if (!File.Exists(processFileBackup))
                {
                    File.Copy(processFile, processFileBackup);
                }

                if (File.Exists(processFile))
                {
                    File.Delete(processFile);
                }

                Encryption.EncryptFile(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(processInfo)), processFile, EncryptionConstants.ENCRYPTION_KEY);
            }
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

        private void btnAccept_Click(object sender, EventArgs e)
        {
            SaveConfig();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private InputFormatResultInfo OpenInputDialog(InputFormatResultInfo resultInfo, bool enabledOptions)
        {
            if (resultInfo.Ok)
            {
                InputFormatForm configForm = new InputFormatForm(resultInfo.Description, resultInfo.Component,
                    resultInfo.PageText, ValueFormatEnum.STEP.ToString(), resultInfo.InputFormatText, resultInfo.FieldTranslation, resultInfo.VariableArray, 
                    enabledOptions);

                configForm.ShowDialog(this);

                if (configForm.result != null)
                {
                    resultInfo.Result = StepBC.ProcessResultFormat(configForm.result, resultInfo.StepValue, resultInfo.InputFormatText);
                }
                else
                {
                    resultInfo.Result = resultInfo.StepValue;
                }
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

        private void dgvStepList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                //if (e.ColumnIndex == 5)
                {
                    if (dgvStepList.CurrentRow.DataBoundItem != null)
                    {
                        RunProcessInfo runProcessInfo = (RunProcessInfo)dgvStepList.CurrentRow.DataBoundItem;
                        InputFormatResultInfo resultInfo = null;

                        if ((resultInfo = OpenInputDialog(StepBC.BuildInputFormat(runProcessInfo.InputValue, ValueFormatEnum.STEP.ToString()
                            , runProcessInfo.ComponentInfo.ComponentType, runProcessInfo.StepDescription
                            , processBC.RetrieveRunStepVariableList((List<RunProcessInfo>) dgvStepList.DataSource, runProcessInfo.StepNumber, runProcessInfo.TaskNumber), ShellForm.language, CommonConstants.INPUT_FIELDS_VISOR_FILE
                            , runProcessInfo.ComponentInfo, processFolder), false)).Ok)
                        {
                            runProcessInfo.InputValue = resultInfo.Result;
                            dgvStepList.Refresh();
                        }
                        else
                        {
                            InputSingleValueForm configForm = new InputSingleValueForm(runProcessInfo.StepDescription, runProcessInfo.ComponentInfo.ComponentType
                                , runProcessInfo.InputValue);

                            configForm.ShowDialog(this);

                            if (configForm.result != null)
                            {
                                runProcessInfo.InputValue = configForm.result;
                                dgvStepList.Refresh();
                            }
                        }
                    }
                }
            }
        }
    }
}
