using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WindowsFormsApp2
{
    public class CheckUser
    {
        public string login { get; set; }

        public bool IsAdmin { get; }

        public string status => IsAdmin ? "Admin" : "User";

        public CheckUser(string login, bool isAdmin)
        {
            login = login.Trim();
            IsAdmin = isAdmin;
        }
    }
}
