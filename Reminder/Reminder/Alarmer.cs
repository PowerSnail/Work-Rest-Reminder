using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder
{
    class Alarmer
    {
        //Fields
        protected bool  muted;
        protected int   volume;

        public bool Muted { get; set; }
        public int Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (value < 0 || value == 0)
                {
                    volume = 0;
                    mute();
                }
                else
                {
                    if (value > 100)
                        volume = 100;
                    else
                        volume = value;
                }
            }
        }
        
        public void mute()
        {
            muted = true;
        }

        public void incrementVol(int increment = 10)
        {
            this.Volume += increment;
        }

        public void reduceVol(int reduction = 10)
        {
            this.Volume -= reduction;
        }

        public Alarmer()
        {
            muted = true;
            volume = 50;
        }


        public void alarm(string content)
        {
            
        }
    }
}
