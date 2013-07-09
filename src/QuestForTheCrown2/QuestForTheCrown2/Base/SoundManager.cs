using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestForTheCrown2.Base
{
    class SoundManager
    {
        private static string _currentBGM = String.Empty;
        private static Dictionary<string, Song> _bgms = new Dictionary<string, Song>();

        public string CurrentBGM { get { return _currentBGM; } }

        public static void PlayBGM(string title)
        {
            if (title != _currentBGM)
            {
                Song song = null; 
                _bgms.TryGetValue(title, out song);

                if (song == null)
                {
                    song = GameContent.LoadContent<Song>("bgm/" + title + ".wav");
                    _bgms.Add(title, song);
                }

                _currentBGM = title;
                MediaPlayer.Play(song);
            }
        }
    }
}
