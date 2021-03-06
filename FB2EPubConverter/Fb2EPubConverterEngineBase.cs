﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ConverterContracts.Settings;
using EPubLibrary;
using FB2EPubConverter;
using FB2EPubConverter.FB2Loaders;
using FB2Library;
using TranslitRu;
using Logger = FB2EPubConverter.Logger;
using ConverterContracts;
using Fb2epubSettings;
using TranslitRuContracts;


namespace Fb2ePubConverter
{

    internal abstract class Fb2EPubConverterEngineBase : IFb2EPubConverterEngine
    {

        protected readonly ImageManager Images = new ImageManager();
        private readonly List<FB2File> _fb2Files = new List<FB2File>();



        /// <summary>
        /// Settings for the converter
        /// </summary>
        public IConverterSettings Settings;




        
        /// <summary>
        /// Loads and parses input file or archive to ePub in memory representation
        /// </summary>
        /// <param name="fileName">file to load</param>
        /// <returns></returns>
        public bool LoadAndCheckFB2Files(string fileName)
        {
            Logger.Log.InfoFormat("Starting to convert {0}", fileName);
            _fb2Files.Clear();
            if (!File.Exists(fileName))
            {
                Logger.Log.ErrorFormat("Unable to locate file {0} on disk.", fileName);
                return false;
            }
            switch (FileTypeDetector.DetectFileType(fileName))
            {
                case FileTypeDetector.FileTypesEnum.FileTypeZIP:
                    Logger.Log.InfoFormat("Loading ZIP : {0}",fileName);
                    var fb2ZipLoader = new FB2ZipFileLoader();
                    try
                    {
                        _fb2Files.AddRange(fb2ZipLoader.LoadFile(fileName, Settings.FB2ImportSettings));
                    }
                    catch (Exception)
                    {
                        Logger.Log.ErrorFormat("Error loading ZIP {0} :",fileName);
                        return false;
                    }
                    break;
                case FileTypeDetector.FileTypesEnum.FileTypeFB2:
                    Logger.Log.InfoFormat("Processing {0} ...", fileName);
                    var fb2FileLoader = new FB2FileLoader();
                    try
                    {
                        _fb2Files.AddRange(fb2FileLoader.LoadFile(fileName,Settings.FB2ImportSettings));
                    }
                    catch (Exception)
                    {
                        Logger.Log.ErrorFormat("Error loading FB2 {0} :", fileName);
                        return false;
                    }
                    break;
                case FileTypeDetector.FileTypesEnum.FileTypeRAR:
                    Logger.Log.InfoFormat("Loading RAR : {0}", fileName);
                    var fb2RarLoader = new FB2RarLoader();
                    try
                    {
                        _fb2Files.AddRange(fb2RarLoader.LoadFile(fileName,Settings.FB2ImportSettings));
                    }
                    catch (Exception)
                    {
                        Logger.Log.ErrorFormat("Error loading RAR {0} :", fileName);
                        return false;
                    }
                    break;
                default:
                    Logger.Log.InfoFormat("File {0} is of unsupported type",fileName);
                    return false;
            }
            return true;
        }



        /// <summary>
        /// Saves the loaded FB2 file(s) to the destination as ePub
        /// </summary>
        /// <param name="fb2File"></param>
        /// <param name="epubFile"></param>
        protected abstract void ConvertContent(FB2File fb2File, IEpubFile epubFile);

        public void Save(string outFileName)
        {
            Logger.Log.DebugFormat("Saving {0}", outFileName);
            try
            {
                Logger.Log.DebugFormat("Saving totally {0} file(s)", _fb2Files.Count);
                foreach (var fb2File in _fb2Files)
                {
                    IEpubFile epubFile = CreateEpub();

                    PassEPubSettings(epubFile);

                    // do it first (before actual conversion) as if there is a problem with generating file name - we do not spend time here on generation
                    string outFile = GenerateOutputFileName(epubFile, outFileName);

                    LoadImagesInMemory(fb2File);

                    ConvertContent(fb2File,epubFile);

                    epubFile.Generate(outFile);

                }
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("Error saving file {0} : {1} - {2}", outFileName, ex.StackTrace, ex);
                throw;
            }

        }

        private void PassEPubSettings(IEpubFile epubFile)
        {
            SetTransliterationOptions(epubFile);
        }

        private void LoadImagesInMemory(FB2File fb2File)
        {
            Images.RemoveAlpha = Settings.FB2ImportSettings.ConvertAlphaPng;
            Images.LoadFromBinarySection(fb2File.Images);
        }

        private string GenerateOutputFileName(IEpubFile epubFile, string outFileName)
        {
            int count = 0;

            string outFolder = Path.GetDirectoryName(outFileName);
            if (string.IsNullOrEmpty(outFolder))
            {
                Logger.Log.DebugFormat("Using output folder : {0}", Settings.OutPutPath);
                outFolder = Settings.OutPutPath;
            }

            string outFile = Path.GetFileNameWithoutExtension(outFileName);

            Logger.Log.DebugFormat("Transliteration of file names set to : {0}", Settings.ConversionSettings.TransliterateFileName);
            if (Settings.ConversionSettings.TransliterateFileName &&
                !string.IsNullOrEmpty(outFile))
            {
                outFile = Rus2Lat.Instance.Translate(outFileName, epubFile.TranslitMode);
                Logger.Log.DebugFormat("New transliterated file name : {0}", outFile);
            }

            if (string.IsNullOrEmpty(outFile))
            {
                outFile = "converted_file";
            }


            string resultFilePath = Path.Combine(outFolder,outFile + ".epub");
            while (File.Exists(resultFilePath) )
            {
                Logger.Log.DebugFormat("{0} file exists , incrementing", resultFilePath);
                resultFilePath = Path.Combine(outFolder,string.Format(@"{0}_{1}.epub",  outFile, count++));
                if (count == int.MaxValue)
                {
                    throw new IndexOutOfRangeException(@"File name counter reached maximum possible limit - consider to clean up the folder"); 
                }
            }
            Logger.Log.DebugFormat("Final output file name : {0}", resultFilePath);
            return resultFilePath;
        }


        private void SetTransliterationOptions(IEpubFile epubFile)
        {
            var setting = new TransliterationSettingsImp
            {
                FileName = string.IsNullOrEmpty(Settings.ResourcesPath) ? @".\Translit\translit.xml" : string.Format(@"{0}\Translit\translit.xml", Settings.ResourcesPath),
            };
            Logger.Log.DebugFormat("Using transliteration rule file : {0}", setting.FileName);
            if (Settings.ConversionSettings.TransliterationSettings.Transliterate)
            {
                setting.Mode = TranslitModeEnum.ExternalRuleFile;
                if (!File.Exists(setting.FileName))
                {
                    Logger.Log.ErrorFormat(@"Unable to locate translation file {0}, switching transliteration off", setting.FileName);
                    setting.Mode = TranslitModeEnum.None;
                }
            }
            else
            {
                setting.Mode = TranslitModeEnum.None;
            }
            Logger.Log.DebugFormat("Transliteration mode : {0}", setting.Mode);
            epubFile.TranslitMode = setting;
        }

        protected abstract IEpubFile CreateEpub();



        public void LoadFB2FileFromXML(XDocument fb2Document)
        {
            _fb2Files.Clear();
            var file = new FB2File();
            try
            {
                file.Load(fb2Document, false);
                _fb2Files.Add(file);
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("Error loading XML document : {0}", ex);
            }
        }
    }
}
