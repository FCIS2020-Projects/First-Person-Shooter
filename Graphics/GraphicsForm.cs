using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace Graphics
{
    public partial class GraphicsForm : Form
    {
        Renderer renderer = new Renderer();
        Thread MainLoopThread;
        bool[] keys = new bool[1024];
        float deltaTime;
        public GraphicsForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();

            MoveCursor();
            

            initialize();
            deltaTime = 0.005f;
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();
        }
        void initialize()
        {
            renderer.Initialize();
            //while (true)
            //{
                renderer.Draw();
                renderer.Update(deltaTime);

            //}

        }
        void MainLoop()
        {
            while (true)
            {
                simpleOpenGlControl1.Refresh();
                textBox5.Text = renderer.zombie.animSt.curr_frame + "";
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            doMovement();
            renderer.Draw();
            renderer.Update(deltaTime);
        }

        private void doMovement()
        {
            float speed = 1f;
            
            if (keys['A'])
                renderer.cam.Strafe(-speed);
            if (keys['D'])
                renderer.cam.Strafe(speed);
            if (keys['S'])
                renderer.cam.Walk(-speed);
            if (keys['W'])
                renderer.cam.Walk(speed);
            if (keys['Z'])
                renderer.cam.Fly(-speed);
            if (keys['C'])
                renderer.cam.Fly(speed);
        }

        float prevX, prevY;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            float speed = 0.05f;

            float delta = e.X - prevX;
            if (delta > 0)
                renderer.cam.Yaw(-speed);
            else if (delta < 0)
                renderer.cam.Yaw(speed);


            delta = e.Y - prevY;
            if (delta > 0)
                renderer.cam.Pitch(-speed);
            else if (delta < 0)
                renderer.cam.Pitch(speed);

            MoveCursor();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            //float r = float.Parse(textBox1.Text);
            //float g = float.Parse(textBox2.Text);
            //float b = float.Parse(textBox3.Text);
            //float a = float.Parse(textBox4.Text);
            //float s = float.Parse(textBox5.Text);
            //renderer.SendLightData(r, g, b, a, s);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            renderer.zombie.StartAnimation(_3D_Models.animType_LOL.STAND);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            renderer.zombie.StartAnimation(_3D_Models.animType_LOL.ATTACK1);
        }

        private void button4_Click(object sender, EventArgs e)
        {

            renderer.zombie.StartAnimation(_3D_Models.animType_LOL.ATTACK2);
        }

        private void button5_Click(object sender, EventArgs e)
        {

            renderer.zombie.StartAnimation(_3D_Models.animType_LOL.RUN);
        }

        private void button6_Click(object sender, EventArgs e)
        {

            renderer.zombie.StartAnimation(_3D_Models.animType_LOL.SPELL1);
        }

        private void button7_Click(object sender, EventArgs e)
        {

            renderer.zombie.StartAnimation(_3D_Models.animType_LOL.SPELL2);
        }

        private void button8_Click(object sender, EventArgs e)
        {

            renderer.zombie.StartAnimation(_3D_Models.animType_LOL.DEATH);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float res = 0;
            if (float.TryParse(textBox1.Text,out res))
            {
                renderer.zombie.AnimationSpeed = res;
                //renderer.Blade.AnimationSpeed = res;
            }
        }


        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width/2+p.X, simpleOpenGlControl1.Size.Height/2+p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X+simpleOpenGlControl1.Size.Width/2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }

        private void SimpleOpenGlControl1_KeyUp(object sender, KeyEventArgs e)
        {
            keys[e.KeyValue] = false;
        }

        private void SimpleOpenGlControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            keys[e.KeyValue] = true;

            //To do 2.5 ==> Animate "Blade" 3D Model using 1, 2, 3 ... different 3D
            /*if (e.KeyChar == '1')
                //...
                renderer.Blade.StartAnimation(_3D_Models.animType.JUMP);
            if (e.KeyChar == '2')
                //...
                renderer.Blade.StartAnimation(_3D_Models.animType.SALUTE);
            if (e.KeyChar == '3')
                renderer.Blade.StartAnimation(_3D_Models.animType.WAVE);
            //...
            label6.Text = "X: " + renderer.cam.GetCameraPosition().x;
            label7.Text = "Y: " + renderer.cam.GetCameraPosition().y;
            label8.Text = "Z: " + renderer.cam.GetCameraPosition().z;*/
        }
        private void SimpleOpenGlControl1_MouseClick(object sender, MouseEventArgs e)
        {
            renderer.bullets.Add(new bullet(renderer.bulletModel, renderer.cam));
            renderer.draw = true;
        }

        private void simpleOpenGlControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
