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
    /// <summary>
    /// Главная форма игры. 
    /// Управляет игрой.
    /// Размещает на себе элементы
    /// сегменты PictureBox.
    /// </summary>
    public partial class FormMain : Form
    {
        // Массив объектов прямоугольников для хранения сегментов картинки.
        private PictureBox[] pbSegments;

        // Длина стороны в прямоугольниках.
        private int numRect = 3;

        // Объект хранения картинки.
        private Bitmap Picture;


        public FormMain()
        {
            InitializeComponent();


        }


        #region Управление программой. События

        /// <summary>
        /// Загрузка картинки.
        /// </summary>
        private void ToolStripButtonLoadPicture_Click(object sender, EventArgs e)
        {
            LoadPicture();
        }



        /// <summary>
        /// Перемещение сегмента при кликании на нём.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PB_Click(object sender, EventArgs e)
        {
            TranslationSegment(sender as PictureBox);
        }



        /// <summary>
        /// Перемешивание сегментов картинки.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButtonMixed_Click(object sender, EventArgs e)
        {
            MixedSegments();
        }



        /// <summary>
        /// Восстанавление картинки.
        /// </summary>
        private void ToolStripButtonRestore_Click(object sender, EventArgs e)
        {
            RestorePicture();
        }



        /// <summary>
        /// Открывается диалоговое окно настроек приложения.
        /// </summary>
        private void ToolStripButtonSetting_Click(object sender, EventArgs e)
        {
            SettingUser();
        }



        /// <summary>
        /// Открываем диалоговое окно с нормальным видом картинки,
        /// для освежения памяти пользователя.
        /// </summary>
        private void ToolStripButtonHelp_Click(object sender, EventArgs e)
        {
            HelpUser();
        }



        /// <summary>
        /// Отслеживание изменения размеров окна
        /// </summary>
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            CorrectSizeSegments();
        }


        #endregion



        #region Логика игры

        /// <summary>
        /// Загрузка картинки и создание сегментов.
        /// </summary>
        private void LoadPicture()
        {
            var ofDlg = new OpenFileDialog();

            // Фильтр показа изображений с определенным расширением.
            ofDlg.Filter = "файлы картинок (*.bmp;*.jpg;*.jpeg;*.gif;)|";
            ofDlg.Filter += "*.bmp;*.jpg;*.jpeg;*.gif|All files (*.*)|*.*";
            ofDlg.FilterIndex = 1;
            ofDlg.RestoreDirectory = true;

            if (ofDlg.ShowDialog() == DialogResult.OK)
            {
                // Загружаем выбранную картинку.
                Picture = new Bitmap(ofDlg.FileName);

                // Создание сегментов
                CreatePictureSegments();
            }
        }



        /// <summary>
        /// Создание сегментов картинки
        /// </summary>
        private void CreatePictureSegments()
        {
            // Удалим предыдущий массив, чтобы создать новый.
            if (pbSegments != null)
            {
                for (int i = 0; i < pbSegments.Length; i++)
                {
                    pbSegments[i].Dispose();
                }
                pbSegments = null;
            }


            // Создаем массив прямоугольников установленного размера.
            pbSegments = new PictureBox[numRect * numRect];


            // Вычислим габаритные размеры прямоугольников.
            int w = ClientSize.Width / numRect;
            int h = ClientSize.Height / numRect;

            // Счетчики порядкового номера по координатам Х и Y в каждом отдельном ряде.
            int countX = 0;
            int countY = 0;

            for (int i = 0; i < pbSegments.Length; i++)
            {
                // Размеры и координаты размещения созданного прямоугольника.
                pbSegments[i] = new PictureBox
                {
                    Width = w,
                    Height = h,
                    Left = countX * w,
                    Top = countY * h
                };


                // Запомним начальные координаты прямоугольника для
                // восстановления перемешанной картинки,
                // и определения полной сборки картинки.
                Point pt = new Point();
                pt.X = pbSegments[i].Left;
                pt.Y = pbSegments[i].Top;

                // сохраним координаты в свойстве Tag каждого прямоугольника
                pbSegments[i].Tag = pt;

                // Считаем прямоугольники по рядам и столбцам.
                countX++;
                if (countX == numRect)
                {
                    countX = 0;
                    countY++;
                }


                pbSegments[i].Parent = this;
                pbSegments[i].BorderStyle = BorderStyle.None;
                pbSegments[i].SizeMode = PictureBoxSizeMode.StretchImage;

                // Новые сегменты должны быть все видимы.
                pbSegments[i].Show();


                // Для всех прямоугольников массива событие клика мыши
                // будет обрабатываться в одной и том же методе.
                pbSegments[i].Click += new EventHandler(PB_Click);
            }


            DrawPicture();

        }



        /// <summary>
        /// Копируем части картинки в соответствующие боксы и 
        /// боксы рисуют свою порцию изображения.
        /// </summary>
        private void DrawPicture()
        {
            if (Picture == null) return;

            int countX = 0;
            int countY = 0;

            for (int i = 0; i < pbSegments.Length; i++)
            {
                int w = Picture.Width / numRect;
                int h = Picture.Height / numRect;
                pbSegments[i].Image = Picture.Clone(new RectangleF(countX * w, countY * h, w, h), Picture.PixelFormat);
                countX++;
                if (countX == numRect)
                {
                    countX = 0;
                    countY++;
                }

            }
        }




        /// <summary>
        /// Перемещение сегмента
        /// </summary>
        /// <param name="pb">активный сегмент</param>
        private void TranslationSegment(PictureBox pb)
        {

            for (int i = 0; i < pbSegments.Length; i++)
            {
                // Сначала определим пустое место на области рисования картинки.
                if (pbSegments[i].Visible == false)
                {
                    // Затем проверим кликнутый прямоугольник и
                    // если у него совпадают координаты по X или Y,
                    // (откидываем из вычисления прямоугольники расположеннные по диагонали)
                    // но при этом он на ближайшем расстоянии от пустого
                    // прямоугольника
                    // (откидываем прямоугольники расположенные через прямоугольник от пустого)
                    if ((pb.Location.X == pbSegments[i].Location.X &&
                         Math.Abs(pb.Location.Y - pbSegments[i].Location.Y) == pbSegments[i].Height)
                        ||
                        (pb.Location.Y == pbSegments[i].Location.Y &&
                         Math.Abs(pb.Location.X - pbSegments[i].Location.X) == pbSegments[i].Width))
                    {
                        // После успешной проверки
                        // меняем местами пустой и кликнутый прямоугольники.
                        Point ptemp = pbSegments[i].Location;
                        pbSegments[i].Location = pb.Location;
                        pb.Location = ptemp;

                        // После каждого хода проверка на полную сборку картинки.
                        //*************** блок проверки ***********************

                        // Если хоть у одного прямоугольника не совпадают
                        // реальные координаты и первичные заканчиваем 
                        // проверку и выходим из метода.
                        for (int j = 0; j < pbSegments.Length; j++)
                        {
                            Point point = (Point)pbSegments[j].Tag;
                            if (pbSegments[j].Location != point)
                            {
                                return;
                            }
                        }

                        // Если у всех прямоугольников совпали реальные и первичные 
                        // координаты - картинка собрана!
                        for (int m = 0; m < pbSegments.Length; m++)
                        {
                            // Делаем видимыми все сегменты картинки.
                            pbSegments[m].Visible = true;

                            // Убираем обрамление прямоугольников.
                            pbSegments[m].BorderStyle = BorderStyle.None; 
                        }

                        //************** окончание блока проверки *************
                    }

                    break;
                }
            }

        }



        /// <summary>
        /// Перемешивание сегментов
        /// </summary>
        private void MixedSegments()
        {
            if (Picture == null) return;

            // Создаем объект генерирования псевдослучайных чисел,
            // для различного набора случайных чисел инициализацию
            // объекта Random производим от счетчика количества
            // миллисекунд прошедших со времени запуска операционной системы.
            Random rand = new Random(Environment.TickCount);

            for (int i = 0; i < pbSegments.Length; i++)
            {
                pbSegments[i].Visible = true;
                int temp = rand.Next(0, pbSegments.Length);
                Point ptR = pbSegments[temp].Location;
                Point ptI = pbSegments[i].Location;
                pbSegments[i].Location = ptR;
                pbSegments[temp].Location = ptI;

                // Бордюр чтобы видно было прямоугольники
                pbSegments[i].BorderStyle = BorderStyle.Fixed3D;
            }

            // Случайным образом выбираем пустой прямоугольник,
            // делаем его невидимым.
            int r = rand.Next(0, pbSegments.Length);
            pbSegments[r].Visible = false;
        }


        /// <summary>
        /// Восстановление картинки
        /// </summary>
        private void RestorePicture()
        {
            for (int i = 0; i < pbSegments.Length; i++)
            {
                Point pt = (Point)pbSegments[i].Tag;
                pbSegments[i].Location = pt;
                pbSegments[i].Visible = true;
            }
        }



        /// <summary>
        /// Настройки пользователя
        /// </summary>
        private void SettingUser()
        {
            var setDlg = new SetDlg
            {
                numRect = this.numRect
            };

            if (setDlg.ShowDialog() == DialogResult.OK)
            {
                this.numRect = setDlg.numRect;


                CreatePictureSegments();
            }
        }



        /// <summary>
        /// Показывается полная картинка для облегчения сборки.
        /// </summary>
        private void HelpUser()
        {
            var helpDlg = new HelpDlg();
            helpDlg.ImageDuplicate = Picture;
            helpDlg.ShowDialog();
        }



        /// <summary>
        /// Корректировка размеров сегментов при изменении размеров окна.
        /// </summary>
        private void CorrectSizeSegments()
        {
            if (pbSegments == null) return;


            // Предыдущие размеры сегментов
            int oldwidth = pbSegments[0].Width;
            int oldheight = pbSegments[0].Height;


            // Новые размеры прямоугольников.
            int w = ClientSize.Width / numRect;
            int h = ClientSize.Height / numRect;



            //int countX = 0; // счетчик прямоугольников по координате Х в одном ряду
            //int countY = 0; // счетчик прямоугольников по координате Y в одном столбце
            for (int i = 0; i < pbSegments.Length; i++)
            {
                pbSegments[i].Width = w;
                pbSegments[i].Height = h;

                // Получим порядковый номер сегмента по координате Х
                int countX = pbSegments[i].Left /= oldwidth;

                // Получим порядковый номер сегмента по координате Y
                int countY = pbSegments[i].Top /= oldheight;


                pbSegments[i].Left = countX * w;
                pbSegments[i].Top = countY * h;

            }
        }



        #endregion

    }
}
