using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProcessAutomation.Framework;

namespace ProcessAutomation.BL
{
    public class BaseForm : Form
    {
        protected void DataGridViewComboboxCellEnter(DataGridView dataGridView, DataGridViewCellEventArgs e)
        {
            if (dataGridView != null)
            {
                bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1);

                if (dataGridView.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
                {
                    dataGridView.BeginEdit(true);
                    ((ComboBox)dataGridView.EditingControl).DroppedDown = true;
                }
            }
        }

        protected void DataGridViewCommitChanges(DataGridView dataGridView)
        {
            if (dataGridView.Rows != null)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dataGridView.EndEdit();
            }
        }

        protected void DataGridViewDeleteRow<T>(DataGridView dataGridView, DataGridViewCellEventArgs e, dynamic dataList)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    T componentInfo = (T)dataGridView.Rows[e.RowIndex].DataBoundItem;

                    if (e.ColumnIndex == dataGridView.Columns.Cast<DataGridViewColumn>().FirstOrDefault(f => f.Name.Contains("Delete")).Index)
                    {
                        try
                        {
                            dataList.Remove(componentInfo);
                            dataGridView.DataSource = null;
                            dataGridView.DataSource = dataList;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
