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
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using Autodesk.Navisworks.Api.ComApi;
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Interop.ComApi;

namespace ClashTest2
{
    public partial class form_ResultViewer : Form
    {

        // List for data
        List<ClashData> dataList = new List<ClashData>();
        List<ClashData> dataHardList = new List<ClashData>();
        List<ClashData> dataSoftList = new List<ClashData>();
        List<ClashData> dataNullList = new List<ClashData>();

        // enum used for the header of columns in ListView
        // When header is changed, change the enum and add from designer
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

        // bool for checked column header
        private bool[] headerBool;

        // number of each result
        private int major_hard = 0, major_soft = 0;
        private int medium_hard = 0, medium_soft = 0;
        private int minor_hard = 0, minor_soft = 0;

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
                    ClashData clashdata = new ClashData();
                    clashdata.ClashType = stringdata[((int)Header.ClashType)];
                    clashdata.HardClashType = stringdata[((int)Header.HardClashType)];
                    clashdata.SoftClashType = stringdata[((int)Header.SoftClashType)];
                    clashdata.Severity = stringdata[((int)Header.Severity)].ToUpper();
                    clashdata.Element1discipline = stringdata[((int)Header.Element1discipline)];
                    clashdata.Element1GUID = stringdata[((int)Header.Element1GUID)];
                    clashdata.Element1Type = stringdata[((int)Header.Element1Type)];
                    clashdata.Element2discipline = stringdata[((int)Header.Element2discipline)];
                    clashdata.Element2GUID = stringdata[((int)Header.Element2GUID)];
                    clashdata.Element2Type = stringdata[((int)Header.Element2Type)];
                    clashdata.ClashDistance = stringdata[((int)Header.ClashDistance)];
                    clashdata.Clearance = stringdata[((int)Header.Clearance)];
                    clashdata.ClashPoint = stringdata[((int)Header.ClashPoint)];
                    clashdata.ClashVolume = stringdata[((int)Header.ClashVolume)];
                    clashdata.Topology = stringdata[((int)Header.Topology)];
                    clashdata.Offset = stringdata[((int)Header.Offset)];
                    clashDataList.Add(clashdata);
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
            if (dataList != null)
            {
                foreach (ClashData clash in dataList)
                {
                    if (clash.ClashType == "Hard")
                    {
                        dataHardList.Add(clash);
                        if (clash.Severity == "MAJOR")
                        {
                            major_hard++;
                        }
                        else if (clash.Severity == "MEDIUM")
                        {
                            medium_hard++;
                        }
                        else
                        {
                            minor_hard++;
                        }
                    }
                    else if (clash.ClashType == "Soft")
                    {
                        dataSoftList.Add(clash);
                        if (clash.Severity == "MAJOR")
                        {
                            major_soft++;
                        }
                        else if (clash.Severity == "MEDIUM")
                        {
                            medium_soft++;
                        }
                        else
                        {
                            minor_soft++;
                        }
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
                folv.AlwaysGroupByColumn = SeverityCol;
                // MVC pattern -> check objectListView 
                folv.SetObjects(dataList);

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
            string serverUrl = "http://117.17.196.92:3116/result"; // 서버 주소를 적절히 변경하세요
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
            guid1 = folv.SelectedItem.GetSubItem((int)Header.Element1GUID).Text;
            guid2 = folv.SelectedItem.GetSubItem((int)Header.Element2GUID).Text;
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
                    folv.Columns[(int)Header.ClashType].Width = 0;
                }
                else
                {
                    folv.Columns[(int)Header.ClashType].Width = 81;
                }
                HardClashTypeCol.IsVisible = headerBool[(int)Header.HardClashType];
                SoftClashTypeCol.IsVisible = headerBool[(int)Header.SoftClashType];
                SeverityCol.IsVisible = headerBool[(int)Header.Severity];
                Elem1disciplineCol.IsVisible = headerBool[(int)Header.Element1discipline];
                Elem1GUIDCol.IsVisible = headerBool[(int)Header.Element1GUID];
                Elem1TypeCol.IsVisible = headerBool[(int)Header.Element1Type];
                Elem2disciplineCol.IsVisible = headerBool[(int)Header.Element2discipline];
                Elem2GUIDCol.IsVisible = headerBool[(int)Header.Element2GUID];
                Elem2TypeCol.IsVisible = headerBool[(int)Header.Element2Type];
                ClashDistCol.IsVisible = headerBool[(int)Header.ClashDistance];
                ClearanceCol.IsVisible = headerBool[(int)Header.Clearance];
                ClashPointCol.IsVisible = headerBool[(int)Header.ClashPoint];
                ClashVolumeCol.IsVisible = headerBool[(int)Header.ClashVolume];
                TopologyCol.IsVisible = headerBool[(int)Header.Topology];
                OffsetCol.IsVisible = headerBool[(int)Header.Offset];

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
            majorSoft.Text = "MAJOR_S:" + major_soft.ToString();
            mediumHard.Text = "MEDIUM_H:" + medium_hard.ToString();
            mediumSoft.Text = "MEDIUM_S:" + medium_soft.ToString();
            minorHard.Text = "MINOR_H:" + minor_hard.ToString();
            minorSoft.Text = "MINOR_S:" + minor_soft.ToString();
            
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
                SelectObjectWithGUID(folv.SelectedItem.GetSubItem((int)Header.Element1GUID).Text);
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
                SelectObjectWithGUID(folv.SelectedItem.GetSubItem((int)Header.Element2GUID).Text);
            }
        }
        #endregion

