﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace OneDriveMapper
{
    public class Settings
    {
        private string filePath;

        public string CID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public char Drive { get; set; } = 'Z';
        public bool AutoRun { get; set; } = true;

        #region IO Logic

        public static Settings Load(string filePath)
        {
            try
            {
                var s = deserialize(File.ReadAllBytes(filePath));
                s.filePath = filePath;
                return s;
            }
            catch
            {
                return new Settings() { filePath = filePath };
            }
        }


        public void Save()
        {
            if (string.IsNullOrEmpty(this.filePath))
                throw new Exception("The filepath is not defined.");

            File.WriteAllBytes(this.filePath, this.serialize());
        }


        private static Settings deserialize(byte[] data)
        {
            using (var ms = new MemoryStream(ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser)))
                return new XmlSerializer(typeof(Settings)).Deserialize(ms) as Settings;
        }


        private byte[] serialize()
        {
            using (var ms = new MemoryStream())
            {
                new XmlSerializer(this.GetType()).Serialize(ms, this);
                return ProtectedData.Protect(ms.ToArray(), null, DataProtectionScope.CurrentUser);
            }
        }

        #endregion
    }
}
