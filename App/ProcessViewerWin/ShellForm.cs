using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using ProcessViewerWin.App.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProcessAutomation.Common.Service;

namespace ProcessViewerWin
{
    public partial class ShellForm : Form
    {
        List<ProcessViewerInfo> processViewerList = null;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private bool running = false;
        private int progress = 0;
        private int maximumProgress = 0;
        private List<ProcessViewerInfo> processToRun = null;
        public static string language;
        private ProcessProgramBC processProgram = null;
        private ProcessLogInfo processLog = null;

        public ShellForm()
        {
            InitializeComponent();
        }

        private void ShellForm_Load(object sender, EventArgs e)
        {
            language = "es-MX";
            LoadLanguageList();
            progressProcess.DisplayStyle = App.Windows.Controls.ProgressBarDisplayText.CustomText;
            gdvProcessList.AllowUserToAddRows = false;
            gdvProcessList.AutoGenerateColumns = false;
            gdvProcessList.ColumnHeadersVisible = true;
            gdvProcessList.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            gdvProcessList.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            gdvProcessList.DefaultCellStyle.ForeColor = Color.Black;

            processViewerList = new List<ProcessViewerInfo>();

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;

            //AddCheckBoxHeader(gdvProcessList, "chkRunHeader", new EventHandler(Run_CheckedChanged), 6);

            RefreshList();

            //if (processViewerList != null && processViewerList.Count > 0)
            //{
            //    if (!processViewerList.Any(a => !a.Run))
            //    {
            //        foreach (Control control in gdvProcessList.Controls)
            //        {
            //            if (control.Name == "chkRunHeader")
            //            {
            //                ((CheckBox)control).Checked = true;
            //            }
            //        }
            //    }
            //}
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (running)
            {
                const int sleepTime = 100;
                Thread.Sleep(sleepTime);
                while (running)
                {
                    if (backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    backgroundWorker.ReportProgress(progress, processViewerList.Count);
                    Thread.Sleep(sleepTime);
                }

                backgroundWorker.ReportProgress(progress, processViewerList.Count);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (running)
            {
                progressProcess.Maximum = maximumProgress;
                progressProcess.Value = progress;

                if (progress < processToRun.Count)
                {
                    progressProcess.CustomText = string.Format("Proceso: {0}", processToRun[progress].ProcessName);

                    if (processLog != null)
                    {
                        lblLog.Text = processLog.StepName;
                    }
                }
            }
        }

        private void RefreshList()
        {
            processViewerList.Clear();
            string[] directoryArray = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory);

            if (directoryArray != null && directoryArray.Length > 0)
            {
                string processViewerFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProcessViewer.json");
                if (File.Exists(processViewerFile))
                {
                    using (StreamReader r = new StreamReader(processViewerFile))
                    {
                        processViewerList = JsonConvert.DeserializeObject<List<ProcessViewerInfo>>(r.ReadToEnd());
                        processViewerList = processViewerList.OrderBy(f => f.ProcessNumber).ToList();
                    }
                }
                else
                {
                    int processNumber = 1;
                    foreach (string directory in directoryArray)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                        string[] fileArray = Directory.GetFiles(directory, "*.dll");
                        if (fileArray != null && fileArray.Length > 0)
                        {
                            fileArray = fileArray.Where(s => !(s.Contains(CommonConstants.BUSINESS_DLL)
                            || s.Contains(CommonConstants.COMMONS_DLL)
                            || s.Contains(CommonConstants.FRAMEWORK_DLL))).ToArray();

                            foreach (string file in fileArray)
                            {
                                processViewerList.Add(new ProcessViewerInfo
                                {
                                    ProcessNumber = processNumber,
                                    ProcessName = Path.GetFileNameWithoutExtension(file),
                                    Description = null,
                                    Folder = directoryInfo.Name,
                                    ProcessPath = Path.Combine(directoryInfo.Name, Path.GetFileName(file)),
                                    Run = false
                                });
                                processNumber++;
                            }
                        }
                    }
                }

                gdvProcessList.DataSource = processViewerList;
                gdvProcessList.Refresh();
            }
        }

