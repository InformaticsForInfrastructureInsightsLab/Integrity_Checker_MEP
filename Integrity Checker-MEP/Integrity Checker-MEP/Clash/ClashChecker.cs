using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using wf = System.Windows.Forms;
using Autodesk.Navisworks.Api.Clash;

namespace Integrity_Checker_MEP
{
    public class ClashChecker : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            Document doc = Application.ActiveDocument;
            
            From_Log form_log = new From_Log();
            ILogger logger = new FormLogger(form_log);
            
            try
            {
                form_log.Show();
                logger.Log("Integrity Check Started.");

                // 1. Get User Settings from Forms
                IFCLoad ifcLoader = new IFCLoad();
                ifcLoader.ShowDialog();
                if (!ifcLoader.load) 
                {
                    logger.Log("Operation cancelled by user at IFC load.");
                    return 0;
                }

                Form_Setting form_setting = new Form_Setting();
                form_setting.ShowDialog();
                if (!form_setting.start)
                {
                    logger.Log("Operation cancelled by user at settings form.");
                    return 0;
                }

                // 2. Initialize Managers
                var clashTestManager = new ClashTestManager(logger, doc);
                var reportGenerator = new ReportGenerator(logger);
                var fileManager = new FileManager(logger);
                var serverUploader = new ServerUploader(logger);
                var clashResultProcessor = new ClashResultProcessor(logger, form_setting, doc, reportGenerator);


                // 3. Main Workflow
                
                // Clear previous results and run new tests
                if (form_setting.export_Result)
                {
                    //doc.GetClash().TestsData.TestsClear();
                    doc.GetClash().TestsData.TestsClear();
                    clashTestManager.RunAllTests();
                    
                    // Process results and generate reports
                    clashResultProcessor.ProcessAndReport();
                }

                // Save Images
                if (form_setting.Save_image)
                {
                    var imageService = new ImageCreation.ImageService(logger);
                    var imageTasks = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.TestsDictFileName)));
                    string sideImagePath = Path.Combine(ProjectSettings.ResultImageFolderPath, ProjectSettings.SideImageFolderName);
                    Directory.CreateDirectory(sideImagePath);
                    imageService.SaveImages(imageTasks, sideImagePath);
                }
                
                // The GetSpaceHeights logic has been removed for this refactoring.
                // It can be added back into the ClashResultProcessor if needed.
                string infoFile = null; 

                // Create Zip and Upload
                if (form_setting.Make_ZipFile)
                {
                    string clashFile = Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.AllInOneReportName);
                    fileManager.CreateZipFile(clashFile, infoFile, form_setting.Save_image, ifcLoader.usedmodel);
                    
                    if (form_setting.Send_toServer)
                    {
                        string zipPath = Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.CompressedZipFileName);
                        string response = serverUploader.SendToServer(zipPath);
                        ShowMessage(response);
                    }
                }

                // Save Log File
                if (form_setting.Save_log)
                {
                    SaveLog(form_log);
                }

                logger.Log("Integrity Check Finished.");
            }
            catch (Exception ex)
            {
                logger.Log("An unhandled exception occurred: " + ex.ToString());
                ShowMessage("An error occurred. Please check the log for details.");
            }
            return 0;
        }

        private void SaveLog(From_Log logForm)
        {
            string path = Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.LogFileName);
            StringBuilder sb = new StringBuilder();
            
            if (logForm.InvokeRequired)
            {
                logForm.Invoke(new Action(() => {
                    foreach (var item in logForm.lst_progress.Items)
                    {
                        sb.AppendLine(item.ToString());
                    }
                }));
            }
            else
            {
                foreach (var item in logForm.lst_progress.Items)
                {
                    sb.AppendLine(item.ToString());
                }
            }
            
            File.WriteAllText(path, sb.ToString());
        }

        private void ShowMessage(string content)
        {
            wf.MessageBox.Show(content);
        }
    }
}