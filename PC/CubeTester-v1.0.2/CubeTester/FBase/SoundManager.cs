using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using System.Windows.Forms;

namespace FBase
{
    public class SoundManager
    {
        SoundPlayer player;
        //WMPLib.WindowsMediaPlayer player;

        Dictionary<string, string> sounds;
        object lckSound = 1;
        bool playSound = true;

        public SoundManager()
        {
            player = new SoundPlayer();
            //player = new WMPLib.WindowsMediaPlayer();
            sounds = new Dictionary<string, string>();
        }

        public bool RegisterSound(string name, string filename)
        {
            string path = Path.Combine(Application.StartupPath, filename);

            if (!File.Exists(path)) return false;

            if (sounds.ContainsKey(name))
            {
                sounds[name] = path;
            }
            else
            {
                sounds.Add(name, path);
            }

            return true;
        }

        public void EnableSound(bool enb)
        {
            playSound = enb;
        }

        public bool PlaySound(string name)
        {
            //ThreadUtil.StartThread(PlaySoundTask, name);

            if (!playSound) return true;

            if (!sounds.ContainsKey(name)) return false;

            //lock (lckSound)
            //{
            //    player.URL = sounds[name];
            //    player.controls.play(); // play in new thread
            //}

            //lock(lckSound)
            //{
                player.SoundLocation = sounds[name];
                player.Play(); // play in new thread
            //}

            return true;
        }

        //public void PlaySoundTask(object obj)
        //{
        //    string name = (string)obj;
        //    if (!sounds.ContainsKey(name)) return;

        //    player.URL = sounds[name];
        //    player.controls.play(); // play in new thread
        //}
    }
}
