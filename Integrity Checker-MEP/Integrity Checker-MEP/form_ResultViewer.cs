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

namespace ClashTest2
{
    public partial class form_ResultViewer : Form
    {
        // Struct which holds all the data
        public struct Data
        {
            public string ClashType;
            public string HardClashType;
            public string SoftClashType;
            public string Severity;
            public string Element1discipline;
            public string Element1GUID;
            public string Element1Type;
            public string Element2discipline;
            public string Element2GUID;
            public string Element2Type;
            public string ClashDistance;
            public string Clearance;
            public string ClashPoint;
            public string ClashVolume;
            public string Topology;
            public string Offset;
        }
        // List containing data structs
        // 1 is hard type, 2 is soft type
        // Can be used when handling the data itself
        public List<Data> hardTypeList = new List<Data>();
        public List<Data> softTypeList = new List<Data>();

        // List containing ListViewItem
        // Used when updating the Listview
        public List<ListViewItem> itemType1 = new List<ListViewItem>();
        public List<ListViewItem> itemType2 = new List<ListViewItem>();

        // enum used for the header of columns in ListView
        // When header is changed, change the enum
        enum Header
        {
            ClashType = 0,
            HardClashType = 1,
            SoftClashType = 2,
            Severity = 3,
            Element1discipline = 4,
            Element1GUID = 5,
            Element1Type = 6,
            Element2discipline = 7,
            Element2GUID = 8,
            Element2Type = 9,
            ClashDistance = 10,
            Clearance = 11,
            ClashPoint = 12,
            ClashVolume = 13,
            Topology = 14,
            Offset = 15,
        }

        private bool[] headerBool;

        private int major_hard = 0, major_soft = 0;
        private int medium_hard = 0, medium_soft = 0;
        private int minor_hard = 0, minor_soft = 0;

        private string guid1;
        private string guid2;

        public form_ResultViewer()
        {
            InitializeComponent();
            doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
        }

        private void click_btn_Load(object sender, EventArgs e)
        {
            // return when the list is not empty
            if (lst_Results.Items.Count > 0)
            {
                return;
            }

            string path = "C:\\objectinfo\\ResultFile.csv";
            StreamReader file = new StreamReader(path);
            string firstLine = file.ReadLine();
            string[] header = firstLine.Split(',');

            SelectHeader selectHeader = new SelectHeader();
            selectHeader.initializeHeaderBool(header.Length);
            selectHeader.StartPosition = FormStartPosition.CenterParent;
            selectHeader.ShowDialog();
            if(selectHeader.isCanceled) return;

            // 업데이트가 끝날때까지 UI 갱신 중지 -> 빠른 속도
            lst_Results.BeginUpdate();

            tog_Hard.Checked = true;
            tog_Soft.Checked = true;
            tog_Hard.Enabled = true;
            tog_Soft.Enabled = true;

            // Add column header
            foreach(string item in header)
            {
                lst_Results.Columns.Add(item);
            }
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                string pattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
                string[] data = Regex.Split(line, pattern);

                for (int i = 0; i < data.Length; i++)
                {
                    // Remove leading and trailing double quotes if present
                    data[i] = data[i].Trim('"');
                }

                Data structData = new Data();
                structData.ClashType = data[((int)Header.ClashType)];
                structData.HardClashType = data[((int)Header.HardClashType)];
                structData.SoftClashType = data[((int)Header.SoftClashType)];
                structData.Severity = data[((int)Header.Severity)].ToUpper();
                structData.Element1discipline = data[((int)Header.Element1discipline)];
                structData.Element1GUID = data[((int)Header.Element1GUID)];
                structData.Element1Type = data[((int)Header.Element1Type)];
                structData.Element2discipline = data[((int)Header.Element2discipline)];
                structData.Element2GUID = data[((int)Header.Element2GUID)];
                structData.Element2Type = data[((int)Header.Element2Type)];
                structData.ClashDistance = data[((int)Header.ClashDistance)];
                structData.Clearance = data[((int)Header.Clearance)];
                structData.ClashPoint = data[((int)Header.ClashPoint)];
                structData.ClashVolume = data[((int)Header.ClashVolume)];
                structData.Topology = data[((int)Header.Topology)];
                structData.Offset = data[((int)Header.Offset)];

                ListViewItem item2 = new ListViewItem(data[0]);
                for (int i = 1; i< data.Length; i++)
                {
                    item2.SubItems.Add(data[i]);
                }
                if (structData.ClashType == "Hard")
                {
                    hardTypeList.Add(structData);
                    itemType1.Add(item2);
                }
                else if (structData.ClashType == "Soft")
                {
                    softTypeList.Add(structData);
                    itemType2.Add(item2);
                }
                
                lst_Results.Items.Add(item2);

                if (structData.Severity == "MAJOR")
                {
                    if(structData.ClashType == "Hard") {
                        major_hard++;
                    }
                    else { major_soft++; }

                    lst_Results.Groups[0].Items.Add(item2);
                    item2.Tag = "MAJOR";
                }
                else if (structData.Severity == "MEDIUM")
                {
                    if(structData.ClashType == "Hard") {
                        medium_hard++;
                    }
                    else { medium_soft++; }

                    lst_Results.Groups[1].Items.Add(item2);
                    item2.Tag = "MEDIUM";

                }
                else if (structData.Severity == "MINOR")
                {
                    if (structData.ClashType == "Hard") {
                        minor_hard++;
                    }
                    else { minor_soft++; }

                    lst_Results.Groups[2].Items.Add(item2);
                    item2.Tag = "MINOR";
                }

            }

