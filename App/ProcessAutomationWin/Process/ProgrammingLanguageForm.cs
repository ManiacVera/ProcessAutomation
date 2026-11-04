using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ProcessAutomation.Process
{
    public partial class ProgrammingLanguageForm : Form
    {
        List<LanguageSourceSettingInfo> languageList = null;
        LanguageSourceSettingInfo currentLanguage = null;

        public ProgrammingLanguageForm(List<LanguageSourceSettingInfo> languageList)
        {
            this.languageList = languageList;

            InitializeComponent();            
            LoadLanguageList();
            LoadLanguage(cmbLanguage.Text);
        }

        //private void TaskForm_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    DataGridViewCommitChanges(dgvComponentList);
        //    DataGridViewCommitChanges(dgvStepList);
        //}

        private void LoadLanguageList()
        {
            cmbLanguage.DataSource = ProcessBC.RetrieveLanguageSourceList();
        }

        private void LoadLanguage(string language)
        {
            if (!string.IsNullOrWhiteSpace(language))
            {
                if (!languageList.Any(a => a.LanguageSource == language.Trim()))
                {
                    currentLanguage = new LanguageSourceSettingInfo();
                    currentLanguage.LanguageSource = language.Trim();

                    languageList.Add(currentLanguage);
                }
                else
                {
                    currentLanguage = languageList.FirstOrDefault(f => f.LanguageSource == language.Trim());
                }

                if (currentLanguage.LibraryList == null)
                {
                    currentLanguage.LibraryList = new List<string>();
                }

                if (currentLanguage.ImportList == null)
                {
                    currentLanguage.ImportList = new List<string>();
                }

                if (currentLanguage.CustomLibraryList == null)
                {
                    currentLanguage.CustomLibraryList = new List<string>();
                }

                lbLibrary.DataSource = currentLanguage.LibraryList;
                lbImport.DataSource = currentLanguage.ImportList;
                lbCustomLibrary.DataSource = currentLanguage.CustomLibraryList;

                LoadTextboxForm();
            }
        }

        private void btnAddLibrary_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLibrary.Text))
            {
                if (!currentLanguage.LibraryList.Contains(txtLibrary.Text.Trim()))
                {
                    currentLanguage.LibraryList.Add(txtLibrary.Text.Trim());
                    lbLibrary.DataSource = null;
                    lbLibrary.DataSource = currentLanguage.LibraryList;
                }
            }
        }

        private void btnAddImport_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtImport.Text))
            {
                if (!currentLanguage.ImportList.Contains(txtImport.Text.Trim()))
                {
                    currentLanguage.ImportList.Add(txtImport.Text.Trim());
                    lbImport.DataSource = null;
                    lbImport.DataSource = currentLanguage.ImportList;
                }
            }
        }

        private void btnAddCustomLibrary_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCustomLibrary.Text))
            {
                if (!currentLanguage.CustomLibraryList.Contains(txtCustomLibrary.Text.Trim()))
                {
                    currentLanguage.CustomLibraryList.Add(txtCustomLibrary.Text.Trim());
                    lbCustomLibrary.DataSource = null;
                    lbCustomLibrary.DataSource = currentLanguage.CustomLibraryList;
                }
            }
        }

        private void btnDeleteLibrary_Click(object sender, EventArgs e)
        {
            if (lbLibrary.SelectedIndex >= 0)
            {
                int selectedIndex = lbLibrary.SelectedIndex;
                currentLanguage.LibraryList.Remove((string)lbLibrary.SelectedItem);

                lbLibrary.DataSource = null;
                lbLibrary.DataSource = currentLanguage.LibraryList;
                lbLibrary.SelectedIndex = selectedIndex - 1;
            }
        }

        private void btnDeleteCustomLibrary_Click(object sender, EventArgs e)
        {
            if (lbCustomLibrary.SelectedIndex >= 0)
            {
                int selectedIndex = lbCustomLibrary.SelectedIndex;
                currentLanguage.CustomLibraryList.Remove((string)lbCustomLibrary.SelectedItem);

                lbCustomLibrary.DataSource = null;
                lbCustomLibrary.DataSource = currentLanguage.CustomLibraryList;
                lbCustomLibrary.SelectedIndex = selectedIndex - 1;
            }
        }

        private void btnDeleteImport_Click(object sender, EventArgs e)
        {
            if (lbImport.SelectedIndex >= 0)
            {
                int selectedIndex = lbImport.SelectedIndex;
                currentLanguage.ImportList.Remove((string)lbImport.SelectedItem);

                lbImport.DataSource = null;
                lbImport.DataSource = currentLanguage.ImportList;
                lbImport.SelectedIndex = selectedIndex - 1;
            }
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTextboxForm();
            LoadLanguage(cmbLanguage.Text);
        }

        private void ProgrammingLanguageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetTextboxForm();
        }

        private void LoadTextboxForm()
        {
            if (currentLanguage != null)
            {
                txtDirectoryLibrary.Text = string.Empty;
                txtExecutable.Text = string.Empty;
                txtCustomLibrary.Text = string.Empty;
                txtLibrary.Text = string.Empty;
                txtImport.Text = string.Empty;

                if (currentLanguage.LibraryDirectory != null)
                {
                    txtDirectoryLibrary.Text = currentLanguage.LibraryDirectory;
                }

                if (currentLanguage.LanguageExecutable != null)
                {
                    txtExecutable.Text = currentLanguage.LanguageExecutable;
                }
            }
        }

        private void SetTextboxForm()
        {
            if (currentLanguage != null)
            {
                if (txtDirectoryLibrary.Text != null)
                {
                    currentLanguage.LibraryDirectory = txtDirectoryLibrary.Text.Trim();
                }

                if (txtDirectoryLibrary.Text != null)
                {
                    currentLanguage.LanguageExecutable = txtExecutable.Text.Trim();
                }
            }
        }
    }
}
