using MySql.Data.MySqlClient;
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
    public partial class RegisterForm : Form
    {
        DB database = new DB();
        public RegisterForm()
        {
            InitializeComponent();
            UserName.Text = "Имя сотрудника";
            UserName.ForeColor = Color.Gray;
            UserSurname.Text = "Фамилия сотрудника";
            UserSurname.ForeColor = Color.Gray;
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

        private void UserName_Enter(object sender, EventArgs e)
        {
            if (UserName.Text == "Имя сотрудника")
            {
                UserName.Text = "";
                UserName.ForeColor = Color.Black;
            }
        }

        private void UserName_Leave(object sender, EventArgs e)
        {
            if (UserName.Text == "")
            {
                UserName.Text = "Имя сотрудника";
                UserName.ForeColor = Color.Gray;
            }
        }

        private void UserSurname_Enter(object sender, EventArgs e)
        {
            if (UserSurname.Text == "Фамилия сотрудника")
            {
                UserSurname.Text = "";
                UserSurname.ForeColor = Color.Black;
            }
        }

        private void UserSurname_Leave(object sender, EventArgs e)
        {
            if (UserSurname.Text == "")
            {
                UserSurname.Text = "Фамилия сотрудника";
                UserSurname.ForeColor = Color.Gray;
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (UserName.Text == "Имя сотрудника")
            {
                MessageBox.Show("Введите имя");
                return;
            }

            if (UserSurname.Text == "Фамилия сотрудника")
            {
                MessageBox.Show("Введите фамилию");
                return;
            }
            if (checkuser())

            {
                return;
            }

            var login = UserName.Text;
            var pass = UserSurname.Text;

            string querystring = $"insert into register(login_user, password_user, is_admin) values('{login}', '{pass}', 0)";

            SqlCommand command = new SqlCommand(querystring, database.getConnection());

            database.openConnection();

            if (command.ExecuteNonQuery() == 1) 
            {
                MessageBox.Show("Аккаунт успешно создан!", "Аккаунт создан");
            }

            else
            {
                MessageBox.Show("Аккаунт не создан!");
            }
            database.closedConnection();
        }

        private Boolean checkuser()
        {
            var login = UserName.Text;
            var pass = UserSurname.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();

            DataTable table = new DataTable();
            string querystring = $"select id_user, login_user, password_user, is_admin from register where login_user = '{login}' and password_user = '{pass}'";

            SqlCommand command = new SqlCommand(querystring, database.getConnection());

            adapter.SelectCommand= command;
            adapter.Fill(table);

            if(table.Rows.Count > 0)
            {
                MessageBox.Show("Пользователь уже существует");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginform = new LoginForm();
            loginform.Show();
        }

    }
}
