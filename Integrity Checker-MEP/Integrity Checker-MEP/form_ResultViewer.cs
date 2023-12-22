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

        private void button1_Click(object sender, EventArgs e)
        {
            // return when the list is not empty
            if (collapsibleListView1.Items.Count > 0)
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
            collapsibleListView1.BeginUpdate();

            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox1.Enabled = true;
            checkBox2.Enabled = true;

            // Add column header
            foreach(string item in header)
            {
                collapsibleListView1.Columns.Add(item);
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
                
                collapsibleListView1.Items.Add(item2);

                if (structData.Severity == "MAJOR")
                {
                    if(structData.ClashType == "Hard") {
                        major_hard++;
                    }
                    else { major_soft++; }

                    collapsibleListView1.Groups[0].Items.Add(item2);
                    item2.Tag = "MAJOR";
                }
                else if (structData.Severity == "MEDIUM")
                {
                    if(structData.ClashType == "Hard") {
                        medium_hard++;
                    }
                    else { medium_soft++; }

                    collapsibleListView1.Groups[1].Items.Add(item2);
                    item2.Tag = "MEDIUM";

                }
                else if (structData.Severity == "MINOR")
                {
                    if (structData.ClashType == "Hard") {
                        minor_hard++;
                    }
                    else { minor_soft++; }

                    collapsibleListView1.Groups[2].Items.Add(item2);
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
            collapsibleListView1.EndUpdate();
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

        private void collapsibleListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (collapsibleListView1.SelectedIndices.Count > 0)
            {
                if (collapsibleListView1.SelectedItems[0].SubItems.Count > 1) {
                    ID_GUID1.Text = "Guid1: " + collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element1GUID].Text + "  Guid2: " + collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element2GUID].Text;
                    guid1 = collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element1GUID].Text;
                    guid2 = collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element2GUID].Text;
                    SelectObjects(guid1, guid2);
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // hard type unchecked
            if (!checkBox1.Checked)
            {
               collapsibleListView1.BeginUpdate();
               foreach (ListViewItem item in collapsibleListView1.Items)
               {
                    item.Remove();
               }
               // only soft type checked
               if (checkBox2.Checked)
               {
                    foreach (ListViewItem item in itemType2)
                    {
                        collapsibleListView1.Items.Add(item);
                        if (item.Tag.ToString() == "MAJOR")
                        {
                            collapsibleListView1.Groups[0].Items.Add(item);
                        }
                        else if (item.Tag.ToString() == "MEDIUM")
                        {
                            collapsibleListView1.Groups[1].Items.Add(item);

                        }
                        else if (item.Tag.ToString() == "MINOR")
                        {
                            collapsibleListView1.Groups[2].Items.Add(item);

                        }
                    }
                    collapsibleListView1.Columns[(int)Header.HardClashType].Width = 0;
                    collapsibleListView1.Columns[(int)Header.ClashDistance].Width = 0;
                    collapsibleListView1.Columns[(int)Header.ClashPoint].Width = 0;
                    collapsibleListView1.Columns[(int)Header.ClashVolume].Width = 0;

                }

                foreach (ListViewGroup group in collapsibleListView1.Groups)
               {
                    if(group.Items.Count == 0)
                    {
                        ListViewItem emptyItem = new ListViewItem(string.Empty);
                        collapsibleListView1.Items.Add(emptyItem);
                        group.Items.Add(emptyItem);
                    }
               }
                showGroupNum();
                collapsibleListView1.EndUpdate();
            }
            // hard type checked
            else if (checkBox1.Checked)
            {
                if(collapsibleListView1.Items.Count == 0)
                {
                    return;
                }
                collapsibleListView1.BeginUpdate();
                foreach (ListViewItem item in collapsibleListView1.Items)
                {
                    if(item.SubItems.Count <= 1)
                    {
                        item.Remove();
                    }
                }
                foreach (ListViewItem item in itemType1)
                {
                    collapsibleListView1.Items.Add(item);
                    if (item.Tag.ToString() == "MAJOR")
                    {
                        collapsibleListView1.Groups[0].Items.Add(item);
                    }
                    else if (item.Tag.ToString() == "MEDIUM")
                    {
                        collapsibleListView1.Groups[1].Items.Add(item);

                    }
                    else if (item.Tag.ToString() == "MINOR")
                    {
                        collapsibleListView1.Groups[2].Items.Add(item);

                    }
                }
                foreach (ListViewGroup group in collapsibleListView1.Groups) {
                    if (group.Items.Count == 0) {
                        ListViewItem emptyItem = new ListViewItem(string.Empty);
                        collapsibleListView1.Items.Add(emptyItem);
                        group.Items.Add(emptyItem);
                    }
                }
                changeColumnHeader(headerBool);
                if (!checkBox2.Checked) {
                    collapsibleListView1.Columns[(int)Header.SoftClashType].Width = 0;
                    collapsibleListView1.Columns[(int)Header.Clearance].Width = 0;
                }
                showGroupNum();
                collapsibleListView1.EndUpdate();
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // soft type unchecked
            if (!checkBox2.Checked)
            {
                collapsibleListView1.BeginUpdate();
                // Remove both soft/hard
                foreach (ListViewItem item in collapsibleListView1.Items)
                {
                    item.Remove();
                }
                // Only hard checked
                if(checkBox1.Checked)
                {
                    foreach (ListViewItem item in itemType1)
                    {
                        collapsibleListView1.Items.Add(item);
                        if (item.Tag.ToString() == "MAJOR")
                        {
                            collapsibleListView1.Groups[0].Items.Add(item);
                        }
                        else if (item.Tag.ToString() == "MEDIUM")
                        {
                            collapsibleListView1.Groups[1].Items.Add(item);

                        }
                        else if (item.Tag.ToString() == "MINOR")
                        {
                            collapsibleListView1.Groups[2].Items.Add(item);

                        }
                    }
                   collapsibleListView1.Columns[(int)Header.SoftClashType].Width = 0;
                   collapsibleListView1.Columns[(int)Header.Clearance].Width = 0;
                }
                
                foreach (ListViewGroup group in collapsibleListView1.Groups)
                {
                    if (group.Items.Count == 0)
                    {
                        ListViewItem emptyItem = new ListViewItem(string.Empty);
                        collapsibleListView1.Items.Add(emptyItem);
                        group.Items.Add(emptyItem);
                    }
                }
                showGroupNum();
                collapsibleListView1.EndUpdate();
            }
            // soft type checked
            else if (checkBox2.Checked)
            {
                if (collapsibleListView1.Items.Count == 0)
                {
                    return;
                }
                collapsibleListView1.BeginUpdate();
                //only soft type checked
                foreach (ListViewItem item in collapsibleListView1.Items)
                {
                    if (item.SubItems.Count <= 1)
                    {
                        item.Remove();
                    }
                }
                foreach (ListViewItem item in itemType2)
                {
                    collapsibleListView1.Items.Add(item);
                    if (item.Tag.ToString() == "MAJOR")
                    {
                        collapsibleListView1.Groups[0].Items.Add(item);
                    }
                    else if (item.Tag.ToString() == "MEDIUM")
                    {
                        collapsibleListView1.Groups[1].Items.Add(item);

                    }
                    else if (item.Tag.ToString() == "MINOR")
                    {
                        collapsibleListView1.Groups[2].Items.Add(item);

                    }
                }
                foreach (ListViewGroup group in collapsibleListView1.Groups) {
                    if (group.Items.Count == 0) {
                        ListViewItem emptyItem = new ListViewItem(string.Empty);
                        collapsibleListView1.Items.Add(emptyItem);
                        group.Items.Add(emptyItem);
                    }
                }
                changeColumnHeader(headerBool);
                if (!checkBox1.Checked) {
                    collapsibleListView1.Columns[(int)Header.HardClashType].Width = 0;
                    collapsibleListView1.Columns[(int)Header.ClashDistance].Width = 0;
                    collapsibleListView1.Columns[(int)Header.ClashPoint].Width = 0;
                    collapsibleListView1.Columns[(int)Header.ClashVolume].Width = 0;
                }
                showGroupNum();
                collapsibleListView1.EndUpdate();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await downloadFromServer();
        }

        public void changeColumnHeader(bool[] changedHeaders)
        {
            headerBool = changedHeaders;
            if (collapsibleListView1.Columns.Count > 0)
            {
                for (int i = 0; i < headerBool.Length; i++)
                {
                    if (headerBool[i] == true)
                    {
                        collapsibleListView1.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                    }
                    else
                    {
                        collapsibleListView1.Columns[i].Width = 0;
                    }
                }
            }
        }

        private void showGroupNum() {
            if (collapsibleListView1.Groups[0].Items.Count > 0) {
                if (collapsibleListView1.Groups[0].Items[0].SubItems.Count > 1) {
                    collapsibleListView1.Groups[0].Header = "MAJOR(" + collapsibleListView1.Groups[0].Items.Count.ToString() + ")";
                }
                else {
                    collapsibleListView1.Groups[0].Header = "MAJOR(0)";
                }
            }
            
            if(collapsibleListView1.Groups[1].Items.Count > 0) {
                if (collapsibleListView1.Groups[1].Items[0].SubItems.Count > 1) {
                    collapsibleListView1.Groups[1].Header = "MEDIUM(" + collapsibleListView1.Groups[1].Items.Count.ToString() + ")";
                }
                else {
                    collapsibleListView1.Groups[1].Header = "MEDIUM(0)";
                }
            }
            if (collapsibleListView1.Groups[2].Items.Count > 0) {
                if (collapsibleListView1.Groups[2].Items[0].SubItems.Count > 1) {
                    collapsibleListView1.Groups[2].Header = "MINOR(" + collapsibleListView1.Groups[2].Items.Count.ToString() + ")";
                }
                else {
                    collapsibleListView1.Groups[2].Header = "MINOR(0)";
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

        private void button2_Click_1(object sender, EventArgs e)
        {
            SelectHeader selectHeader = new SelectHeader();
            selectHeader.getHeaderBool(headerBool, headerBool.Length);
            selectHeader.StartPosition = FormStartPosition.CenterParent;
            selectHeader.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (collapsibleListView1.SelectedIndices.Count > 0)
            {
                if (collapsibleListView1.SelectedItems[0].SubItems.Count > 1)
                {
                    //MessageBoxEx.Show(this, collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element1GUID].Text);
                    doc.CurrentSelection.Clear();
                    SelectObjectWithGUID(collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element1GUID].Text);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (collapsibleListView1.SelectedIndices.Count > 0)
            {
                if (collapsibleListView1.SelectedItems[0].SubItems.Count > 1) {
                    //MessageBoxEx.Show(this, collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element2GUID].Text);
                    doc.CurrentSelection.Clear();
                    SelectObjectWithGUID(collapsibleListView1.SelectedItems[0].SubItems[(int)Header.Element2GUID].Text);
                }
            }
        }

        public Document doc;
        //부재에 칠할 색
        Color[] colors = { Color.Green, Color.Red };
        bool trans = false;
        bool hide = false;
        ModelItemCollection invertItemCollection = new ModelItemCollection();
        public void SelectObjects(string _guid1, string _guid2) {
            doc.Models.ResetAllTemporaryMaterials();
            doc.Models.ResetAllHiddenToModelState();
            
            ModelItem item1 = SelectObjectWithGUID(_guid1); //item1 선택
            ModelItem item2 = SelectObjectWithGUID(_guid2); //item2 선택
            invertItemCollection.CopyFrom(doc.CurrentSelection.SelectedItems); //item1, item2가 선택되어 있음
            invertItemCollection.Invert(doc); // 선택 반전 (전체 - item1 - item2)
            ColorTarget();
            ColorTarget(item1, 0);
            ColorTarget(item2, 1);

            MoveCamBetween(item1, item2);
        }

        /// <summary>
        /// guid를 가진 오브젝트를 index에 맞는 색으로 칠하기
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="index"></param>
        void ColorTarget(ModelItem item = null, int index = -1) {
            
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
            SelectObjects(guid1, guid2);
        }

        /// <summary>
        /// guid를 이용해 오브젝트를 찾고 선택함
        /// </summary>
        /// <param name="guid"></param>
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
        public void MoveCamBetween(ModelItem item1, ModelItem item2) {
            try {
                Viewpoint vpoint = doc.CurrentViewpoint.CreateCopy();

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
