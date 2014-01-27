using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.XPath;

namespace Dinobenz.Deployment
{
    public class Utils
    {
        #region Xml
        public static List<Client> GetClient()
        {
            List<Client> lstClient = new List<Client>();

            XPathDocument xpathDoc = new XPathDocument(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ClientSettings"]));
            XPathNavigator xpathNavi = xpathDoc.CreateNavigator();
            XPathNodeIterator xpathNode = xpathNavi.Select("/ClientList/Client");

            while (xpathNode.MoveNext())
            {
                XPathNodeIterator xpathChildNode = xpathNode.Current.SelectChildren(XPathNodeType.Element);

                Client c = new Client();
                c.ProjectName = xpathChildNode.Current.SelectSingleNode("ProjectName").Value;
                c.Path = xpathChildNode.Current.SelectSingleNode("Path").Value;
                lstClient.Add(c);
            }

            return lstClient;
        }

        public static Client GetClient(string projectName)
        {
            Client c = null;

            XPathDocument xpathDoc = new XPathDocument(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ClientSettings"]));
            XPathNavigator xpathNavi = xpathDoc.CreateNavigator();
            XPathNodeIterator xpathNode = xpathNavi.Select("/ClientList/Client[ProjectName=\"" + projectName + "\"]");

            if (xpathNode.Count > 0)
            {
                xpathNode.MoveNext();
                XPathNodeIterator xpathChildNode = xpathNode.Current.SelectChildren(XPathNodeType.Element);

                c = new Client();
                c.ProjectName = xpathChildNode.Current.SelectSingleNode("ProjectName").Value;
                c.Path = xpathChildNode.Current.SelectSingleNode("Path").Value;
            }

            return c;
        }

        public static List<Server> GetServer()
        {
            List<Server> lstServer = new List<Server>();

            XPathDocument xpathDoc = new XPathDocument(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ServerSettings"]));
            XPathNavigator xpathNavi = xpathDoc.CreateNavigator();
            XPathNodeIterator xpathNode = xpathNavi.Select("/ServerList/Server");

            while (xpathNode.MoveNext())
            {
                XPathNodeIterator xpathChildNode = xpathNode.Current.SelectChildren(XPathNodeType.Element);

                Server s = new Server();
                s.ProjectName = xpathChildNode.Current.SelectSingleNode("ProjectName").Value;
                s.Path = xpathChildNode.Current.SelectSingleNode("Path").Value;
                s.ClientName = xpathChildNode.Current.SelectSingleNode("ClientName").Value;
                lstServer.Add(s);
            }

            return lstServer;
        }

        public static List<Server> GetServerByClientName(string clientName)
        {
            List<Server> lstServer = new List<Server>();

            XPathDocument xpathDoc = new XPathDocument(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ServerSettings"]));
            XPathNavigator xpathNavi = xpathDoc.CreateNavigator();
            XPathNodeIterator xpathNode = xpathNavi.Select("/ServerList/Server[ClientName=\"" + clientName + "\"]");

            while (xpathNode.MoveNext())
            {
                XPathNodeIterator xpathChildNode = xpathNode.Current.SelectChildren(XPathNodeType.Element);

                Server s = new Server();
                s.ProjectName = xpathChildNode.Current.SelectSingleNode("ProjectName").Value;
                s.Path = xpathChildNode.Current.SelectSingleNode("Path").Value;
                s.ClientName = xpathChildNode.Current.SelectSingleNode("ClientName").Value;
                lstServer.Add(s);
            }

            return lstServer;
        }

        public static Server GetServerByProjectName(string projectName)
        {
            Server s = null;

            XPathDocument xpathDoc = new XPathDocument(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ServerSettings"]));
            XPathNavigator xpathNavi = xpathDoc.CreateNavigator();
            XPathNodeIterator xpathNode = xpathNavi.Select("/ServerList/Server[ProjectName=\"" + projectName + "\"]");

            while (xpathNode.MoveNext())
            {
                XPathNodeIterator xpathChildNode = xpathNode.Current.SelectChildren(XPathNodeType.Element);

                s = new Server();
                s.ProjectName = xpathChildNode.Current.SelectSingleNode("ProjectName").Value;
                s.Path = xpathChildNode.Current.SelectSingleNode("Path").Value;
                s.ClientName = xpathChildNode.Current.SelectSingleNode("ClientName").Value;
            }

            return s;
        }
        #endregion

        #region File Browser
        public static List<string> GetFolders(string path)
        {
            List<string> lstFolder = null;

            DirectoryInfo dir = new DirectoryInfo(path);

            if (dir.GetDirectories().Length > 0)
            {
                lstFolder = new List<string>();

                foreach (DirectoryInfo item in dir.GetDirectories())
                {
                    lstFolder.Add(item.Name);
                }
            }

            return lstFolder;
        }

        public static List<string> GetFiles(string path)
        {
            List<string> lstFile = null;

            DirectoryInfo dir = new DirectoryInfo(path);

            if (dir.GetFiles().Length > 0)
            {
                lstFile = new List<string>();

                foreach (FileInfo item in dir.GetFiles())
                {
                    lstFile.Add(item.Name);
                }
            }

            return lstFile;
        }

        public static List<string> GetFoldersWithFiles(string path)
        {
            List<string> lstItems = new List<string>();
            
            List<string> lstFolders = Utils.GetFolders(path);
            List<string> lstFiles = Utils.GetFiles(path);

            if (lstFolders != null)
            {
                lstItems.AddRange(lstFolders.ToArray());
            }

            if (lstFiles != null)
            {
                lstItems.AddRange(lstFiles.ToArray());
            }

            return lstItems;
        }

        public static bool UploadFile(string sourceFileName, string destFileName, ref string message)
        {
            bool ret = true;
            try
            {
                string directoryName = destFileName.Substring(0, destFileName.LastIndexOf('\\'));
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                if (!directoryInfo.Exists)
                {
                    Directory.CreateDirectory(directoryName);
                }

                //throw new ArgumentNullException("sourceFileName");

                File.Copy(sourceFileName, destFileName, true);
            }
            catch (UnauthorizedAccessException)
            {
                message = "Unauthorized access";
                ret = false;
            }
            catch (PathTooLongException)
            {
                message = "Path too long";
                ret = false;
            }
            catch (DirectoryNotFoundException)
            {
                message = "Directtory not found";
                ret = false;
            }
            catch (FileNotFoundException)
            {
                message = "File not found";
                ret = false;
            }
            catch (IOException ex)
            {
                message = ex.Message;
                ret = false;
            }
            return ret;
        }
        #endregion
    }
}
