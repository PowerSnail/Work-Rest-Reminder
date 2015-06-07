using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Reminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StreamWriter logFile = new StreamWriter("events.log", true);
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        List<Event> events = new List<Event>();
        List<string> categories = new List<string>();
        int currentID = 0;
        int runningID = 0;
        AlarmWindow aw;

        //Fields for editing purposes
        int edCat = 0;
        int edEve = 0;

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            aw = new AlarmWindow(this);
        }

        // =====================<Event>=====================

        public void addEvent(Event e, bool suppressEdit = false)
        {
            addCategoryIfExist(e.Category);

            int n = events.Count();
            events.Add(e);
            lbEventList.Items.Add(e);
            printWLog("New Event Added: " + e.ToString());
            btnStart.IsEnabled = true;

            if (!suppressEdit)
                editEvent(n);
        }

        public void editEvent(int c)
        {
            Event e = events[c];
            printWLog("Editing event: " + e.Name + '|' + e.Category);
            edEve = c;

            tbName.Text = e.Name;
            spEditEvent.Width = 200;
            cbCategory.SelectedIndex = categories.IndexOf(e.Category);
            tbMinute.Text = (e.ExpectedT / 60).ToString();
            
            tbName.SelectAll();
            tbName.Focus();
        }

        private void saveEvent()
        {
            if (cbCategory.SelectedIndex == categories.Count
                || cbCategory.SelectedIndex < 0 || tbName.Text == "" || tbMinute.Text == "")
                MessageBox.Show("More fields to fill!");
            else
            {
                addCategoryIfExist(events[edEve].Category);
                events[edEve].renew(tbName.Text,
                                    int.Parse(tbMinute.Text) * 60,
                                    categories[cbCategory.SelectedIndex]);
            }
        }

        private void removeEvent(int c)
        {
            events.RemoveAt(c);
            lbEventList.Items.RemoveAt(c);
        }

        private void exitSpEditEvent()
        {
            spEditEvent.Width = 0;
            this.btnStart.Focus();
        }

        internal void nextEvent()
        {
            if (events.Count <= ++currentID)
            {
                printWLog("No more task is available!");
                --currentID;
                btnStart.IsEnabled = false;
                pause();
            }
            else
            {
                printWLog("Start " + events[currentID].ToString() + "\n" + events[currentID].Words);
                start();
            }
        }

        internal Event getCurrentEvent()
        {
            return events[runningID];
        }

        // =====================</Event>=====================

        // =====================<Category>=====================

        private void addCategory(string cat, bool suppressEdit = true)
        {
            if (cat != null)
            {
                categories.Insert(0, cat);
                lbCategoryList.Items.Insert(0, cat);
                TextBlock tbC = new TextBlock();
                tbC.Text = cat;
                cbCategory.Items.Insert(0, cat);
                if (!suppressEdit)
                    editCategory(0);
            }
        }

        private void editCategory(int c)
        {
            spCategorySet.Width = 200;
            tbCatName.Text = categories[c];
            edCat = c;
            tbCatName.SelectAll();
            tbCatName.Focus();
        }

        private void saveCategory()
        {
            categories[edCat] = tbCatName.Text;
            lbCategoryList.Items[edCat] = categories[edCat];
            TextBlock tbC = new TextBlock();
            tbC.Text = tbCatName.Text;
            cbCategory.Items[edCat] = tbC;
        }

        private void exitSpCatSet()
        {
            spCategorySet.Width = 0;
            cbCategory.SelectedIndex = edCat;
            this.btnStart.Focus();
        }

        private void removeCategory()
        {
            if (MessageBox.Show("Do you really want to delete this category? ", "Warning!",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool existEvent = false;
                int i = -1;
                while (!existEvent && ++i < events.Count)
                {
                    if (events[i].Category == categories[edCat])
                        existEvent = true;
                }

                if (existEvent)
                    MessageBox.Show("Failed! related events exist!");
                else
                    categories.RemoveAt(edCat);
                cbCategory.Items.RemoveAt(edCat);
            }
        }

        private void addCategoryIfExist(string cat, bool suppressEdit = true)
        {
            if (!categories.Contains(cat))
                addCategory(cat, suppressEdit);
        }

        // =====================</Category>=====================

        // =====================<Control>=====================

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (events[runningID].tick())
                reportEnd();
        }

        private void reportEnd()
        {
            string str = "The current session has ended!";
            dispatcherTimer.Stop();
            if (events.Count() - currentID > 1)
                str += "\n" + events[currentID + 1].Words;
            aw.enter(str);
            printWLog(str);
        }

        internal void start()
        {
            printWLog("timer starts at " + DateTime.Now);
            dispatcherTimer.Start();
        }

        internal void pause()
        {
            printWLog("timer stops at " + DateTime.Now);
            dispatcherTimer.Stop();
        }

        internal void printWLog(string info)
        {
            string str = "\n" + info;
            tbStatus.Text = str + tbStatus.Text;

            logFile.Write(str);
            logFile.Flush();
        }

        // =====================</Control>=====================

        // =====================<UI Invoked>=====================

        // Quick Buttons
        private void btnAddSW_Click(object sender, RoutedEventArgs e)
        {
            addEvent(new Event("work", 25 * 60, "work"), true);
            addEvent(new Event("rest", 5 * 60, "rest"), true);
        }

        private void btnAddSR_Click(object sender, RoutedEventArgs e)
        {
            addEvent(new Event("rest", 5 * 60, "rest"), true);
        }

        // Event Related

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            saveEvent();
        }

        private void spEditEvent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                saveEvent();
                exitSpEditEvent();
            }
            if (e.Key == Key.Enter)
            {
                exitSpEditEvent(); // TODO when exit, set focus to main window
            }

        }

        private void btnNewEvent_Click(object sender, RoutedEventArgs e)
        {
            addEvent(new Event("untitled", 25));
        }

        private void btnOpenEventList_Click(object sender, RoutedEventArgs e)
        {
            spEventsList.Width = 200;
        }

        // Timer Controling
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (runningID != currentID)
            {
                events[runningID].endEvent();
                events[currentID].startEvent();
            }
            runningID = currentID;
            start();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            printWLog("Pause!");
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            nextEvent();

        }


        // Category Related
        private void btnNewCat_Click(object sender, RoutedEventArgs e)
        {
            if (tbCatName.Text != categories[edCat])
            {

                if (MessageBox.Show("Do you want to save your changes?", "Notice", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    saveCategory();

                string str = "new category";
                while (categories.Contains(str))
                    str += "_1";
                addCategory(str, false);
            }

        }

        private void btnSaveCat_Click(object sender, RoutedEventArgs e)
        {
            saveCategory();
            exitSpCatSet();
        }

        private void btnEditCatCancle_Click(object sender, RoutedEventArgs e)
        {
            exitSpCatSet();
        }

        private void btnRemoveCat_Click(object sender, RoutedEventArgs e)
        {
            removeCategory();
        }

        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCategory.SelectedIndex == cbCategory.Items.Count - 1)
            {
                string str = "new category";
                while (categories.Contains(str))
                    str += "_1";
                addCategory(str, false);
            }
        }

        private void cbCategory_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategory.Items.Add("New...");
            cbCategory.SelectionChanged += cbCategory_SelectionChanged;
        }

        private void menuItemOpenCatList_Click(object sender, RoutedEventArgs e)
        {
            spCategoryList.Width = 200;
        }

        // Other
        private void tbStatus_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //tbStatus.Text = "hah";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            printWLog("\n------------" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "-------------" +
                "\nThe window is loaded!\n");
        }

        private void menuItemDebug_Click(object sender, RoutedEventArgs e)
        {
            addEvent(new Event("test" + DateTime.Now.Second, 60), true);
            
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            printWLog("Shutting down at " + DateTime.Now.ToString());
            Application.Current.Shutdown();
        }

        // =====================</UI Invoked>=====================
    }
}
