using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDApp
{
    public partial class frmCRUD : Form
    {
        // Create database variables
        SqlConnection sqlConn;              //  Connection to DB
        SqlCommand    sqlCmd;               //  DB command
        DataTable     table;                //  DB table
        SqlDataReader sqlReader = null;     //  Data Reader

        public frmCRUD()
        {
            InitializeComponent();
        }

        //  This method sets the connection string
        //  and also opens the connection.
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

        //  This method attempts to add the new
        //  record to the acme_widget database.
        //  Note: No validation is done here.
        private void AttemptToAddNewRecord()
        {
            SetConnectionString();

            try
            {
                //  Set SQL insert command
                sqlCmd = new SqlCommand("INSERT INTO employee VALUES (@first_name, @last_name, @address, @city, @state_code, @zip, @phone, @dept_code, @salary)", sqlConn);

                //  "Map up" the @first_name, etc.
                //  with their textbox equivalents
                sqlCmd.Parameters.AddWithValue("@first_name",   txtFirstName.Text);
                sqlCmd.Parameters.AddWithValue("@last_name",    txtLastName.Text);
                sqlCmd.Parameters.AddWithValue("@address",      txtAddress.Text);
                sqlCmd.Parameters.AddWithValue("@city",         txtCity.Text);
                sqlCmd.Parameters.AddWithValue("@state_code",   txtState.Text);
                sqlCmd.Parameters.AddWithValue("@zip",          txtZip.Text);
                sqlCmd.Parameters.AddWithValue("@phone",        txtPhone.Text);
                sqlCmd.Parameters.AddWithValue("@dept_code",    Int32.Parse(txtDepartment.Text));
                sqlCmd.Parameters.AddWithValue("@salary",       decimal.Parse(txtSalary.Text));

                //  Add, update, delete
                sqlCmd.ExecuteNonQuery();

                sqlConn.Close();

                MessageBox.Show("Record Successfully Saved");

                //  Update the DataGridView
                UploadData();

                //  Clear out all textboxes in GUI
                ClearTheGUI();
            }
            catch (Exception ex)
            {
                ShowMessage("Error Attempting To Add Record\n\n" +
                            ex.Message,
                            "ERROR ADDING RECORD");

                //  Close the database connection.
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

        //  This method attempts to update an
        //  existing database record. Even after
        //  multiple attempts to fix, this method
        //  still does not work. Sorry about that.
        private void AttemptToUpdateExistingRecord()
        {
            SetConnectionString();

            try
            {
                FillUpEmployeeTextBoxes();

                //  Set SQL update command
                sqlCmd = new SqlCommand("UPDATE employee SET first_name=@first_name, last_name=@last_name, address=@address, city=@city, state_code=@state_code, zip=@zip, phone=@phone, dept_code=@dept_code, salary=@salary WHERE employee_id=@employee_id", sqlConn);

                //  "Map up" the @first_name, etc.
                //  with their textbox equivalents
                sqlCmd.Parameters.AddWithValue("@first_name",       txtFirstName.Text);
                sqlCmd.Parameters.AddWithValue("@last_name ",       txtLastName.Text);
                sqlCmd.Parameters.AddWithValue("@address",          txtAddress.Text);
                sqlCmd.Parameters.AddWithValue("@city",             txtCity.Text);
                sqlCmd.Parameters.AddWithValue("@state_code",       txtState.Text);
                sqlCmd.Parameters.AddWithValue("@zip",              txtZip.Text);
                sqlCmd.Parameters.AddWithValue("@phone",            txtPhone.Text);
                sqlCmd.Parameters.AddWithValue("@dept_code",        Int32.Parse(txtDepartment.Text));
                sqlCmd.Parameters.AddWithValue("@salary",           Decimal.Parse(txtSalary.Text));
                sqlCmd.Parameters.AddWithValue("@employee_id",      Int32.Parse(txtID.Text));

                //  Execute the query.
                sqlCmd.ExecuteNonQuery();

                //  Close the database connection.
                sqlConn.Close();

                MessageBox.Show("Record Successfully Updated");

                //  Update the DataGridView
                UploadData();

                //  Clear out all textboxes in GUI
                ClearTheGUI();
            }
            catch (Exception ex)
            {
                ShowMessage("Error Attempting To Update Record\n\n" +
                            ex.Message,
                            "ERROR UPDATING RECORD");

                //  Close the database connection.
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

        //  This method attempts to delete an
        //  existing database record.
        private void AttemptToDeleteExistingRecord()
        {
            SetConnectionString();

            try
            {
                FillUpEmployeeTextBoxes();

                //  Set SQL delete command
                sqlCmd = new SqlCommand("DELETE FROM employee WHERE employee_id=@id", sqlConn);

                //  "Map up" id with its
                //  textbox equivalent.
                sqlCmd.Parameters.AddWithValue("@id", Int32.Parse(txtID.Text));

                //  Execute the query.
                sqlCmd.ExecuteNonQuery();

                //  Close the database connection.
                sqlConn.Close();

                MessageBox.Show("Record Successfully Deleted");

                //  Update the DataGridView
                UploadData();

                //  Clear out all textboxes in GUI
                ClearTheGUI();
            }
            catch (Exception ex)
            {
                ShowMessage("Error Attempting To Delete Record\n\n" +
                            ex.Message,
                            "ERROR DELETING RECORD");

                //  Close the database connection.
                sqlConn.Close();
            }
        }

        //  This method should just set the
        //  datagridview to the one desired
        //  record. It does not work.
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

        //  This method updates the datagridview
        //  by showing in it all employee records.
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
                //  Loop through all controls. If the
                //  current control is a textbox, then
                //  clear out its text contents.
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

        //  When the datagridview selected cell
        //  is clicked (selected), fill up the
        //  GUI textboxes with the associated
        //  database fields.
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
