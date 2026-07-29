using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase
{
    public class Alarm
    {
        public DateTime datetime;
        public string location;
        public string desc;
    }

    public class AlarmManager
    {
        List<Alarm> alarms;
        bool silenceBuzzer;
        string lastAlarm = "";

        public AlarmManager()
        {
            alarms = new List<Alarm>();
        }

        public void SilenceBuzzer(bool enb)
        {
            silenceBuzzer = enb;
        }

        public string GetLastAlarm()
        {
            return lastAlarm;
        }

        private void AddAlarmInternal(string location, string alarmText)
        {
            Alarm alm = new Alarm()
            {
                datetime = DateTime.Now,
                location = location,
                desc = alarmText
            };

            alarms.Add(alm);
            silenceBuzzer= false;

            Util.logger.LogMessage("Error", alarmText);
            lastAlarm = alarmText;
        }

        public void AddAlarm(string location, string format, params object[] args)
        {
            string text = string.Format(format, args);
            AddAlarmInternal(location, text);
        }


        public void ResetAlarms()
        {
            alarms.Clear();
        }

        public List<Alarm> GetAlarms()
        {
            return alarms;
        }

        public int AlarmCount()
        {
            return alarms.Count;
        }
    }
}
