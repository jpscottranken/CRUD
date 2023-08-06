using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDApp
{
    public partial class frmCRUD : Form
    {
        // Create database variables
        SqlConnection sqlConn;
        SqlCommand sqlCmd;
        DataTable table;
        SqlDataReader sqlReader = null;

        public frmCRUD()
        {
            InitializeComponent();
        }

        private void SetConnectionString()
        {
            var connString = @"Server=(localdb)\MSSQLLocalDB;Database=acme_widget;Integrated Security=SSPI";

            // Create SqlConnection based on connString
            sqlConn = new SqlConnection(connString);

            // Open Connection
            sqlConn.Open();
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            AttemptToAddNewRecord();
        }

        private void addMenuItem_Click(object sender, EventArgs e)
        {
            AttemptToAddNewRecord();
        }

        private void AttemptToAddNewRecord()
        {
            SetConnectionString();

            try
            {
                sqlCmd = new SqlCommand("INSERT INTO employee VALUES (@first_name, @last_name, @address, @city, @state_code, @zip, @phone, @dept_code, @salary)", sqlConn);

                sqlCmd.Parameters.AddWithValue("@first_name",   txtFirstName.Text);
                sqlCmd.Parameters.AddWithValue("@last_name",    txtLastName.Text);
                sqlCmd.Parameters.AddWithValue("@address",      txtAddress.Text);
                sqlCmd.Parameters.AddWithValue("@city",         txtCity.Text);
                sqlCmd.Parameters.AddWithValue("@state_code",   txtState.Text);
                sqlCmd.Parameters.AddWithValue("@zip",          txtZip.Text);
                sqlCmd.Parameters.AddWithValue("@phone",        txtPhone.Text);
                sqlCmd.Parameters.AddWithValue("@dept_code",    Int32.Parse(txtDepartment.Text));
                sqlCmd.Parameters.AddWithValue("@salary",       txtSalary.Text);

                sqlCmd.ExecuteNonQuery();

                sqlConn.Close();

                MessageBox.Show("Record Successfully Saved");

                UploadData();

                ClearTheGUI();
            }
            catch (Exception ex)
            {
                ShowMessage("Error Attempting To Add Record\n\n" +
                            ex.Message,
                            "ERROR ADDING RECORD");

                sqlConn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            AttemptToUpdateExistingRecord();
        }

        private void updateMenuItem_Click(object sender, EventArgs e)
        {
            AttemptToUpdateExistingRecord();
        }

        private void AttemptToUpdateExistingRecord()
        {
            SetConnectionString();

            try
            {
                FillUpEmployeeTextBoxes();

                sqlCmd = new SqlCommand("UPDATE employee SET first_name=@fn, last_name=@ln, address=@add, city=@ci, state_code=@st, zip=@z, phone=@ph, dept_code=@dc, salary=@sal WHERE employee_id=@eid", sqlConn);

                sqlCmd.Parameters.AddWithValue("@eid", Int32.Parse(txtID.Text));
                sqlCmd.Parameters.AddWithValue("@fn", txtFirstName.Text);
                sqlCmd.Parameters.AddWithValue("@ln", txtLastName.Text);
                sqlCmd.Parameters.AddWithValue("@add", txtAddress.Text);
                sqlCmd.Parameters.AddWithValue("@ci", txtCity.Text);
                sqlCmd.Parameters.AddWithValue("@st", txtState.Text);
                sqlCmd.Parameters.AddWithValue("@z",  txtZip.Text);
                sqlCmd.Parameters.AddWithValue("@ph", txtPhone.Text);
                sqlCmd.Parameters.AddWithValue("@dc", Int32.Parse(txtDepartment.Text));
                sqlCmd.Parameters.AddWithValue("@sal", Decimal.Parse(txtSalary.Text));

                sqlCmd.ExecuteNonQuery();

                sqlConn.Close();

                MessageBox.Show("Record Successfully Updated");

                UploadData();

                ClearTheGUI();
            }
            catch (Exception ex)
            {
                ShowMessage("Error Attempting To Update Record\n\n" +
                            ex.Message,
                            "ERROR UPDATING RECORD");

                sqlConn.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            AttemptToDeleteExistingRecord();
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            AttemptToDeleteExistingRecord();
        }

        private void AttemptToDeleteExistingRecord()
        {
            SetConnectionString();

            try
            {
                FillUpEmployeeTextBoxes();

                sqlCmd = new SqlCommand("DELETE FROM employee WHERE employee_id=@id", sqlConn);

                sqlCmd.Parameters.AddWithValue("@id", Int32.Parse(txtID.Text));

                sqlCmd.ExecuteNonQuery();

                sqlConn.Close();

                MessageBox.Show("Record Successfully Deleted");

                UploadData();

                ClearTheGUI();
            }
            catch (Exception ex)
            {
                ShowMessage("Error Attempting To Delete Record\n\n" +
                            ex.Message,
                            "ERROR DELETING RECORD");

                sqlConn.Close();
            }
        }

        private void SearchForRecordID(string id)
        {
            SetConnectionString();

            sqlCmd = new SqlCommand("SELECT * FROM employee WHERE employee_id=@id", sqlConn);
            sqlCmd.Parameters.AddWithValue("@id", int.Parse(id));

            DataView dv = table.DefaultView;
            dv.RowFilter = string.Format("last_name LIKE '%{0}%'", id);
            dgvAcmeWidget.DataSource = dv.ToTable();

            sqlConn.Close();
        }

        private void UploadData()
        {
            SetConnectionString();

            sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConn;

            sqlCmd.CommandText = "SELECT * FROM employee";

            sqlReader = sqlCmd.ExecuteReader();

            table = new DataTable();
            table.Load(sqlReader);
            sqlReader.Close();
            sqlConn.Close();

            dgvAcmeWidget.DataSource = table;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearTheGUI();
        }

        private void clearMenuItem_Click(object sender, EventArgs e)
        {
            ClearTheGUI();
        }

        private void ClearTheGUI()
        {
            try
            {
                foreach (Control c in gbEmployeeInfo.Controls)
                {
                    if (c is TextBox)
                    {
                        ((TextBox)c).Clear();
                    }
                }

                txtFirstName.Focus();
            }
            catch (Exception ex)
            {
                ShowMessage("Error Clearing TextBoxes\n\n" +
                            ex.Message, "ERROR CLEARING TEXTBOXES");
            }
         }

        private void btnExit_Click(object sender, EventArgs e)
        {
            ExitProgramOrNot();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            ExitProgramOrNot();
        }

        private void ExitProgramOrNot()
        {
            try
            {
                DialogResult dialog = MessageBox.Show(
                       "Do You Really Want To Exit?",
                       "EXIT NOW?",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "ERROR");
            }
        }

        private void ShowMessage(string msg, string title)
        {
            MessageBox.Show(msg, title,
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
        }

        private void frmCRUD_Load(object sender, EventArgs e)
        {
            UploadData();
        }

        private void dgvAcmeWidget_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FillUpEmployeeTextBoxes();
        }

        private void FillUpEmployeeTextBoxes()
        {
            try
            {
                txtID.Text = dgvAcmeWidget.SelectedRows[0].Cells[0].Value.ToString();
                txtFirstName.Text = dgvAcmeWidget.SelectedRows[0].Cells[1].Value.ToString();
                txtLastName.Text = dgvAcmeWidget.SelectedRows[0].Cells[2].Value.ToString();
                txtAddress.Text = dgvAcmeWidget.SelectedRows[0].Cells[3].Value.ToString();
                txtCity.Text = dgvAcmeWidget.SelectedRows[0].Cells[4].Value.ToString();
                txtState.Text = dgvAcmeWidget.SelectedRows[0].Cells[5].Value.ToString();
                txtZip.Text = dgvAcmeWidget.SelectedRows[0].Cells[6].Value.ToString();
                txtPhone.Text = dgvAcmeWidget.SelectedRows[0].Cells[7].Value.ToString();
                txtDepartment.Text = dgvAcmeWidget.SelectedRows[0].Cells[8].Value.ToString();
                txtSalary.Text = dgvAcmeWidget.SelectedRows[0].Cells[9].Value.ToString();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "ERROR");
            }
        }
    }
}