        private void LoadLanguageList()
        {
            List<ComboBoxInfo> comboList = new List<ComboBoxInfo>
            {
                new ComboBoxInfo("Español", "es-MX"),
                new ComboBoxInfo("English", "en-US")
            };

            cmbLanguage.Items.Clear();
            cmbLanguage.DataSource = null;
            cmbLanguage.DataSource = comboList;
            cmbLanguage.DisplayMember = "Text";
            cmbLanguage.ValueMember = "Value";
            cmbLanguage.SelectedIndex = 0;

            ChangeLanguage(cmbLanguage.SelectedValue.ToString());
        }

        private void ChangeLanguage(string language = "es-MX")
        {
            ShellForm.language = language;
            ResourceManager languageResource = new ResourceManager(string.Format("ProcessViewerWin.App.Windows.i18n.ShellForm_{0}", language), Assembly.GetExecutingAssembly());

            if (languageResource != null)
            {
                colDescription.HeaderText = languageResource.GetString("DescriptionHeader");
                colFolder.HeaderText = languageResource.GetString("FolderHeader");
                colShowInput.HeaderText = languageResource.GetString("ShowInputHeader");
                colProcessNumber.HeaderText = languageResource.GetString("SymbolNumberHeader");
                colProcessName.HeaderText = languageResource.GetString("ProcessHeader");
                colRun.HeaderText = languageResource.GetString("RunHeader");
                gpoProcessList.Text = languageResource.GetString("ProcessList");
                gpoProcessViewer.Text = languageResource.GetString("ProcessViewer");
                btnRefresh.Text = languageResource.GetString("Refresh");
                btnRegenerate.Text = languageResource.GetString("Regenerate");
                btnRun.Text = languageResource.GetString("Run");
                btnSave.Text = languageResource.GetString("Save");
                btnSelectAll.Text = languageResource.GetString("Select");                
                lblTitle.Text = languageResource.GetString("Title");
                btnUnselectAll.Text = languageResource.GetString("Unselect");
                //languageResource.GetString("Spanish");
                //languageResource.GetString("English");                
                //languageResource.GetString("Language");
            }
        }

        private void RegenerateConfigFile()
        {
            if (MessageBox.Show("El archivo de configuración se va a crear nuevamente por completo, configuraciones guardardas se van a perder.\r\n¿Estas seguro?", "Visor de Procesos", MessageBoxButtons.OKCancel)
                == DialogResult.OK)
            {
                processViewerList.Clear();
                string[] directoryArray = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory);
                int processNumber = 1;
                foreach (string directory in directoryArray)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    string[] fileArray = Directory.GetFiles(directory, "*.*").Where(s => s.EndsWith(".dll") || s.EndsWith(".exe")).ToArray();
                    if (fileArray != null && fileArray.Length > 0)
                    {
                        fileArray = fileArray.Where(s => !(s.Contains(CommonConstants.BUSINESS_DLL)
                        || s.Contains(CommonConstants.COMMONS_DLL)
                        || s.Contains(CommonConstants.FRAMEWORK_DLL)))
                            .Select(f => Path.GetFileNameWithoutExtension(f))
                            .ToArray();

                        var fileDuplicates = from filePath in fileArray
                                             group filePath by filePath into files
                                             where files.Count() > 1
                                             select files;

                        string processFileJson = null;
                        foreach (var file in fileDuplicates)
                        {
                            string fileDll = file.Key + ".dll";
                            processFileJson = GetProcessFile(Path.Combine(directoryInfo.Name, fileDll), directoryInfo.Name);

                            processViewerList.Add(new ProcessViewerInfo
                            {
                                ProcessNumber = processNumber,
                                ProcessName = Path.GetFileNameWithoutExtension(fileDll),
                                Description = null,
                                Folder = directoryInfo.Name,
                                ProcessFile = processFileJson,
                                ProcessPath = Path.Combine(directoryInfo.Name, fileDll),
                                Run = true
                            });
                            processNumber++;
                        }
                    }
                }

