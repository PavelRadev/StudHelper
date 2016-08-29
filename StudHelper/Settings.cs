using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace StudHelper
{
    public class Settings
    {
        public static string ConnectionString = "Server=6037f828-b1e7-4d80-81ae-a53600a98c60.sqlserver."
            + "sequelizer.com;Database=db6037f828b1e74d8081aea53600a98c60;User ID=jgmrkuenmqyodqub;Password=BejVapM"
            + "45tnRSkEsVFyuakJsXiDGSixtyAC5ZgQhLHTeJAWsgNUJpBcqCY6ZTejU;";
        public static string DefaultFileName= "Settings.shcfg";
        //имена таблиц(используются в запросах к базе)
        public static string users_table = "users";
        public static string Events_table = "Events";
        public static string Groups_table = "Groups";
        public static string Courses_table = "Courses";
        public static string Rights_table = "Rights";
        public static string lessons_table = "lessons";
        public static string course_users_table = "course_users";
        public static string dictionary_table = "Dictionary_main";
        public static string dictionary_fav_table = "Dictionary_fav";
        
        public static void ExecFile(MetroWindow mw, string filename)
        {
            try
            {
                string fullpath = Directory.GetCurrentDirectory().ToString() + @"\" + filename;
                if (File.Exists(fullpath))
                {
                    String[] values = File.ReadAllText(fullpath).Split('\n');
                    Settings.ConnectionString = values[0];
                    Settings.users_table = values[1];
                    Settings.Events_table = values[2];
                    Settings.Groups_table = values[3];
                    Settings.Courses_table = values[4];
                    Settings.Rights_table = values[5];
                    Settings.lessons_table = values[6];
                    Settings.course_users_table = values[7];
                    Settings.dictionary_table = values[8];
                    Settings.dictionary_fav_table = values[9];
                    Globals.cn = new SqlConnection(Settings.ConnectionString);
                }
                else
                {
                    using (StreamWriter sw = File.CreateText(fullpath))
                    {
                        sw.WriteLine(Settings.ConnectionString);
                        sw.WriteLine(Settings.users_table);
                        sw.WriteLine(Settings.Events_table);
                        sw.WriteLine(Settings.Groups_table);
                        sw.WriteLine(Settings.Courses_table);
                        sw.WriteLine(Settings.Rights_table);
                        sw.WriteLine(Settings.lessons_table);
                        sw.WriteLine(Settings.course_users_table);
                        sw.WriteLine(Settings.dictionary_table);
                        sw.WriteLine(Settings.dictionary_fav_table);
                    }
                }
            }
            catch (Exception ex)
            {
                mw.ShowMessageAsync("Ошибка", ex.ToString());
            }
        }
        public static void ReWriteSettingsFile(MetroWindow mw,string filename)
        {
            try
            {
                string fullpath = Directory.GetCurrentDirectory().ToString() + @"\" + filename;
                using (StreamWriter sw = File.CreateText(fullpath))
                {
                    sw.Write(Settings.ConnectionString + "\n");
                    sw.Write(Settings.users_table + "\n");
                    sw.Write(Settings.Events_table + "\n");
                    sw.Write(Settings.Groups_table + "\n");
                    sw.Write(Settings.Courses_table + "\n");
                    sw.Write(Settings.Rights_table + "\n");
                    sw.Write(Settings.lessons_table + "\n");
                    sw.Write(Settings.course_users_table + "\n");
                    sw.Write(Settings.dictionary_table + "\n");
                    sw.Write(Settings.dictionary_fav_table);
                }
            }
            catch (Exception ex)
            {
                mw.ShowMessageAsync("Ошибка ", ex.ToString());
            }
        }
    }
}
