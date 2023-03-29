using System;
using System.Collections.Generic;
using System.ComponentModel;
using SD = System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.IO;

namespace WindowsFormsApp2
{
    public partial class MainCore : Form
    {

        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted,

        }

        private readonly CheckUser _user;

        DB database = new DB();

        int selectedRow;

        public MainCore(CheckUser user)
        {   
            InitializeComponent();
            _user = user;
        }


        private void IsAdmin()
        {
            button_new_zapis.Enabled= _user.IsAdmin;
            button_dell.Enabled = _user.IsAdmin;
            button_izmenit.Enabled= _user.IsAdmin;
            Export.Enabled= _user.IsAdmin;
            button_save.Enabled= _user.IsAdmin;
            toolStripButton1.Enabled = _user.IsAdmin;
        }

        private void CreateColums()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("name_sotrudniki", "Имя");
            dataGridView1.Columns.Add("surname_sotrudniki", "Фамилия");
            dataGridView1.Columns.Add("hours_work", "Рабочие часы");
            dataGridView1.Columns.Add("doljnost", "Должность");
            dataGridView1.Columns.Add("otdel", "Отдел");
            dataGridView1.Columns.Add("IsNew", String.Empty);
        }

        private void ClearField()
        {
            textBox_ID.Text = "";
            textBox_name.Text = "";
            textBox_surname.Text = "";
            textBox_hours.Text = "";
            textBox_doljnost.Text = "";
            textBox_otdel.Text = "";
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetInt32(3), record.GetString(4), record.GetString(5), RowState.ModifiedNew);
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.openConnection();

            string querystring = $"select * from users";

            SqlCommand command = new SqlCommand(querystring, database.getConnection());

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            CloseButton.ForeColor = Color.DarkRed;
        }

        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            CloseButton.ForeColor = Color.Black;
        }

        Point lastpoint;
        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastpoint.X;
                this.Top += e.Y - lastpoint.Y;
            }
        }

        private void MainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            lastpoint = new Point(e.X, e.Y);
        }

        private void MainCore_Load(object sender, EventArgs e)
        {
            toolStrip1.Text = $"{_user.login}: {_user.status}";
            IsAdmin();
            CreateColums();
            RefreshDataGrid(dataGridView1);
        }

        private void button_dell_Click(object sender, EventArgs e)
        {
            deleteRow();
            ClearField();
        }

        private void update()
        {
            database.openConnection();

            for(int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[6].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from users where id = {id}";

                    var command = new SqlCommand(deleteQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }

                if(rowState == RowState.Modified)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var surname = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var hours = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var doljnost = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var otdel = dataGridView1.Rows[index].Cells[5].Value.ToString();

                    var changeQuery = $"update users set name_sotrudniki = '{name}', surname_sotrudniki = '{surname}', hours_work = '{hours}', doljnost = '{doljnost}', otdel = '{otdel}' where id = {id}";

                    var command = new SqlCommand(changeQuery, database.getConnection());
                    command.ExecuteNonQuery();
                }
            }

            database.closedConnection();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
            if(e.RowIndex >= 0)  
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow]; 

                textBox_ID.Text = row.Cells[0].Value.ToString();
                textBox_name.Text = row.Cells[1].Value.ToString();
                textBox_surname.Text = row.Cells[2].Value.ToString();
                textBox_hours.Text = row.Cells[3].Value.ToString();
                textBox_doljnost.Text = row.Cells[4].Value.ToString();
                textBox_otdel.Text = row.Cells[5].Value.ToString();


            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
            ClearField();  
        }

        private void button_new_zapis_Click(object sender, EventArgs e)
        {
            ADD_form addfrm= new ADD_form();
            addfrm.Show();
        }


        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchstring = $"select * from users where concat (id, name_sotrudniki, surname_sotrudniki, hours_work) like '%" + textBox_search.Text + "%'";

            SqlCommand com = new SqlCommand(searchstring, database.getConnection());

            database.openConnection();

            SqlDataReader read = com.ExecuteReader();

            while(read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
        }


        private void textBox_search_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible= false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
                return;
            }

            dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            update();
        }


        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell .RowIndex;

            var id = textBox_ID.Text;
            var name = textBox_name.Text;
            var surname = textBox_surname.Text;
            var doljonst = textBox_doljnost.Text;
            var otdel = textBox_otdel.Text;
            int hours;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBox_hours.Text, out hours))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(id,name,surname,hours,doljonst,otdel);
                    dataGridView1.Rows[selectedRowIndex].Cells[6].Value = RowState.Modified;
                }
                else
                {
                    MessageBox.Show("Часы должны быть в числовом формате");
                }
            }

        }
        private void button_izmenit_Click(object sender, EventArgs e)
        {
            Change();
            ClearField();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ClearField();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
             if (dataGridView1.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.ApplicationClass MExcel = new Microsoft.Office.Interop.Excel.ApplicationClass();
                MExcel.Application.Workbooks.Add(Type.Missing);
                for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
                {
                    MExcel.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
                }
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        MExcel.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    }
                }
                MExcel.Columns.AutoFit();
                MExcel.Rows.AutoFit();
                MExcel.Columns.Font.Size = 12;
                MExcel.Visible = true;
            }
            else
            {
                MessageBox.Show("No records found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }
    }
    }

