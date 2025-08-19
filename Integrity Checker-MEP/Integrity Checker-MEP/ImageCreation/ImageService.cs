using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Integrity_Checker_MEP.ImageCreation
{
    // This class replaces ImageCreator_new and ImageCreatorBase
    public class ImageService
    {
        private readonly ILogger _logger;
        private readonly SimpleImageStrategy _simpleStrategy;
        private readonly ComplexImageStrategy _complexStrategy;

        // This replaces the static outed_results_list
        private List<(string, ClashResult)> _processedResultsList = new List<(string, ClashResult)>();

        public ImageService(ILogger logger)
        {
            _logger = logger;
            _simpleStrategy = new SimpleImageStrategy(logger);
            _complexStrategy = new ComplexImageStrategy(logger);
        }

        public void SaveImages(Dictionary<string, List<string>> names, string directoryPath)
        {
            _logger.Log("이미지 추출 작업 시작");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Step 1: Process clash results to get the list of results to image
            ProcessClashResultsForImaging(names);
            stopwatch.Stop();
            _logger.Log($"간섭리스트 추출 완료, 경과시간 : {stopwatch.ElapsedMilliseconds}ms; {_processedResultsList.Count}개의 이미지 저장을 시작합니다");

            stopwatch.Restart();

            // Step 2: Handle guid_dictionary for complex strategy (moved from original ImageCreator_new)
            var keysToRemove = new List<Guid>();
            foreach (var entry in _complexStrategy.guid_dictionary)
            {
                if (entry.Value.Count == 1)
                {
                    keysToRemove.Add(entry.Key);
                }
            }
            foreach (var key in keysToRemove)
            {
                _complexStrategy.guid_dictionary.Remove(key);
            }

            // Step 3: Generate images using strategies
            foreach (var p in _processedResultsList)
            {
                if (p.Item1 == null || p.Item2 == null) continue;

                string test_display_name = p.Item1;
                ClashResult result = p.Item2;

                _logger.Log($"{test_display_name} {result.DisplayName} 이미지 추출");
                _simpleStrategy.MakeImage(result, directoryPath, test_display_name);
                _complexStrategy.MakeImage(result, directoryPath, test_display_name);
            }
            stopwatch.Stop();
            _logger.Log($"모든 이미지 저장 완료, 경과시간: {stopwatch.ElapsedMilliseconds}ms");

            // Step 4: Clean up temp textures (moved from original ImageCreator_new)
            DeleteTempTextures();
        }

        // This method replaces the static ImageCreatorBase.compare_tests_with_json
        private void ProcessClashResultsForImaging(Dictionary<string, List<string>> names)
        {
            _processedResultsList.Clear(); // Clear previous results

            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;

            var tempProcessedResults = new System.Collections.Concurrent.ConcurrentBag<(string, ClashResult)>();

            Parallel.ForEach(oDCT.Tests.Cast<ClashTest>(), (ClashTest test) =>
            {
                if (names.ContainsKey(test.DisplayName))
                {
                    List<ClashResult> outedResults = new List<ClashResult>();
                    ExtractClashResults(test, outedResults); // Renamed from 'extract'

                    Parallel.ForEach(outedResults, (ClashResult r) =>
                    {
                        List<string> results = names[test.DisplayName];
                        if (results.Contains(r.DisplayName))
                        {
                            tempProcessedResults.Add((test.DisplayName, r));
                            if (!(r.Distance > 0))
                            {
                                lock (_complexStrategy.guid_dictionary) // Ensure thread-safety
                                {
                                    if (!_complexStrategy.guid_dictionary.ContainsKey(r.Item1.InstanceGuid))
                                    {
                                        _complexStrategy.guid_dictionary[r.Item1.InstanceGuid] = new List<ModelItem>();
                                    }
                                    _complexStrategy.guid_dictionary[r.Item1.InstanceGuid].Add(r.Item2);
                                }
                            }
                        }
                    });
                }
            });
            _processedResultsList.AddRange(tempProcessedResults);
        }

        private void ExtractClashResults(GroupItem group, List<ClashResult> outedResults)
        {
            foreach (SavedItem child in group.Children)
            {
                GroupItem child_group = child as GroupItem;
                if (child_group != null)
                {
                    ExtractClashResults(child_group, outedResults);
                }
                else
                {
                    ClashResult result = child as ClashResult;
                    if (result != null)
                    {
                        outedResults.Add(result);
                    }
                }
            }
        }

        // This method replaces the deleteTexture method from ImageCreator_new
        private void DeleteTempTextures()
        {
            try
            {
                string tempFolderPath = Path.GetTempPath();
                string ogsFolderPath = Path.Combine(tempFolderPath, "ogs");

                if (Directory.Exists(ogsFolderPath))
                {
                    string[] ogsSubDirectories = Directory.GetDirectories(ogsFolderPath);
                    if (ogsSubDirectories.Length > 0)
                    {
                        string firstSubDirectory = ogsSubDirectories[0];
                        string[] foldersInFirstSubDirectory = Directory.GetDirectories(firstSubDirectory);
                        if (foldersInFirstSubDirectory.Length > 0)
                        {
                            string firstFolderPath = foldersInFirstSubDirectory[0];
                            
                            DirectoryInfo directoryInfo = new DirectoryInfo(firstFolderPath);
                            FileInfo[] files = directoryInfo.GetFiles();
                            long fileSizeInBytes = 0;
                            foreach (FileInfo file in files)
                            {
                                fileSizeInBytes += file.Length;
                            }
        
                            long fileSizeInGB = fileSizeInBytes / (1024 * 1024 * 1024);

                            if (fileSizeInGB > 5) // Magic number, consider moving to ProjectSettings
                            {
                                Directory.Delete(firstFolderPath, true);
                                _logger.Log($"Deleted temp texture folder: {firstFolderPath} (size > 5GB)");
                            }
                        }
                        else
                        {
                            _logger.Log("No folder in subdirectory of ogs folder.");
                        }
                    }
                    else
                    {
                        _logger.Log("No folder in ogs folder.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error deleting temp textures: {ex.ToString()}");
            }
        }
    }
}
