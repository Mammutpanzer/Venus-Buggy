using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace VenusBuggy
{
    class Config
    {
        //string Width;
        //string Height;
        string volumeMusic;
        string volumeEffects;

        string s;

        public Config()
        {
            s = "config/config.txt";
        }

        public void reset()
        {
            //Width = "800";
            //Height = "600";
            volumeMusic = "100";
            volumeEffects = "100";
            if (File.Exists(s))
            {
                File.Delete(s);
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(s))
            {
                // Templatewerte in File schreiben
                //file.WriteLine(Width);
                //file.WriteLine(Height);
                file.WriteLine(volumeMusic);
                file.WriteLine(volumeEffects);
            }
        }

        /// <summary>
        /// Gibt die jeweilige Zeile der config.txt aus.
        /// </summary>
        /// <param name="line">0 = volumeMusic; 1 = volumeEffects</param>
        /// <returns>Gibt die Integerzahl aus der jeweiligen Zeile aus.</returns>
        public int getValue(int line)
        {
            int result = 0;
            if (!File.Exists(s))
            {
                reset();
            }
            try
            {
                result = Convert.ToInt32(File.ReadLines(s).Skip(line - 1).Take(1).First());
            }
            catch
            {
                reset();
                getValue(line);
            }

            return result;
        }

    }
}