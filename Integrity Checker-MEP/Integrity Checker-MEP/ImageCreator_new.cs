using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api.Clash;
using detail;
using System.Windows.Forms;
using System.IO;
using Autodesk.Navisworks.Api;
using System.Windows.Input;

namespace Integrity_Checker_MEP
{
    using clashResultDictType = Dictionary<string, List<string>>;

    public class ImageCreator_new
    {
        private ImageCreatorSimple simple;
        private ImageCreatorComplex complex;

        private From_Log log;

        public ImageCreator_new(From_Log log, form_ImageOption image_option)
        {
            simple = new ImageCreatorSimple(log, image_option);
            complex = new ImageCreatorComplex(log, image_option);
            this.log = log;
        }

        public void save_image(clashResultDictType names, string path)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ImageCreatorBase.compare_tests_with_json(names, complex);
                stopwatch.Stop();
                log.UpdateLog($"간섭리스트 추출 완료, " +
                    $"경과시간 : {stopwatch.ElapsedMilliseconds}ms; " +
                    $"{ImageCreatorBase.outed_results_list.Count}개의 이미지 저장을 시작합니다");

                stopwatch = new Stopwatch();
                stopwatch.Start();

                var keysToRemove = new List<Guid>(); // 삭제할 키를 저장할 리스트
                foreach (var entry in complex.guid_dictionary)
                {
                    if (entry.Value.Count == 1)
                    {
                        keysToRemove.Add(entry.Key); // 삭제할 키를 따로 저장
                    }
                }
                // 컬렉션 수정은 루프 이후에 처리
                foreach (var key in keysToRemove)
                {
                    complex.guid_dictionary.Remove(key);
                }

                foreach (pair<string, ClashResult> p in ImageCreatorBase.outed_results_list)
                {
                    string test_display_name = p.first;
                    ClashResult result = p.second;

                    if (result == null) continue;

                    log.UpdateLog($"{test_display_name} {result.DisplayName} 이미지 추출");
                    simple.make_image(result, path, test_display_name); //단순이미지
                    complex.make_image(result, path, test_display_name); //복합이미지

                    deleteTexture();
                }
                stopwatch.Stop();
                log.UpdateLog($"모든 이미지 저장 완료, " +
                    $"경과시간: {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception e) {
                MessageBox.Show(e.ToString());
            }
            
        }

        private void deleteTexture()
        {
            try
            {
                string tempFolderPath = Path.GetTempPath(); // Get temp folder directory
                string ogsFolderPath = Path.Combine(tempFolderPath, "ogs");

                // Get first folder directory from ogs folder
                string[] ogsSubDirectories = Directory.GetDirectories(ogsFolderPath);
                if (ogsSubDirectories.Length > 0)
                {
                    string firstSubDirectory = ogsSubDirectories[0];

                    // Get first folder directory from subdirectory
                    string[] foldersInFirstSubDirectory = Directory.GetDirectories(firstSubDirectory);
                    if (foldersInFirstSubDirectory.Length > 0)
                    {
                        string firstFolderPath = foldersInFirstSubDirectory[0];

                        // For Debugging (Check if directory is temp/ogs/(number)/(number)_TextureCache
                        /*if (pathDebug)
                        {
                            MessageBox.Show(firstFolderPath);
                            pathDebug = false;
                        }*/

                        // Get file size info
                        DirectoryInfo directoryInfo = new DirectoryInfo(firstFolderPath);
                        FileInfo[] files = directoryInfo.GetFiles();
                        long fileSizeInBytes = 0;
                        foreach (FileInfo file in files)
                        {
                            fileSizeInBytes += file.Length;
                        }

                        long fileSizeInGB = fileSizeInBytes / (1024 * 1024 * 1024); // 크기를 기가바이트로 변환

                        // Delete TextureCache folder if size is more than 5GB
                        if (fileSizeInGB > 5)
                        {
                            Directory.Delete(firstFolderPath, true);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No folder in subdirectory");
                    }
                }
                else
                {
                    MessageBox.Show("No folder in ogs folder");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
