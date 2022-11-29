using System;
using System.Collections.Generic;
using System.Text;

namespace BeCloser.Models
{
    class Teacher
    {
        public string name { set; get; }
        public string email { set; get; }
        public string uid { set; get; }
        public Teacher (string n, string e, string id)
        {
            name = n;
            email = e;
            uid = id;
        }
    }
}
