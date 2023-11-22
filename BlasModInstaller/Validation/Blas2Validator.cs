﻿using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BlasModInstaller.Validation
{
    internal class Blas2Validator : IValidator
    {
        private readonly string _exeName = "Blasphemous 2.exe";
        private readonly string _defaultPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Blasphemous 2";

        public async Task InstallModdingTools()
        {
            using (WebClient client = new WebClient())
            {
                string toolsPath = "https://github.com/BrandenEK/BlasII.ModdingTools/raw/main/modding-tools.zip";
                string downloadPath = Core.DataCache + "/Blas2tools.zip";
                string installPath = Core.SettingsHandler.Config.Blas2RootFolder;

                await client.DownloadFileTaskAsync(new Uri(toolsPath), downloadPath);

                using (ZipFile zipFile = ZipFile.Read(downloadPath))
                {
                    foreach (ZipEntry file in zipFile)
                        file.Extract(installPath, ExtractExistingFileAction.OverwriteSilently);
                }

                File.Delete(downloadPath);
            }
        }

        public void SetRootPath(string path)
        {
            Core.SettingsHandler.Config.Blas2RootFolder = path;
        }

        public bool IsRootFolderValid
        {
            get
            {
                string path = Core.SettingsHandler.Config.Blas2RootFolder;
                if (File.Exists(path + "\\" + _exeName))
                {
                    Directory.CreateDirectory(path + "\\Modding\\disabled");
                    return true;
                }

                return false;
            }
        }

        public bool AreModdingToolsInstalled
        {
            get
            {
                return Directory.Exists(Core.SettingsHandler.Config.Blas2RootFolder + "\\MelonLoader");
            }
        }

        public bool AreModdingToolsUpdated
        {
            get
            {
                string filePath = Core.SettingsHandler.Config.Blas2RootFolder + "\\MelonLoader\\net6\\MelonLoader.dll";
                return File.Exists(filePath) && FileVersionInfo.GetVersionInfo(filePath).FileMajorPart == 2;
            }
        }

        public string ExeName => _exeName;
        public string DefaultPath => _defaultPath;
    }
}
