using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Data.SqlClient;
using System.Data;
using MahApps.Metro.Controls.Dialogs;

namespace StudHelper
{
    public enum WorkingMode
    {
        View = 1,
        CourseTables = 2,
        MainTables = 3,
        Meeting = 4,
        Dictionary = 5
    }
    public class Globals: MetroWindow
    {
        public static SqlConnection cn = new SqlConnection(Settings.ConnectionString);

        public static string _login = "admin";
        public static string _rightGroup = "admin";
        public static int _userid;
        public static bool _su = true;


        public static string _compilerUrl, _name, _institution, _group, current_course_name;
        public static string current_course = "1";
        public static int _rights;
        public static int _currentLesson = -1;
        public static int _lastPassed = -1;

        //Наборы данных
        public static DataSet Users = new DataSet();
        public static DataSet Rights = new DataSet();
        public static DataSet Groups = new DataSet();
        public static DataSet Courses = new DataSet();
        public static DataSet Events = new DataSet();
        public static DataSet CoursesStart = new DataSet();
        public static DataSet DictionaryDataSet = new DataSet();
        public static DataSet DictionaryFavDataSet = new DataSet();
        public static DataSet DictionaryGridDataSet = new DataSet();

        //Команды
        public static SqlCommand getUsers = new SqlCommand("Select * from " + Settings.users_table, cn);
        public static SqlCommand getRights = new SqlCommand("Select * from " + Settings.Rights_table, cn);
        public static SqlCommand getGroups = new SqlCommand("Select * from " + Settings.Groups_table, cn);
        public static SqlCommand getCourses = new SqlCommand("Select * from " + Settings.Courses_table, cn);
        public static SqlCommand getEvents = new SqlCommand("Select * from " + Settings.Events_table, cn);
        public static SqlCommand getDictionary = new SqlCommand("Select * from " + Settings.dictionary_table, cn);

        //Адаптеры
        public static SqlDataAdapter GroupsAdapter = new SqlDataAdapter(getGroups);
        public static SqlDataAdapter UsersAdapter = new SqlDataAdapter(getUsers);
        public static SqlDataAdapter RightsAdapter = new SqlDataAdapter(getRights);
        public static SqlDataAdapter CoursesAdapter = new SqlDataAdapter(getCourses);
        public static SqlDataAdapter CoursesStartAdapter = new SqlDataAdapter(getCourses);
        public static SqlDataAdapter EventsAdapter = new SqlDataAdapter(getEvents);
        public static SqlDataAdapter DictionaryAdapter = new SqlDataAdapter(getDictionary);

        public static void RightsInfo(MetroWindow win)
        {
            win.ShowMessageAsync("О правах доступа", "Пользователь: стартовый набор полномочий(просмотр, "
                + "изменение данных своего аккаунта)"
                + "\n\nПреподаватель: набор полномочий, необходимых для администрирования учебного процесса"
                + "\n\nАдминистратор: наибольшее колличество полномочий(возможность полного администрирования "
                + "базы данных, и программы)");
        }

        public static void FixConnection(SqlConnection cn)
        {
            try
            {
                cn.Open();
                cn.Close();
            }
            catch
            {
                cn.Close();
            }
        }

        public static void Default_Settings()
        {
            Settings.ConnectionString = "Server=6037f828-b1e7-4d80-81ae-a53600a98c60.sqlserver.sequelizer.com;Database="
            +"db6037f828b1e74d8081aea53600a98c60;User ID=jgmrkuenmqyodqub;Password=BejVapM45tnRSkEsVFyuakJsXiDG"
            +"SixtyAC5ZgQhLHTeJAWsgNUJpBcqCY6ZTejU;";

            Settings.users_table = "users";
            Settings.Events_table = "Events";
            Settings.Groups_table = "Groups";
            Settings.Courses_table = "Courses";
            Settings.Rights_table = "Rights";
            Settings.lessons_table = "lessons";
            Settings.course_users_table = "course_users";
        }

        public static void WrongFieldColor(object tb)
        {
            //Выделение контрола красным цветом, сигнализирующим об ошибке
            ((Control)tb).Background = new SolidColorBrush(Color.FromArgb(255, 210, 0, 24));
        }

        public static void DefaultfieldColor(object tb)
        {
            //Приведение контрола в стандартное состояние
            ((Control)tb).Background = new SolidColorBrush(Color.FromArgb(255, 37, 37, 37));
        }

        public static void ErrorSign(MetroWindow win, Exception ex)
        {
            //Шаблон сообщения об ошибке(немного сокращает код)
            win.ShowMessageAsync("Ошибка", ex.Message);
        }

        public static bool CheckConnection(MetroWindow win, SqlConnection cn)
        {
            try
            {
                cn.Open();
                cn.Close();
                return true;
            }
            catch(Exception ex)
            {
                FixConnection(cn);
                ErrorSign(win, ex);
                return false;
            }
        }

        public static void DefaultTopButtonsColor(object btn1, object btn2, object btn3, object btn4)
        {
            ((Control)btn1).Background = new SolidColorBrush(Color.FromArgb(255, 37, 37, 37));
            ((Control)btn2).Background = new SolidColorBrush(Color.FromArgb(255, 37, 37, 37));
            ((Control)btn3).Background = new SolidColorBrush(Color.FromArgb(255, 37, 37, 37));
            ((Control)btn4).Background = new SolidColorBrush(Color.FromArgb(255, 37, 37, 37));
        }

        public static void ClearControl(object obj)
        {
            DefaultfieldColor(obj);
            if (obj is TextBox)
            {
                ((TextBox)obj).Clear();
            }
            else if (obj is ComboBox)
            {
                ((ComboBox)obj).SelectedIndex = -1;
            }
            else if (obj is DatePicker)
            {
                ((DatePicker)obj).SelectedDate = null;
            }
            else if (obj is CheckBox)
            {
                ((CheckBox)obj).IsChecked = false;
            }
            else if (obj is RichTextBox)
            {
                ((RichTextBox)obj).Document.Blocks.Clear();
            }
        }
        public static void ApplyBaseChanges(SqlConnection cn, SqlDataAdapter adapter, DataSet dataSet, 
            string TableName, MetroWindow mw)
        {
            //Сохранение изменений таблицы при каждом изменении текущей ячейки таблицы
            //Создает дополнительную нагрузку, но исключает ситуацию, когда большое количество введенной 
            //информации не может быть 
            //сохранено из за незначительной ошибки в одной ячейке
            try
            {
                cn.Open();
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.UpdateCommand = builder.GetUpdateCommand();
                adapter.DeleteCommand = builder.GetDeleteCommand();
                adapter.InsertCommand = builder.GetInsertCommand();
                adapter.Update(dataSet.Tables[TableName]);
                cn.Close();
            }
            catch (Exception ex)
            {
                Globals.FixConnection(cn);
                Globals.ErrorSign(mw, ex);
            }
        }
    }
}
