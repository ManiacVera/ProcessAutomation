using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Process
{
    public partial class DatabaseForm : Form
    {
        DatabaseInfo databaseInfo = null;

        public DatabaseForm(DatabaseInfo databaseInfo)
        {
            this.databaseInfo = databaseInfo;
            InitializeComponent();
            LoadDatabaseTypeList();
            LoadDatabaseForm();
        }

        private void LoadDatabaseTypeList()
        {
            cmbDatabaseType.DataSource = ProcessBC.RetrieveDatabaseTypeList();
        }

        private void LoadDatabaseForm()
        {
            if (databaseInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(databaseInfo.DatabaseType))
                {
                    cmbDatabaseType.SelectedIndex = cmbDatabaseType.FindStringExact(databaseInfo.DatabaseType);
                }

                txtDatabaseName.Text = databaseInfo.DatabaseName?.Trim();
                txtPassword.Text = databaseInfo.Password?.Trim();
                txtScheme.Text = databaseInfo.Scheme?.Trim();
                txtServer.Text = databaseInfo.Server?.Trim();
                txtUser.Text = databaseInfo.UserName?.Trim();
                txtPort.Text = databaseInfo.Port.ToString();
                chkTrustedConnection.Checked = databaseInfo.TrustedConnection;

                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    try
                    {
                        txtPassword.Text = Encryption.Decrypt(txtPassword.Text);
                    }
                    catch { }
                }
            }
        }

        private void SetDatabaseForm()
        {
            if (databaseInfo != null)
            {
                databaseInfo.DatabaseType = (string)cmbDatabaseType.SelectedItem;
                databaseInfo.DatabaseName = txtDatabaseName.Text?.Trim();
                databaseInfo.Password = txtPassword.Text?.Trim();
                databaseInfo.Scheme = txtScheme.Text?.Trim();
                databaseInfo.Server = txtServer.Text?.Trim();
                databaseInfo.UserName = txtUser.Text?.Trim();

                if (!string.IsNullOrWhiteSpace(txtPort.Text) && Regex.IsMatch(txtPort.Text.Trim(), "[0-9]+"))
                {
                    databaseInfo.Port = Convert.ToInt32(txtPort.Text?.Trim());
                }
                else
                {
                    databaseInfo.Port = 1433;
                }

                databaseInfo.TrustedConnection = chkTrustedConnection.Checked;

                if (string.IsNullOrWhiteSpace(databaseInfo.Scheme))
                {
                    databaseInfo.Scheme = "dbo";
                }

                if (!string.IsNullOrWhiteSpace(databaseInfo.Password))
                {
                    try
                    {
                        databaseInfo.Password = Encryption.Encrypt(databaseInfo.Password);
                    }
                    catch { }
                }
            }
        }

        private void DatabaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetDatabaseForm();
        }
    }
}
