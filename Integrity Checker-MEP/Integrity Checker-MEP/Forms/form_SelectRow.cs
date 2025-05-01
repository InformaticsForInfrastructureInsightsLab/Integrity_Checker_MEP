using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClashTest2
{
    public partial class SelectHeader : Form
    {
        private int headerNum;
        public bool[] selectedHeader;
        public bool isCanceled;
        
        public SelectHeader()
        {
            InitializeComponent();
        }

        public void initializeHeaderBool(int num)
        {
            headerNum = num;
            selectedHeader = new bool[headerNum];
            for(int i = 0;i< headerNum; i++) {
                selectedHeader[i] = true;
                checkedListBox1.SetItemChecked(i, true);
            }
        }
        public void getHeaderBool(bool[] headerBool, int num)
        {
            headerNum = num;
            selectedHeader = headerBool;
            for (int i = 0; i < headerNum; i++)
            {
                checkedListBox1.SetItemChecked(i, selectedHeader[i]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isCanceled = true;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < headerNum;i++)
            {
                selectedHeader[i] = false;
            }
            if(checkedListBox1.CheckedIndices.Count > 0) {
               for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
               {
                    selectedHeader[checkedListBox1.CheckedIndices[i]] = true;
               }
            }
            
            form_ResultViewer form1 = Application.OpenForms.OfType<form_ResultViewer>().FirstOrDefault();
            form1.changeColumnHeader(selectedHeader);
            this.Close();
        }
    }
}
