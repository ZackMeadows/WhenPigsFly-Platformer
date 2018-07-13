// ------------------------------
//
// Author: Zack Meadows
// Project: When Pigs Fly
//
// Created 11/27/2015
//
// ------------------------------
// Asset Loader
// Code inspired from
// https://danielsaidi.wordpress.com/2010/01/26/load-all-content-files-in-a-folder-in-xna/
// Created 11/27/2015
// ------------------------------

using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WhenPigsFly
{
    public static class AssetLoader
    {
        public static Dictionary<string, T> LoadContent<T>(this ContentManager contentManager, string contentFolder)
        {
            // Prepare Directory Check
            // -------------------------
            DirectoryInfo directory = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
            if (!directory.Exists)
                return null;
            // -------------------------
            // Prepare Files for Dictionary
            // -------------------------
            Dictionary<String, T> result = new Dictionary<String, T>();
            FileInfo[] files = directory.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }
            return result;
            // -------------------------
        }
    }
}
