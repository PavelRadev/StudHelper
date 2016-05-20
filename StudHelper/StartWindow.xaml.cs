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
using MahApps.Metro.Controls.Dialogs;
using System.Data;

namespace StudHelper
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : MetroWindow
    {
        string rightgroup, customSettingsFile;

        public StartWindow()
        {
            InitializeComponent();
            Globals.CheckConnection(this, Globals.cn);
            Globals.Groups.Clear();
            Globals.GroupsAdapter.Fill(Globals.Groups, "Groups");
            GroupCB.ItemsSource = Globals.Groups.Tables["Groups"].DefaultView;
            GroupCB.DisplayMemberPath = "name";
            GroupCB.SelectedValuePath = "id";
            Settings.ExecFile(this, Settings.DefaultFileName);
            this.FillTextBoxes();
            SaveInFile.IsEnabled = false;
        }

        #region Вкладка входа

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Globals.DefaultfieldColor(sender);
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Globals.DefaultfieldColor(sender);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.cn.Open();

                //Запрос на выборку элементов соответствующих введенному логину и паролю
                SqlCommand cmd1 = new SqlCommand("Select name from " + Settings.users_table + " where login='" 
                    + LoginTextBox.Text + "' and pass='" + PasswordTextBox.Password + "'", Globals.cn);
                SqlDataReader dr1;
                dr1 = cmd1.ExecuteReader();

                int count = 0;      //счетчик количества записей

                //Проверка наличия записей после выполнения запроса на выборку
                while (dr1.Read()) { count += 1; }
                dr1.Close();

                if (count == 1)
                {
                    //Закрытие формы входа, соединения с базой, и переход на основную форму
                    Globals._login = LoginTextBox.Text.ToString();
                    DateTime Today = DateTime.Now;
                    string _LastVisit = Today.ToString("yyyy-MM-dd HH:mm:ss");


                    SqlCommand LoginUpd = new SqlCommand("Update " + Settings.users_table + " set LastVisit = '" 
                        + _LastVisit + "' where login='" + Globals._login + "'", Globals.cn);

                    LoginUpd.ExecuteNonQuery();


                    SqlCommand getGroup = new SqlCommand("Select " + Settings.Rights_table + ".name from " 
                        + Settings.Rights_table + ", " + Settings.users_table + " where (" + Settings.users_table 
                        + ".rights=" + Settings.Rights_table + ".id) and " + Settings.users_table + ".login = '" 
                        + Globals._login + "'", Globals.cn);

                    Globals._rightGroup = (string)getGroup.ExecuteScalar();


                    SqlCommand getRights = new SqlCommand("Select su_rights from " + Settings.Rights_table + " where name = '" + Globals._rightGroup + "'", Globals.cn);
                    Globals._su = (bool)getRights.ExecuteScalar();
                    SqlCommand getId = new SqlCommand("Select id from " + Settings.users_table + " where login = '" + Globals._login + "'", Globals.cn);
                    Globals._userid = (int)getId.ExecuteScalar();

                    Globals.FixConnection(Globals.cn);

                    MainWindow mw1 = new MainWindow();
                    mw1.Show();
                    this.Close();
                }
                else if (count != 1)
                {
                    //Строка состояния, сигнализирующуя негативном результате проверки
                    Globals.WrongFieldColor(LoginTextBox);
                    Globals.WrongFieldColor(PasswordTextBox);
                }
                Globals.cn.Close();
            }
            catch(Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }
        #endregion

        #region Вкладка регистрации
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Создание подключения с базой на локальном сервере, и его открытие
                Globals.cn.Open();
                
                //Проверка кода доступа на наличие совпадений в базе
                SqlCommand check = new SqlCommand("Select name from " + Settings.Rights_table + " where pass='" + RegAccessTb.Text + "'", Globals.cn);
                SqlDataReader checkDr;
                checkDr = check.ExecuteReader();
                int count1 = 0;
                while (checkDr.Read()) { count1 += 1; }
                checkDr.Close();

                if (!String.IsNullOrWhiteSpace(RegAccessTb.Text))
                {
                    if (count1 == 1)
                    {
                        //Инструкция, выполняемая при условии заполненности текстового поля кода доступа, и наличия совпадений в базе данных
                        rightgroup = (string)check.ExecuteScalar();
                    }
                    else
                    {
                        Globals.WrongFieldColor(RegAccessTb);
                    }
                }
                else 
                {
                    Globals.WrongFieldColor(RegAccessTb);
                }

                //Проверка логина на наличие совпадений в базе
                SqlCommand checkLog = new SqlCommand("Select id from " + Settings.users_table + " where login='" + RegLoginTb.Text + "'", Globals.cn);
                SqlDataReader checkLogin;
                checkLogin = checkLog.ExecuteReader();
                int count2 = 0;
                while (checkLogin.Read()) { count2 += 1; }
                checkLogin.Close();
                if (!String.IsNullOrWhiteSpace(RegLoginTb.Text))
                {
                    if (count2 != 0)
                    {
                        Globals.WrongFieldColor(RegLoginTb);
                    }
                }
                else
                {
                    Globals.WrongFieldColor(RegLoginTb);
                }


                if (!String.IsNullOrWhiteSpace(RegLoginTb.Text) && !String.IsNullOrWhiteSpace(RegAccessTb.Text)
                    && !String.IsNullOrWhiteSpace(RegNameTb.Text) && !String.IsNullOrWhiteSpace(RegPassTb.Text))
                {
                    if (count1 == 1 && count2 == 0)
                    {
                        //Инструкция, выполняемая при заполнени всех текстовых полей, и соответствий кода группы и логина значениям из базы

                        //Переменная, хранящая id группы, выбранной в комбобоксе
                        int group = Convert.ToInt32(GroupCB.SelectedValue);

                        //Запрос на выборку id из таблицы rights, соответствующего введенному коду доступа
                        SqlCommand getRights = new SqlCommand("Select id from " + Settings.Rights_table + " where pass='" 
                            + RegAccessTb.Text + "'", Globals.cn);
                        int rights = (int)getRights.ExecuteScalar();

                        //Запрос на добавление полученных данных
                        DateTime date = DateTime.Now;
                        string _date = date.ToString("yyyy-MM-dd HH:mm:ss");
                        //MessageBox.Show(_date);
                        SqlCommand UserReg = new SqlCommand("Insert into " + Settings.users_table 
                            + "(name, rights, LastVisit, [group], login, pass) values('" + RegNameTb.Text + "', '" 
                            + rights + "', '" + _date + "', '" + group + "','" + RegLoginTb.Text + "', '" 
                            + RegPassTb.Text + "')", Globals.cn);
                        UserReg.ExecuteNonQuery();

                        //Информационное сообщение, сигнализирующее о успешном завершении регистрации
                        this.ShowMessageAsync("Статус регистрации:", "Успешно \n-Имя пользователя: " + RegNameTb.Text 
                            + "\n-Логин: "
                            +"" + RegLoginTb.Text + "\n-Группа: " + rightgroup + "");

                        //Приведение формы в исходный вид
                        Globals.ClearControl(RegAccessTb);
                        Globals.ClearControl(RegNameTb);
                        Globals.ClearControl(GroupCB);
                        Globals.ClearControl(StTb);
                        Globals.ClearControl(RegLoginTb);
                        Globals.ClearControl(RegPassTb);
                    }
                }
                else
                {
                    //Инстркуция, выполняемая при отсутствии данных в любом из полей
                    this.ShowMessageAsync("Ошибка", "Одно, или несколько полей формы не заполнены, регистрация "
                        +"невозможна");
                }
                Globals.cn.Close();
            }
            catch(Exception ex)
            {
                Globals.ErrorSign(this, ex);
                Globals.FixConnection(Globals.cn);
            }
        }

        private void GroupCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Globals.cn.Open();
                SqlCommand com = new SqlCommand("Select institution from " + Settings.Groups_table + " where id='" 
                    + GroupCB.SelectedValue + "'", Globals.cn);
                StTb.Text = (string)com.ExecuteScalar();
                Globals.cn.Close();
            }
            catch
            {
            }
        }

        private void RegAccessTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            Globals.DefaultfieldColor(sender);
        }

        private void RegLoginTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            Globals.DefaultfieldColor(sender);
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Создание подключения с базой на локальном сермере, и его открытие
                Globals.cn.Open();
                
                //Проверка кода доступа на наличие совпадений в базе
                SqlCommand check = new SqlCommand("Select name from " + Settings.Rights_table + " where pass='" + RegAccessTb.Text + "'", Globals.cn);
                SqlDataReader checkDr;
                checkDr = check.ExecuteReader();
                int count1 = 0;
                while (checkDr.Read()) { count1 += 1; }
                checkDr.Close();

                if (!String.IsNullOrWhiteSpace(RegAccessTb.Text))
                {
                    if (count1 == 1)
                    {
                        //Инструкция, выполняемая при условии заполненности текстового поля кода доступа, и наличия совпадений в базе данных
                        rightgroup = (string)check.ExecuteScalar();
                    }
                    else
                    {
                        Globals.WrongFieldColor(RegAccessTb);
                    }
                }
                else 
                {
                    Globals.WrongFieldColor(RegAccessTb);
                }

                //Проверка логина на наличие совпадений в базе
                SqlCommand checkLog = new SqlCommand("Select id from " + Settings.users_table + " where login='" + RegLoginTb.Text + "'", Globals.cn);
                SqlDataReader checkLogin;
                checkLogin = checkLog.ExecuteReader();
                int count2 = 0;
                while (checkLogin.Read()) { count2 += 1; }
                checkLogin.Close();
                if (!String.IsNullOrWhiteSpace(RegLoginTb.Text))
                {
                    if (count2 != 0)
                    {
                        Globals.WrongFieldColor(RegLoginTb);
                    }
                }
                else
                {
                    Globals.WrongFieldColor(RegLoginTb);
                }


                if (!String.IsNullOrWhiteSpace(RegLoginTb.Text) && !String.IsNullOrWhiteSpace(RegAccessTb.Text)
                    && !String.IsNullOrWhiteSpace(RegNameTb.Text) && !String.IsNullOrWhiteSpace(RegPassTb.Text))
                {
                    if (count1 == 1 && count2 == 0)
                    {
                        this.ShowMessageAsync("Информация","Ввведенные данные соответствуют требованиям. Регистрация может быть продолжена.");
                    }
                }
                else
                {
                    //Инстркуция, выполняемая при отсутствии данных в любом из полей
                    this.ShowMessageAsync("Ошибка", "Одно, или несколько полей формы не заполнены, регистрация "
                        +"невозможна");
                }
                Globals.cn.Close();
            }
            catch(Exception ex)
            {
                Globals.ErrorSign(this, ex);
                Globals.FixConnection(Globals.cn);
            }
        }
        #endregion

        private void RightsInfoButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.RightsInfo(this);
        }

        private void CheckFields_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDatabase()==true)
            {
                this.ShowMessageAsync("Success", "Соединение установлено, структура базы соответствует требованиям");
            }
        }
        private bool CheckDatabase()
        {
            bool connection = false;
            try
            {
                SqlConnection check = new SqlConnection(ConnectionStringTB.Password);
                if (Globals.CheckConnection(this, check) == true)
                {
                    connection = true;
                }
                else
                {
                    connection = false;
                }
                Globals.FixConnection(check);
                int i;
                check.Open();
                SqlCommand cmd1 = new SqlCommand("Select id, name, rights, LastVisit, [group], login, pass, current_course from " + Users_table_TB.Text, check);
                i = (int)cmd1.ExecuteScalar();
                SqlCommand cmd2 = new SqlCommand("Select id, DateTime, [User], Change from " + events_table_TB.Text, check);
                i = (int)cmd2.ExecuteScalar();
                SqlCommand cmd3 = new SqlCommand("Select id, name, Institution from " + Groups_table_TB.Text, check);
                i = (int)cmd3.ExecuteScalar();
                SqlCommand cmd4 = new SqlCommand("Select id, name, pass, su_rights from " + rights_table_TB.Text, check);
                i = (int)cmd4.ExecuteScalar();
                SqlCommand cmd5 = new SqlCommand("Select id, name, compiler from " + Courses_table_TB.Text, check);
                i = (int)cmd5.ExecuteScalar();
                SqlCommand cmd6 = new SqlCommand("Select id, title, read_link, edit_link, course_id from " + Lessons_table_TB.Text, check);
                i = (int)cmd6.ExecuteScalar();
                SqlCommand cmd7 = new SqlCommand("Select id, [User], curr_less, LastVisit, course_id from " + Lessons_users_table_TB.Text, check);
                i = (int)cmd7.ExecuteScalar();
                SqlCommand cmd8 = new SqlCommand("Select id, definition, description, course from " + Dictionary_table_TB.Text, check);
                i = (int)cmd8.ExecuteScalar();
                SqlCommand cmd9 = new SqlCommand("Select id, user_id, definition_id from "+dictionary_fav_table_TB.Text, check);
                i = (int)cmd9.ExecuteScalar();
                check.Close();
                return true;
            }
            catch
            {
                if (connection == true)
                {
                    this.ShowMessageAsync("Error","Соединение с базой установлено, но её структура не соответствует требованиям программы");
                }
                else if (connection == false)
                {
                    this.ShowMessageAsync("Error", "Соединение с базой не было установлено");
                }
                return false;
            }
        }

        private void SetGlobals_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDatabase() == true)
            {
                Settings.ConnectionString = ConnectionStringTB.Password;
                Settings.users_table = Users_table_TB.Text;
                Settings.Events_table = events_table_TB.Text;
                Settings.Groups_table = Groups_table_TB.Text;
                Settings.Rights_table = rights_table_TB.Text;
                Settings.Courses_table = Courses_table_TB.Text;
                Settings.lessons_table = Lessons_table_TB.Text;
                Settings.course_users_table = Lessons_users_table_TB.Text;
                Settings.dictionary_table = Dictionary_table_TB.Text;
                Settings.dictionary_fav_table = dictionary_fav_table_TB.Text;
                Globals.cn = new SqlConnection(Settings.ConnectionString);
                if (DefaultSettings.IsChecked == true)
                {
                    Settings.ReWriteSettingsFile(this, Settings.DefaultFileName);
                }
                if (SaveInFile.IsChecked == true)
                {
                    Settings.ReWriteSettingsFile(this, customSettingsFile);
                }
                this.ShowMessageAsync("Success","Изменения были применены");
            }
        }

        private void DefaultGlobals_Click(object sender, RoutedEventArgs e)
        {
            Globals.Default_Settings();
            this.FillTextBoxes();
            SaveInFile.IsChecked = false;
            SaveInFile.IsEnabled = false;
        }
        private void FillTextBoxes()
        {
            ConnectionStringTB.Password = Settings.ConnectionString;
            Users_table_TB.Text = Settings.users_table;
            events_table_TB.Text = Settings.Events_table;
            Groups_table_TB.Text = Settings.Groups_table;
            rights_table_TB.Text = Settings.Rights_table;
            Courses_table_TB.Text = Settings.Courses_table;
            Lessons_table_TB.Text = Settings.lessons_table;
            Lessons_users_table_TB.Text = Settings.course_users_table;
            Dictionary_table_TB.Text = Settings.dictionary_table;
            dictionary_fav_table_TB.Text = Settings.dictionary_fav_table;
        }

        private void LoadSettings_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".sh";
            dlg.Filter = "StudHelper configuration files (*.shcfg)|*.shcfg";
            Nullable<bool> result = dlg.ShowDialog();
            if(result == true)
            {
                customSettingsFile = System.IO.Path.GetFileName(dlg.FileName);
                Settings.ExecFile(this, System.IO.Path.GetFileName(dlg.FileName));
                SaveInFile.IsEnabled = true;
            }
            this.FillTextBoxes();
        }
    }
}
