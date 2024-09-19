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
using System.Collections.Concurrent;
using System.Threading;

namespace Integrity_Checker_MEP
{
    using static System.Net.Mime.MediaTypeNames;
    using clashResultDictType = Dictionary<string, List<string>>;
    abstract class ImageCreatorBase
    {
        protected static Mutex mutex = new Mutex();
        public abstract void CreateAndFillImage(string path, clashResultDictType clash_result_dict);
        public abstract void image_thread(clashResultDictType clash_result_dict, ClashTest test);

        protected void extract(GroupItem group, ref List<ClashResult> outedResults)
        {
            foreach (SavedItem child in group.Children)
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

                // is this a group?
                if (child_group != null)
                {
                    // operate on the group's children
                    extract(child_group, ref outedResults);
                }
                else
                {
                    // Not a group, so must be a result.
                    ClashResult result = child as ClashResult;
                    if (outedResults != null)
                    {
                        outedResults.Add(result);
                    }
                }
            }
        }
    }

    class ImageCreatorSimple : ImageCreatorBase { 
        public ImageCreatorSimple() { }

        override public void CreateAndFillImage(string path, clashResultDictType clash_result_dict) 
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;

            

            Parallel.ForEach(oDCT.Tests.Cast<ClashTest>(), (ClashTest test) =>
            {
                image_thread(clash_result_dict, test);
            });
        }

        public override void image_thread(clashResultDictType clash_result_dict, ClashTest test)
        {
            lock (clash_result_dict)
            {
                // 없으면 패스
                if (!clash_result_dict.ContainsKey(test.DisplayName))
                    return;
            }

            List<ClashResult> outed_results = new List<ClashResult>();
            extract(test, ref outed_results);
        }
    }

    class ImageCreatorComplex : ImageCreatorBase
    {
        public ImageCreatorComplex() { }

        public override void CreateAndFillImage(string path, clashResultDictType clash_result_dict)
        {
            throw new NotImplementedException();
        }

        public override void image_thread(clashResultDictType clash_result_dict, ClashTest test)
        {
            throw new NotImplementedException();
        }
    }
}//namespace Integrity_Checker_MEP