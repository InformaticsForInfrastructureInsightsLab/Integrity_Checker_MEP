using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using Autodesk.Navisworks.Api;
using Color = Autodesk.Navisworks.Api.Color;
using ListviewTest;
using Integrity_Checker_MEP;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Integrity_Checker_MEP.Forms;

namespace ClashTest2
{
    public partial class form_ResultViewer : Form
    {

        // List for data
        public List<ClashData> dataList = new List<ClashData>();
        List<ClashData> dataHardList = new List<ClashData>();
        List<ClashData> dataSoftList = new List<ClashData>();
        List<ClashData> dataNullList = new List<ClashData>();

        // enum used for the header of columns in ListView
        // When header is changed, change the enum and add from designer
        enum Header
        {
            Element1Guid,
            Element2Guid,
            Type,
            MovabilityValue,
            Topology,
            HardClashType,
            SoftClashType,
            Severity,
            Adjusted_Severity,
            Clearance,
            MovabilityResult,
            MovablSpace,
            MovableDistance,
            Offset,
            Penetration,
            ABS_Volume_Diff,
            ABS_Volume_SUM,
            ClashVolume
        }

        // bool for checked column header
        private bool[] headerBool;

        // number of each result
        public int major_hard = 0, major_soft = 0;
        public int medium_hard = 0, medium_soft = 0;
        public int minor_hard = 0, minor_soft = 0;

        // string for 2 selected guid
        private string guid1;
        private string guid2;

        #region Form

        /// <summary>
        /// Initialize form
        /// </summary>
        public form_ResultViewer()
        {


            InitializeComponent();

            /*dataList = getData();
            addDataToList();*/


            folv.ShowGroups = true;
            doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
        }