                if (processViewerList.Count == 0)
                {
                    RefreshList();
                }
                else
                {
                    string processViewerFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProcessViewer.json");
                    if (File.Exists(processViewerFile))
                    {
                        File.Delete(processViewerFile);
                    }

                    File.WriteAllText(processViewerFile, JsonConvert.SerializeObject(processViewerList, Formatting.Indented));

                    RefreshList();
                }

                MessageBox.Show("Regeneración terminada");
            }
        }

        private void SaveConfig()
        {
            if (gdvProcessList.DataSource != null)
            {
                List<ProcessViewerInfo> processList = (List<ProcessViewerInfo>)gdvProcessList.DataSource;
                if (processList != null && processList.Count > 0)
                {
                    int processNumber = 1;
                    processList = processList.OrderBy(o => o.ProcessNumber).Select(s =>
                    {
                        s.ProcessNumber = processNumber;
                        processNumber++;
                        return s;
                    }).ToList();

                    string processViewerFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProcessViewer.json");
                    if (File.Exists(processViewerFile))
                    {
                        File.Delete(processViewerFile);
                    }

                    File.WriteAllText(processViewerFile, JsonConvert.SerializeObject(processList, Formatting.Indented));
                    RefreshList();
                }

                MessageBox.Show("Configuración guardada");
            }
        }

        private void ProcessRun()
        {
            bool stopProcess = false;

            if (processProgram == null)
            {
                processProgram = new ProcessProgramBC(AppDomain.CurrentDomain.BaseDirectory, GetLog);
            }

            if (!running)
            {
                btnRun.Text = "Stop";
                List<ProcessViewerInfo> processList = (List<ProcessViewerInfo>)gdvProcessList.DataSource;

                if (processList != null && processList.Count > 0)
                {
                    processToRun = processList.Where(f => f.Run).ToList();

                    if (processToRun != null && processToRun.Count > 0)
                    {
                        processToRun = processToRun.OrderBy(o => o.ProcessNumber).ToList();
                        progress = 0;
                        running = true;
                        maximumProgress = processToRun.Count();
                        backgroundWorker.RunWorkerAsync();
                        
                        Task.Run(() =>
                        {
                            //const string commandFormat = "/C dotnet \"{0}\"";
                            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                            foreach (ProcessViewerInfo processInfo in processToRun)
                            {
                                if (!stopProcess)
                                {
                                    //ProcessStartInfo processStart = new ProcessStartInfo("cmd.exe");
                                    //processStart.WorkingDirectory = Path.GetDirectoryName(Path.Combine(baseDirectory, processInfo.ProcessPath));
                                    //processStart.Arguments = string.Format(commandFormat, Path.GetFileName(processInfo.ProcessPath));
                                    //Process cmd = Process.Start(processStart);
                                    //cmd.WaitForExit();

                                    processProgram.ExecuteProcess(Path.Combine(baseDirectory, processInfo.ProcessFile), processInfo.Folder);
                                }
                                else
                                {
                                    break;
                                }

                                progress++;
                            }

                            Thread.Sleep(1000);

                            maximumProgress = 0;
                            running = false;
                        });
                    }
                }

                btnRun.Text = "Run";
            }
            else
            {
                if (MessageBox.Show("Los procesos estan corriendo\r\n¿Desea detenerlos?", "Visor de Procesos", MessageBoxButtons.YesNo)
                    == DialogResult.Yes)
                {
                    stopProcess = true;
                    btnRun.Text = "Run";
                    running = false;
                    processProgram.StopProgram();
                }
            }
        }

        public void GetLog(ProcessLogInfo processLog)
        {
            this.processLog = processLog;
        }

        private string GetProcessFile(string processPath, string outputFolder)
        {
            string processFile = null;
            List<ProcessViewerInfo> processList = (List<ProcessViewerInfo>)gdvProcessList.DataSource;

            if (!string.IsNullOrEmpty(processPath))
            {
                const string commandFormat = "/C dotnet \"{0}\" -f";
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                ProcessStartInfo processStart = new ProcessStartInfo("cmd.exe");
                processStart.WorkingDirectory = Path.GetDirectoryName(Path.Combine(baseDirectory, processPath));
                processStart.Arguments = string.Format(commandFormat, Path.GetFileName(processPath));
                processStart.RedirectStandardOutput = true;
                Process cmd = Process.Start(processStart);
                cmd.WaitForExit();

                while (!cmd.StandardOutput.EndOfStream)
                {
                    processFile = cmd.StandardOutput.ReadLine();
                }

                if (!string.IsNullOrEmpty(processFile))
                {
                    processFile = Path.Combine(outputFolder, processFile);
                }
            }

            return processFile;
        }

        private void AddCheckBoxHeader(DataGridView gridView, string name, EventHandler clickEvent, int columnIndex = 0)
        {
            //    DataGridViewCheckBoxColumn  checkBoxColumn = new DataGridViewCheckBoxColumn();
            //    checkBoxColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //    gridView.Columns.Add(checkBoxColumn);

            CheckBox checkBoxControl = new CheckBox();
            
            //Get the column header cell bounds
            Rectangle rect = gridView.GetCellDisplayRectangle(columnIndex, -1, true);
            checkBoxControl.Size = new Size(15, 15);

            //Change the location of the CheckBox to make it stay on the header
            checkBoxControl.Location = new System.Drawing.Point(rect.Location.X + ((rect.Width - checkBoxControl.Width) / 2), rect.Location.Y + ((rect.Height - checkBoxControl.Height) / 2));
            checkBoxControl.TabIndex = 0;
            //checkBoxControl.Text = title;
            checkBoxControl.Name = name;
            checkBoxControl.CheckedChanged += clickEvent;

            //Add the CheckBox into the DataGridView
            gridView.Controls.Add(checkBoxControl);
            //gridView.Columns[columnIndex].Visible = false;
        }

        private void Run_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gdvProcessList.Rows)
            {
                ((DataGridViewCheckBoxCell)row.Cells[6]).Value = ((CheckBox)sender).Checked;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            ProcessRun();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (processViewerList != null && processViewerList.Count > 0)
            {
                List<ProcessViewerInfo> processList = (List<ProcessViewerInfo>)gdvProcessList.DataSource;

                foreach (ProcessViewerInfo processViewer in processList)
                {
                    processViewer.Run = true;
                }

                gdvProcessList.Refresh();
            }
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            if (processViewerList != null && processViewerList.Count > 0)
            {
                List<ProcessViewerInfo> processList = (List<ProcessViewerInfo>)gdvProcessList.DataSource;

                foreach (ProcessViewerInfo processViewer in processList)
                {
                    processViewer.Run = false;
                }

                gdvProcessList.Refresh();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void btnRegenerate_Click(object sender, EventArgs e)
        {
            RegenerateConfigFile();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void gdvProcessList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 5)
                {
                    ProcessViewerInfo processViewer = (ProcessViewerInfo)gdvProcessList.CurrentRow.DataBoundItem;
                    InputForm inputForm = new InputForm(processViewer.ProcessName, processViewer.ProcessFile, processViewer.Folder);
                    inputForm.ShowDialog(this);
                }
            }
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {

        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxInfo comboInfo = (ComboBoxInfo)((ComboBox)sender).SelectedItem;

            if (comboInfo != null)
            {
                ChangeLanguage(comboInfo.Value);
            }
        }
    }
}
