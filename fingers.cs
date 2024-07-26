using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eVerification
{
    public partial class Fingers : Form
    {
        public Fingers()
        {
            InitializeComponent();
        }

        public void DisplayImages(List<Image> images)
        {
            // Assuming you have 10 PictureBox controls named pictureBox1, pictureBox2, ..., pictureBox10
            var panels = new List<Panel> { panel3, panel4, panel5, panel6, panel7, panel8, panel9, panel10, panel11, panel12, panel13, panel14, panel15, panel16 };

            for (int i = 0; i < images.Count && i < panels.Count; i++)
            {
                panels[i].BackgroundImage = images[i];
                panels[i].BackgroundImageLayout = ImageLayout.Stretch; // Adjust the image layout as needed
            }
        }


    }
}
