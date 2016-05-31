using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DA5
{
    public partial class filemanagement : Form
    {
        public string patch = "";
        public filemanagement()
        {
            InitializeComponent();
        }

        private void opemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            patch = "";
            OpenFileDialog ofile = new OpenFileDialog();
            ofile.Filter = "NA file| *.na";
            if (ofile.ShowDialog() == DialogResult.OK)
            {
                patch = output.Text = ofile.FileName;
            }
            if (patch != "")
            {
                getfolder(patch, 64, listfoder);
            }
        }
        /// <summary>
        /// Lấy thông tin file/folder
        /// </summary>
        /// <param name="patch">Đường dẫn tập tin NA</param>
        /// <param name="pos">Vị trí bắt đầu của vùng mô tả thư mục</param>
        /// <param name="l">ListBox muốn thêm nội dung vào</param>

        private void getfolder(string patch, int pos, ListBox l)
        {
            l.Items.Clear();
            string fileformat = "", name = "";
            int count = 0, countread = 32; ;
            byte[] entry = new byte[32];
            FileStream fs = new FileStream(patch, FileMode.Open, FileAccess.Read);
            fs.Seek(pos, SeekOrigin.Begin);
            fs.Read(entry, 0, 32);
            // Dò tìm các entry trên thư mục hiện tại
            while(entry[0] == '%' && entry[1] != 0 && entry[2] != 0 && countread < 512)
            {
                name = "";
                fileformat = "";
                // Đọc phần định dạng file từ byte 0 -> byte 4 của entry hiện tại
                for (int i = 1; i < 5; i++)
                {
                    byte t = entry[i];
                    if (t != 0x00)
                        fileformat += (char)t;
                }
                // Đọc phần tên của tập tin
                for (int i = 5; i < 21; i++)
                {
                    byte t = entry[i];
                    if (t != 0x00)
                        name += (char)t;
                }
                count++;
                name = count.ToString() + ". " + name;
                if (fileformat != "SYS")
                    name = name + "." + fileformat;
                l.Items.Add(name);
                fs.Read(entry, 0, 32);
                countread += 32;
            }
            if (count == 0)
                l.Items.Add("Can not find folder or file");
            fs.Close(); 
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Tạo mới file nA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] valnull = new byte[1024];
            byte[] sign = new byte[64];
            sign[0] = (byte)'%';
            sign[1] = (byte)'N';
            sign[2] = (byte)'A';
            sign[3] = (byte)'F';
            //---------------------
            for (int i = 4; i < 64; i++)
                sign[i] = 0;
            for (int i = 0; i < 1024; i++)
                valnull[i] = 0;
            //---------------------
            //Chọn đường dẫn lưu tập tin
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "NA file| *.na";
            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                patch = output.Text = saveFileDlg.FileName;
            }
            //Tạo tập tin khi có đường dẫn
            if (patch != "")
            {
                if (File.Exists(patch))
                    File.Delete(patch);
                FileStream fs = new FileStream(patch, FileMode.Append, FileAccess.Write);
                //Thêm phần dành riêng
                fs.Write(sign, 0, 64);
                //Thêm vào phần mô tả hệ thống thư mục (18 Sector)
                for (int i = 0; i < 9; i++)
                    fs.Write(valnull, 0, valnull.Length);
                    //Thêm vào bảng FAT
                for (int i = 0; i < 12288; i++)
                    fs.Write(valnull, 0, 1024);
                fs.Close();
            }
        }

        private void listfoder_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string t = listfoder.GetItemText(listfoder.SelectedItem);
            MessageBox.Show(t);
        }

        
    }
}
