﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.GLE;

using OpenTK;
using OpenTK.Graphics.OpenGL; using GL = SamSeifert.GLE.GLR;
using SamSeifert.GLE.CAD;

namespace SamSeifert.GLE.Forms
{
    public partial class FormCVBase : Form
    {
        const float BoardDim = 10;
        const float partSize = BoardDim / 25;

        float ObserveDistance = 30;
        const float BoardDrop = 1;

        private bool GLB = true;
        private bool ALIVE = true;

        public FormCVBase()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GLB = false;

            this.cadHandler1.checkedListBox1.SetItemCheckState(0, CheckState.Checked);

            this.glControl1.MouseWheel += new MouseEventHandler(this.glControl1_MouseWheel);

            GL.ClearColor(Color.LightBlue);
            GL.MatrixMode(MatrixMode.Modelview); // Always the default

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

//            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            //            GL.Enable(EnableCap.Blend);
            //            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.DstAlpha);
            //            GL.BlendFunc(BlendingFactorSrc.OneMinusDstAlpha, BlendingFactorDest.One);

            //            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            Application.Idle += new EventHandler(this.Application_Idle);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (this.GLB) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            new CameraDescriptor(
                this.glControl1.Width,
                this.glControl1.Height,
                60,
                true,
                ObserveDistance * 0.1f,
                ObserveDistance * 2 + 100,
                Matrix4.Identity).SendToGL();

            GL.Translate(0, 0, -ObserveDistance);

            GL.PushMatrix();
            {
                GL.Rotate(this._FloatPanDeltaY, Vector3.UnitX);
                GL.Rotate(this._FloatPanDeltaX, Vector3.UnitY);

                GL.Light(LightName.Light0, LightParameter.Position,
                    new float[] { 0, 500, 500, 1 });

                GL.Enable(EnableCap.Lighting);
                GL.Disable(EnableCap.ColorMaterial);

                foreach (var o in this.cadHandler1.checkedListBox1.Items)
                {
                    var co = o as CadHandler.CadObjectHolder;
                    if (co != null)
                    {
                        if (co.indent == 0)
                        {
                            co.held.Draw(true);
                        }
                    }
                }

                GL.Enable(EnableCap.ColorMaterial);
                GL.Disable(EnableCap.Lighting);

                GL.LineWidth(2);
                {
                    GL.Begin(PrimitiveType.Lines);
                    {
                        GL.Color3(Color.Red);
                        GL.Vertex3(0, 0, 0);
                        GL.Vertex3(BoardDim / 2, 0, 0);

                        GL.Color3(Color.Green);
                        GL.Vertex3(0, 0, 0);
                        GL.Vertex3(0, BoardDim / 2, 0);

                        GL.Color3(Color.Blue);
                        GL.Vertex3(0, 0, 0);
                        GL.Vertex3(0, 0, BoardDim / 2);
                    }
                    GL.End();
                }
                GL.LineWidth(1);

            }
            GL.PopMatrix();

            this.glControl1.SwapBuffers();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (this.ALIVE)
            {
                this._TimeSpanF = (float)(this._StopWatch.Elapsed.TotalSeconds);
                this._StopWatch.Restart();


                if (this.glControl1.IsIdle)
                {
                    this.accumulatorCounter++;
                    this.glControl1.Invalidate();
                }

                this.accumulator += this._TimeSpanF;

                if (accumulator > 1.0f)
                {
                    this.fpsGL = (this.accumulatorCounter) / this.accumulator;
                    this.labelFPS.Text = this.fpsGL.ToString("00.00");
                    this.accumulator = 0;
                    this.accumulatorCounter = 0;

                }
            }
        }

        private float _TimeSpanF = 0;
        private float accumulator = 0;
        private float fpsGL = 0;
        private int accumulatorCounter = 0;
        private Stopwatch _StopWatch = new Stopwatch();




















        private float _FloatPanDeltaX = 45;
        private float _FloatPanDeltaY = 45;
        bool _MouseBoolDown = false;
        Point _MousePointLast = new Point();

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            this._MouseBoolDown = true;
            this._MousePointLast = e.Location;
        }

        private void glControl1_MouseLeave(object sender, EventArgs e)
        {
            this._MouseBoolDown = false;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            this._MouseBoolDown = false;
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._MouseBoolDown)
            {
                this._FloatPanDeltaX += e.X - this._MousePointLast.X;
                this._FloatPanDeltaY += e.Y - this._MousePointLast.Y;
                this._FloatPanDeltaY = Math.Max(-90, Math.Min(90, this._FloatPanDeltaY));
                this._MousePointLast = e.Location;
            }
        }

        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            const float mult = 1.1f;
            if (e.Delta > 0) ObserveDistance *= mult;
            else if (e.Delta < 0) ObserveDistance /= mult;
        }

        private void FormBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.cadHandler1.GLDelete();
        }
    }
}
