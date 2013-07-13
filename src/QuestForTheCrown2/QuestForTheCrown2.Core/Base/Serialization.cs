using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace QuestForTheCrown2.Base
{
    public static class Serialization
    {
        #region Load
        public static ItemType Load<ItemType>(string path)
            where ItemType : class
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                // Seleciona o arquivo mais recente
                var dbFiles = from dbFile in new[] { path, path + "~" }
                              where store.FileExists(dbFile)
                              let lastWriteTime = store.GetLastWriteTime(dbFile)
                              orderby lastWriteTime descending
                              select Deserialize<ItemType>(store, dbFile);

                return dbFiles.FirstOrDefault(d => d != null);
            }
        }

        static ItemType Deserialize<ItemType>(IsolatedStorageFile store, string file) where ItemType : class
        {
            try
            {
                
                using (var fs = store.OpenFile(file, FileMode.Open, FileAccess.Read))
                {
                    var ser = new BinaryFormatter();
                    return (ItemType)ser.Deserialize(fs);
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Save
        public static void Save<ItemType>(this ItemType obj, string path)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                var writePath = GetSaveFileName(store, path);
                using (var fs = store.OpenFile(writePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    try
                    {
                        var ser = new BinaryFormatter();
                        ser.Serialize(fs, obj);

                        if (writePath != path)
                            store.CopyFile(writePath, path, true);
                        if (store.FileExists(path + "~"))
                            store.DeleteFile(path + "~");
                    }
                    catch { }
                }
            }
        }

        private static string GetSaveFileName(IsolatedStorageFile store, string path)
        {
            var dbFiles = from dbFile in new[] { path, path + "~" }
                          let lastWriteTime = store.FileExists(dbFile) ? store.GetLastWriteTime(dbFile) : DateTimeOffset.MinValue
                          orderby lastWriteTime
                          select dbFile;

            return dbFiles.First();
        }
        #endregion

        #region Extensions
        public static byte[] AsHex(this string text)
        {
            byte[] bytes = new byte[text.Length / 2];

            for (int i = 0; i < text.Length; i += 2)
            {
                bytes[i / 2] = byte.Parse(text[i].ToString() + text[i + 1].ToString(),
                    System.Globalization.NumberStyles.HexNumber);
            }

            return bytes;
        }

        public static string ToHex(this byte[] array)
        {
            var bob = new System.Text.StringBuilder(array.Length);

            foreach (byte singleByte in array)
            {
                bob.Append(singleByte.ToString("X2"));
            }
            return bob.ToString();
        }
        #endregion
    }
}
