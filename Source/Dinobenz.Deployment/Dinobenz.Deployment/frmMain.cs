using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Dinobenz.Deployment
{
    public partial class frmMain : Form
    {
        private string _MessageTitle = Application.ProductName;

        public frmMain()
        {
            InitializeComponent();

            SetInitialControls();
        }

        private void SetInitialControls()
        {
            lblVersion.Text = string.Format("v{0}", Application.ProductVersion);

            btnCancel.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Text = string.Empty;

            System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            ImageList lstImg = new ImageList();
            lstImg.Images.Add("Folder", Image.FromStream(thisExe.GetManifestResourceStream("Dinobenz.Deployment.folder.gif")));
            trvSource.ImageList = lstImg;

            List<Client> lstClient = Utils.GetClient();
            cboSource.DisplayMember = "ProjectName";
            cboSource.ValueMember = "ProjectName";
            cboSource.DataSource = lstClient;
        }

        private void cboSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            trvSource.Nodes.Clear();

            if (!string.IsNullOrEmpty(cboSource.SelectedValue.ToString()))
            {
                TreeNode root = new TreeNode(cboSource.SelectedValue.ToString());
                root.ImageKey = "Folder";
                root.SelectedImageKey = "Folder";
                root.StateImageKey = "Folder";

                Client c = Utils.GetClient(cboSource.SelectedValue.ToString());
                List<string> lstFolder = Utils.GetFoldersWithFiles(c.Path);
                if (lstFolder != null)
                {
                    foreach (string folder in lstFolder)
                    {
                        TreeNode node = new TreeNode(folder);

                        if (folder.IndexOf('.') > 0)
                        {
                            string ext = folder.Substring(folder.LastIndexOf('.'));

                            if (!trvSource.ImageList.Images.ContainsKey(ext))
                            {
                                trvSource.ImageList.Images.Add(ext, Icons.IconFromExtensionShell(ext, Icons.SystemIconSize.Small));
                            }
                            
                            node.ImageKey = ext;
                            node.SelectedImageKey = ext;
                            node.StateImageKey = ext;
                        }
                        else
                        {
                            node.ImageKey = "Folder";
                            node.SelectedImageKey = "Folder";
                            node.StateImageKey = "Folder";

                            List<TreeNode> lstChild = GetChildNodes(string.Format(@"{0}\{1}", c.Path, folder));
                            if (lstChild != null)
                            {
                                node.Nodes.AddRange(lstChild.ToArray());
                            }
                        }

                        root.Nodes.Add(node);
                    }
                }

                trvSource.AfterCheck += new TreeViewEventHandler(trvSource_AfterCheck);
                root.Expand();
                trvSource.Nodes.Add(root);
                trvSource.Nodes[0].Checked = true;

                cbxlDestination.Items.Clear();
                List<Server> lstServer = Utils.GetServerByClientName(root.Text);
                if (lstServer != null)
                {
                    foreach (Server item in lstServer)
                    {
                        cbxlDestination.Items.Add(item.ProjectName);
                        cbxlDestination.SetItemChecked(cbxlDestination.Items.Count - 1, true);
                    }
                }
            }
        }

        void trvSource_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                foreach (TreeNode item in e.Node.Nodes)
                {
                    item.Checked = e.Node.Checked;
                }
            }
        }

        private List<TreeNode> GetChildNodes(string path)
        {
            List<TreeNode> lstNodes = null;

            List<string> lstFolder = Utils.GetFoldersWithFiles(path);

            if (lstFolder != null)
            {
                lstNodes = new List<TreeNode>();

                foreach (string folder in lstFolder)
                {
                    TreeNode child = new TreeNode(folder);

                    if (folder.IndexOf('.') > 0)
                    {
                        string ext = folder.Substring(folder.LastIndexOf('.'));

                        if (!trvSource.ImageList.Images.ContainsKey(ext))
                        {
                            trvSource.ImageList.Images.Add(ext, Icons.IconFromExtensionShell(ext, Icons.SystemIconSize.Small));
                        }

                        child.ImageKey = ext;
                        child.SelectedImageKey = ext;
                        child.StateImageKey = ext;
                    }
                    else
                    {
                        child.ImageKey = "Folder";
                        child.SelectedImageKey = "Folder";
                        child.StateImageKey = "Folder";

                        List<TreeNode> lstChild = GetChildNodes(string.Format(@"{0}\{1}", path, folder));
                        if (lstChild != null)
                        {
                            child.Nodes.AddRange(lstChild.ToArray());
                        }
                    }

                    lstNodes.Add(child);
                }
            }

            return lstNodes;
        }

        private void btnDeploy_Click(object sender, EventArgs e)
        {
            List<CopyFile> lstCopyFile = GetCopyFile();

            if (lstCopyFile != null && lstCopyFile.Count > 0)
            {
                cboSource.Enabled = false;
                trvSource.Enabled = false;
                cbxlDestination.Enabled = false;
                btnDeploy.Enabled = false;
                btnCancel.Enabled = true;
                toolStripProgressBar1.Visible = true;

                backgroundWorker1.RunWorkerAsync(lstCopyFile);
            }
            else
            {
                MessageBox.Show("Please specified at least 1 item.", _MessageTitle);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private List<string> GetSelectedChildNodes(string path, TreeNodeCollection nodes)
        {
            List<string> lstSelectedItems = new List<string>();

            foreach (TreeNode node in nodes)
            {
                string localPath = path + @"\" + node.Text;

                if (node.Checked)
                {
                    if (node.Text.IndexOf('.') > 0)
                    {
                        lstSelectedItems.Add(localPath);
                    }
                }

                if (node.Nodes.Count > 0)
                {
                    List<string> lstSelectedChildItems = GetSelectedChildNodes(localPath, node.Nodes);
                    if (lstSelectedChildItems != null)
                    {
                        lstSelectedItems.AddRange(lstSelectedChildItems.ToArray());
                    }
                }
            }

            return lstSelectedItems;
        }

        private List<string> GetSourceItems()
        {
            List<string> lstSourceItems = new List<string>();
            string strSourcePath = string.Empty;

            foreach (TreeNode root in trvSource.Nodes)
            {
                strSourcePath = root.Text;

                if (root.Nodes.Count > 0)
                {
                    List<string> lstSelectedChildItems = GetSelectedChildNodes(string.Empty, root.Nodes);
                    if (lstSelectedChildItems != null)
                    {
                        lstSourceItems.AddRange(lstSelectedChildItems.ToArray());
                    }
                }
            }

            return lstSourceItems;
        }

        private List<string> GetDestItems()
        {
            List<string> lstDestinationItems = new List<string>();

            if (cbxlDestination.CheckedItems.Count > 0)
            {
                for (int i = 0; i < cbxlDestination.CheckedItems.Count; i++)
                {
                    lstDestinationItems.Add(cbxlDestination.CheckedItems[i].ToString());
                }
            }

            return lstDestinationItems;
        }

        private List<CopyFile> GetCopyFile()
        {
            List<CopyFile> lstCopyFile = new List<CopyFile>();
            List<string> lstSourceItems = GetSourceItems();
            List<string> lstDestinationItems = GetDestItems();

            string sourcePath = cboSource.SelectedValue.ToString();
            Client c = Utils.GetClient(sourcePath);

            foreach (string destination in lstDestinationItems)
            {
                Server s = Utils.GetServerByProjectName(destination);
                foreach (string source in lstSourceItems)
                {
                    string sourceFileName = c.Path + source;
                    string destFileName = s.Path + source;
                    CopyFile cp = new CopyFile();
                    cp.SourceFileName = sourceFileName;
                    cp.DestFileName = destFileName;
                    lstCopyFile.Add(cp);
                }
            }

            return lstCopyFile;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<CopyFile> lspCopyFile = e.Argument as List<CopyFile>;
            decimal percent = 100M / lspCopyFile.Count;

            foreach (CopyFile cp in lspCopyFile)
            {
                int item = lspCopyFile.IndexOf(cp) + 1;
                int progress = (int)(percent * item);
                decimal d1 = percent * item;
                string strUserState = string.Format("Copying... '{0}' to '{1}'", cp.SourceFileName, cp.DestFileName);

                string message = string.Empty;
                bool ret = Utils.UploadFile(cp.SourceFileName, cp.DestFileName, ref message);

                if (!ret)
                {
                    throw new Exception(message);
                }

                backgroundWorker1.ReportProgress(progress, strUserState);

                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //System.Threading.Thread.Sleep(100);
            }

            backgroundWorker1.ReportProgress(100, string.Empty);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = e.UserState as string;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // Cancel
                MessageBox.Show("Cancel", _MessageTitle);
            }
            else if (e.Error != null)
            {
                // Error
                MessageBox.Show(e.Error.Message, _MessageTitle);
            }
            else
            {
                // Done
                MessageBox.Show("Done", _MessageTitle);
            }

            cboSource.Enabled = true;
            trvSource.Enabled = true;
            cbxlDestination.Enabled = true;
            btnDeploy.Enabled = true;
            btnCancel.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Text = string.Empty;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}