        /// <summary>
        /// Gets data from C:\\objectinfo\\ResultFile.csv
        /// </summary>
        /// <returns></returns>
        List<ClashData> getData()
        {
            List<ClashData> clashDataList = new List<ClashData>();

            try
            {
                string path = "C:\\objectinfo\\ResultFile.csv";
                StreamReader file = new StreamReader(path);
                string firstLine = file.ReadLine();

                while (!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    string pattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
                    string[] stringdata = Regex.Split(line, pattern);

                    for (int i = 0; i < stringdata.Length; i++)
                    {
                        // Remove leading and trailing double quotes if present
                        stringdata[i] = stringdata[i].Trim('"');
                    }                  
                    clashDataList.Add(new ClashData(stringdata));
                }
                return clashDataList;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("File not found: " + ex.Message);
                return null;
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Directory not found: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Add data to each hard/soft list
        /// Increase count
        /// </summary>
        void addDataToList()
        {
            if (dataList == null) return;

            foreach (ClashData clash in dataList)
            {
                if (clash.HardClashType == "   ")
                {
                    dataSoftList.Add(clash);
                    switch(clash.Adjusted_Severity)
                    {
                        case "Major":
                            major_soft++;  break;
                        case "Medium":
                            medium_soft++; break;
                        case "Minor":
                            minor_soft++; break;
                    }
                }
                else if (clash.SoftClashType == "   ")
                {
                    dataHardList.Add(clash);
                    switch (clash.Adjusted_Severity)
                    {
                        case "Major":
                            major_hard++; break;
                        case "Medium":
                            medium_hard++; break;
                        case "Minor":
                            minor_hard++; break;
                    }
                }
            }
        }

        /// <summary>
        /// when button "Load" is clicked
        /// load data to listview from list
        /// use FastObjectListView <see href="https://objectlistview.sourceforge.net/cs/index.html"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void click_btn_Load(object sender, EventArgs e)
        {
            // return when the list is not empty
            if (folv.Items.Count > 0)
            {
                return;
            }
            try
            {
                string path = "C:\\objectinfo\\ResultFile.csv";
                StreamReader file = new StreamReader(path);
                string firstLine = file.ReadLine();
                string[] header = firstLine.Split(',');

                SelectHeader selectHeader = new SelectHeader();
                selectHeader.initializeHeaderBool(header.Length);
                selectHeader.StartPosition = FormStartPosition.CenterParent;
                selectHeader.ShowDialog();
                if (selectHeader.isCanceled) return;

                dataList = getData();
                addDataToList();

                // Only group by severity -> if canceled can be grouped by other headers
                folv.AlwaysGroupByColumn = Adjusted_Severity;
                // MVC pattern -> check objectListView 
                folv.SetObjects(dataList);
                folv.BuildGroups(Adjusted_Severity, SortOrder.None);

                tog_Hard.Checked = true;
                tog_Soft.Checked = true;
                tog_Hard.Enabled = true;
                tog_Soft.Enabled = true;

                // resize column header
                changeColumnHeader(headerBool);

                // Show Severity_Clashtype
                showEachClashNuminfo();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("File not found: " + ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Directory not found: " + ex.Message);
            }
        }

        /// <summary>
        /// Download ResultFile.csv from server
        /// </summary>
        /// <returns></returns>
        private async Task downloadFromServer()
        {
            string serverUrl = "http://117.17.196.59:3116/final"; // 서버 주소를 적절히 변경하세요
            string downloadDir = "C:\\objectinfo\\"; // 다운로드할 디렉토리를 적절히 변경하세요

            using (HttpClient httpClient = new HttpClient())
            {
                // GET 요청을 보내고 응답을 받습니다.
                HttpResponseMessage response = await httpClient.GetAsync(serverUrl);

                if (response.IsSuccessStatusCode)
                {
                    // 응답으로 받은 파일을 저장합니다.
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        string filePath = Path.Combine(downloadDir, "ResultFile.csv");
                        using (FileStream fileStream = File.Create(filePath))
                        {
                            await contentStream.CopyToAsync(fileStream);
                        }
                        Console.WriteLine($"파일 다운로드 완료: {filePath}");
                    }
                }
                else
                {
                    Console.WriteLine($"요청 실패. 상태 코드: {response.StatusCode}");
                }
            }
        }

        /// <summary>
        /// When selected row is changed in the listview
        /// Get item's guid1 and guid2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void folv_SelectionChanged(object sender, EventArgs e)
        {
            guid1 = folv.SelectedItem.GetSubItem((int)Header.Element1Guid).Text;
            guid2 = folv.SelectedItem.GetSubItem((int)Header.Element2Guid).Text;
            showGUID();
            SelectClash(guid1, guid2);
        }

        /// <summary>
        /// Shows GUID under the listview
        /// </summary>
        private void showGUID()
        {
            ID_GUID1.Text = "Guid1: " + guid1 + "  Guid2: " + guid2;
        }

        /// <summary>
        /// When Hard button checked is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tog_Hard_CheckedChanged(object sender, EventArgs e)
        {
            if (tog_Hard.Checked && tog_Soft.Checked)
            {
                folv.SetObjects(dataList);
            }
            else if (!tog_Hard.Checked && tog_Soft.Checked)
            {
                folv.SetObjects(dataSoftList);
            }
            else if (tog_Hard.Checked && !tog_Soft.Checked)
            {
                folv.SetObjects(dataHardList);
            }
            else if (!tog_Hard.Checked && !tog_Soft.Checked)
            {
                folv.SetObjects(dataNullList);
            }

        }

        /// <summary>
        /// When Soft button is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tog_Soft_CheckedChanged(object sender, EventArgs e)
        {
            if (tog_Hard.Checked && tog_Soft.Checked)
            {
                folv.SetObjects(dataList);
            }
            else if (!tog_Hard.Checked && tog_Soft.Checked)
            {
                folv.SetObjects(dataSoftList);
            }
            else if (tog_Hard.Checked && !tog_Soft.Checked)
            {
                folv.SetObjects(dataHardList);
            }
            else if (!tog_Hard.Checked && !tog_Soft.Checked)
            {
                folv.SetObjects(dataNullList);
            }

        }

        /// <summary>
        /// When Download button is clicked
        /// Download from server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void click_btn_Download(object sender, EventArgs e)
        {
            await downloadFromServer();
        }

        /// <summary>
        /// Change column header width or hide from list
        /// Primary column cannot be hidden in ObjectListView
        /// </summary>
        /// <param name="changedHeaders">Initial header bool values or changed bool values from form_SelectRow</param>
        public void changeColumnHeader(bool[] changedHeaders)
        {
            headerBool = changedHeaders;

            if(folv.Columns.Count >0)
            {
                // Primary column cannot be hidden -> width = 0 instead of changing IsVisible
                // Change accordingly
                if (headerBool[0] == false)
                {
                    folv.Columns[(int)Header.Element1Guid].Width = 0;
                }
                else
                {
                    folv.Columns[(int)Header.Element1Guid].Width = 81;
                }
                Element2Guid.IsVisible = headerBool[(int)Header.Element2Guid];
                Type.IsVisible = headerBool[(int)Header.Type];
                MovabilityValue.IsVisible = headerBool[(int)Header.Severity];
                Topology.IsVisible = headerBool[(int)Header.Topology];
                HardClashType.IsVisible = headerBool[(int)Header.HardClashType];
                SoftClashType.IsVisible = headerBool[(int)Header.SoftClashType];
                Severity.IsVisible = headerBool[(int)Header.Severity];
                Clearance.IsVisible = headerBool[(int)Header.Clearance];
                MovabilityResult.IsVisible = headerBool[(int)Header.MovabilityResult];
                Offset.IsVisible = headerBool[(int)Header.Offset];

                folv.RebuildColumns();
            }
        }

        /// <summary>
        /// Show label for each Severity_ClashType
        /// Show numbers of each Severity_ClashType
        /// </summary>
        private void showEachClashNuminfo() {
            majorHard.Visible = true;
            majorSoft.Visible = true;
            mediumHard.Visible = true;
            mediumSoft.Visible = true;
            minorHard.Visible = true;
            minorSoft.Visible = true;

            majorHard.Text = "MAJOR_H:" + major_hard.ToString();
            mediumHard.Text = "MEDIUM_H:" + medium_hard.ToString();
            minorHard.Text = "MINOR_H:" + minor_hard.ToString();

            majorSoft.Text = "MAJOR_H:" + major_soft.ToString();
            mediumSoft.Text = "MEDIUM_H:" + medium_soft.ToString();
            minorSoft.Text = "MINOR_H:" + minor_soft.ToString();
        }

        /// <summary>
        /// When SelectHeader button clicked
        /// Load SelectRow Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void click_btn_SelectHeader(object sender, EventArgs e)
        {
            SelectHeader selectHeader = new SelectHeader();
            selectHeader.getHeaderBool(headerBool, headerBool.Length);
            selectHeader.StartPosition = FormStartPosition.CenterParent;
            selectHeader.ShowDialog();
        }

        /// <summary>
        /// When Item1 button clicked
        /// Select Item1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void click_btn_Item1(object sender, EventArgs e)
        {
            if(folv.SelectedIndices.Count > 0)
            {
                doc.CurrentSelection.Clear();
                SelectObjectWithGUID(folv.SelectedItem.GetSubItem((int)Header.Element1Guid).Text);
            }

        }

        /// <summary>
        /// When Item2 button clicked
        /// Select Item2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void click_btn_Item2(object sender, EventArgs e)
        {
            if (folv.SelectedIndices.Count > 0)
            {
                doc.CurrentSelection.Clear();
                SelectObjectWithGUID(folv.SelectedItem.GetSubItem((int)Header.Element2Guid).Text);
            }
        }

        private void click_btn_load_chat(object sender, EventArgs e)
        {
            Chat chat = new Chat();
            Chat.ShowMyWindow();
        }

        private void click_btn_dashboard(object sender, EventArgs e)
        {
            Form_Dashboard dashboard = new Form_Dashboard();
            dashboard.ShowDialog();
        }
        #endregion

        #region Navisworks

        public Document doc;
        Color[] colors = { Color.Green, Color.Red }; //부재에 칠할 색
        public bool trans = false;
        public bool hide = false;
        ModelItemCollection invertItemCollection = new ModelItemCollection(); // 선택 부재 외 나머지 부재

        /// <summary>
        /// 간섭 결과가 선택 되었을 때 호출. 현재 모델에 선택한 간섭 표시
        /// </summary>
        /// <param name="_guid1"></param>
        /// <param name="_guid2"></param>
        public void SelectClash(string _guid1, string _guid2) {
            // 추후 모델이 없을 때 load가 되지 않도록 수정하기 
            if (doc.Models.Count < 1)
            {
                return;
            }
            //모든 부재의 색, 숨김 초기화
            doc.Models.ResetAllTemporaryMaterials();
            doc.Models.ResetAllHiddenToModelState();
            
            ModelItem item1 = SelectObjectWithGUID(_guid1); //item1 선택
            ModelItem item2 = SelectObjectWithGUID(_guid2); //item2 선택
            invertItemCollection.CopyFrom(doc.CurrentSelection.SelectedItems); //item1, item2가 선택되어 있음
            invertItemCollection.Invert(doc); // 선택 반전 (전체 - item1 - item2)
            ColorTarget(); // item1, item2 외 모든 부재 색칠하기
            ColorTarget(item1, 0); // item1 색칠하기
            ColorTarget(item2, 1); // item2 색칠하기

            FocusClash(item1, item2); // 카메라 이동
        }

        /// <summary>
        /// guid를 가진 오브젝트를 index에 맞는 색으로 칠하기
        /// </summary>
        /// <param name="item">색 칠할 대상. 비어있으면 선택하지 않은 부재를 대상으로 함</param>
        /// <param name="index">0:item1, 1:item2, -1:그 외 모든 부재</param>
        void ColorTarget(ModelItem item = null, int index = -1) {
            //대상 미지정 : 선택하지 않은 모든 부재를 숨기거나 투명화
            if (index == -1) {
                if (trans) {
                    doc.Models.OverrideTemporaryTransparency(invertItemCollection, 10);
                }
                if (hide) {
                    doc.Models.SetHidden(invertItemCollection, true);
                }
                doc.CurrentSelection.Clear();
            }
            else {
                if (item == null) {
                    MessageBox.Show("선택한 간섭에 대한 부재가 없습니다.\nitem코드 : " + (index+1));
                    return;
                }

                ModelItemCollection modelItemCollection = new ModelItemCollection();
                modelItemCollection.Add(item);
                doc.Models.OverrideTemporaryColor(modelItemCollection, colors[index]);
                doc.CurrentSelection.Clear();
            }   
        }

        /// <summary>
        /// 스크린샷 저장 c:\objectinfo\Screenshot [guid1] - [guid2].jpg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Screenshot_Click(object sender, EventArgs e) {
            string path = @"C:\objectinfo\";
            string fileName = $"Screenshot {guid1} - {guid2}.jpg";
            Autodesk.Navisworks.Api.View currentView = Autodesk.Navisworks.Api.Application.ActiveDocument.ActiveView;
            Bitmap bmp = Autodesk.Navisworks.Api.Application.ActiveDocument.ActiveView.GenerateImage(ImageGenerationStyle.ScenePlusOverlay, currentView.Width, currentView.Height);
            bmp.Save(path + fileName);
            
        }



        /// <summary>
        /// 현재 카메라의 사영 방식을 투시<->직교 전환
        /// </summary>
        private void Change_ViewpointProjection() {
            Viewpoint vp = Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentViewpoint.CreateCopy();
            if(vp.Projection == ViewpointProjection.Orthographic) {
                vp.Projection = ViewpointProjection.Perspective;
            }
            else {
                vp.Projection = ViewpointProjection.Orthographic;
            }
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentViewpoint.CopyFrom(vp);
        }

        /// <summary>
        /// 간섭 선택 시 나머지 부재들에 대한 옵션(없음/투명화/숨기기)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdo_CheckedChanged(object sender, EventArgs e) {
            trans = false;
            hide = false;
            if (rdo_hide.Checked) {
                hide = true;
            }
            else if (rdo_trans.Checked) {
                trans = true;
            }
            else if (rdo_none.Checked) {

            }
            SelectClash(guid1, guid2);
        }

        /// <summary>
        /// guid를 이용해 오브젝트를 찾고 선택함
        /// </summary>
        /// <param name="guid">검색할 부재의 guid</param>
        /// <param name="addtoselect">검색한 부재를 Selection에 추가할지 말지(false:추가하지 않음)</param>
        public ModelItem SelectObjectWithGUID(string guid, bool addtoselect = true) {
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
            if (item != null && addtoselect) {
                doc.CurrentSelection.Add(item);
            }
            return item;
        }

        /// <summary>
        /// item1, item2를 한 화면에 담을 수 있을 정도로 대상을 확대함
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        public void FocusClash(ModelItem item1, ModelItem item2) {
            try {
                Viewpoint vpoint = doc.CurrentViewpoint.CreateCopy();
                vpoint.Rotation = new Rotation3D(0.424708200277859, 0.175919896606164, 0.339851142979997, 0.820473238570283);
                ModelItemCollection modelItemCollection = new ModelItemCollection();
                modelItemCollection.Add(item1);
                modelItemCollection.Add(item2);

                BoundingBox3D bbox = modelItemCollection.BoundingBox(true);
                vpoint.ZoomBox(bbox);
                doc.CurrentViewpoint.CopyFrom(vpoint);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion

    }
}
