using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder
{
    public class Event
    {
        protected TimeSpan  ts;
        protected int       elapsedT;
        protected bool      finished;
        protected static Dictionary<string, TimeSpan> tss = new Dictionary<string,TimeSpan>();
        protected string    words;
        protected int       snoozeT;

        public string Name { get; set; }
        public string Category { get; set; }
        public int ExpectedT { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Words { get; set; }
        
        public TimeSpan Ts
        {
            get
            {
                if (!finished)
                    ts = new TimeSpan(0, 0, elapsedT);
                return ts;
            }
        }

        public Event(string name = "untitled" , int expectedT = 25 * 60, string category = "other")
        {
            this.Name = name;
            this.Category = category;
            this.ExpectedT = expectedT;
            this.elapsedT = 0;
        }

        public void renew(string _name, int _expectedT, string _category)
        {
            this.Name = _name;
            this.ExpectedT = _expectedT;
            this.Category = _category;
        }

        public bool tick()
        {
            return (++elapsedT - this.ExpectedT == 0);
        }

        public void startEvent(DateTime start)
        {
            this.Start = start;
        }

        public void startEvent()
        {
            this.Start = DateTime.Now;
        }

        public TimeSpan endEvent(DateTime end)
        {
            this.End = end;
            ts = end - this.Start;
            finished = true;
            if (!tss.ContainsKey(Category))
                tss.Add(Category, ts);
            else
                tss[Category] += ts;
            return ts;
        }

        public TimeSpan endEvent()
        {
            return endEvent(DateTime.Now);
        }

        public void snoozeEvent(int minute)
        {
            ExpectedT += minute * 60;
            snoozeT += minute * 60; 
        }

        public static Dictionary<string, TimeSpan> getStats()
        {
            return tss;
        }

        public static Event standardWork(string name = "work")
        {
            return new Event(name, (25 * 60), "work");
        }

        public static Event standardRest()
        {
            return new Event("rest", 5 * 60, "rest");
        }

        public override string ToString()
        {
            return this.Name + ": " + (this.ExpectedT / 60) + " minutes";
        }
    }

}
