using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Interop.ComApi;
using Autodesk.Navisworks.Api.ComApi;
using System.Security.Claims;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

namespace Integrity_Checker_MEP
{
    using clashResultDictType = Dictionary<string, List<string>>;

    class ExtractionList
    {
        protected List<ClashResult> outedResults;

        public ExtractionList() {
            outedResults = new List<ClashResult>();
        }

        public void make_list(clashResultDictType result_dict)
        {
            // 현재 나비스에 떠 있는 테스트 결과를 가져온다
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;

            foreach (ClashTest test in oDCT.Tests)
            {
                if (result_dict.ContainsKey(test.DisplayName))
                {
                    foreach (SavedItem child in test.Children)
                    {
                        
                        outedResults.Add(search_result(child));
                    }
                }
            }
            doc.Models.ResetAllHidden();
        }

        public void create_fill_images()
        {

        }

        private ClashResult search_result(SavedItem child)
        {
            /* If we only wanted to access first-level children
                 * without reference to whether they were groups or results
                 * then we could:
                 *
                 * // access groups and results via shared interface
                 * IClashResult result = child as IClashResult;
                 * 
                 * operate on that and not recurse further. */

            // GroupItem is the base-class of ClashResultGroup which defines
            // group-like behaviour, if we needed to access ay ClashResultGroup properties

            // we could cast to that equivalently... 
            GroupItem child_group = child as GroupItem;
            while (child_group != null) {
                child_group = child as GroupItem;
            }
            ClashResult result = child as ClashResult;
            return result;
        }
    }

    class ImageCreator : ExtractionList
    {
        // 너비와 높이 수동 조정
        private int width = 500;
        private int height = 500;

        // 투명도
        public int transparancy;

        // 카메라 배율
        public double setMagnification = 1500.0;

        private form_ImageOption form_imageOption;
        private From_Log log;

        public ImageCreator(form_ImageOption fi, From_Log form_Log) : base()
        {
            form_imageOption = fi;

            transparancy = fi.transparant ? fi.transparancy : 0;
            log = form_Log;
        }
    }
}// namespace Integrity_Checker_MEP