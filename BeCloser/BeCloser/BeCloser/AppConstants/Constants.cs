using BeCloser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeCloser.AppConstants
{
    class Constants
    {
        public static List<Teacher> teachers = new List<Teacher>()
        {
            new Teacher("Nora Ahmed", "nahmed@uni.edu", "JCp9SxqgPHXnMct2Hl4qVNwIOHJ2"),
            new Teacher("Sara Khalid", "skhalid@uni.edu", "0XcrgqcokNMlLcaasC1qz5CUPrV2"),
            new Teacher("Abeer Saleh", "asaleh@uni.edu", "FDpOjVUzdRP9qD35Rj8Vr7HUBgJ3"),
            new Teacher("Rahaf Mohammed", "rmohammed@uni.edu", "RHw9rz4aGKTI3Wn7yBOwwrbS7pm2")
        };

        public static string find(string name)
        {
            return teachers.Find(n => n.name == name).uid;
        }
    }
}
