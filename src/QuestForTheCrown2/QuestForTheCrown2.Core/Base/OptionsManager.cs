using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace QuestForTheCrown2.Base
{
    public class Options
    {
        #region Graphical Options
        /// <summary>
        /// Resolution ( Width )
        /// </summary>   
        public int ResolutionWidth { get; set; }
        
        /// <summary>
        /// Resolution ( Height )
        /// </summary>
        public int ResolutionHeight { get; set; }

        /// <summary>
        /// Is the game fullscreen?
        /// </summary>
        public bool Fullscreen { get; set; }
        #endregion Graphical Options

        #region Controller Options
        /// <summary>
        /// Invert bow aim?
        /// </summary>
        public bool InvertAim { get; set; }
        #endregion Controller Options

        #region Constructor
        /// <summary>
        /// Creates an Options instance with the default settings.
        /// </summary>
        public Options()
        {
            ResolutionWidth = 1280;
            ResolutionHeight = 720;
            Fullscreen = false;
            InvertAim = false;
        }
        #endregion Constructor
    }

    public static class OptionsManager
    {
        /// <summary>
        /// Game File.
        /// </summary>
        public const string OptionsFile = "GameOptions.xml";

        /// <summary>
        /// Current Options
        /// </summary>
        public static Options CurrentOptions { get; private set; }

        /// <summary>
        /// Loads the game options.
        /// </summary>
        public static void LoadOptions()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                if( !store.FileExists(OptionsFile) )
                {
                    store.CreateFile(OptionsFile);
                    CurrentOptions = new Options();
                }
                else
                {
                    try
                    {
                        string content;

                        using (StreamReader sr = new StreamReader(store.OpenFile(OptionsFile, FileMode.Open)))
                        {
                            content = sr.ReadToEnd();
                            sr.Close();
                        }

                        XDocument doc = XDocument.Parse(content);
                        XElement root = doc.Element("options");

                        CurrentOptions = new Options
                        {
                            ResolutionWidth = int.Parse(root.Element("ResolutionWidth").Value),
                            ResolutionHeight = int.Parse(root.Element("ResolutionHeight").Value),
                            Fullscreen = bool.Parse(root.Element("Fullscreen").Value),
                            InvertAim = bool.Parse(root.Element("InvertAim").Value)
                        };
                    }
                    catch
                    {
                        store.CreateFile(OptionsFile).Close();
                        CurrentOptions = new Options();
                    }
                }
            }
        }

        public static void SaveOptions()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                if (CurrentOptions == null)
                {
                    CurrentOptions = new Options();
                }

                StringBuilder content = new StringBuilder();

                content.AppendLine("<options>");

                content.AppendLine("<ResolutionWidth>" + CurrentOptions.ResolutionWidth.ToString() + "</ResolutionWidth>");
                content.AppendLine("<ResolutionHeight>" + CurrentOptions.ResolutionHeight.ToString() + "</ResolutionHeight>");
                content.AppendLine("<Fullscreen>" + CurrentOptions.Fullscreen.ToString() + "</Fullscreen>");
                content.AppendLine("<InvertAim>" + CurrentOptions.InvertAim.ToString() + "</InvertAim>");

                content.AppendLine("</options>");

                if (store.FileExists(OptionsFile))
                {
                    store.DeleteFile(OptionsFile);
                }

                using (StreamWriter sw = new StreamWriter(store.OpenFile(OptionsFile, FileMode.Create)))
                {
                    sw.WriteLine(content.ToString());
                }
            }
        }
    }
}
