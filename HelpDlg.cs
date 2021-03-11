using System;
using System.Drawing;
using System.Windows.Forms;


//====================================================
// Описание работы классов и методов исходника на:
// https://www.interestprograms.ru
// Исходные коды программ и игр
// Автор исходных кодов Клочков Павел
//====================================================


namespace mosaic
{
    // Класс диалогового окна помощи пользователю
    public partial class HelpDlg : Form
    {
        public HelpDlg()
        {
            InitializeComponent();
        }

        public Image ImageDuplicate = null;

        // Загрузка картинки на форму посредством PictureBox.
        private void Form3_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = ImageDuplicate;
        }



    }
}