        #region Navisworks

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

        private void Make_ClippingBox() {
            //단면처리 사용
            //단면처리 모드 - 상자
            //시점 변경
            //시점 방향이 아닌 방향으로 상자 scale조정
            //모델 투명화 끄기
            //모델에 초점 맞추기
            //스크린샷
        }

        /*
        public static void SetPlaneSectioningItem(Point3D CenterPoint3D, double meter, bool Linked = false, bool Enabled = true, Viewpoint currView = null)
        {
            try
            {
                ComApi.InwOpState10 state;
                state = ComApiBridge.State;



                ComApi.InwLUnitVec3f sectionPlaneNormal = (ComApi.InwLUnitVec3f)state.ObjectFactory(Autodesk.Navisworks.Api.Interop.ComApi.nwEObjectType.eObjectType_nwLUnitVec3f, null, null);

                ComApi.InwLPlane3f sectionPlane = (ComApi.InwLPlane3f)state.ObjectFactory(Autodesk.Navisworks.Api.Interop.ComApi.nwEObjectType.eObjectType_nwLPlane3f, null, null);
                ComApi.InwClippingPlaneColl2 clipColl = (ComApi.InwClippingPlaneColl2)state.CurrentView.ClippingPlanes();
                clipColl.CreatePlane(1);
                clipColl.CreatePlane(2);
                clipColl.CreatePlane(3);
                clipColl.CreatePlane(4);
                clipColl.CreatePlane(5);
                clipColl.CreatePlane(6);

                // 6개의 분면을 모두 생성. (직(정)육면체를 만들기 위해서 한면만 가리려면 한개만 생성해도 됨
                clipColl.Linked = Linked;

                //단면평면 연결 설정 여부



                ComApi.InwOaClipPlane cliPlane = (ComApi.InwOaClipPlane)state.CurrentView.ClippingPlanes()[1];

                //생성한 1번 분면을 가져온다.
                cliPlane.Alignment = ComApi.nwEClipPlaneAlignment.eAlignment_FRONT;

                //정렬을 TOP으로 설정
                sectionPlaneNormal.SetValue(0, 1, 0);

                //노말벡터 설정 ( y방향으로 1을 설정하면 TOP을 의미함) 

                //※노말벡터 = 한 표면에서 수직으로 향하는 벡터
                sectionPlane.SetValue(sectionPlaneNormal, CenterPoint3D.Y - meter);

                //Y방향으로 일정 거리만큼 InwLPlane3f 생성 
                cliPlane.Plane = sectionPlane;

                //생성한 1번 분면의 plane을 방금 생성하고 설정한 sectionPlane으로 설정
                cliPlane.Enabled = Enabled;

                //1번 분면의 존재여부

                //아래는 반복 과정을 의미함.




                cliPlane = (ComApi.InwOaClipPlane)state.CurrentView.ClippingPlanes()[2];
                cliPlane.Alignment = ComApi.nwEClipPlaneAlignment.eAlignment_BACK;
                sectionPlaneNormal.SetValue(0, -1, 0);
                sectionPlane.SetValue(sectionPlaneNormal, -CenterPoint3D.Y - meter);
                cliPlane.Plane = sectionPlane;
                cliPlane.Enabled = Enabled;



                cliPlane = (ComApi.InwOaClipPlane)state.CurrentView.ClippingPlanes()[3];
                cliPlane.Alignment = ComApi.nwEClipPlaneAlignment.eAlignment_TOP;
                sectionPlaneNormal.SetValue(0, 0, -1);
                sectionPlane.SetValue(sectionPlaneNormal, -CenterPoint3D.Z - meter);
                cliPlane.Plane = sectionPlane;
                cliPlane.Enabled = Enabled;



                cliPlane = (ComApi.InwOaClipPlane)state.CurrentView.ClippingPlanes()[4];
                cliPlane.Alignment = ComApi.nwEClipPlaneAlignment.eAlignment_BOTTOM;
                sectionPlaneNormal.SetValue(0, 0, 1);
                sectionPlane.SetValue(sectionPlaneNormal, CenterPoint3D.Z - meter);
                cliPlane.Plane = sectionPlane;
                cliPlane.Enabled = Enabled;



                cliPlane = (ComApi.InwOaClipPlane)state.CurrentView.ClippingPlanes()[5];
                cliPlane.Alignment = ComApi.nwEClipPlaneAlignment.eAlignment_LEFT;
                sectionPlaneNormal.SetValue(1, 0, 0);
                sectionPlane.SetValue(sectionPlaneNormal, CenterPoint3D.X - meter);
                cliPlane.Plane = sectionPlane;
                cliPlane.Enabled = Enabled;



                cliPlane = (ComApi.InwOaClipPlane)state.CurrentView.ClippingPlanes()[6];
                cliPlane.Alignment = ComApi.nwEClipPlaneAlignment.eAlignment_RIGHT;
                sectionPlaneNormal.SetValue(-1, 0, 0);
                sectionPlane.SetValue(sectionPlaneNormal, -CenterPoint3D.X - meter);
                cliPlane.Plane = sectionPlane;
                cliPlane.Enabled = Enabled; 
            }
            catch { }
        }

        */
        public void SetPlaneSectioningItem(Point3D CenterPoint3D, double meter, bool Enabled = true, Viewpoint currView = null)
        {



            currView.InternalClipPlanes.SetMode(Autodesk.Navisworks.Api.Interop.LcOaClipPlaneSetMode.eMODE_BOX);

            //InternalClipPlanes는 숨겨진 프로퍼티이다. 모드를 박스 모드로 바꿔준다.
            Point3D minPoint = new Point3D(CenterPoint3D.X - meter, CenterPoint3D.Y - meter, CenterPoint3D.Z - meter);
            Point3D maxPoint = new Point3D(CenterPoint3D.X + meter, CenterPoint3D.Y + meter, CenterPoint3D.Z + meter);
            currView.InternalClipPlanes.SetBox(new BoundingBox3D(minPoint, maxPoint));

            //중심점으로부터 거리만큼 박스를 만들어준다. 현재 여기서는 일정 크기만큼 박싱을 해주지만 본인이 원하는 지점의 위치에서 원하는 박스를 만들어서 사용하면 됨.
            currView.InternalClipPlanes.SetEnabled(Enabled);

            //존재여부설정

            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentViewpoint.CopyFrom(currView);

            //현재 뷰포인트에 카피시켜준다.

        }
        private void button1_Click(object sender, EventArgs e)
        {
            string stringPos = folv.SelectedItem.GetSubItem((int)Header.ClashPoint).Text;
            MessageBox.Show(stringPos);
            string[] coordinates = stringPos.Split(',');
            Point3D centerPos = new Point3D(float.Parse(coordinates[0]), float.Parse(coordinates[1]), float.Parse(coordinates[2]));
            Viewpoint currView = doc.CurrentViewpoint.CreateCopy();
            SetPlaneSectioningItem(centerPos, 50, true,currView);
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
