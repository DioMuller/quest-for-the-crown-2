using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace QuestForTheCrown2.Base
{
    /// <summary>
    /// Manages Sound Effects and Background music.
    /// </summary>
    class SoundManager
    {
        #region Attributes
        /// <summary>
        /// Curreng BGM.
        /// </summary>
        private static string _currentBGM = String.Empty;

        /// <summary>
        /// Loaded songs.
        /// </summary>
        private static Dictionary<string, Song> _bgms = new Dictionary<string, Song>();

        private static Dictionary<string, SoundEffect> _ses = new Dictionary<string,SoundEffect>();
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Current background music playing.
        /// </summary>
        public string CurrentBGM { get { return _currentBGM; } }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Changes background music.
        /// </summary>
        /// <param name="title">BGM title.</param>
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

        /// <summary>
        /// Play Sound Effect
        /// </summary>
        /// <param name="name">SE name.</param>
        public static void PlaySound(string name)
        {
            SoundEffect se = null;
            _ses.TryGetValue(name, out se);

            if (se == null)
            {
                se = GameContent.LoadContent<SoundEffect>("sound/" + name + ".wav");
                _ses.Add(name, se);
            }

            se.Play();
        }
        #endregion Methods
    }
}
