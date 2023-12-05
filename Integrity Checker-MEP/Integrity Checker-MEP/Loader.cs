using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using wf = System.Windows.Forms;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api.DocumentParts;
using Application = Autodesk.Navisworks.Api.Application;
using ClashTest2;

namespace Integrity_Checker_MEP {
    public class MainClass2{
        public int Execute(params string[] parameters) {
            // current document
    
            form_ResultViewer rv = new form_ResultViewer();
            rv.Show();


            return 0;
        }

        //todo 닫기 구현

        public Document doc;
        //부재에 칠할 색
        Color[] colors = {Color.Green, Color.Red};

        public void SelectObjects(string guid1, string guid2) {
            doc = Application.ActiveDocument;
            doc.Models.ResetAllTemporaryMaterials();
            ColorTarget(guid1, 0);
            ColorTarget(guid2, 1);
        }

        /// <summary>
        /// guid를 가진 오브젝트를 index에 맞는 색으로 칠하기
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="index"></param>
        void ColorTarget(string guid, int index) {
            if(!(index == 0 || index == 1)) {
                ShowMessage($"Error : Wrong index({index})");
                return;
            }

            SelectObjectWithGUID(guid);
            doc.Models.OverrideTemporaryColor(doc.CurrentSelection.SelectedItems, colors[index]);
            doc.CurrentSelection.Clear();
        }

        /// <summary>
        /// guid를 이용해 오브젝트를 찾고 선택함
        /// </summary>
        /// <param name="guid"></param>
        public void SelectObjectWithGUID(string guid) {
            // 검색 객체 생성
            Search search = new Search();
            // 검색 범위 지정
            search.Selection.SelectAll();
            // 검색 조건 생성(요소-IfcGUID == guid)
            SearchCondition condition = SearchCondition.HasPropertyByDisplayName("요소", "IfcGUID").EqualValue(new VariantData(guid));
            // 검색 조건 적용
            search.SearchConditions.Add(condition);
            // collect model item (if found)
            ModelItem item = search.FindFirst(doc, false);
            if (item != null) {
                doc.CurrentSelection.Add(item);
            }
        }

        void ShowMessage(string content) {
            wf.MessageBox.Show(content);
        }
    }


}
