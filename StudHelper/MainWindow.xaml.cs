using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using System.Data.SqlClient;
using MahApps.Metro.Controls.Dialogs;
using System.Data;
using Awesomium.Core;

namespace StudHelper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : MetroWindow
    {
        public WorkingMode CurrentMode = WorkingMode.Meeting;

        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeMode(WorkingMode.Meeting, MeetingModeButton);

            Globals.CoursesStart.Clear();
            Globals.CoursesStartAdapter.Fill(Globals.CoursesStart, Settings.Courses_table);
            CoursesCB.ItemsSource = Globals.CoursesStart.Tables[Settings.Courses_table].DefaultView;
            CoursesCB.DisplayMemberPath = "name";
            CoursesCB.SelectedValuePath = "id";

            FillElements();

            MainBrowser.Source = "https://onedrive.live.com/redir?resid=2886097FC058C8D2!780&authkey=!ABd_tNLjz5E7xxo&ithint=file%2cdocx".ToUri();
        }

        public void ChangeMode(WorkingMode wm, object btn)
        {
            if (wm == WorkingMode.Meeting)
            {
                MeetingGrid.Visibility = System.Windows.Visibility.Visible;
                ViewGrid.Visibility = System.Windows.Visibility.Hidden;
                CourseChangeGrid.Visibility = System.Windows.Visibility.Hidden;
                DatabaseEditGrid.Visibility = System.Windows.Visibility.Hidden;
                DictionaryGrid.Visibility = System.Windows.Visibility.Hidden;
                UserMenu.IsOpen = false;
                Globals.DefaultTopButtonsColor(ViewModeButton, MainTablesModeButton, DictionaryTablesModeButton, CourseTablesModeButton);
            }
            else if (wm == WorkingMode.View)
            {
                Globals.DefaultTopButtonsColor(CourseTablesModeButton, MainTablesModeButton, DictionaryTablesModeButton, MeetingModeButton);
                ViewGrid.Visibility = System.Windows.Visibility.Visible;
                MeetingGrid.Visibility = System.Windows.Visibility.Hidden;
                CourseChangeGrid.Visibility = System.Windows.Visibility.Hidden;
                DatabaseEditGrid.Visibility = System.Windows.Visibility.Hidden;
                DictionaryGrid.Visibility = System.Windows.Visibility.Hidden;
                UserMenu.IsOpen = false;
            }
            else if (wm == WorkingMode.CourseTables)
            {
                Globals.DefaultTopButtonsColor(ViewModeButton, MainTablesModeButton, DictionaryTablesModeButton, MeetingModeButton);
                ViewGrid.Visibility = System.Windows.Visibility.Hidden;
                MeetingGrid.Visibility = System.Windows.Visibility.Hidden;
                CourseChangeGrid.Visibility = System.Windows.Visibility.Visible;
                DatabaseEditGrid.Visibility = System.Windows.Visibility.Hidden;
                DictionaryGrid.Visibility = System.Windows.Visibility.Hidden;
                UserMenu.IsOpen = true;
            }
            else if (wm == WorkingMode.MainTables)
            {
                Globals.DefaultTopButtonsColor(ViewModeButton, CourseTablesModeButton, DictionaryTablesModeButton, MeetingModeButton);
                ViewGrid.Visibility = System.Windows.Visibility.Hidden;
                MeetingGrid.Visibility = System.Windows.Visibility.Hidden;
                CourseChangeGrid.Visibility = System.Windows.Visibility.Hidden;
                DictionaryGrid.Visibility = System.Windows.Visibility.Hidden;
                DatabaseEditGrid.Visibility = System.Windows.Visibility.Visible;
                UserMenu.IsOpen = true;
            }
            else if (wm == WorkingMode.Dictionary)
            {
                Globals.DefaultTopButtonsColor(ViewModeButton, CourseTablesModeButton, MainTablesModeButton, MeetingModeButton);
                ViewGrid.Visibility = System.Windows.Visibility.Hidden;
                MeetingGrid.Visibility = System.Windows.Visibility.Hidden;
                CourseChangeGrid.Visibility = System.Windows.Visibility.Hidden;
                DatabaseEditGrid.Visibility = System.Windows.Visibility.Hidden;
                DictionaryGrid.Visibility = System.Windows.Visibility.Visible;
                UserMenu.IsOpen = true;
            }
            ((Control)btn).Background = new SolidColorBrush(Color.FromArgb(255, 43, 87, 154));
            ((Control)btn).Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            CurrentMode = wm;
        }

        private void ViewModeButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.CheckConnection(this, Globals.cn);
            ChangeMode(WorkingMode.View, sender);
        }

        private void CourseTablesModeButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.CheckConnection(this, Globals.cn);
            ChangeMode(WorkingMode.CourseTables, sender);
        }

        private void MeetingModeButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.CheckConnection(this, Globals.cn);
            ChangeMode(WorkingMode.Meeting, sender);
        }

        private void MainTablesModeButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.CheckConnection(this, Globals.cn);
            ChangeMode(WorkingMode.MainTables, sender);
        }
       
        private void CourseChooseButton_Click(object sender, RoutedEventArgs e)
        {
            FillAllFields();
            ChangeMode(WorkingMode.View, ViewModeButton);
        }

        private void DictionaryTablesModeButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.CheckConnection(this, Globals.cn);
            ChangeMode(WorkingMode.Dictionary, sender);
        }

        private void FillElements()
        {
            try
            {
                usernameBlock.Content = "Вы вошли, как " + Globals._login;
                MeetNameLabel.Content = "Здравствуйте, " + Globals._login;

                Globals.CoursesStart.Clear();
                Globals.CoursesStartAdapter.Fill(Globals.CoursesStart, Settings.Courses_table);
                CoursesCB.Items.Refresh();


                CabLogin.Text = Globals._login;

                Globals.FixConnection(Globals.cn);
                Globals.CheckConnection(this, Globals.cn);

                Globals.cn.Open();


                SqlCommand getname = new SqlCommand("Select name from " + Settings.users_table + " where login = '" 
                    + Globals._login + "'", Globals.cn);
                Globals._name = (string)getname.ExecuteScalar();
                CabName.Text = Globals._name;



                SqlCommand getgroup = new SqlCommand("Select " + Settings.Groups_table + ".name from "
                    + Settings.Groups_table + ", " + Settings.users_table + " where (" + Settings.users_table + ".login = '"
                    + Globals._login + "') and (" + Settings.users_table + ".[group] = " + Settings.Groups_table 
                    + ".id)", Globals.cn);

                Globals._group = (string)getgroup.ExecuteScalar();
                CabGroup.Text = Globals._group;



                SqlCommand getRightsName = new SqlCommand("Select " + Settings.Rights_table + ".name from " + Settings.Rights_table
                    + ", " + Settings.users_table + " where (" + Settings.users_table + ".login = '" + Globals._login
                    + "') and (" + Settings.users_table + ".rights = " + Settings.Rights_table + ".id)", Globals.cn);

                Globals._rightGroup = (string)getRightsName.ExecuteScalar();
                CabRights.Text = Globals._rightGroup;



                SqlCommand getRights = new SqlCommand("Select Rights from " + Settings.users_table + " where login = '" 
                    + Globals._login + "'", Globals.cn);
                Globals._rights = (int)getRights.ExecuteScalar();



                SqlCommand getUserId = new SqlCommand("Select id from " + Settings.users_table + " where login = '"
                    + Globals._login + "'", Globals.cn);
                Globals._userid = (int)getUserId.ExecuteScalar();


                if (Globals._rights == 3)
                {
                    MainTablesModeButton.IsEnabled = true;

                    //Добавление в лог новой записи, сигнализирующей о входе пользователя а админ-панель

                    //DateTime datetime = DateTime.Now;
                    //string _DateTime = datetime.ToString("g");

                    string _DateTime = DateTime.Now.ToString("g");
                    SqlCommand update = new SqlCommand("Insert into " + Settings.Events_table + "(DateTime, Change, [User]) values('" 
                        + _DateTime + "','" + Globals._login + " enter the admin panel','" + Globals._userid
                        + "')", Globals.cn);
                    update.ExecuteNonQuery();

                    //Заполнение таблицы "Пользователи"
                    Globals.Users.Clear();
                    Globals.UsersAdapter.Fill(Globals.Users, Settings.users_table);
                    UsersGrid.ItemsSource = Globals.Users.Tables[Settings.users_table].DefaultView;

                    //Заполнение таблицы "Права"
                    Globals.Rights.Clear();
                    Globals.RightsAdapter.Fill(Globals.Rights, Settings.Rights_table);
                    RightsGrid.ItemsSource = Globals.Rights.Tables[Settings.Rights_table].DefaultView;

                    //Заполнение таблицы "Группы"
                    Globals.Groups.Clear();
                    Globals.GroupsAdapter.Fill(Globals.Groups, Settings.Groups_table);
                    GroupsGrid.ItemsSource = Globals.Groups.Tables[Settings.Groups_table].DefaultView;

                    //Заполнение таблицы "Учебные курсы"
                    Globals.Courses.Clear();
                    Globals.CoursesAdapter.Fill(Globals.Courses, Settings.Courses_table);
                    CourcesGrid.ItemsSource = Globals.Courses.Tables[Settings.Courses_table].DefaultView;

                    //Заполнение таблицы "события"
                    Globals.Events.Clear();
                    Globals.EventsAdapter.Fill(Globals.Events, Settings.Events_table);
                    EventsGrid.ItemsSource = Globals.Events.Tables[Settings.Events_table].DefaultView;

                    //Заполнение таблицы "Словарь"
                    Globals.DictionaryGridDataSet.Clear();
                    Globals.DictionaryAdapter.Fill(Globals.DictionaryGridDataSet, Settings.dictionary_table);
                    DictionaryDataGrid.ItemsSource = Globals.DictionaryGridDataSet.Tables[Settings.dictionary_table].DefaultView;

                    //Заполнение комбобоксов
                    UsersRights.ItemsSource = Globals.Rights.Tables[Settings.Rights_table].DefaultView;
                    UsersRights.DisplayMemberPath = "name";
                    UsersRights.SelectedValuePath = "id";
                    UsersGroup.ItemsSource = Globals.Groups.Tables[Settings.Groups_table].DefaultView;
                    UsersGroup.DisplayMemberPath = "name";
                    UsersGroup.SelectedValuePath = "id";
                    UsersCourse.ItemsSource = Globals.CoursesStart.Tables[Settings.Courses_table].DefaultView;
                    UsersCourse.DisplayMemberPath = "name";
                    UsersCourse.SelectedValuePath = "id";
                    DictionaryCourse.ItemsSource = Globals.CoursesStart.Tables[Settings.Courses_table].DefaultView;
                    DictionaryCourse.DisplayMemberPath = "name";
                    DictionaryCourse.SelectedValuePath = "id";
                }
                else
                {
                    MainTablesModeButton.IsEnabled = false;
                }



                SqlCommand getInst = new SqlCommand("Select " + Settings.Groups_table + ".Institution from " + Settings.Groups_table
                    + ", " + Settings.users_table + " where (" + Settings.users_table + ".login = '" + Globals._login + "') and (" 
                    + Settings.users_table + ".[group] = " + Settings.Groups_table + ".id)", Globals.cn);

                Globals._institution = (string)getInst.ExecuteScalar();
                CabUniversity.Text = Globals._institution;

                if (Globals._su == true)
                {
                    CourseTablesModeButton.IsEnabled = true;
                }
                else
                {
                    CourseTablesModeButton.IsEnabled = false;
                }

                statusBarLabel.Content = "Соединение установлено";
                Globals.cn.Close();
            }
            catch (Exception ex)
            {
                Globals.ErrorSign(this, ex);
                Globals.FixConnection(Globals.cn);
            }
        }
        private void FillAllFields()
        {
            try
            {
                Globals.cn.Open();

                Globals.current_course = CoursesCB.SelectedValue.ToString();

                SqlCommand getCourseName = new SqlCommand("Select name from "+Settings.Courses_table+"", Globals.cn);
                Globals.current_course_name = (string)getCourseName.ExecuteScalar();

                //Проверка наличия пользователя в соответствующей таблице выбранного ранее курса
                SqlCommand checkUser = new SqlCommand("Select [User] from " + Settings.course_users_table + " where [User] = '" 
                    + Globals._userid + "' and course_id = '" + Globals.current_course + "'", Globals.cn);
                SqlDataReader dr1 = checkUser.ExecuteReader();
                int count = 0;
                while (dr1.Read()) { count += 1; }
                dr1.Close();
                if (count == 1)
                {
                    DateTime Today = DateTime.Now;
                    string _LastVisit = Today.ToString("yyyy-MM-dd HH:mm:ss");
                    SqlCommand updUser = new SqlCommand("Update " + Settings.course_users_table + " set LastVisit='" 
                        + _LastVisit + "' where [User] = '" + Globals._userid + "' and course_id = '"+Globals.current_course+"'", 
                        Globals.cn);

                    updUser.ExecuteNonQuery();


                    SqlCommand updMainTable = new SqlCommand("Update "+Settings.users_table+" set current_course = '" 
                        + Globals.current_course + "' where id = '" + Globals._userid + "'", Globals.cn);
                    updMainTable.ExecuteNonQuery();
                }
                else
                {
                    string _LastVisit = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    SqlCommand updUser = new SqlCommand("Insert into " + Settings.course_users_table + "([User], curr_less, "
                        +"LastVisit, course_id) values('" + Globals._userid + "', 1, '" + _LastVisit + "', '" 
                        + Globals.current_course + "')", Globals.cn);

                    updUser.ExecuteNonQuery();



                    SqlCommand updMainTable = new SqlCommand("Update " + Settings.users_table + " set current_course = '" 
                        + Globals.current_course + "' where id = '" + Globals._userid + "'", Globals.cn);
                    updMainTable.ExecuteNonQuery();
                }

                //Заполнение списка уроков из базы выбранного курса
                DataSet mainDs = new DataSet();



                SqlCommand fillList = new SqlCommand("Select * from " + Settings.lessons_table + " where course_id='" 
                    + Globals.current_course + "'", Globals.cn);

                SqlDataAdapter fl = new SqlDataAdapter(fillList);
                fl.Fill(mainDs, Settings.lessons_table);
                lessList.ItemsSource = mainDs.Tables[Settings.lessons_table].DefaultView;
                lessList.DisplayMemberPath = "title";

                //Заполнение таблиц в панели администрирования курсов
                Users.Clear();
                Course.Clear();

                SqlCommand getUsers = new SqlCommand("Select * from " + Settings.course_users_table + " where course_id='"
                    + Globals.current_course + "'", Globals.cn);


                SqlCommand getCourse = new SqlCommand("Select * from " + Settings.lessons_table + " where course_id='"
                    + Globals.current_course + "'", Globals.cn);

                SqlDataAdapter da1 = new SqlDataAdapter(getUsers);
                SqlDataAdapter da2 = new SqlDataAdapter(getCourse);
                da1.Fill(Users, Settings.course_users_table);
                da2.Fill(Course, Settings.lessons_table);
                UsersTable.ItemsSource = Users.Tables[Settings.course_users_table].DefaultView;
                CoursesTable.ItemsSource = Course.Tables[Settings.lessons_table].DefaultView;

                //Заполнение элементов в панели администрирвоания курса
                LoginTB.ItemsSource = Globals.Users.Tables[Settings.users_table].DefaultView;
                LoginTB.DisplayMemberPath = "name";
                LoginTB.SelectedValuePath = "id";

                LectionsCB.ItemsSource = mainDs.Tables[Settings.lessons_table].DefaultView;
                LectionsCB.DisplayMemberPath = "title";
                LectionsCB.SelectedValuePath = "id";

                //Изменение набора ссылок, в зависимости от группы пользователя
                if (Globals._su == false)
                {
                    lessList.SelectedValuePath = "read_link";
                }
                else
                {
                    lessList.SelectedValuePath = "edit_link";
                }
                //Заполнение переменной, хранящей номер последнего пройденного задания
                SqlCommand getCurrLess = new SqlCommand("Select curr_less from " + Settings.course_users_table + " "
                        + "where [User] = '" + Globals._userid + "' and course_id='" + Globals.current_course 
                        + "'", Globals.cn);
                Globals._lastPassed = (int)getCurrLess.ExecuteScalar();
                ProgressLabel.Content = Globals._lastPassed + "/" + lessList.Items.Count;
                Globals._currentLesson = -1;

                //Заполнение переменной, использующейся в виде ссылки на онлайн компилятор
                SqlCommand getCompiler = new SqlCommand("Select compiler from Courses where id = "
                    +"'" + Globals.current_course + "'", Globals.cn);
                Globals._compilerUrl = (string)getCompiler.ExecuteScalar();

                //Заполнение вкладки словаря

                UpdateDictionaryMainList();

                UpdateDictionaryFavList();

                Globals.cn.Close();
            }
            catch(Exception ex)
            {
                Globals.ErrorSign(this, ex);
                Globals.FixConnection(Globals.cn);
            }
        }

        #region Вкладка просмотра
        private void FinTS_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Globals._lastPassed < Globals._currentLesson)
                {
                    //Изменение данных текущего урока в таблице пользователей выбранного курса
                    Globals._lastPassed = lessList.SelectedIndex + 1;
                    SqlCommand updCurr = new SqlCommand("Update " + Settings.course_users_table + " set curr_less = "
                        + "'" + Globals._lastPassed + "' where [User] = '" + Globals._userid + "' and course_id='" 
                        + Globals.current_course + "'", Globals.cn);
                    Globals.cn.Open();
                    updCurr.ExecuteNonQuery();
                    Globals.cn.Close();
                    statusBarLabel.Content = "Действие выполнено. Последний пройденный: " + Globals._lastPassed;
                    ProgressLabel.Content = Globals._lastPassed + "/" + lessList.Items.Count;
                }
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void FinTS_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Globals._lastPassed >= Globals._currentLesson && Globals._currentLesson > 1)
            {
                try
                {
                    //Изменение данных текущего урока в таблице пользователей выбранного курса
                    Globals._lastPassed = lessList.SelectedIndex;
                    SqlCommand updCurr = new SqlCommand("Update " + Settings.course_users_table + " set curr_less = "
                        + "'" + Globals._lastPassed + "' where [User] = '" + Globals._userid + "' and course_id='"
                        + Globals.current_course + "'", Globals.cn);
                    Globals.cn.Open();
                    updCurr.ExecuteNonQuery();
                    Globals.cn.Close();
                    statusBarLabel.Content = "Действие выполнено. Последний пройденный: " + Globals._lastPassed;
                    ProgressLabel.Content = Globals._lastPassed + "/" + lessList.Items.Count;
                }
                catch (Exception ex)
                {
                    MainBrowser.Visibility = System.Windows.Visibility.Hidden;
                    this.ShowMessageAsync("Ошибка", "Извините, при выполнении действия возникла ошибка \nОшибка:\n" + ex);
                }
            }
            else
            {
                Globals._currentLesson = 1;
                statusBarLabel.Content = "Данное действие невозможно";
            }
        }

        private void lessList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Globals._currentLesson = lessList.SelectedIndex + 1;
                //Изменение состояния переключателя, сигнализирующего о пройденности урока
                if (Globals._lastPassed < Globals._currentLesson)
                {
                    FinTS.IsChecked = false;
                }
                else
                {
                    FinTS.IsChecked = true;
                }
                if (lessList.SelectedIndex != -1)
                {
                    Uri open = new Uri((string)lessList.SelectedValue);
                    MainBrowser.Source = open;
                    statusBarLabel.Content = "Урок доступен, загрузка начата";
                }
                else
                {
                    MainBrowser.Source = "https://onedrive.live.com/redir?resid=2886097FC058C8D2!780&authkey=!ABd_tNLjz5E7xxo&ithint=file%2cdocx".ToUri();
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка", "Извините, при выполнении действия возникла ошибка \nОшибка:\n" + ex);
            }
        }

        private void CompilerBtn_Click(object sender, RoutedEventArgs e)
        {
            Uri compilerUri = new Uri(Globals._compilerUrl);
            MainBrowser.Source = compilerUri;
        }

        private void HomePageButton_Click(object sender, RoutedEventArgs e)
        {
            MainBrowser.Source = "https://onedrive.live.com/redir?resid=2886097FC058C8D2!780&authkey=!ABd_tNLjz5E7xxo&ithint=file%2cdocx".ToUri();
        }

        private void ChangeCourse_Click(object sender, RoutedEventArgs e)
        {
            Globals.CheckConnection(this, Globals.cn);
            ChangeMode(WorkingMode.Meeting, MeetingModeButton);
        }

        private void usernameBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            //Анимация наведения кнопки входа в личный кабинет
            usernameBlock.Background = new SolidColorBrush(Color.FromArgb(255, 43, 87, 154));
        }

        private void usernameBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            //Анимация наведения кнопки входа в личный кабинет
            usernameBlock.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
        }

        private void usernameBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserMenu.IsOpen = true;
        }

        private void UpdateFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            FillAllFields();
            FillElements();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangeUserButton_Click(object sender, RoutedEventArgs e)
        {
            StartWindow sw = new StartWindow();
            sw.Show();
            this.Close();
        }
        #endregion

        private void UserMenu_ClosingFinished(object sender, RoutedEventArgs e)
        {
            ChangeMode(WorkingMode.View, ViewModeButton);
        }

        private void RightsInfoButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.RightsInfo(this);
        }

        #region Вкладка редактирования курса

        DataSet Course = new DataSet();
        DataSet Users = new DataSet();

        private void UsersRadio_Checked(object sender, RoutedEventArgs e)
        {
            AddStudentGrid.Visibility = System.Windows.Visibility.Visible;
            LessonAddBtn.Visibility = System.Windows.Visibility.Hidden;
        }

        private void LectionsRadio_Checked(object sender, RoutedEventArgs e)
        {
            AddStudentGrid.Visibility = System.Windows.Visibility.Hidden;
            LessonAddBtn.Visibility = System.Windows.Visibility.Visible;
        }

        private void CoursesTable_CurrentCellChanged(object sender, EventArgs e)
        {
            //Сохранение изменений таблицы при каждом изменении текущей ячейки таблицы
            //Создает дополнительную нагрузку, но исключает ситуацию, когда большое количество введенной информации не может быть 
            //сохранено из за незначительной ошибки в одной ячейке
            try
            {

                SqlCommand getCourse = new SqlCommand("Select * from " + Settings.lessons_table + " where course_id='"
                        + Globals.current_course + "'", Globals.cn);
                Globals.cn.Open();
                SqlDataAdapter da2 = new SqlDataAdapter(getCourse);
                SqlCommandBuilder builder = new SqlCommandBuilder(da2);
                da2.UpdateCommand = builder.GetUpdateCommand();
                da2.DeleteCommand = builder.GetDeleteCommand();
                da2.InsertCommand = builder.GetInsertCommand();
                da2.Update(Course.Tables[Settings.lessons_table]);
                Globals.cn.Close();
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void UsersTable_CurrentCellChanged(object sender, EventArgs e)
        {
            //Сохранение изменений таблицы при каждом изменении текущей ячейки таблицы
            //Создает дополнительную нагрузку, но исключает ситуацию, когда большое количество введенной информации не может быть 
            //сохранено из за незначительной ошибки в одной ячейке
            try
            {
                SqlCommand getUsers = new SqlCommand("Select * from " + Settings.course_users_table + " where course_id='"
                        + Globals.current_course + "'", Globals.cn);
                Globals.cn.Open();
                SqlDataAdapter da1 = new SqlDataAdapter(getUsers);
                SqlCommandBuilder builder = new SqlCommandBuilder(da1);
                da1.UpdateCommand = builder.GetUpdateCommand();
                da1.DeleteCommand = builder.GetDeleteCommand();
                da1.InsertCommand = builder.GetInsertCommand();
                da1.Update(Users.Tables[Settings.course_users_table]);
                Globals.cn.Close();
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Globals.ClearControl(LoginTB);
            Globals.ClearControl(LectionsCB);
            Globals.ClearControl(datePicker);
            Globals.ClearControl(TitleTB);
            Globals.ClearControl(readTB);
            Globals.ClearControl(editTB);
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            //Попытка добавления информации при нажатии на соответствующую кнопку
            try
            {
                if (UsersRadio.IsChecked == true && LectionsRadio.IsChecked == false)
                {
                    Globals.cn.Open();
                    DateTime date = datePicker.SelectedDate.Value;
                    string _selectedDate = date.ToString("yyyy-MM-dd");
                    SqlCommand addCmd = new SqlCommand("Insert into " + Settings.course_users_table + "([User], curr_less, LastVisit, course_id) values('" 
                        + LoginTB.SelectedValue + "','" + LectionsCB.SelectedValue + "','" + _selectedDate + "', '"+Globals.current_course+"')", Globals.cn);
                    addCmd.ExecuteNonQuery();
                    Globals.cn.Close();
                }
                else if (UsersRadio.IsChecked == false && LectionsRadio.IsChecked == true)
                {
                    Globals.cn.Open();
                    SqlCommand addCmd = new SqlCommand("Insert into " + Settings.lessons_table + "(title, read_link, edit_link, course_id) values('" 
                        + TitleTB.Text + "','" + readTB.Text + "','" + editTB.Text + "', '"+Globals.current_course+"')", Globals.cn);
                    addCmd.ExecuteNonQuery();
                    Globals.cn.Close();
                }
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }
        #endregion

        private void UsersGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            Globals.ApplyBaseChanges(Globals.cn, Globals.UsersAdapter, Globals.Users, Settings.users_table, this);
        }

        private void RightsGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            Globals.ApplyBaseChanges(Globals.cn, Globals.RightsAdapter, Globals.Rights, Settings.Rights_table, this);
        }

        private void GroupsGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            Globals.ApplyBaseChanges(Globals.cn, Globals.GroupsAdapter, Globals.Groups, Settings.Groups_table, this);
        }

        private void CourcesGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            Globals.ApplyBaseChanges(Globals.cn, Globals.CoursesAdapter, Globals.Courses, Settings.Courses_table, this);
        }

        private void EditRB_Checked(object sender, RoutedEventArgs e)
        {
            //Изменение таблицы "пользователи", и приведение её в режим редактирования
            Globals.Users.Clear();
            Globals.getUsers.CommandText = "Select * from users";
            Globals.UsersAdapter.Fill(Globals.Users, "Users");
            UsersGrid.ItemsSource = Globals.Users.Tables[Settings.users_table].DefaultView;
            UsersGrid.IsReadOnly = false;
        }

        private void ViewRB_Checked(object sender, RoutedEventArgs e)
        {
            //Изменение таблицы "пользователи", и приведение её в режим просмотра
            Globals.Users.Clear();
            Globals.getUsers.CommandText = "Select " + Settings.users_table + ".id, " + Settings.users_table + ".name, " 
                + Settings.Rights_table + ".name as Rights, " + Settings.users_table + ".LastVisit, " + Settings.Groups_table 
                + ".name as [Group], " + Settings.users_table + ".login, " + Settings.Courses_table + ".name as Course from " 
                + Settings.users_table + ", " + Settings.Rights_table + ", " + Settings.Groups_table + ", " + Settings.Courses_table 
                + " where (" + Settings.users_table + ".rights = " + Settings.Rights_table + ".id) and (" + Settings.users_table 
                + ".[group] = " + Settings.Groups_table + ".id) and (" + Settings.users_table + ".current_course = " 
                + Settings.Courses_table + ".id)";



            Globals.UsersAdapter.Fill(Globals.Users, Settings.users_table);
            UsersGrid.ItemsSource = Globals.Users.Tables[Settings.users_table].DefaultView;
            UsersGrid.IsReadOnly = true;
        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            //попытка добавления нового пользователя, при нажатии на соответствующую кнопку
            try
            {
                Globals.cn.Open();
                DateTime date = UsersDate.SelectedDate.Value;
                string _selectedDate = date.ToString("yyyy-MM-dd HH:mm:ss");
                SqlCommand newUserAdd = new SqlCommand("Insert into " + Settings.users_table + "(name, rights, LastVisit, "
                    +"[group], login, pass, current_course) values('" + UsersName.Text + "', " 
                    + UsersRights.SelectedValue + ",'" + _selectedDate + "', " + UsersGroup.SelectedValue 
                    + ", '" + Userslogin.Text + "','" + UsersPass.Text + "', " + UsersCourse.SelectedValue 
                    + ")", Globals.cn);
                newUserAdd.ExecuteNonQuery();
                DateTime datetime = DateTime.Now;
                string _DateTime = datetime.ToString("g");
                SqlCommand update = new SqlCommand("Insert into " + Settings.Events_table + "(DateTime, Change, [User]) values('" 
                    + _DateTime + "','New user was added with login: " + Userslogin.Text + "','" 
                    + Globals._userid + "')", Globals.cn);
                update.ExecuteNonQuery();
                ViewRB.IsChecked = true;
                Globals.cn.Close();

                //Приведение заполняемых полей к стандартному виду
                Globals.ClearControl(UsersName);
                Globals.ClearControl(Userslogin);
                Globals.ClearControl(UsersPass);
                Globals.ClearControl(UsersCourse);
                Globals.ClearControl(UsersRights);
                Globals.ClearControl(UsersGroup);
                Globals.ClearControl(UsersDate);
                this.ShowMessageAsync("Успешно", "Действие выполнено");
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void UsersClearBtn_Click(object sender, RoutedEventArgs e)
        {
            //очистка полей формы добавления нового пользователя
            Globals.ClearControl(UsersName);
            Globals.ClearControl(Userslogin);
            Globals.ClearControl(UsersPass);
            Globals.ClearControl(UsersCourse);
            Globals.ClearControl(UsersRights);
            Globals.ClearControl(UsersGroup);
            Globals.ClearControl(UsersDate);
        }

        private void RightsClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Globals.ClearControl(RightsName);
            Globals.ClearControl(RightsPass);
            Globals.ClearControl(RightsSU);
        }

        private void RightsAdd_Click(object sender, RoutedEventArgs e)
        {
            //Попытка добавления новой группы прав
            try
            {
                Globals.cn.Open();
                SqlCommand addRight = new SqlCommand("Insert into " + Settings.Rights_table + "(name, pass, su_rights) values('" 
                    + RightsName.Text + "','" + RightsPass.Text + "','" + RightsSU.IsChecked + "')", Globals.cn);
                addRight.ExecuteNonQuery();
                Globals.Rights.Clear();
                Globals.RightsAdapter.Fill(Globals.Rights, Settings.Rights_table);
                RightsGrid.ItemsSource = Globals.Rights.Tables[Settings.Rights_table].DefaultView;
                Globals.cn.Close();
                Globals.cn.Open();
                DateTime datetime = DateTime.Now;
                string _DateTime = datetime.ToString("g");
                SqlCommand update = new SqlCommand("Insert into " + Settings.Events_table + "(DateTime, Change, [User]) values('" 
                    + _DateTime + "','New rights group was added with name: " + RightsName.Text + "','" 
                    + Globals._userid + "')", Globals.cn);
                update.ExecuteNonQuery();

                //Очистка заполняемых полей
                Globals.cn.Close();
                Globals.ClearControl(RightsName);
                Globals.ClearControl(RightsPass);
                Globals.ClearControl(RightsSU);
                this.ShowMessageAsync("Успешно", "Действие выполнено");
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void GroupsClear_Click(object sender, RoutedEventArgs e)
        {
            Globals.ClearControl(GroupName);
            Globals.ClearControl(GroupInstitution);
        }

        private void GroupsAdd_Click(object sender, RoutedEventArgs e)
        {
            //попытка добавления новой учебной группы
            try
            {
                Globals.cn.Open();
                SqlCommand addGroup = new SqlCommand("Insert into " + Settings.Groups_table + "(name, institution) values('" 
                    + GroupName.Text + "','" + GroupInstitution.Text + "')", Globals.cn);
                addGroup.ExecuteNonQuery();
                Globals.Groups.Clear();
                Globals.GroupsAdapter.Fill(Globals.Groups, Settings.Groups_table);
                GroupsGrid.ItemsSource = Globals.Groups.Tables[Settings.Groups_table].DefaultView;
                DateTime datetime = DateTime.Now;
                string _DateTime = datetime.ToString("g");

                SqlCommand update = new SqlCommand("Insert into " + Settings.Events_table + "(DateTime, Change, [User]) values('" 
                    + _DateTime + "','New group was added with name: " + GroupName.Text + "','" + Globals._userid+"')", Globals.cn);
                
                update.ExecuteNonQuery();
                Globals.cn.Close();

                //Очистка заполняемых полей панели добавления группы
                Globals.ClearControl(GroupName);
                Globals.ClearControl(GroupInstitution);
                this.ShowMessageAsync("Успешно", "Действие выполнено");
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void CoursesClear_Click(object sender, RoutedEventArgs e)
        {
            //Очистка заполняемых полей панели добавления учебного курса
            Globals.ClearControl(CoursesName);
            Globals.ClearControl(CoursesTable);
            Globals.ClearControl(CoursesUsers);
            Globals.ClearControl(CoursesCompiler);
        }

        private void CoursesAdd_Click(object sender, RoutedEventArgs e)
        {
            //попытка добавления новго учебного курса
            try
            {
                Globals.cn.Open();
                SqlCommand addCourse = new SqlCommand("Insert into " + Settings.Courses_table + "(name, compiler) values('"
                    + CoursesName.Text + "','" + CoursesCompiler.Text + "')", Globals.cn);
                addCourse.ExecuteNonQuery();
                Globals.Courses.Clear();
                Globals.CoursesAdapter.Fill(Globals.Courses, Settings.Courses_table);
                CourcesGrid.ItemsSource = Globals.Courses.Tables[Settings.Courses_table].DefaultView;
                DateTime datetime = DateTime.Now;
                string _DateTime = datetime.ToString("g");
                SqlCommand update = new SqlCommand("Insert into " + Settings.Events_table + "(DateTime, Change, [User]) values('" + _DateTime
                    + "','New Course was added with name: " + CoursesName.Text + "','" + Globals._userid + "')", Globals.cn);
                update.ExecuteNonQuery();
                Globals.cn.Close();

                //Очистка заполняемых полей панели добавления учебного курса
                Globals.ClearControl(CoursesName);
                Globals.ClearControl(AdminCoursesTable);
                Globals.ClearControl(CoursesUsers);
                Globals.ClearControl(CoursesCompiler);
                this.ShowMessageAsync("Успешно", "Действие выполнено");
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            //Поиск, осуществляемый по любой таблице, в зависимости от выбранной вкладки
            try
            {
                if (UsersTab.IsSelected == true)
                {
                    EditRB.IsChecked = true;
                    if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Users.Clear();
                        Globals.getUsers.CommandText = "Select * from " + Settings.users_table + " where id like '%" + IdSearchTB.Text + "%'";
                        Globals.UsersAdapter.Fill(Globals.Users, Settings.users_table);
                        UsersGrid.ItemsSource = Globals.Users.Tables[Settings.users_table].DefaultView;
                    }
                    else if (String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Users.Clear();
                        Globals.getUsers.CommandText = "Select * from " + Settings.users_table + " where name like '%" + NameSearchTB.Text + "%'";
                        Globals.UsersAdapter.Fill(Globals.Users, Settings.users_table);
                        UsersGrid.ItemsSource = Globals.Users.Tables[Settings.users_table].DefaultView;
                    }
                    else if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Users.Clear();
                        Globals.getUsers.CommandText = "Select * from " + Settings.users_table + " where id like '%" + IdSearchTB.Text 
                            + "%' and name like '%" + NameSearchTB.Text + "%'";
                        Globals.UsersAdapter.Fill(Globals.Users, Settings.users_table);
                        UsersGrid.ItemsSource = Globals.Users.Tables[Settings.users_table].DefaultView;
                    }
                    else
                    {
                        Globals.Users.Clear();
                        Globals.getUsers.CommandText = "Select * from " + Settings.users_table;
                        Globals.UsersAdapter.Fill(Globals.Users, Settings.users_table);
                        UsersGrid.ItemsSource = Globals.Users.Tables[Settings.users_table].DefaultView;
                    }
                }
                if (RightsTab.IsSelected == true)
                {
                    if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Rights.Clear();
                        Globals.getRights.CommandText = "Select * from " + Settings.Rights_table + " where id like '%" + IdSearchTB.Text + "%'";
                        Globals.RightsAdapter.Fill(Globals.Rights, Settings.Rights_table);
                        RightsGrid.ItemsSource = Globals.Rights.Tables[Settings.Rights_table].DefaultView;
                    }
                    else if (String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Rights.Clear();
                        Globals.getRights.CommandText = "Select * from " + Settings.Rights_table + " where name like '%" + NameSearchTB.Text + "%'";
                        Globals.RightsAdapter.Fill(Globals.Rights, Settings.Rights_table);
                        RightsGrid.ItemsSource = Globals.Rights.Tables[Settings.Rights_table].DefaultView;
                    }
                    else if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Rights.Clear();
                        Globals.getRights.CommandText = "Select * from " + Settings.Rights_table + " where id like '%" + IdSearchTB.Text 
                            + "%' and name like '%" + NameSearchTB.Text + "%'";
                        Globals.RightsAdapter.Fill(Globals.Rights, Settings.Rights_table);
                        RightsGrid.ItemsSource = Globals.Rights.Tables[Settings.Rights_table].DefaultView;
                    }
                    else
                    {
                        Globals.Rights.Clear();
                        Globals.getRights.CommandText = "Select * from " + Settings.Rights_table;
                        Globals.RightsAdapter.Fill(Globals.Rights, Settings.Rights_table);
                        RightsGrid.ItemsSource = Globals.Rights.Tables[Settings.Rights_table].DefaultView;
                    }
                }
                if (GroupsTab.IsSelected == true)
                {
                    if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Groups.Clear();
                        Globals.getGroups.CommandText = "Select * from " + Settings.Groups_table + " where id like '%" + IdSearchTB.Text + "%'";
                        Globals.GroupsAdapter.Fill(Globals.Groups, Settings.Groups_table);
                        GroupsGrid.ItemsSource = Globals.Groups.Tables[Settings.Groups_table].DefaultView;
                    }
                    else if (String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Groups.Clear();
                        Globals.getGroups.CommandText = "Select * from " + Settings.Groups_table + " where name like '%" + NameSearchTB.Text + "%'";
                        Globals.GroupsAdapter.Fill(Globals.Groups, Settings.Groups_table);
                        GroupsGrid.ItemsSource = Globals.Groups.Tables[Settings.Groups_table].DefaultView;
                    }
                    else if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Groups.Clear();
                        Globals.getGroups.CommandText = "Select * from " + Settings.Groups_table + " where id like '%" + IdSearchTB.Text 
                            + "%' and name like '%" + NameSearchTB.Text + "%'";
                        Globals.GroupsAdapter.Fill(Globals.Groups, Settings.Groups_table);
                        GroupsGrid.ItemsSource = Globals.Groups.Tables[Settings.Groups_table].DefaultView;
                    }
                    else
                    {
                        Globals.Groups.Clear();
                        Globals.getGroups.CommandText = "Select * from " + Settings.Groups_table;
                        Globals.GroupsAdapter.Fill(Globals.Groups, Settings.Groups_table);
                        GroupsGrid.ItemsSource = Globals.Groups.Tables[Settings.Groups_table].DefaultView;
                    }
                }
                if (CoursesTab.IsSelected == true)
                {
                    if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Courses.Clear();
                        Globals.getCourses.CommandText = "Select * from " + Settings.Courses_table + " where id like '%" 
                            + IdSearchTB.Text + "%'";
                        Globals.CoursesAdapter.Fill(Globals.Courses, Settings.Courses_table);
                        CourcesGrid.ItemsSource = Globals.Courses.Tables[Settings.Courses_table].DefaultView;
                    }
                    else if (String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Courses.Clear();
                        Globals.getCourses.CommandText = "Select * from " + Settings.Courses_table + " where name like '%" + NameSearchTB.Text + "%'";
                        Globals.CoursesAdapter.Fill(Globals.Courses, Settings.Courses_table);
                        CourcesGrid.ItemsSource = Globals.Courses.Tables[Settings.Courses_table].DefaultView;
                    }
                    else if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Courses.Clear();
                        Globals.getCourses.CommandText = "Select * from " + Settings.Courses_table + " where id like '%" + IdSearchTB.Text 
                            + "%' and name like '%" + NameSearchTB.Text + "%'";
                        Globals.CoursesAdapter.Fill(Globals.Courses, Settings.Courses_table);
                        CourcesGrid.ItemsSource = Globals.Courses.Tables[Settings.Courses_table].DefaultView;
                    }
                    else
                    {
                        Globals.Courses.Clear();
                        Globals.getCourses.CommandText = "Select * from " + Settings.Courses_table;
                        Globals.CoursesAdapter.Fill(Globals.Courses, Settings.Courses_table);
                        CourcesGrid.ItemsSource = Globals.Courses.Tables[Settings.Courses_table].DefaultView;
                    }
                }
                if (EventsTab.IsSelected == true)
                {
                    if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Events.Clear();
                        Globals.getEvents.CommandText = "Select * from " + Settings.Events_table + " where id like '%" + IdSearchTB.Text + "%'";
                        Globals.EventsAdapter.Fill(Globals.Events, Settings.Events_table);
                        EventsGrid.ItemsSource = Globals.Events.Tables[Settings.Events_table].DefaultView;
                    }
                    else if (String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Events.Clear();
                        Globals.getEvents.CommandText = "Select * from " + Settings.Events_table + " where [User] like '%" + NameSearchTB.Text + "%'";
                        Globals.EventsAdapter.Fill(Globals.Events, Settings.Events_table);
                        EventsGrid.ItemsSource = Globals.Events.Tables[Settings.Events_table].DefaultView;
                    }
                    else if (!String.IsNullOrWhiteSpace(IdSearchTB.Text) && !String.IsNullOrWhiteSpace(NameSearchTB.Text))
                    {
                        Globals.Events.Clear();
                        Globals.getEvents.CommandText = "Select * from " + Settings.Events_table + " where (id like '%" + IdSearchTB.Text 
                            + "%') and ([User] like '%" + NameSearchTB.Text + "%')";
                        Globals.EventsAdapter.Fill(Globals.Events, Settings.Events_table);
                        EventsGrid.ItemsSource = Globals.Events.Tables[Settings.Events_table].DefaultView;
                    }
                    else
                    {
                        Globals.Events.Clear();
                        Globals.getEvents.CommandText = "Select * from " + Settings.Events_table;
                        Globals.EventsAdapter.Fill(Globals.Events, Settings.Events_table);
                        EventsGrid.ItemsSource = Globals.Events.Tables[Settings.Events_table].DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void SqlBtn_Click(object sender, RoutedEventArgs e)
        {
            //Попытка выполнение пользовательского SQL запроса, и выведение соответствующих сообщений в импровизированную консоль
            try
            {
                string command = SqlTB.Text;
                StatusLabel.Text += "\n Команда получена";
                SqlCommand sql = new SqlCommand(command, Globals.cn);
                Globals.cn.Open();
                StatusLabel.Text += "\n Соединение открыто";
                sql.ExecuteNonQuery();
                StatusLabel.Text += "\n Запрос выполнен";
                Globals.cn.Close();
                StatusLabel.Text += "\n Соединение закрыто";
                StatusLabel.Text += "\n Запрос выполнен";

                //При успешном выполнении запроса, введение данных о нем в лог
                Globals.cn.Open();
                DateTime datetime = DateTime.Now;
                string _DateTime = datetime.ToString("g");
                SqlCommand update = new SqlCommand("Insert into " + Settings.Events_table + "(DateTime, Change, [User]) values('" 
                    + _DateTime + "','User execute the SQL command','" + Globals._userid + "')", Globals.cn);
                update.ExecuteNonQuery();
                Globals.cn.Close();
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                StatusLabel.Text += "\n Выполнение прервано";
                StatusLabel.Text += "\n Ошибка: " + ex.Message;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Globals._rights == 3)
            {
                //Введение в лог записи, сигнализирующей о выходе пользователя
                Globals.cn.Open();
                DateTime datetime = DateTime.Now;
                string _DateTime = datetime.ToString("g");
                SqlCommand update = new SqlCommand("Insert into " + Settings.Events_table + "(DateTime, Change, [User]) values('" + _DateTime 
                    + "','" + Globals._login + " exit the admin panel','" + Globals._userid + "')", Globals.cn);
                update.ExecuteNonQuery();
                Globals.cn.Close();
            }
        }

        private void EventsTab_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Обновление таблицы с событиями каждый раз, когда выбирается вкладка содержащая данную таблицу
            Globals.EventsAdapter.Fill(Globals.Events, Settings.Events_table);
            EventsGrid.Items.Refresh();
            EventsGrid.ItemsSource = Globals.Events.Tables[Settings.Events_table].DefaultView;
        }

        private void DictionaryMainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SqlCommand getDescription = new SqlCommand("Select description from " + Settings.dictionary_table
                    + " where id = " + DictionaryMainList.SelectedValue, Globals.cn);
                Globals.cn.Open();
                DescriptionMain.Text = (string)getDescription.ExecuteScalar();
                Globals.cn.Close();
            }
            catch
            {
                Globals.FixConnection(Globals.cn);
            }
        }

        private void UpdateDictionaryFavList()
        {
            DataSet DictionaryFavDataSet = new DataSet();

            SqlCommand filldictionaryFav = new SqlCommand("Select " + Settings.dictionary_table + ".id, "
                + Settings.dictionary_table + ".definition from " + Settings.dictionary_table + ","
                + Settings.dictionary_fav_table + " where " + Settings.dictionary_table + ".id = "
                + Settings.dictionary_fav_table + ".definition_id and " + Settings.dictionary_table + ".course = "
                + Globals.current_course + " and " + Settings.dictionary_fav_table + ".user_id = " + Globals._userid
                + " order by " + Settings.dictionary_table + ".definition", Globals.cn);

            SqlDataAdapter D2 = new SqlDataAdapter(filldictionaryFav);
            D2.Fill(DictionaryFavDataSet, Settings.dictionary_table);
            DictionaryFavList.ItemsSource = DictionaryFavDataSet.Tables[Settings.dictionary_table].DefaultView;
            DictionaryFavList.DisplayMemberPath = "definition";
            DictionaryFavList.SelectedValuePath = "id";
            DescriptionFav.Text = "Выберите термин из списка избранных";
        }

        private void SaveToFav_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.FixConnection(Globals.cn);
                Globals.cn.Open();
                SqlCommand addToFav = new SqlCommand("Insert into " + Settings.dictionary_fav_table
                    + "(user_id, definition_id) values(" + Globals._userid + "," + DictionaryMainList.SelectedValue
                    + ")", Globals.cn);
                addToFav.ExecuteNonQuery();
                UpdateDictionaryFavList();
                Globals.cn.Close();
            }
            catch
            {
                Globals.FixConnection(Globals.cn);
            }
        }

        private void DictionaryFavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SqlCommand getDescription = new SqlCommand("Select description from " + Settings.dictionary_table
                    + " where id = " + DictionaryFavList.SelectedValue, Globals.cn);
                Globals.FixConnection(Globals.cn);
                Globals.cn.Open();
                DescriptionFav.Text = (string)getDescription.ExecuteScalar();
                Globals.cn.Close();
            }
            catch
            {
                Globals.FixConnection(Globals.cn);
            }
        }

        private void DeleteFromFav_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.FixConnection(Globals.cn);
                Globals.cn.Open();
                SqlCommand delFromFav = new SqlCommand("DELETE from " + Settings.dictionary_fav_table
                    + " where definition_id = "+DictionaryFavList.SelectedValue+" and user_id="+Globals._userid, Globals.cn);
                delFromFav.ExecuteNonQuery();
                UpdateDictionaryFavList();
                Globals.cn.Close();
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }
        private void UpdateDictionaryMainList()
        {
            DataSet DictionaryMainDataSet = new DataSet();

            SqlCommand filldictionary = new SqlCommand("Select * from " + Settings.dictionary_table + " where course='"
                + Globals.current_course + "' order by definition", Globals.cn);

            SqlDataAdapter D1 = new SqlDataAdapter(filldictionary);
            D1.Fill(DictionaryMainDataSet, Settings.dictionary_table);
            DictionaryMainList.ItemsSource = DictionaryMainDataSet.Tables[Settings.dictionary_table].DefaultView;
            DictionaryMainList.DisplayMemberPath = "definition";
            DictionaryMainList.SelectedValuePath = "id";

            DescriptionMain.Text = "Выберите термин из списка";
        }

        private void DescriptionCopyToBuffer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(DescriptionMain.Text);
        }

        private void DescriptionFavCopyToBuffer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(DescriptionFav.Text);
        }

        private void DictionaryAdd_Click(object sender, RoutedEventArgs e)
        {
            //попытка добавления нового термина в словарь
            try
            {
                Globals.cn.Open();
                SqlCommand addDefinition = new SqlCommand("insert into " + Settings.dictionary_table
                    + "(definition, description, course) values(N'" + DictionaryDefinition.Text + "',N'" 
                    + DescriptionTB.Text + "', "+ DictionaryCourse.SelectedValue +")", Globals.cn);
                addDefinition.ExecuteNonQuery();

                Globals.DictionaryGridDataSet.Clear();
                Globals.DictionaryAdapter.Fill(Globals.DictionaryGridDataSet, Settings.dictionary_table);
                DictionaryDataGrid.ItemsSource = Globals.DictionaryGridDataSet.Tables[Settings.dictionary_table].DefaultView;

                DateTime datetime = DateTime.Now;
                string _DateTime = datetime.ToString("g");
                SqlCommand update = new SqlCommand("Insert into " + Settings.Events_table + "(DateTime, Change, [User]) values('" + _DateTime
                    + "','New Definition was added with title: " + DictionaryDefinition.Text + "','" + Globals._userid + "')", Globals.cn);
                update.ExecuteNonQuery();
                Globals.cn.Close();

                //Очистка заполняемых полей панели добавления учебного курса
                Globals.ClearControl(DictionaryDefinition);
                Globals.ClearControl(DictionaryCourse);
                Globals.ClearControl(DescriptionTB);
                this.ShowMessageAsync("Успешно", "Действие выполнено");
            }
            catch (Exception ex)
            {
                Globals.FixConnection(Globals.cn);
                Globals.ErrorSign(this, ex);
            }
        }

        private void DictionaryDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            Globals.ApplyBaseChanges(Globals.cn, Globals.DictionaryAdapter, Globals.DictionaryGridDataSet, Settings.dictionary_table, this);
        }

    }
}
