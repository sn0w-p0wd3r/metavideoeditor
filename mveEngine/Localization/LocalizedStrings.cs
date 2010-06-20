﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System.Collections;


namespace mveEngine
{
    public class MBPropertySet : System.Collections.Hashtable
    {

    }

    public class LocalizedStrings
    {
        private List<object> data = new List<object>();
        public MBPropertySet LocalStrings = new MBPropertySet();
        private MBPropertySet localStringsReverse = new MBPropertySet();


        public LocalizedStrings()
        {
            //Copy strings from program files
            try
            {
                string folderApp = System.Reflection.Assembly.GetExecutingAssembly().Location;
                foreach (FileInfo fi in Directory.GetParent(folderApp).GetFiles("strings-*.xml"))
                {
                    File.Move(fi.FullName, Path.Combine(ApplicationPaths.AppLocalizationPath, fi.Name));
                }
            }
            catch { }
            //start with our main string data - others can be added at a later time
            AddStringData(BaseStrings.FromFile(LocalizedStringData.GetFileName()));
        }

        public void AddStringData(object stringData)
        {
            //translate our object definition into a properyset for mcml lookups
            // and a reverse dictionary so we can lookup keys by value
            foreach (var field in stringData.GetType().GetFields())
            {
                if (field != null)
                {
                    try
                    {
                        LocalStrings[field.Name] = field.GetValue(stringData) as string;
                        localStringsReverse[field.GetValue(stringData) as string] = field.Name;
                    }
                    catch (Exception ex)
                    {
                        Logger.ReportException("Error adding string element.", ex);
                    }
                }
            }

        }

        public string GetString(string key)
        {
            //return the string from our propertyset
            return LocalStrings[key] as string ?? "";
        }

        public string GetKey(string str)
        {
            //return the key from our reverse-lookup dictionary
            return localStringsReverse[str] as string ?? "";
        }

    }
}
