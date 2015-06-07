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
using System.Windows.Threading;
using System.Windows.Shapes;

namespace Reminder
{
    /// <summary>
    /// Interaction logic for AlarmWindow.xaml
    /// </summary>
    public partial class AlarmWindow : Window
    {
        //Field
        bool    exiting = false;
        int     origin;
        double  targetWidth;
        int     k;
        double  dw;

        bool isSubMouse = false, isSubMove = false, isSubTrans = false;

        DispatcherTimer timer = new DispatcherTimer();
        MainWindow parentW;

        // Constructor
        public AlarmWindow(MainWindow _sender)
        {
            InitializeComponent();
            parentW = (MainWindow) _sender;
            this.Show();
            origin = (int)(SystemParameters.PrimaryScreenWidth);
            timer.Interval = TimeSpan.FromMilliseconds(25);
            changeWindowWidth(70);
            exit();
        }

        /* -------------------------------------------------- *
         * UI changing and movement control                   *
         * -------------------------------------------------- */
        
        private void changeWindowWidth(double _width)
        {
            this.Width = _width;
            this.Left = origin - _width;
        }

        private void stretchWindowTo(int _width) 
        {
            // When moving, opacity remain 1.0
            //parentW.printWLog("stretch window to " + _width);
            unSubscribeMouseEL();
            recoverOpacity();

            targetWidth = _width;
            k = (_width > this.Width) ? 1 : -1;
            dw = Math.Sqrt(2 * 4 * Math.Abs(_width - this.Width));

            SubscribeMoveWindow();
            timer.Start();            
        }

        private void timer_Tick_MoveWindow(object sender, EventArgs e)
        {
            double newW = this.Width + dw * k;
            dw = (dw < 3) ? 3 : dw - 3;
            if ((newW - targetWidth) * k >= 0)
            {
                changeWindowWidth(targetWidth);

                resetTimer();
                unSubscribeMoveWindow();
                //parentW.printWLog("End of a movement");

                if (exiting) { exited(); }

                return;
            }
            changeWindowWidth(newW);
        }

        internal void exit()
        {
            //parentW.printWLog("exiting alarm window");
            stretchWindowTo(70);
            exiting = true;
        }

        internal void enter()
        {
            exiting = false;
            //parentW.printWLog("entering alarm window");
            stretchWindowTo(500);
        }

        internal void enter(string str)
        {
            tbContent.Text = str;
            enter();
        }  // overload for sending info to alarm window



        /* --------------------------------------------------------------- *
         * Transparency controls                                           *
         * --------------------------------------------------------------- */

        private void startTurnTransparent(string enteringpoint = "none")
        {
            SubscribeChangeOpacity();
            //parentW.printWLog("start Turn to Transparent! entering point: " + enteringpoint);
            timer.Start();
        }

        private void recoverOpacity()
        {
            //parentW.printWLog("recoverOpacity");
            timer.Stop();
            unSubscribeChangeOpacity();
            this.Opacity = 1.0;
        }

        private void timer_Tick_setTransparency(object Sender, EventArgs e)
        {
            Opacity -= 0.05;
            if (this.Opacity < 0.40)  // Terminates change of opacity
            {
                this.Opacity = 0.3;
                timer.Stop();
                unSubscribeChangeOpacity();
                //parentW.printWLog("Stop turn to transparent!");
            }
        }

        /* -------------------------------------------------------------- *
         * Main buttons event handlers                                    *
         * -------------------------------------------------------------- */

        private void Suspend_Click(object sender, RoutedEventArgs e)
        {
            parentW.start();
            exit();
        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            wpSnoozeTime.Height = 38;
            tbSnoozeTime.SelectAll();
            tbSnoozeTime.Focus();
        }

        private void tbSnoozeTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int minute = int.Parse(tbSnoozeTime.Text);
                parentW.getCurrentEvent().snoozeEvent(minute);
                parentW.start();
                wpSnoozeTime.Height = 0;
                exit();
            }

            if (e.Key == Key.Escape)
                wpSnoozeTime.Height = 0;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            parentW.nextEvent();
            exit();
        }

        private void Next_MouseEnter(object sender, MouseEventArgs e)
        {
            recoverOpacity();
        }

        private void Next_MouseLeave(object sender, MouseEventArgs e)
        {
            startTurnTransparent("mouse leave");
        }


        /* --------------------------------------------------------------------------- *
         * Methods that seals the subsription of dynamically controlled event handlers *
         * --------------------------------------------------------------------------- */

        private void SubscribeChangeOpacity()
        {
            if (isSubTrans)
                return;

            isSubTrans = true;
            timer.Tick += timer_Tick_setTransparency;
        }

        private void SubscribeMoveWindow()
        {
            if (isSubMove) return;

            isSubMove = true;
            timer.Tick += timer_Tick_MoveWindow;
        }

        private void unSubscribeChangeOpacity()
        {
            timer.Tick -= timer_Tick_setTransparency;
            isSubTrans = false;
        }

        private void unSubscribeMoveWindow()
        {
            timer.Tick -= timer_Tick_MoveWindow;
            isSubMove = false;
        }

        private void SubscribeMouseEL()
        {
            if (isSubMouse)
                return;

            isSubMouse = true;
            Next.MouseEnter += Next_MouseEnter;
            Next.MouseLeave += Next_MouseLeave;
        }

        private void unSubscribeMouseEL()
        {
            Next.MouseLeave -= Next_MouseLeave;
            Next.MouseEnter -= Next_MouseEnter;
            isSubMouse = false;
        }

        private void resetTimer()
        {
            //parentW.printWLog("reset Timer");
            timer.Stop();
        }

        private void exited()
        {
            exiting = false;
            //parentW.printWLog("\t this is an exiting movement");
            startTurnTransparent("exited");
            SubscribeMouseEL();
        }
    }
}
