using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LumenWebControl.Models;

namespace NaoBasicControl
{
    public partial class Form1 : Form
    {
        private static string ip;
        private const int port = 9559;
        private static string text;
        private static MotionRepository model;

        public Form1()
        {
            InitializeComponent();
        }

        private void setModel()
        {
            ip = textBoxIp.Text;
            text = textBoxText.Text;
            model = new MotionRepository(ip, port, text);
        }

        private void btnSay_Click(object sender, EventArgs e)
        {
            setModel();
            model.Speak();
        }

        private void btnStand_Click(object sender, EventArgs e)
        {
            setModel();
            model.Stand();
        }

        private void btnStandInit_Click(object sender, EventArgs e)
        {
            setModel();
            model.StandInit();
        }

        private void btnStandZero_Click(object sender, EventArgs e)
        {
            setModel();
            model.StandZero();
        }

        private void btnCrouch_Click(object sender, EventArgs e)
        {
            setModel();
            model.Crouch();
        }

        private void btnSit_Click(object sender, EventArgs e)
        {
            setModel();
            model.Sit();
        }

        private void btnSitRelax_Click(object sender, EventArgs e)
        {
            setModel();
            model.SitRelax();
        }

        private void btnLyingBelly_Click(object sender, EventArgs e)
        {
            setModel();
            model.LyingBelly();
        }

        private void btnLyingBack_Click(object sender, EventArgs e)
        {
            setModel();
            model.LyingBack();
        }

        private void btnStiffOff_Click(object sender, EventArgs e)
        {
            setModel();
            model.SafeStiffnessOff();
        }

        
    }
}
