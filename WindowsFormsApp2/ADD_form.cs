using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySqlX.XDevAPI.Relational;

namespace WindowsFormsApp2
{
    public partial class ADD_form : Form
    {

        DB database = new DB();
        public ADD_form()
        {
            InitializeComponent();
        }

        private void button_new_zapis_Click(object sender, EventArgs e)
        {
            database.openConnection();

            var name = textBox_name.Text;
            var surname = textBox_surname.Text;
            var doljnost = textBox_doljnost.Text;
            var otdel = textBox1.Text;
            int hours;

            if(int.TryParse(textBox_hours.Text, out hours))
            {
                var addQuery = $"insert into users (name_sotrudniki, surname_sotrudniki, hours_work, doljnost, otdel) values ('{name}', '{surname}', '{hours}', '{doljnost}', '{otdel}')";

                var command = new SqlCommand(addQuery, database.getConnection());

                command.ExecuteNonQuery();

                MessageBox.Show("Запись создана", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            else
            {
                MessageBox.Show("Запись не удалось создать", "Формат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            database.closedConnection();

        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
