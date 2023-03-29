using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {

        DB database = new DB();


        private void CreateColums()
        {
            dataGridView1.Columns.Add("id_user", "id");
            dataGridView1.Columns.Add("login_user", "Имя");
            dataGridView1.Columns.Add("password_user", "Фамилия");
            var checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.HeaderText = "IsAdmin";
            dataGridView1.Columns.Add(checkColumn);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetBoolean(3));
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.openConnection();

            string querystring = $"select * from register";

            SqlCommand command = new SqlCommand(querystring, database.getConnection());

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();

            database.closedConnection();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            database.openConnection();
            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                var isadmin = dataGridView1.Rows[index].Cells[3].Value.ToString();

                var changeQuery = $"UPDATE register SET is_admin = '{isadmin}' WHERE id_user = '{id}'";

                var command = new SqlCommand(changeQuery, database.getConnection());
                command.ExecuteNonQuery();
            }
            database.closedConnection();
            RefreshDataGrid(dataGridView1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefreshDataGrid(dataGridView1);
        }

        private void Registration_but_Click(object sender, EventArgs e)
        {
            database.openConnection();

            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = Convert.ToInt32(dataGridView1.Rows[selectedRowIndex].Cells[0].Value);
            var deleteQuery = $"DELETE FROM register WHERE id_user = {id}";

            var command = new SqlCommand(deleteQuery, database.getConnection());
            command.ExecuteNonQuery();

            database.closedConnection();

            RefreshDataGrid(dataGridView1);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
