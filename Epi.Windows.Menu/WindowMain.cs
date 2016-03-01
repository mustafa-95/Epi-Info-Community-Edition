﻿using Epi.Windows.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Epi.Windows.Menu
{
    public partial class WindowMain : Form
    {
        private ElementHost host;
        MainWindow form = null;
        public WindowMain() 
        {
            InitializeComponent();
            this.MaximumSize = new System.Drawing.Size(857, 745);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            if (this.Width == 659 && this.Height == 641)
            {
               this.Size = new Size(690, 600);               
            }
          
            host = new ElementHost();
            host.Dock = DockStyle.Fill;
            form = new MainWindow();
 
            host.Child = form;
            this.Controls.Add(host);
        }       

        

        

       

        

        

       
    }
}
