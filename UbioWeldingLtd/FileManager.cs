﻿using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace UbioWeldingLtd
{
	public static class FileManager
	{

		private static string _configFile = FileManager.FULLPATHNAME(Constants.settingXmlFilePath, Constants.settingXmlConfigFileName);
		private static string _moduleListFile = FileManager.FULLPATHNAME(Constants.settingXmlFilePath, Constants.settingXmlListFileName);

		private static string[] comments =
			{
				Constants.CommentOutText(Constants.setupGeneralLine1),
				Constants.CommentOutText(Constants.setupGeneralLine2),
				Constants.CommentOutText(Constants.setupGeneralLine3),
				
				Constants.CommentOutText(Constants.setupVector2Line1),
				Constants.CommentOutText(Constants.setupVector4Line1),
				Constants.CommentOutText(Constants.setupSubmoduleLine1),
				Constants.CommentOutText(Constants.setupModulesToIgnoreLine1),
				Constants.CommentOutText(Constants.setupModulesToMultiplyLine1),
				Constants.CommentOutText(Constants.setupMaximizedAttribtesLine1),
				Constants.CommentOutText(Constants.setupAveragedAttribtesLine1),
				Constants.CommentOutText(Constants.setupUnchangedAttribtesLine1),
				Constants.CommentOutText(Constants.setupBreakingAttribtesLine1),
				Constants.CommentOutText(Constants.setupAddingAttributeEntryLine1),
			};


		/// <summary>
		/// catches unknown nodes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void serializer_UnknownNode
		(object sender, XmlNodeEventArgs e)
		{
			Dbg.log("Unknown Node:" + e.Name + "\t" + e.Text);
		}


		/// <summary>
		/// catches unknown attribute nodes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void serializer_UnknownAttribute
		(object sender, XmlAttributeEventArgs e)
		{
			System.Xml.XmlAttribute attr = e.Attr;
			Dbg.log("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
		}


		/// <summary>
		/// loads configfile and prepares it with all needed lists
		/// </summary>
		/// <returns></returns>
		public static WeldingConfiguration loadConfig()
		{

			WeldingConfiguration configuration = new WeldingConfiguration();
			ModuleLists moduleList = new ModuleLists();
			FileStream FileStream;

			if (System.IO.File.Exists(_configFile))
			{
				XmlSerializer configSerializer = new XmlSerializer(typeof(WeldingConfiguration));
				configSerializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
				configSerializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
				FileStream = new FileStream(_configFile, FileMode.Open);
				configuration = (WeldingConfiguration)configSerializer.Deserialize(FileStream);
				FileStream.Close();

				if (configuration.MainWindowXPosition > (Screen.width - Constants.guiScreenEdgeClearance))
				{
					configuration.MainWindowXPosition = Screen.width - Constants.guiScreenEdgeClearance;
				}
				if (configuration.MainWindowYPosition > (Screen.height - Constants.guiScreenEdgeClearance))
				{
					configuration.MainWindowYPosition = Screen.height - Constants.guiScreenEdgeClearance;
				}
				FileStream.Close();
			}

			if (System.IO.File.Exists(_moduleListFile))
			{
				XmlSerializer moduleListSerializer = new XmlSerializer(typeof(ModuleLists));
				moduleListSerializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
				moduleListSerializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
				FileStream = new FileStream(_moduleListFile, FileMode.Open);
				moduleList = (ModuleLists)moduleListSerializer.Deserialize(FileStream);
				FileStream.Close();

				configuration.vector2CurveModules = moduleList.vector2CurveModules != null ? WeldingHelpers.convertFromToStringArray(moduleList.vector2CurveModules) : new string[0];
				configuration.vector4CurveModules = moduleList.vector4CurveModules != null ? WeldingHelpers.convertFromToStringArray(moduleList.vector4CurveModules) : new string[0];
				configuration.subModules = moduleList.subModules != null ? WeldingHelpers.convertFromToStringArray(moduleList.subModules) : new string[0];
				configuration.modulesToIgnore = moduleList.modulesToIgnore != null ? WeldingHelpers.convertFromToStringArray(moduleList.modulesToIgnore) : new string[0];
				configuration.modulesToMultiply = moduleList.modulesToMultiply != null ? WeldingHelpers.convertFromToStringArray(moduleList.modulesToMultiply) : new string[0];
				configuration.maximizedModuleAttributes = moduleList.maximizedModuleAttributes != null ? WeldingHelpers.convertFromToStringArray(moduleList.maximizedModuleAttributes) : new string[0];
				configuration.minimizedModuleAttributes = moduleList.minimizedModuleAttributes != null ? WeldingHelpers.convertFromToStringArray(moduleList.minimizedModuleAttributes) : new string[0];
				configuration.averagedModuleAttributes = moduleList.averagedModuleAttributes != null ? WeldingHelpers.convertFromToStringArray(moduleList.averagedModuleAttributes) : new string[0];
				configuration.unchangedModuleAttributes = moduleList.unchangedModuleAttributes != null ? WeldingHelpers.convertFromToStringArray(moduleList.unchangedModuleAttributes) : new string[0];
				configuration.breakingModuleAttributes = moduleList.breakingModuleAttributes != null ? WeldingHelpers.convertFromToStringArray(moduleList.breakingModuleAttributes) : new string[0];
				FileStream.Close();
			}
			else
			{
				configuration.vector2CurveModules = Constants.basicVector2CurveModules;
				configuration.vector4CurveModules = Constants.basicVector4CurveModules;
				configuration.subModules = Constants.basicSubModules;
				configuration.modulesToIgnore = Constants.basicModulesToIgnore;
				configuration.modulesToMultiply = Constants.basicModulesToMultiply;
				configuration.maximizedModuleAttributes = Constants.basicMaximizedModuleAttributes;
				configuration.minimizedModuleAttributes = Constants.basicMinimizedModuleAttributes;
				configuration.averagedModuleAttributes = Constants.basicAveragedModuleAttributes;
				configuration.unchangedModuleAttributes = Constants.basicUnchangedModuleAttributes;
				configuration.breakingModuleAttributes = Constants.basicBreakingModuleAttributes;
			}
			Debug.Log(string.Format("{0} Config was loaded", Constants.logPrefix));
			return configuration;
		}

		/// <summary>
		/// saves the config file and the modulelist file so that it is possible to change them without the need to recompile
		/// </summary>
		/// <param name="configToSave"></param>
		public static void saveConfig(WeldingConfiguration configToSave)
		{
			WeldingConfiguration configuration = (WeldingConfiguration)configToSave.clone();
			ModuleLists moduleList = new ModuleLists();
			TextWriter fileStreamWriter;
			if (configuration == null)
			{
				configuration = new WeldingConfiguration();
			}
			configuration.vector2CurveModules = null;
			configuration.vector4CurveModules = null;
			configuration.subModules = null;
			configuration.modulesToIgnore = null;
			configuration.modulesToMultiply = null;
			configuration.maximizedModuleAttributes = null;
			configuration.minimizedModuleAttributes = null;
			configuration.averagedModuleAttributes = null;
			configuration.unchangedModuleAttributes = null;
			configuration.breakingModuleAttributes = null;

			XmlSerializer configSerializer = new XmlSerializer(typeof(WeldingConfiguration));
			fileStreamWriter = new StreamWriter(_configFile);
			configSerializer.Serialize(fileStreamWriter, configuration);
			fileStreamWriter.Close();

			//ModuleList from the constants or the actual config, depending on length of the array, in case user did add some entries
			moduleList.vector2CurveModules = WeldingHelpers.convertStringFromToArray(configToSave.vector2CurveModules != null && (configToSave.vector2CurveModules.Length > Constants.basicVector2CurveModules.Length) ? configToSave.vector2CurveModules : Constants.basicVector2CurveModules);
			moduleList.vector4CurveModules = WeldingHelpers.convertStringFromToArray(configToSave.vector4CurveModules != null && (configToSave.vector4CurveModules.Length > Constants.basicVector4CurveModules.Length) ? configToSave.vector4CurveModules : Constants.basicVector4CurveModules);
			moduleList.subModules = WeldingHelpers.convertStringFromToArray(configToSave.subModules != null && (configToSave.subModules.Length > Constants.basicSubModules.Length) ? configToSave.subModules : Constants.basicSubModules);
			moduleList.modulesToIgnore = WeldingHelpers.convertStringFromToArray(configToSave.modulesToIgnore != null && (configToSave.modulesToIgnore.Length > Constants.basicModulesToIgnore.Length) ? configToSave.modulesToIgnore : Constants.basicModulesToIgnore);
			moduleList.modulesToMultiply = WeldingHelpers.convertStringFromToArray(configToSave.modulesToMultiply != null && (configToSave.modulesToMultiply.Length > Constants.basicModulesToMultiply.Length) ? configToSave.modulesToMultiply : Constants.basicModulesToMultiply);
			moduleList.maximizedModuleAttributes = WeldingHelpers.convertStringFromToArray(configToSave.maximizedModuleAttributes != null && (configToSave.maximizedModuleAttributes.Length > Constants.basicMaximizedModuleAttributes.Length) ? configToSave.maximizedModuleAttributes : Constants.basicMaximizedModuleAttributes);
			moduleList.minimizedModuleAttributes = WeldingHelpers.convertStringFromToArray(configToSave.minimizedModuleAttributes != null && (configToSave.minimizedModuleAttributes.Length > Constants.basicMinimizedModuleAttributes.Length) ? configToSave.minimizedModuleAttributes : Constants.basicMinimizedModuleAttributes);
			moduleList.averagedModuleAttributes = WeldingHelpers.convertStringFromToArray(configToSave.averagedModuleAttributes != null && (configToSave.averagedModuleAttributes.Length > Constants.basicAveragedModuleAttributes.Length) ? configToSave.averagedModuleAttributes : Constants.basicAveragedModuleAttributes);
			moduleList.unchangedModuleAttributes = WeldingHelpers.convertStringFromToArray(configToSave.unchangedModuleAttributes != null && (configToSave.unchangedModuleAttributes.Length > Constants.basicUnchangedModuleAttributes.Length) ? configToSave.unchangedModuleAttributes : Constants.basicUnchangedModuleAttributes);
			moduleList.breakingModuleAttributes = WeldingHelpers.convertStringFromToArray(configToSave.breakingModuleAttributes != null && (configToSave.breakingModuleAttributes.Length > Constants.basicBreakingModuleAttributes.Length) ? configToSave.breakingModuleAttributes : Constants.basicBreakingModuleAttributes);

			XmlSerializer moduleListSerializer = new XmlSerializer(typeof(ModuleLists));
			fileStreamWriter = new StreamWriter(_moduleListFile);
			moduleListSerializer.Serialize(fileStreamWriter, moduleList);

			fileStreamWriter.WriteLine("");
			foreach (string s in comments)
			{
				fileStreamWriter.WriteLine(s);
			}
			fileStreamWriter.Close();
			Debug.Log(string.Format("{0} Config was saved", Constants.logPrefix));
		}

		private static string root_dir = null;
		public static string FULLPATHNAME(string pathname)
		{
			if (null == root_dir) {
				root_dir = Assembly.GetExecutingAssembly().Location
								   .Replace(new FileInfo(Assembly.GetExecutingAssembly().Location).Name, "")
								   .Replace(Constants.GAMEDATA, "")
								   .Replace(Constants.ROOT, "")
								   .Replace(Constants.BASE, "")
								   ;
				// change this to KSPUtil.ApplicationRootPath - better!
				// NOTE: But this will prevent the plugin from finding his way if someone move it out of the net.lisias.ksp rootdir...
			}
			return Path.Combine(root_dir, pathname);
		}

		public static string FULLPATHNAME(string basedir, string pathname)
		{
			return FULLPATHNAME(Path.Combine(basedir, pathname));
		}

		public static string PATHNAME(string basedir, string dir, string filename)
		{
			return Path.Combine(Path.Combine(basedir, dir), filename);
		}
	}
}
