using BAG.Common;
using BAG.Common.Data;
using BAG.Common.Data.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Default.WebUI.Controllers
{
    public class BackupController
    {

        private Guid SiteSettingId;
        private UnitOfWork Unit;
        private static readonly Object PADLOCK_ACTION = new Object();

        private string Name;
        private Data Data = null;


        private string ZipFolder()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            dir = Path.Combine(dir, @"App_Data\Backup\" + Name + @"\" + Name + ".zip");
            return dir;
        }

        private string BackupFolder
        {
            get
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                dir = Path.Combine(dir, @"App_Data\Backup\" + Name + @"\");
                return dir;
            }
        }
        private string BackupDataFolder()
        {
            return BackupDataFolder(null);
        }
        private string BackupDataFolder(string subFolder)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            dir = Path.Combine(dir, @"App_Data\Backup\" + Name + @"\data\" + (string.IsNullOrWhiteSpace(subFolder) ? string.Empty : subFolder + @"\"));
            return dir;
        }
        private string ViewsFolder
        {
            get
            {
                string name = Regex.Replace(Data.SiteSetting.Name, " ", string.Empty);
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                dir = Path.Combine(dir, @"Views\" + SiteSettingId.ToString());
                return dir;
            }
        }
        private string ContentFolder
        {
            get
            {
                string name = Regex.Replace(Data.SiteSetting.Name, " ", string.Empty);
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                dir = Path.Combine(dir, @"Content\" + SiteSettingId.ToString());
                return dir;
            }
        }

        private BackupController(UnitOfWork unit, string name, Guid siteSettingId)
        {
            this.SiteSettingId = siteSettingId;
            this.Unit = unit;
            this.Name = name;

        }

        public static void Delete(string name)
        {
            BackupController bc = new BackupController(null, name, Guid.NewGuid());
            try
            {
                Directory.Delete(bc.BackupFolder, recursive: true);
            }
            catch (Exception)
            {
                ErrorCode ec = ErrorCode.COULD_NOT_DELETE_FOLDER;
                throw new BackupNotPossibleException(name, ec, "Folder".PairedWith(bc.BackupFolder));
            }
            StoreProgress.Done(name);
        }
        public static ErrorCode Download(string name, out FileStream file, out string filename)
        {
            ErrorCode ec = ErrorCode.SUCCESS;
            try
            {
                BackupController bc = new BackupController(null, name, Guid.NewGuid());
                string path = bc.ZipFolder();
                file = new FileStream(path, FileMode.Open, FileAccess.Read);
                filename = Path.GetFileName(path);
            }
            catch (Exception)
            {
                ec = ErrorCode.COULD_NOT_DOWNLOAD_ZIP;
                throw new BackupNotPossibleException(name, ec);
            }
            return ec;
        }

        public static ErrorCode Store(UnitOfWork unit, string name, Guid siteSettingId)
        {
            unit = _Globals.Instance.ChangeSiteSettingId(siteSettingId, unit);
            if (StoreProgress.Get(name) != ErrorCode.NULL)
            {
                return ErrorCode.NAME_ALREADY_EXISTS;
            }

            StoreProgress.Set(name, ErrorCode.RUNNING);
            Task.Run(() =>
            {
                try
                {
                    lock (PADLOCK_ACTION)
                    {
                        if (string.IsNullOrWhiteSpace(name) || name.Where(c => !char.IsLetterOrDigit(c)).Count() > 0)
                        {
                            StoreProgress.Set(name, ErrorCode.NAME_INVALID);
                            throw new BackupNotPossibleException(name, "Name".PairedWith(name));
                        }

                        BackupController bc = new BackupController(unit, name, siteSettingId);
                        bc.SetDatas();

                        bc.VerifyStore();

                        Directory.CreateDirectory(bc.BackupDataFolder("Context"));

                        try
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            XmlSerializer serializer = new XmlSerializer(bc.Data.GetType());
                            using (MemoryStream stream = new MemoryStream())
                            {
                                serializer.Serialize(stream, bc.Data);
                                stream.Position = 0;
                                xmlDocument.Load(stream);
                                xmlDocument.Save(bc.BackupDataFolder("Context") + @"appdata.xml");
                                stream.Close();
                            }
                        }
                        catch (Exception e)
                        {
                            StoreProgress.Set(name, ErrorCode.COULD_NOT_SAVE_TO_XML);
                            throw new BackupNotPossibleException(e, name);
                        }

                        try
                        {
                            bc.CopyDirectoryContentWithIncludedFiles(bc.ContentFolder, bc.BackupDataFolder("Content"));
                        }
                        catch (Exception e)
                        {
                            StoreProgress.Set(name, ErrorCode.COULD_NOT_COPY_FOLDER);
                            throw new BackupNotPossibleException(e, name, "From".PairedWith(bc.ContentFolder), "To".PairedWith(bc.BackupDataFolder("Content")));
                        }
                        try
                        {
                            bc.CopyDirectoryContentWithIncludedFiles(bc.ViewsFolder, bc.BackupDataFolder("Views"));
                        }
                        catch (Exception e)
                        {
                            StoreProgress.Set(name, ErrorCode.COULD_NOT_COPY_FOLDER);
                            throw new BackupNotPossibleException(e, name, "From".PairedWith(bc.ViewsFolder), "To".PairedWith(bc.BackupDataFolder("Views")));
                        }

                        ZipFile.CreateFromDirectory(bc.BackupDataFolder(), bc.ZipFolder(), CompressionLevel.Fastest, true);
                        StoreProgress.Done(name);
                    }
                }
                catch (Exception e)
                {
                    StoreProgress.SetFailureIfRunning(name);
                    throw new BackupNotPossibleException(e, name);
                }
            });
            return StoreProgress.Get(name);
        }
        public static ErrorCode Restore(UnitOfWork unit, string name, out Guid ssid)
        {
            ErrorCode result = ErrorCode.SUCCESS;
            lock (PADLOCK_ACTION)
            {
                try
                {
                    BackupController bc = new BackupController(unit, name, Guid.NewGuid());

                    bc.VerifyRestore();

                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(bc.BackupDataFolder("Context") + @"appdata.xml");
                        string xmlString = xmlDocument.OuterXml;

                        using (StringReader read = new StringReader(xmlString))
                        {
                            Type outType = typeof(Data);

                            XmlSerializer serializer = new XmlSerializer(outType);
                            using (XmlReader reader = new XmlTextReader(read))
                            {
                                bc.Data = (Data)serializer.Deserialize(reader);
                                reader.Close();
                            }

                            read.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        result = ErrorCode.COULD_NOT_LOAD_XML;
                        throw new BackupNotPossibleException(e, name, result);
                    }

                    bc.Data.SiteSetting.Name = name;
                    bc.Data.SiteSetting.IsDefault = false;

                    try
                    {
                        bc.CopyDirectoryContentWithIncludedFiles(bc.BackupDataFolder("Content"), bc.ContentFolder);
                    }
                    catch (Exception e)
                    {
                        result = ErrorCode.COULD_NOT_COPY_FOLDER;
                        throw new BackupNotPossibleException(e, name, result, "From".PairedWith(bc.BackupDataFolder("Content")), "To".PairedWith(bc.ContentFolder));
                    }
                    try
                    {
                        bc.CopyDirectoryContentWithIncludedFiles(bc.BackupDataFolder("Views"), bc.ViewsFolder);
                    }
                    catch (Exception e)
                    {
                        result = ErrorCode.COULD_NOT_COPY_FOLDER;
                        throw new BackupNotPossibleException(e, name, result, "From".PairedWith(bc.BackupDataFolder("Views")), "To".PairedWith(bc.ViewsFolder));
                    }


                    ssid = bc.SiteSettingId;
                    bc.SaveDataToUnit();
                }
                catch (Exception e)
                {
                    if (result == ErrorCode.SUCCESS)
                    {
                        result = ErrorCode.UNKNOWN;
                    }
                    ssid = Guid.Empty;
                    throw new BackupNotPossibleException(e, name, result);
                }
            }
            return result;
        }


        private void SaveDataToUnit()
        {
            RemapGuids();

            Unit.SiteSettingRepository.Insert(Data.SiteSetting);
            Unit = _Globals.Instance.ChangeSiteSettingId(Data.SiteSetting.Id, Unit);
            Data.SiteManagers.ForEach(sm => Unit.SiteManagerRepository.Insert(sm));
            Data.WidgetManagers.ForEach(wm => Unit.WidgetManagerRepository.Insert(wm));
            Unit.Save();
        }

        private void RemapGuids()
        {

            Dictionary<Guid, Guid> guidMap = new Dictionary<Guid, Guid>();

            guidMap.Add(Data.SiteSetting.Id, SiteSettingId);
            Data.SiteManagers.ForEach(sm => guidMap.Add(sm.Id, Guid.NewGuid()));
            Data.WidgetManagers.ForEach(wm => guidMap.Add(wm.Id, Guid.NewGuid()));

            RemapGuids(Data.SiteSetting, guidMap);
            Data.SiteManagers.ForEach(sm => RemapGuids(sm, guidMap));
            Data.WidgetManagers.ForEach(wm => RemapGuids(wm, guidMap));
        }
        private void RemapGuids(BaseEntity baseEntity, Dictionary<Guid, Guid> guidMap)
        {
            if (baseEntity == null)
            {
                return;
            }
            List<BaseEntity> baseEntities = baseEntity.RemapAllGuids((pid, dte) => GetValueFromGuidMap(guidMap, pid, dte));
            if (baseEntities == null)
            {
                return;
            }
            baseEntities.ForEach(be => RemapGuids(be, guidMap));
        }

        private Guid GetValueFromGuidMap(Dictionary<Guid, Guid> guidMap, Guid key, bool throwException = false)
        {
            Guid result = Guid.Empty;
            if (guidMap.TryGetValue(key, out result))
            {
                return result;
            }
            if (!throwException)
            {
                return Guid.Empty;
            }
            ErrorCode ec = ErrorCode.GUIDMAP_NOT_VALIDE;
            throw new BackupNotPossibleException(Name, ec);
        }
        private List<Guid> GetValueFromGuidMap(Dictionary<Guid, Guid> guidMap, IEnumerable<Guid> keys, bool dontThrowException = false)
        {
            List<Guid> result = new List<Guid>();
            foreach (Guid key in keys)
            {
                Guid r = GetValueFromGuidMap(guidMap, key, dontThrowException);
                if (r != Guid.Empty)
                {
                    result.Add(r);
                }
            }
            return result;
        }


        private void SetDatas()
        {
            Data data = new Data();
            data.SiteSetting = Unit.SiteSettingRepository.Find(s => s.Id == SiteSettingId);
            if (data.SiteSetting == null)
            {
                StoreProgress.Set(Name, ErrorCode.SITE_SETTING_ID_NOT_FOUND);
                throw new BackupNotPossibleException();
            }
            data.SiteManagers = Unit.SiteManagerRepository.Get().Where(sm => sm.SiteSettingId == SiteSettingId).ToList();
            data.WidgetManagers = Unit.WidgetManagerRepository.Get().Where(wm => wm.SitesettingId == SiteSettingId).ToList();
            this.Data = data;
        }

        private void VerifyStore()
        {
            bool result = true;
            result = result && !Directory.Exists(BackupDataFolder("Content"));
            result = result && !Directory.Exists(BackupDataFolder("Views"));
            result = result && !Directory.Exists(BackupDataFolder("Context"));
            if (!result)
            {
                StoreProgress.Set(Name, ErrorCode.BACKUPFOLDER_ALREADY_EXISTS);
                throw new BackupNotPossibleException(StoreProgress.Get(Name).Message());
            }

            result = result && Directory.Exists(ContentFolder);
            if (!result)
            {
                StoreProgress.Set(Name, ErrorCode.CONTENTFOLDER_NOT_EXISTS);
                throw new BackupNotPossibleException(StoreProgress.Get(Name).Message());
            }

            result = result && Directory.Exists(ViewsFolder);
            if (!result)
            {
                StoreProgress.Set(Name, ErrorCode.VIEWSFOLDER_NOT_EXISTS);
                throw new BackupNotPossibleException(StoreProgress.Get(Name).Message());
            }
        }
        private void VerifyRestore()
        {
            bool result = true;
            result = result && Directory.Exists(BackupDataFolder("Content"));
            result = result && Directory.Exists(BackupDataFolder("Views"));
            result = result && Directory.Exists(BackupDataFolder("Context"));


            if (result)
            {
                result = result && File.Exists(BackupDataFolder("Context") + @"appdata.xml");
            }

            if (!result)
            {
                ErrorCode ec = ErrorCode.BACKUPDATA_NOT_EXISTS_COMPLETELY;
                throw new BackupNotPossibleException(Name, ec, "Folder".PairedWith(BackupDataFolder()));
            }
        }

        private void CopyDirectoryContentWithIncludedFiles(string dirCopySource, string dirCopyTarget)
        {

            string[] subDirectories = Directory.GetDirectories(dirCopySource);

            if (!Directory.Exists(dirCopyTarget.ToString()))
            {
                Directory.CreateDirectory(dirCopyTarget.ToString());
            }

            foreach (string subDirectory in subDirectories)
            {
                string newDirectoryPath = subDirectory;
                if (newDirectoryPath.LastIndexOf(@"\") == (newDirectoryPath.Length - 1))
                {
                    newDirectoryPath = newDirectoryPath.Substring(0, newDirectoryPath.Length - 1);
                }
                StringBuilder newTargetPath = new StringBuilder();
                newTargetPath.Append(dirCopyTarget);
                newTargetPath.Append(newDirectoryPath.Substring(newDirectoryPath.LastIndexOf(@"\")));
                CopyDirectoryContentWithIncludedFiles(newDirectoryPath, newTargetPath.ToString());
            }


            string[] fileNames = Directory.GetFiles(dirCopySource);
            foreach (string fileSource in fileNames)
            {
                StringBuilder fileTarget = new StringBuilder();
                {
                    fileTarget.Append(dirCopyTarget);
                    fileTarget.Append(fileSource.Substring(fileSource.LastIndexOf(@"\")));
                }
                File.Copy(fileSource, fileTarget.ToString(), true);
            }
        }
    }

    public class Data
    {
        public SiteSetting SiteSetting { get; set; }
        public List<WidgetManager> WidgetManagers { get; set; }
        public List<SiteManager> SiteManagers { get; set; }
    }

    [Serializable()]
    public class BackupNotPossibleException : System.Exception
    {
        public BackupNotPossibleException() : base() { }
        public BackupNotPossibleException(string message) : base(message) { }
        public BackupNotPossibleException(string message, System.Exception inner) : base(message, inner) { }

        public BackupNotPossibleException(System.Exception inner, string name, params KeyValuePair<string, object>[] infos) : base(GetErrorMessage(name, infos), inner) { }
        public BackupNotPossibleException(string name, params KeyValuePair<string, object>[] infos) : base(GetErrorMessage(name, infos)) { }
        public BackupNotPossibleException(System.Exception inner, string name, ErrorCode ec, params KeyValuePair<string, object>[] infos) : base(GetErrorMessage(name, ec, infos), inner) { }
        public BackupNotPossibleException(string name, ErrorCode ec, params KeyValuePair<string, object>[] infos) : base(GetErrorMessage(name, ec, infos)) { }

        private static string GetErrorMessage(string name, params KeyValuePair<string, object>[] infos)
        {
            return GetErrorMessage(name, null, infos);
        }
        private static string GetErrorMessage(string name, ErrorCode? ec, params KeyValuePair<string, object>[] infos)
        {
            string message = ec == null ? StoreProgress.Get(name).Message() : ec.Value.Message();

            var info = string.Join(", ", infos.Select(s => s.Key + "=" + s.Value.ToString()));
            return "[" + name + "] " + message + (string.IsNullOrEmpty(info) ? string.Empty : " -> ") + info;
        }

        protected BackupNotPossibleException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }


    public enum ErrorCode { NULL, RUNNING, UNKNOWN, SITE_SETTING_ID_NOT_FOUND, BACKUPFOLDER_ALREADY_EXISTS, CONTENTFOLDER_NOT_EXISTS, VIEWSFOLDER_NOT_EXISTS, BACKUPDATA_NOT_EXISTS_COMPLETELY, COULD_NOT_SAVE_TO_XML, COULD_NOT_COPY_FOLDER, COULD_NOT_LOAD_XML, COULD_NOT_VERIFY_RESTORE, SUCCESS, NAME_INVALID, NAME_ALREADY_EXISTS, COULD_NOT_DOWNLOAD_ZIP, COULD_NOT_DELETE_FOLDER, GUIDMAP_NOT_VALIDE };
    public static class Extensions
    {
        public static KeyValuePair<string, object> PairedWith(this string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }

        public static bool HasFailure(this ErrorCode ec)
        {
            return ec != ErrorCode.SUCCESS && ec != ErrorCode.RUNNING && ec != ErrorCode.NULL;
        }

        public static string Message(this ErrorCode ec)
        {
            switch (ec)
            {
                case ErrorCode.BACKUPDATA_NOT_EXISTS_COMPLETELY:
                    return "Backup-Ordner für diesen Namen nicht gefunden oder nur teilweise vorhanden.";
                case ErrorCode.BACKUPFOLDER_ALREADY_EXISTS:
                    return "Backup-Ordner für diesen Namen bereits vorhanden.";
                case ErrorCode.CONTENTFOLDER_NOT_EXISTS:
                    return "Content-Ordner für diese Id wurde nicht gefunden.";
                case ErrorCode.COULD_NOT_COPY_FOLDER:
                    return "Ordner konnte nicht in das Zielverzeichnis kopiert werden.";
                case ErrorCode.COULD_NOT_DOWNLOAD_ZIP:
                    return "Fehler beim Download der ZIP-Datei.";
                case ErrorCode.COULD_NOT_LOAD_XML:
                    return "Fehler beim Laden der XML-Datei.";
                case ErrorCode.COULD_NOT_SAVE_TO_XML:
                    return "Fehler beim Speichern der XML-Datei.";
                case ErrorCode.NAME_ALREADY_EXISTS:
                    return "Der gewählte Name existiert bereits in einer (noch) nicht erfolgreich abgeschlossenen Anfrage.";
                case ErrorCode.NAME_INVALID:
                    return "Der gewählte Name ist nicht gültig.";
                case ErrorCode.NULL:
                    return "Es wurde kein fehler-Code definiert.";
                case ErrorCode.RUNNING:
                    return "Die Anfrage wird noch bearbeitet.";
                case ErrorCode.SITE_SETTING_ID_NOT_FOUND:
                    return "Keine Daten unter dieser Id gefunden.";
                case ErrorCode.SUCCESS:
                    return "Anfrage erfolgreich bearbeitet.";
                case ErrorCode.UNKNOWN:
                    return "Ein unbekannter Fehler ist aufgetreten.";
                case ErrorCode.COULD_NOT_DELETE_FOLDER:
                    return "Konnte Ordner nicht löschen.";
                case ErrorCode.VIEWSFOLDER_NOT_EXISTS:
                    return "Views-Ordner für diese Id wurde nicht gefunden.";
                case ErrorCode.GUIDMAP_NOT_VALIDE:
                    return "GuidMap ist nicht gültig. Wert nicht gefunden.";
                default:
                    return "Fehlerart ist unbekannt.";
            }
        }
    }

    internal static class StoreProgress
    {
        private static readonly Object PADLOCK_LOOKUP = new Object();

        private static readonly Dictionary<string, ErrorCode> ProgressMap = new Dictionary<string, ErrorCode>();

        public static void SetFailureIfRunning(string name)
        {
            ErrorCode ec = Get(name);
            if (ec == ErrorCode.RUNNING || ec == ErrorCode.NULL || ec == ErrorCode.SUCCESS)
            {
                Set(name, ErrorCode.UNKNOWN);
            }
        }
        public static void Set(string name, ErrorCode ec)
        {
            lock (PADLOCK_LOOKUP)
            {
                ProgressMap[name] = ec;
            }
        }
        public static void Done(string name)
        {
            lock (PADLOCK_LOOKUP)
            {
                ProgressMap.Remove(name);
            }
        }
        public static ErrorCode Get(string name)
        {
            lock (PADLOCK_LOOKUP)
            {
                ErrorCode result;
                if (ProgressMap.TryGetValue(name, out result))
                {
                    return result;
                }
                else
                {
                    return ErrorCode.NULL;
                }
            }
        }

        public static IEnumerable<string> GetAllUnsuccessfulStoreTaskNames()
        {
            return GetAllUnsuccessfulStoreTaskNames(new List<string>());
        }
        public static IEnumerable<string> GetAllUnsuccessfulStoreTaskNames(IEnumerable<string> additional)
        {
            lock (PADLOCK_LOOKUP)
            {
                return ProgressMap.Where(kvp => kvp.Value != ErrorCode.SUCCESS).Select(kvp => kvp.Key).Except(additional).Union(additional);
            }
        }


    }
}