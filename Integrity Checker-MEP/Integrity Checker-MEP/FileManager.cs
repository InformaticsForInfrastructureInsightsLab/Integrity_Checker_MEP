using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Integrity_Checker_MEP
{
    public class FileManager
    {
        private readonly ILogger _logger;

        public FileManager(ILogger logger)
        {
            _logger = logger;
        }

        public void CreateZipFile(string clashFile, string infoFile, bool saveImage, IEnumerable<string> usedModels)
        {
            _logger.Log("압축파일 만들기 시작");
            try
            {
                string compressedFolderPath = Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.CompressedFolderName);
                if (Directory.Exists(compressedFolderPath))
                {
                    Directory.Delete(compressedFolderPath, true);
                }
                Directory.CreateDirectory(compressedFolderPath);

                // Copy result file
                if (string.IsNullOrEmpty(clashFile))
                {
                    _logger.Log("(경고) 간섭 결과 파일이 없습니다.");
                }
                else
                {
                    File.Copy(clashFile, Path.Combine(compressedFolderPath, Path.GetFileName(clashFile)), true);
                }

                // Copy info file
                if (string.IsNullOrEmpty(infoFile))
                {
                    _logger.Log("(경고) 객체 정보 파일이 없습니다.");
                }
                else
                {
                    File.Copy(Path.Combine(ProjectSettings.OutputFolderPath, infoFile), Path.Combine(compressedFolderPath, infoFile), true);
                }

                // Copy model files
                string[] files = Directory.GetFiles(ProjectSettings.ModelFolderPath);
                foreach (string file in files)
                {
                    if (file.Contains(".ifc") && !file.Contains(".ifc."))
                    {
                        string modelName = Path.GetFileNameWithoutExtension(file);
                        // A bit of a simplification here, assuming the file name contains one of the model names.
                        foreach(string standardName in ProjectSettings.ModelNames)
                        {
                            if(modelName.Contains(standardName))
                            {
                                File.Copy(file, Path.Combine(compressedFolderPath, standardName + ".ifc"), true);
                                break;
                            }
                        }
                    }
                }

                // Copy image files
                if (saveImage)
                {
                    if (Directory.Exists(ProjectSettings.ResultImageFolderPath))
                    {
                        DirectoryCopy(ProjectSettings.ResultImageFolderPath, Path.Combine(compressedFolderPath, "ResultImage"), true);
                    }
                    else
                    {
                        _logger.Log("(경고) 결과 이미지 폴더가 없습니다.");
                    }
                }

                // Create used model info file
                File.WriteAllLines(Path.Combine(compressedFolderPath, ProjectSettings.UsedModelInfoFileName), usedModels);
                
                // Create zip archive
                string zipPath = Path.Combine(ProjectSettings.OutputFolderPath, ProjectSettings.CompressedZipFileName);
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                ZipFile.CreateFromDirectory(compressedFolderPath, zipPath);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
            }
            _logger.Log("압축파일 만들기 완료");
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            Directory.CreateDirectory(destDirName);

            foreach (FileInfo file in dir.GetFiles())
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
