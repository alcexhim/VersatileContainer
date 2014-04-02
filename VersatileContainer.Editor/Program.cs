using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VersatileContainer.Editor
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            UniversalEditor.ObjectModels.VersatileContainer.VersatileContainerObjectModel vcom = new UniversalEditor.ObjectModels.VersatileContainer.VersatileContainerObjectModel();
            UniversalEditor.DataFormats.VersatileContainer.VersatileContainerDataFormat vcdf = new UniversalEditor.DataFormats.VersatileContainer.VersatileContainerDataFormat();
            vcdf.Open(@"C:\Users\Michael Becker\Desktop\New folder\HatsuneMiku.vmo");
            vcdf.Load<UniversalEditor.ObjectModels.VersatileContainer.VersatileContainerObjectModel>(ref vcom);
            vcdf.Close();

            /*
            UniversalEditor.Plugins.Multimedia3D.ObjectModels.Model.ModelObjectModel pmdo = new UniversalEditor.Plugins.Multimedia3D.ObjectModels.Model.ModelObjectModel();
            UniversalEditor.DataFormats.AniMiku.ExtendedPMD.ExtendedPMDDataFormat pmdf = new UniversalEditor.DataFormats.AniMiku.ExtendedPMD.ExtendedPMDDataFormat();
            pmdf.Open(@"C:\Applications\AniMiku\models\Mamama_Miku\MikuProjectDIVAstyleMB.apmd");
            pmdf.Load<UniversalEditor.Plugins.Multimedia3D.ObjectModels.Model.ModelObjectModel>(ref pmdo);
            pmdf.Close();
            */

			try
			{
				Application.Run(new MainForm());
			}
			catch (System.IO.FileNotFoundException ex)
			{
				MessageBox.Show("Cannot find one or more necessary libraries that are required for this program to run.  Contact the program vendor to resolve this issue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

            DeleteTemporaryPath();
		}

        private static string TemporaryPath = null;
        public static string GetTemporaryPath()
        {
            if (TemporaryPath == null)
            {
                Random random = new Random();
                TemporaryPath = System.IO.Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar.ToString() + "~FSE" + random.Next(0, 9999).ToString().PadLeft(4, '0');
                while (System.IO.Directory.Exists(TemporaryPath))
                {
                    // Attempt to find another random temporary path.
                    TemporaryPath = System.IO.Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar.ToString() + "~FSE" + random.Next().ToString().PadLeft(4, '0');
                }

                System.IO.Directory.CreateDirectory(TemporaryPath);
            }
            return TemporaryPath;
        }
        public static void DeleteTemporaryPath()
        {
            if (TemporaryPath == null) return;
            if (System.IO.Directory.Exists(TemporaryPath))
            {
                System.IO.Directory.Delete(TemporaryPath, true);
            }
        }
	}
}