            // column 사이즈 재조정
            changeColumnHeader(headerBool);

            // groupHeader 조정
            showGroupNum();

            // 오른쪽에 각 Severity_Clashtype 표시
            showEachClashNuminfo();

            // 리스트뷰를 refresh해서 보여줌
            lst_Results.EndUpdate();
        }

        private async Task downloadFromServer()
        {
            string serverUrl = "http://117.17.196.92:6060/result"; // 서버 주소를 적절히 변경하세요
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

        private void lst_Results_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_Results.SelectedItems.Count == 1)
            {
                if (lst_Results.SelectedItems[0].SubItems.Count > 1) {
                    ID_GUID1.Text = "Guid1: " + lst_Results.SelectedItems[0].SubItems[(int)Header.Element1GUID].Text + "  Guid2: " + lst_Results.SelectedItems[0].SubItems[(int)Header.Element2GUID].Text;
                    guid1 = lst_Results.SelectedItems[0].SubItems[(int)Header.Element1GUID].Text;
                    guid2 = lst_Results.SelectedItems[0].SubItems[(int)Header.Element2GUID].Text;
                    SelectClash(guid1, guid2);
                }

            }
        }

        private void tog_Hard_CheckedChanged(object sender, EventArgs e)
        {
            // hard type unchecked
            if (!tog_Hard.Checked)
            {
               lst_Results.BeginUpdate();
               foreach (ListViewItem item in lst_Results.Items)
               {
                    item.Remove();
               }
               // only soft type checked
               if (tog_Soft.Checked)
               {
                    foreach (ListViewItem item in itemType2)
                    {
                        lst_Results.Items.Add(item);
                        if (item.Tag.ToString() == "MAJOR")
                        {
                            lst_Results.Groups[0].Items.Add(item);
                        }
                        else if (item.Tag.ToString() == "MEDIUM")
                        {
                            lst_Results.Groups[1].Items.Add(item);

                        }
                        else if (item.Tag.ToString() == "MINOR")
                        {
                            lst_Results.Groups[2].Items.Add(item);

                        }
                    }
                    lst_Results.Columns[(int)Header.HardClashType].Width = 0;
                    lst_Results.Columns[(int)Header.ClashDistance].Width = 0;
                    lst_Results.Columns[(int)Header.ClashPoint].Width = 0;
                    lst_Results.Columns[(int)Header.ClashVolume].Width = 0;

                }

                foreach (ListViewGroup group in lst_Results.Groups)
               {
                    if(group.Items.Count == 0)
                    {
                        ListViewItem emptyItem = new ListViewItem(string.Empty);
                        lst_Results.Items.Add(emptyItem);
                        group.Items.Add(emptyItem);
                    }
               }
                showGroupNum();
                lst_Results.EndUpdate();
            }
            // hard type checked
            else if (tog_Hard.Checked)
            {
                if(lst_Results.Items.Count == 0)
                {
                    return;
                }
                lst_Results.BeginUpdate();
                foreach (ListViewItem item in lst_Results.Items)
                {
                    if(item.SubItems.Count <= 1)
                    {
                        item.Remove();
                    }
                }
                foreach (ListViewItem item in itemType1)
                {
                    lst_Results.Items.Add(item);
                    if (item.Tag.ToString() == "MAJOR")
                    {
                        lst_Results.Groups[0].Items.Add(item);
                    }
                    else if (item.Tag.ToString() == "MEDIUM")
                    {
                        lst_Results.Groups[1].Items.Add(item);

                    }
                    else if (item.Tag.ToString() == "MINOR")
                    {
                        lst_Results.Groups[2].Items.Add(item);

                    }
                }
                foreach (ListViewGroup group in lst_Results.Groups) {
                    if (group.Items.Count == 0) {
                        ListViewItem emptyItem = new ListViewItem(string.Empty);
                        lst_Results.Items.Add(emptyItem);
                        group.Items.Add(emptyItem);
                    }
                }
                changeColumnHeader(headerBool);
                if (!tog_Soft.Checked) {
                    lst_Results.Columns[(int)Header.SoftClashType].Width = 0;
                    lst_Results.Columns[(int)Header.Clearance].Width = 0;
                }
                showGroupNum();
                lst_Results.EndUpdate();
            }

        }

        private void tog_Soft_CheckedChanged(object sender, EventArgs e)
        {
            // soft type unchecked
            if (!tog_Soft.Checked)
            {
                lst_Results.BeginUpdate();
                // Remove both soft/hard
                foreach (ListViewItem item in lst_Results.Items)
                {
                    item.Remove();
                }
                // Only hard checked
                if(tog_Hard.Checked)
                {
                    foreach (ListViewItem item in itemType1)
                    {
                        lst_Results.Items.Add(item);
                        if (item.Tag.ToString() == "MAJOR")
                        {
                            lst_Results.Groups[0].Items.Add(item);
                        }
                        else if (item.Tag.ToString() == "MEDIUM")
                        {
                            lst_Results.Groups[1].Items.Add(item);

                        }
                        else if (item.Tag.ToString() == "MINOR")
                        {
                            lst_Results.Groups[2].Items.Add(item);

                        }
                    }
                   lst_Results.Columns[(int)Header.SoftClashType].Width = 0;
                   lst_Results.Columns[(int)Header.Clearance].Width = 0;
                }
                
                foreach (ListViewGroup group in lst_Results.Groups)
                {
                    if (group.Items.Count == 0)
                    {
                        ListViewItem emptyItem = new ListViewItem(string.Empty);
                        lst_Results.Items.Add(emptyItem);
                        group.Items.Add(emptyItem);
                    }
                }
                showGroupNum();
                lst_Results.EndUpdate();
            }
            // soft type checked
            else if (tog_Soft.Checked)
            {
                if (lst_Results.Items.Count == 0)
                {
                    return;
                }
                lst_Results.BeginUpdate();
                //only soft type checked
                foreach (ListViewItem item in lst_Results.Items)
                {
                    if (item.SubItems.Count <= 1)
                    {
                        item.Remove();
                    }
                }
                foreach (ListViewItem item in itemType2)
                {
                    lst_Results.Items.Add(item);
                    if (item.Tag.ToString() == "MAJOR")
                    {
                        lst_Results.Groups[0].Items.Add(item);
                    }
                    else if (item.Tag.ToString() == "MEDIUM")
                    {
                        lst_Results.Groups[1].Items.Add(item);

                    }
                    else if (item.Tag.ToString() == "MINOR")
                    {
                        lst_Results.Groups[2].Items.Add(item);

                    }
                }
                foreach (ListViewGroup group in lst_Results.Groups) {
                    if (group.Items.Count == 0) {
                        ListViewItem emptyItem = new ListViewItem(string.Empty);
                        lst_Results.Items.Add(emptyItem);
                        group.Items.Add(emptyItem);
                    }
                }
                changeColumnHeader(headerBool);
                if (!tog_Hard.Checked) {
                    lst_Results.Columns[(int)Header.HardClashType].Width = 0;
                    lst_Results.Columns[(int)Header.ClashDistance].Width = 0;
                    lst_Results.Columns[(int)Header.ClashPoint].Width = 0;
                    lst_Results.Columns[(int)Header.ClashVolume].Width = 0;
                }
                showGroupNum();
                lst_Results.EndUpdate();
            }
        }

        private async void click_btn_Download(object sender, EventArgs e)
        {
            await downloadFromServer();
        }

        public void changeColumnHeader(bool[] changedHeaders)
        {
            headerBool = changedHeaders;
            if (lst_Results.Columns.Count > 0)
            {
                for (int i = 0; i < headerBool.Length; i++)
                {
                    if (headerBool[i] == true)
                    {
                        lst_Results.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                    }
                    else
                    {
                        lst_Results.Columns[i].Width = 0;
                    }
                }
            }
        }

        private void showGroupNum() {
            if (lst_Results.Groups[0].Items.Count > 0) {
                if (lst_Results.Groups[0].Items[0].SubItems.Count > 1) {
                    lst_Results.Groups[0].Header = "MAJOR(" + lst_Results.Groups[0].Items.Count.ToString() + ")";
                }
                else {
                    lst_Results.Groups[0].Header = "MAJOR(0)";
                }
            }
            
            if(lst_Results.Groups[1].Items.Count > 0) {
                if (lst_Results.Groups[1].Items[0].SubItems.Count > 1) {
                    lst_Results.Groups[1].Header = "MEDIUM(" + lst_Results.Groups[1].Items.Count.ToString() + ")";
                }
                else {
                    lst_Results.Groups[1].Header = "MEDIUM(0)";
                }
            }
            if (lst_Results.Groups[2].Items.Count > 0) {
                if (lst_Results.Groups[2].Items[0].SubItems.Count > 1) {
                    lst_Results.Groups[2].Header = "MINOR(" + lst_Results.Groups[2].Items.Count.ToString() + ")";
                }
                else {
                    lst_Results.Groups[2].Header = "MINOR(0)";
                }
            }
        }

        private void showEachClashNuminfo() {
            majorHard.Visible = true;
            majorSoft.Visible = true;
            mediumHard.Visible = true;
            mediumSoft.Visible = true;
            minorHard.Visible = true;
            minorSoft.Visible = true;

            majorHard.Text = "MAJOR_H:" + major_hard.ToString();
            majorSoft.Text = "MAJOR_S:" + major_soft.ToString();
            mediumHard.Text = "MEDIUM_H:" + medium_hard.ToString();
            mediumSoft.Text = "MEDIUM_S:" + medium_soft.ToString();
            minorHard.Text = "MINOR_H:" + minor_hard.ToString();
            minorSoft.Text = "MINOR_S:" + minor_soft.ToString();
            
        }

        private void click_btn_SelectHeader(object sender, EventArgs e)
        {
            SelectHeader selectHeader = new SelectHeader();
            selectHeader.getHeaderBool(headerBool, headerBool.Length);
            selectHeader.StartPosition = FormStartPosition.CenterParent;
            selectHeader.ShowDialog();
        }

        private void click_btn_Item1(object sender, EventArgs e)
        {
            if (lst_Results.SelectedIndices.Count > 0)
            {
                if (lst_Results.SelectedItems[0].SubItems.Count > 1)
                {
                    //MessageBoxEx.Show(this, collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element1GUID].Text);
                    doc.CurrentSelection.Clear();
                    SelectObjectWithGUID(lst_Results.SelectedItems[0].SubItems[(int)Header.Element1GUID].Text);
                }
            }
        }

        private void click_btn_Item2(object sender, EventArgs e)
        {
            if (lst_Results.SelectedIndices.Count > 0)
            {
                if (lst_Results.SelectedItems[0].SubItems.Count > 1) {
                    doc.CurrentSelection.Clear();
                    SelectObjectWithGUID(lst_Results.SelectedItems[0].SubItems[(int)Header.Element2GUID].Text);
                }
            }
        }

        public Document doc;
        Color[] colors = { Color.Green, Color.Red }; //부재에 칠할 색
        bool trans = false;
        bool hide = false;
        ModelItemCollection invertItemCollection = new ModelItemCollection(); // 선택 부재 외 나머지 부재

        /// <summary>
        /// 간섭 결과가 선택 되었을 때 호출. 현재 모델에 선택한 간섭 표시
        /// </summary>
        /// <param name="_guid1"></param>
        /// <param name="_guid2"></param>
        public void SelectClash(string _guid1, string _guid2) {
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

    }
}
