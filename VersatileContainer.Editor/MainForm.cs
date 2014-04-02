using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using UniversalEditor.ObjectModels.VersatileContainer;
using UniversalEditor.DataFormats.VersatileContainer;
using UniversalEditor.ObjectModels.FileSystem;

namespace VersatileContainer.Editor
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			RefreshEditor();
		}

		private FileSystemObjectModel vcom = new FileSystemObjectModel();
		private UniversalEditor.ObjectModelReference _omr = null;

		private void FileOpen_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			vcom = new FileSystemObjectModel();
			if (_omr == null)
			{
				_omr = vcom.MakeReference();
			}
			ofd.Filter = UniversalEditor.Common.Dialog.GetCommonDialogFilter(_omr);
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					UniversalEditor.Common.Reflection.GetAvailableObjectModel<FileSystemObjectModel>(ofd.FileName, ref vcom);
				}
				catch (UniversalEditor.DataFormatException ex)
				{
					MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				RefreshEditor();
			}
		}

		private Folder currentFolder = null;
		private void RefreshEditor()
		{
			lv.Items.Clear();
			tv.Nodes.Clear();

			if (vcom == null)
			{
				this.Text = "Versatile Container Editor";
				return;
			}
			else
			{
				this.Text = System.IO.Path.GetFileName(vcom.FileName) + " - Versatile Container Editor";
			}

			txtComment.Text = vcom.Title;
			/*
			txtSectionAlignment.Text = vcdf.SectionAlignment.ToString();
			txtVersionMajor.Text = vcdf.Version.Major.ToString();
			txtVersionMinor.Text = vcdf.Version.Minor.ToString();
			txtVersionBuild.Text = vcdf.Version.Build.ToString();
			txtVersionRevision.Text = vcdf.Version.Revision.ToString();
			*/

			RefreshTreeView();
			RefreshListView();
		}

		private void RefreshTreeView()
		{
			TreeNode tnTopLevel = new TreeNode();
			tnTopLevel.Text = System.IO.Path.GetFileName(vcom.FileName);
			tnTopLevel.Tag = null;
			foreach (Folder folder in vcom.Folders)
			{
				RecursiveAddTreeNode(folder, tnTopLevel.Nodes);
			}
			tv.Nodes.Add(tnTopLevel);
		}

		private void RecursiveAddTreeNode(Folder folder, TreeNodeCollection collection)
		{
			TreeNode tn = new TreeNode();
			tn.Tag = folder;
			tn.Text = folder.Name;
			foreach (Folder folder1 in folder.Folders)
			{
				RecursiveAddTreeNode(folder1, tn.Nodes);
			}
			collection.Add(tn);
		}

		private void mnuFileExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void FileSave_Click(object sender, EventArgs e)
		{
			FileSaveAs_Click(sender, e);
		}
		private void FileSaveAs_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = UniversalEditor.Common.Dialog.GetCommonDialogFilter(vcom.MakeReference());
			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				/*
				vcom.Properties.Clear();
				vcom.Properties.Add("Title", "Kio's Miku Hatsune");
				vcom.Properties.Add("Subject", String.Empty);
				vcom.Properties.Add("Category", String.Empty);
				vcom.Properties.Add("Keywords", String.Empty);
				vcom.Properties.Add("Comments", String.Empty);
				vcom.Properties.Add("Source", String.Empty);
				vcom.Properties.Add("Author", "kio");
				vcom.Properties.Add("RevisionNumber", String.Empty);
				vcom.Properties.Add("CheckedBy", String.Empty);
				vcom.Properties.Add("Client", String.Empty);
				vcom.Properties.Add("DateCompleted", String.Empty);
				vcom.Properties.Add("Department", String.Empty);
				vcom.Properties.Add("Destination", String.Empty);
				vcom.Properties.Add("Disposition", String.Empty);
				vcom.Properties.Add("Division", String.Empty);
				vcom.Properties.Add("DocumentNumber", String.Empty);
				vcom.Properties.Add("Editor", String.Empty);
				vcom.Properties.Add("ForwardTo", String.Empty);
				vcom.Properties.Add("Group", String.Empty);
				vcom.Properties.Add("Language", String.Empty);
				vcom.Properties.Add("Mailstop", String.Empty);
				vcom.Properties.Add("Matter", String.Empty);
				vcom.Properties.Add("CheckedBy", String.Empty);
				vcom.Properties.Add("Office", String.Empty);
				vcom.Properties.Add("Owner", String.Empty);
				vcom.Properties.Add("Project", String.Empty);
				vcom.Properties.Add("Publisher", String.Empty);
				vcom.Properties.Add("Purpose", String.Empty);
				vcom.Properties.Add("ReceivedFrom", String.Empty);
				vcom.Properties.Add("RecordedBy", String.Empty);
				vcom.Properties.Add("RecordedDate", String.Empty);
				vcom.Properties.Add("Reference", String.Empty);
				vcom.Properties.Add("Source", String.Empty);
				vcom.Properties.Add("Status", String.Empty);
				vcom.Properties.Add("TelephoneNumber", String.Empty);
				vcom.Properties.Add("Typist", String.Empty);
				*/

				UniversalEditor.DataFormatReference[] dfrs = UniversalEditor.Common.Reflection.GetAvailableDataFormats(sfd.FileName);
				UniversalEditor.DataFormat df = dfrs[0].Create();
                df.EnableWrite = true;
                df.ForceOverwrite = true;

				df.Open(sfd.FileName);

                vcom.Title = txtComment.Text;
				df.Save(vcom);
				df.Close();

				tv.Nodes[0].Text = System.IO.Path.GetFileName(sfd.FileName);
			}
		}

		private void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null)
			{
				currentFolder = null;
				return;
			}
			currentFolder = (e.Node.Tag as Folder);
			RefreshListView();
		}

		private void RefreshListView()
		{
			lv.Items.Clear();
			if (currentFolder != null)
			{
				foreach (File file in currentFolder.Files)
				{
					ListViewItem lvi = new ListViewItem();
					lvi.Text = file.Name;
					lvi.Tag = file;
					lvi.SubItems.Add(file.GetDataAsByteArray().Length.ToString());
					if (file is CompressedFile)
					{
						lvi.SubItems.Add((file as CompressedFile).CompressionMethod.ToString());
					}
					else
					{
						lvi.SubItems.Add("(none)");
					}
					lv.Items.Add(lvi);
				}
			}
			else
			{
				foreach (File file in vcom.Files)
				{
					ListViewItem lvi = new ListViewItem();
					lvi.Text = file.Name;
					lvi.Tag = file;
					lvi.SubItems.Add(file.GetDataAsByteArray().Length.ToString());
					if (file is CompressedFile)
					{
						lvi.SubItems.Add((file as CompressedFile).CompressionMethod.ToString());
					}
					else
					{
						lvi.SubItems.Add("(none)");
					}
					lv.Items.Add(lvi);
				}
			}
		}

		private void lv_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			File file = (lv.Items[e.Item].Tag as File);
			if (file != null)
			{
				file.Name = e.Label;
			}
		}

		private void FileNew_Click(object sender, EventArgs e)
		{
			vcom = new FileSystemObjectModel();
			RefreshEditor();
		}

        private void lv_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (lv.SelectedItems.Count > 0)
            {
                List<string> FileNames = new List<string>();
                foreach (ListViewItem lvi in lv.SelectedItems)
                {
                    File file = (lvi.Tag as File);
                    string FileName = Program.GetTemporaryPath() + System.IO.Path.DirectorySeparatorChar.ToString() + file.Name;

                    file.Save(FileName);
                    FileNames.Add(FileName);
                }

                DataObject dobj = new DataObject("FileDrop", FileNames.ToArray());
                lv.DoDragDrop(dobj, DragDropEffects.Copy);
            }
        }

        private void lv_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void lv_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNames = (e.Data.GetData("FileDrop") as string[]);
			Folder parentFolder = null;
			if (tv.SelectedNode != null)
			{
				parentFolder = (tv.SelectedNode.Tag as Folder);
			}

			for (int i = fileNames.Length - 1; i > -1; i--)
			{
				string fileName = fileNames[i];

				if (System.IO.Directory.Exists(fileName))
				{
					RecursiveAddDirectory(fileName, tv.Nodes[0]);
				}
				else
				{
					RecursiveAddFile(fileName, parentFolder);
				}
            }
        }

		private void RecursiveAddFile(string fileName, Folder parentFolder)
		{
			File file = new File();
			file.Name = System.IO.Path.GetFileName(fileName);
			file.SetDataAsByteArray(System.IO.File.ReadAllBytes(fileName));

			if (parentFolder != null)
			{
				parentFolder.Files.Add(file);
			}
			else
			{
				vcom.Files.Add(file);

				ListViewItem lvi = new ListViewItem();
				lvi.Text = file.Name;
				lvi.Tag = file;
				lvi.SubItems.Add(file.GetDataAsByteArray().Length.ToString());
				lv.Items.Add(lvi);
			}
		}

		private void RecursiveAddDirectory(string fileName, TreeNode parentFolderNode)
		{
			Folder parentFolder = null;
			if (parentFolderNode != null)
			{
				parentFolder = (parentFolderNode.Tag as Folder);
			}

			Folder folder = new Folder();
			folder.Name = System.IO.Path.GetFileName(fileName);

			TreeNode tn = new TreeNode();
			tn.Text = folder.Name;
			tn.Tag = folder;

			string[] files = System.IO.Directory.GetFiles(fileName);
			string[] folders = System.IO.Directory.GetDirectories(fileName);
			foreach (string file in files)
			{
				RecursiveAddFile(file, folder);
			}
			foreach (string file in folders)
			{
				RecursiveAddDirectory(file, tn);
			}

			if (parentFolder != null)
			{
				parentFolder.Folders.Add(folder);
				parentFolderNode.Nodes.Add(tn);
			}
			else
			{
				vcom.Folders.Add(folder);
				tv.Nodes[0].Nodes.Add(tn);
			}
		}

		private void mnuContextTreeViewAddNewFolder_Click(object sender, EventArgs e)
		{
			Folder parent = (tv.SelectedNode.Tag as Folder);

			string folderName = "New Folder";
			if (parent != null && parent.Folders.Count > 0)
			{
				folderName = "New Folder (" + (parent.Folders.Count + 1).ToString() + ")";
			}
			else if (parent == null && vcom.Folders.Count > 0)
			{
				folderName = "New Folder (" + (vcom.Folders.Count + 1).ToString() + ")";
			}

			Folder newfol = new Folder();
			newfol.Name = folderName;
			if (parent == null)
			{
				vcom.Folders.Add(newfol);
			}
			else
			{
				parent.Folders.Add(newfol);
			}

			TreeNode tn = tv.SelectedNode.Nodes.Add(String.Empty);
			tn.Tag = newfol;
			tn.Text = folderName;
			tn.EnsureVisible();
			tn.BeginEdit();
		}

		private void mnuContextTreeView_Opening(object sender, CancelEventArgs e)
		{
			mnuContextTreeViewAdd.Visible = (tv.SelectedNode != null);
			mnuContextTreeViewSep1.Visible = (tv.SelectedNode != null);
			
			mnuContextTreeViewCut.Enabled = (tv.SelectedNode != null);
			mnuContextTreeViewCopy.Enabled = (tv.SelectedNode != null);
			mnuContextTreeViewDelete.Enabled = (tv.SelectedNode != null);
		}

		private void tv_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (e.Node == null) return;
			Folder folder = (e.Node.Tag as Folder);
			if (folder == null) return;

			if (e.Label == null)
			{
				e.CancelEdit = true;
			}
			else
			{
				folder.Name = e.Label;
			}
		}

		private void EditDelete_Click(object sender, EventArgs e)
		{
			if (tv.Focused)
			{
				Folder folder = (tv.SelectedNode.Tag as Folder);
				if (folder != null)
				{
					TreeNode parentNode = tv.SelectedNode.Parent;
					Folder parentFolder = null;
					if (parentNode != null)
					{
						parentFolder = (parentNode.Tag as Folder);
					}

					if (parentFolder != null)
					{
						parentFolder.Folders.Remove(folder);
					}
					else
					{
						vcom.Folders.Remove(folder);
					}

					tv.SelectedNode.Remove();
				}
			}
			else if (lv.Focused)
			{
				Folder folder = null;
				if (tv.SelectedNode != null)
				{
					folder = (tv.SelectedNode.Tag as Folder);
				}

				File.FileCollection files = null;
				if (folder != null)
				{
					files = folder.Files;
				}
				else
				{
					files = vcom.Files;
				}

				foreach (ListViewItem lvi in lv.SelectedItems)
				{
					File file = (lvi.Tag as File);
					if (file != null)
					{
						files.Remove(file);
					}
				}

				while (lv.SelectedItems.Count > 0)
				{
					lv.Items.Remove(lv.SelectedItems[0]);
				}
			}
		}
	}
}
