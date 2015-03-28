using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //simulation variables
        private ushort PC;
        private short AC;
        private ushort currinstr;
        private System.Collections.Stack ret;
        private bool hasbeenup;
        private bool hastoggled;
        private ushort sonarint;
        private ushort sonaren;
        private ushort sonalarm;
        private ushort sonalarmval;
        private ushort[] distances;

        //displays
        private ushort LEDs;
        private ushort LED1617;
        private ushort XLEDs;

        //inputs
        private ushort switches;
        private ushort switches1617;
        private ushort PBs;

        //odometry variables
        private short xpos;
        private short xvel;
        private short ypos;
        private short yvel;
        private ushort theta;
        private short lpos;
        private short lvel;
        private short rpos;
        private short rvel;

        //MIF file related variables
        string MIFFilePath;
        string[] MIFFile;
        Dictionary<ushort, ushort> codelines;
        Dictionary<int, int> filelines;

        //timer counter for the internal DE2Bot timer
        private ushort timercount;

        //dictionaries used to easily update LED values
        private Dictionary<int, PictureBox> LEDDic;
        private Dictionary<int, PictureBox> XLEDDic;

        public Form1()
        {
            //form initialization
            InitializeComponent();
            
            //initialize the simulation timer (defaults to 100 Hz, Hz being operations per second)
            simtimer = new System.Windows.Forms.Timer();
            simtimer.Interval = 10;

            //initialize the sim
            init();
        }

        private void init()
        {
            //zero out all values
            switches = 0;
            switches1617 = 0;
            LEDs = 0;
            LED1617 = 0;
            XLEDs = 0;
            PBs = 0x00FF;
            AC = 0;
            hasbeenup = false;
            hastoggled = false;

            //reset odometry values
            xpos = 0;
            xvel = 0;
            ypos = 0;
            yvel = 0;
            theta = 0;
            lpos = 0;
            lvel = 0;
            rpos = 0;
            rvel = 0;

            //default sonar values, all interrupts enabled, all sensors disabled
            sonarint = 0x00FF;
            sonaren = 0;
            sonalarm = 0;
            sonalarmval = 0;

            //initialize the dictionaries used to update LEDs
            LEDDic = new Dictionary<int, PictureBox>{
                {0,led0},
                {1,led1},
                {2,led2},
                {3,led3},
                {4,led4},
                {5,led5},
                {6,led6},
                {7,led7},
                {8,led8},
                {9,led9},
                {10,led10},
                {11,led11},
                {12,led12},
                {13,led13},
                {14,led14},
                {15,led15},
                {16,led16},
                {17,led17}
            };
            XLEDDic = new Dictionary<int, PictureBox>{
                {0, xled0},
                {1, xled1},
                {2, xled2},
                {3, xled3},
                {4, xled4},
                {5, xled5},
                {6, xled6},
                {7, xled7}
            };
            //initialize the odometry timer
            odotimer = new System.Windows.Forms.Timer();
            odotimer.Interval = 100;

            //initialize the inner timer of the DE2Bot and initialize count to 0
            innertimer = new System.Windows.Forms.Timer();
            innertimer.Interval = 100;

            //initialize interrupt timer to 10 Hz clock, and disable it by default
            inttimer = new System.Windows.Forms.Timer();
            inttimer.Interval = 100;
            inttimer.Enabled = false;

            //initialize PC stack
            ret = new System.Collections.Stack();

            distances = new ushort[8];
            distances.Initialize();
        }

        #region switches
        private void sw15_Click(object sender, EventArgs e)
        {
            ushort mask = 0x8000;
            if ((switches & mask) > 0)
            {
                sw15.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw15.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw14_Click(object sender, EventArgs e)
        {
            ushort mask = 0x4000;
            if ((switches & mask) > 0)
            {
                sw14.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw14.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw13_Click(object sender, EventArgs e)
        {
            ushort mask = 0x2000;
            if ((switches & mask) > 0)
            {
                sw13.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw13.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw12_Click(object sender, EventArgs e)
        {
            ushort mask = 0x1000;
            if ((switches & mask) > 0)
            {
                sw12.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw12.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw11_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0800;
            if ((switches & mask) > 0)
            {
                sw11.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw11.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw10_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0400;
            if ((switches & mask) > 0)
            {
                sw10.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw10.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw9_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0200;
            if ((switches & mask) > 0)
            {
                sw9.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw9.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw8_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0100;
            if ((switches & mask) > 0)
            {
                sw8.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw8.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw7_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0080;
            if ((switches & mask) > 0)
            {
                sw7.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw7.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw6_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0040;
            if ((switches & mask) > 0)
            {
                sw6.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw6.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw5_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0020;
            if ((switches & mask) > 0)
            {
                sw5.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw5.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw4_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0010;
            if ((switches & mask) > 0)
            {
                sw4.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw4.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw3_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0008;
            if ((switches & mask) > 0)
            {
                sw3.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw3.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw2_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0004;
            if ((switches & mask) > 0)
            {
                sw2.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw2.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw1_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0002;
            if ((switches & mask) > 0)
            {
                sw1.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw1.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw0_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0001;
            if ((switches & mask) > 0)
            {
                sw0.Image = Properties.Resources.sw_down;
                switches -= mask;
                return;
            }
            sw0.Image = Properties.Resources.sw_up;
            switches += mask;
        }

        private void sw16_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0001;
            if ((switches1617 & mask) > 0)
            {
                sw16.Image = Properties.Resources.sw_down;
                switches1617 -= mask;
                return;
            }
            sw16.Image = Properties.Resources.sw_up;
            switches1617 += mask;
        }

        private void sw17_Click(object sender, EventArgs e)
        {
            ushort mask = 0x0002;
            if ((switches1617 & mask) > 0)
            {
                sw17.Image = Properties.Resources.sw_down;
                switches1617 -= mask;
                if (hasbeenup)
                    hastoggled = true;
                return;
            }
            sw17.Image = Properties.Resources.sw_up;
            switches1617 += mask;
            hasbeenup = true;
        }
        #endregion switches

        #region PBs
        private void pb3_MouseDown(object sender, MouseEventArgs e)
        {
            pb3.Image = Properties.Resources.pb_down;
            PBs = (ushort)(PBs & 0xFFF7);
        }

        private void pb3_MouseUp(object sender, MouseEventArgs e)
        {
            pb3.Image = Properties.Resources.pb_up;
            PBs = (ushort)(PBs | 0x0008);
        }

        private void pb2_MouseDown(object sender, MouseEventArgs e)
        {
            pb2.Image = Properties.Resources.pb_down;
            PBs = (ushort)(PBs & 0xFFFB);
        }

        private void pb2_MouseUp(object sender, MouseEventArgs e)
        {
            pb2.Image = Properties.Resources.pb_up;
            PBs = (ushort)(PBs | 0x0004);
        }

        private void pb1_MouseDown(object sender, MouseEventArgs e)
        {
            pb1.Image = Properties.Resources.pb_down;
            PBs = (ushort)(PBs | 0xFFFD);
        }

        private void pb1_MouseUp(object sender, MouseEventArgs e)
        {
            pb1.Image = Properties.Resources.pb_up;
            PBs = (ushort)(PBs | 0x0002);
        }

        private void pb0_MouseDown(object sender, MouseEventArgs e)
        {
            pb0.Image = Properties.Resources.pb_down;
            PBs = (ushort)(PBs & 0xFFFE);
        }

        private void pb0_MouseUp(object sender, MouseEventArgs e)
        {
            pb0.Image = Properties.Resources.pb_up;
            PBs = (ushort)(PBs | 0x0001);
        }
        #endregion PBs

        private void updateLEDs()
        {
            for (int i = 0; i < 16; i++)
            {
                if((LEDs & (1 << i)) > 0)
                {
                    LEDDic[i].Image = Properties.Resources.led_on;
                }
                else
                    LEDDic[i].Image = Properties.Resources.led_off;
            }
            for (int i = 0; i < 2; i++)
            {
                if ((LED1617 & (1 << i)) > 0)
                {
                    LEDDic[i+16].Image = Properties.Resources.led_on;
                }
                else
                    LEDDic[i+16].Image = Properties.Resources.led_off;
            }
            for (int i = 0; i < 8; i++)
            {
                if((XLEDs & (1 << i)) > 0)
                {
                    XLEDDic[i].Image = Properties.Resources.xled_on;
                }
                else
                    XLEDDic[i].Image = Properties.Resources.xled_off;
            }
        }

        private void odotimerTick(object sender, EventArgs e)
        {
            lpos += (short)(lvel / 10);
            rpos += (short)(rvel / 10);
            double speed = (lvel + rvel)/20;
            if(theta < 181)
            {
                xpos += Convert.ToInt16(speed * Math.Cos(Convert.ToDouble(theta) * (Math.PI / 180)));
                ypos += Convert.ToInt16(speed * Math.Sin(Convert.ToDouble(theta) * (Math.PI / 180)));
            }
            else
            {
                double temp = Convert.ToDouble(theta) - 360;
                xpos += Convert.ToInt16(speed * Math.Cos(temp * (Math.PI / 180)));
                ypos += Convert.ToInt16(speed * Math.Sin(temp * (Math.PI / 180)));
            }
            double angle = Math.Atan2(ypos, xpos) * (180/Math.PI);
            if (angle < 0)
                angle += 360;
            theta = Convert.ToUInt16(angle);
        }

        private void simTimerTick(object sender, EventArgs e)
        {
            simStep();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            timercount++;
        }

        private void IntTimerTick(object sender, EventArgs e)
        {
            ret.Push(PC);
            PC = 0x0001;
        }

        private void simStep()
        {
            unHighlightText();
            sonarInts();
            currinstr = codelines[PC];
            PC++;
            decode();
            updateBoxes();
            highlightText();
        }

        private void decode()
        {
            ushort opcode = (ushort)(currinstr & 0xFC00);
            ushort operand = (ushort)(currinstr & 0x03FF);
            short signedoperand;
            if ((operand & 0x0200) > 0)
                signedoperand = (short)(operand | 0xFC00);
            else
                signedoperand = (short)operand;
            switch (opcode)
            {
                case 0:
                    return;
                case 0x0400:
                    AC = (short)codelines[operand];
                    break;
                case 0x0800:
                    codelines[operand] = (ushort)AC;
                    break;
                case 0x0C00:
                    AC += (short)codelines[operand];
                    break;
                case 0x1000:
                    AC -= (short)codelines[operand];
                    break;
                case 0x1400:
                    PC = operand;
                    break;
                case 0x1800:
                    if (AC < 0)
                        PC = operand;
                    break;
                case 0x1C00:
                    if (AC > 0)
                        PC = operand;
                    break;
                case 0x2000:
                    if (AC == 0)
                        PC = operand;
                    break;
                case 0x2400:
                    AC = (short)(AC & codelines[operand]);
                    break;
                case 0x2800:
                    AC = (short)(AC | codelines[operand]);
                    break;
                case 0x2C00:
                    AC = (short)(AC ^ codelines[operand]);
                    break;
                case 0x3000:
                    AC = (short)(AC << signedoperand);
                    break;
                case 0x3400:
                    AC += signedoperand;
                    break;
                case 0x3800:
                    AC = (short)codelines[codelines[operand]];
                    break;
                case 0x3C00:
                    codelines[codelines[operand]] = (ushort)AC;
                    break;
                case 0x4000:
                    ret.Push(PC);
                    PC = operand;
                    break;
                case 0x4400:
                    PC = (ushort)ret.Pop();
                    break;
                case 0x4800:
                    In(operand);
                    break;
                case 0x4C00:
                    Out(operand);
                    break;
                default:
                    return;
            }

        }

        #region toolstrip
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simtimer.Enabled = true;
            simtimer.Tick += simTimerTick;
            innertimer.Enabled = true;
            innertimer.Tick += TimerTick;
            simSpeedToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
            stopbutton.Enabled = true;
            startToolStripMenuItem.Enabled = false;
            startbutton.Enabled = false;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simtimer.Enabled = false;
            simtimer.Tick -= simTimerTick;
            innertimer.Enabled = false;
            innertimer.Tick -= TimerTick;
            simSpeedToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = false;
            stopbutton.Enabled = false;
            startToolStripMenuItem.Enabled = true;
            startbutton.Enabled = true;
        }

        private void hzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simtimer.Interval = 1000;
        }

        private void hzToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            simtimer.Interval = 100;
        }

        private void hzToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            simtimer.Interval = 10;
        }

        private void kHzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simtimer.Interval = 1;
        }
        #endregion toolstrip

        private void In(ushort operand)
        {
            switch (operand)
            {
                case 0:
                    AC = (short)switches;
                    break;
                case 2:
                    AC = (short)timercount;
                    break;
                case 3:
                    ushort result = 0;
                    if(hastoggled)
                    {
                        result = (ushort)(result | (1 << 4));
                    }
                    result = (ushort)(result | (ushort)((switches1617 & 0x0001)<<3));
                    result = (ushort)(result | (ushort)((PBs & 0x000D)>>1));
                    AC = (short)result;
                    break;
                case 0x0080:
                    AC = lpos;
                    break;
                case 0x0082:
                    AC = lvel;
                    break;
                case 0x0088:
                    AC = rpos;
                    break;
                case 0x008A:
                    AC = rvel;
                    break;
                case 0x0091:
                    AC = 0x00FF;
                    break;
                case 0x0092:
                    AC = 0;
                    break;
                case 0x0098:
                    AC = 0;
                    break;
                case 0x0099:
                    AC = 0;
                    break;
                case 0x00A8:
                    AC = (short)distances[0];
                    break;
                case 0x00A9:
                    AC = (short)distances[1];
                    break;
                case 0x00AA:
                    AC = (short)distances[2];
                    break;
                case 0x00AB:
                    AC = (short)distances[3];
                    break;
                case 0x00AC:
                    AC = (short)distances[4];
                    break;
                case 0x00AD:
                    AC = (short)distances[5];
                    break;
                case 0x00AE:
                    AC = (short)distances[6];
                    break;
                case 0x00AF:
                    AC = (short)distances[7];
                    break;
                case 0x00B0:
                    AC = (short)sonalarm;
                    break;
                case 0x00C0:
                    AC = xpos;
                    break;
                case 0x00C1:
                    AC = ypos;
                    break;
                case 0x00C2:
                    AC = (short)theta;
                    break;
                default:
                    break;

            }
        }

        private void Out(ushort operand)
        {
            switch(operand)
            {
                case 1:
                    LEDs = (ushort)AC;
                    updateLEDs();
                    break;
                case 2:
                    innertimer.Enabled = false;
                    innertimer.Tick -= TimerTick;
                    timercount = 0;
                    innertimer.Enabled = true;
                    innertimer.Tick += TimerTick;
                    break;
                case 4:
                    SSEG1box.Text = AC.ToString("X");
                    break;
                case 5:
                    SSEG2box.Text = AC.ToString("X");
                    break;
                case 6:
                    LCDbox.Text = AC.ToString("X");
                    break;
                case 7:
                    XLEDs = (ushort)AC;
                    LED1617 = (ushort)((AC & 0x0300) >> 8);
                    updateLEDs();
                    break;
                case 0x000A:
                    beepbox.Text = AC.ToString("X");
                    break;
                case 0x000C:
                    inttimer.Enabled = false;
                    inttimer.Tick -= IntTimerTick;
                    inttimer.Interval = AC;
                    inttimer.Enabled = true;
                    inttimer.Tick += IntTimerTick;
                    break;
                case 0x0083:
                    lvel = AC;
                    break;
                case 0x008B:
                    rvel = AC;
                    break;
                case 0x00B0:
                    sonalarmval = (ushort)AC;
                    break;
                case 0x00B1:
                    sonarint = (ushort)AC;
                    sonarintbox.Text = sonarint.ToString("X");
                    break;
                case 0x00B2:
                    sonaren = (ushort)AC;
                    sonarenbox.Text = sonaren.ToString("X");
                    break;
                case 0x00C3:
                    xpos = 0;
                    xvel = 0;
                    ypos = 0;
                    yvel = 0;
                    theta = 0;
                    lpos = 0;
                    lvel = 0;
                    rpos = 0;
                    rvel = 0;
                    break;
                default:
                    break;

            }
        }

        private void stepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simStep();
        }

        private void sonarInts()
        {
            for (int i = 0; i < 8; i++)
            {
                ushort mask = (ushort)(1 << i);
                if((sonaren & mask) > 0)
                {
                    if (distances[i] < sonalarmval)
                    {
                        sonalarm = (ushort)(sonalarm | mask);
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                ushort mask = (ushort)(1 << i);
                if ((sonaren & mask) > 0)
                {
                    if ((sonarint & mask) > 0)
                    {
                        ret.Push(PC);
                        PC = 0x0002;
                    }
                }
            }
        }

        private void openMIFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog broswer = new OpenFileDialog();
            broswer.Filter = "MIF Files|*.mif;*.MIF";
            broswer.ShowDialog();
            MIFFilePath = broswer.FileName;
            try
            {
                loadFile();
            }
            catch (Exception f)
            {
                MessageBox.Show("Could not read from MIF file:\n" + f.Message, "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = true;
                stepbutton.Enabled = false;
                return;
            }
            simulateToolStripMenuItem.Enabled = true;
            resetbutton.Enabled = true;
            stepbutton.Enabled = true;
            startbutton.Enabled = true;
        }

        private void loadFile()
        {
            MIFFile = System.IO.File.ReadAllLines(MIFFilePath);
            string[] line;
            codelines = new Dictionary<ushort,ushort>();
            filelines = new Dictionary<int, int>();
            for(int i = 12; i < MIFFile.Length - 1; i++)
            {
                line = MIFFile[i].Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries);
                codelines.Add(Convert.ToUInt16("0x" + line[0], 16), Convert.ToUInt16("0x" + line[2].Substring(0, 4), 16));
                filelines.Add(Convert.ToInt32(line[0], 16), i);
            }
            PC = codelines.Keys.ElementAt(0);
            filetextbox.Text = System.IO.File.ReadAllText(MIFFilePath);
            highlightText();
            //reset the simulator
            init();
            updateBoxes();
            updateLEDs();
        }

        private void updateBoxes()
        {
            xposbox.Text = xpos.ToString();
            xvelbox.Text = xvel.ToString();
            yposbox.Text = ypos.ToString();
            yvelbox.Text = yvel.ToString();
            thetabox.Text = theta.ToString();
            lposbox.Text = lpos.ToString();
            lvelbox.Text = lvel.ToString();
            rposbox.Text = rpos.ToString();
            rvelbox.Text = rvel.ToString();
            PCbox.Text = PC.ToString("X");
            ACbox.Text = AC.ToString("X");
        }

        #region Sensor input boxes
        private void dist0box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                distances[0] = Convert.ToUInt16(dist0box.Text);
            }
            catch
            {
                MessageBox.Show("Inavild value entered", "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = false;
                stepbutton.Enabled = false;
                return;
            }
        }

        private void dist1box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                distances[1] = Convert.ToUInt16(dist0box.Text);
            }
            catch
            {
                MessageBox.Show("Inavild value entered", "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = false;
                stepbutton.Enabled = false;
                return;
            }
        }

        private void dist2box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                distances[2] = Convert.ToUInt16(dist0box.Text);
            }
            catch
            {
                MessageBox.Show("Inavild value entered", "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = false;
                stepbutton.Enabled = false;
                return;
            }
        }

        private void dist3box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                distances[3] = Convert.ToUInt16(dist0box.Text);
            }
            catch
            {
                MessageBox.Show("Inavild value entered", "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = false;
                stepbutton.Enabled = false;
                return;
            }
        }

        private void dist4box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                distances[4] = Convert.ToUInt16(dist0box.Text);
            }
            catch
            {
                MessageBox.Show("Inavild value entered", "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = false;
                stepbutton.Enabled = false;
                return;
            }
        }

        private void dist5box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                distances[5] = Convert.ToUInt16(dist0box.Text);
            }
            catch
            {
                MessageBox.Show("Inavild value entered", "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = false;
                stepbutton.Enabled = false;
                return;
            }
        }

        private void dist6box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                distances[6] = Convert.ToUInt16(dist0box.Text);
            }
            catch
            {
                MessageBox.Show("Inavild value entered", "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = false;
                stepbutton.Enabled = false;
                return;
            }
        }

        private void dist7box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                distances[7] = Convert.ToUInt16(dist0box.Text);
            }
            catch
            {
                MessageBox.Show("Inavild value entered", "Error", MessageBoxButtons.OK);
                simulateToolStripMenuItem.Enabled = false;
                resetbutton.Enabled = false;
                stepbutton.Enabled = false;
                return;
            }
        }
        #endregion sensors

        private void resetSimulationToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            PC = codelines.Keys.ElementAt(0);
            filetextbox.Text = System.IO.File.ReadAllText(MIFFilePath);
            highlightText();
            init();
            updateBoxes();
            updateLEDs();
        }

        private void stepbutton_Click(object sender, EventArgs e)
        {
            simStep();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PC = codelines.Keys.ElementAt(0);
            filetextbox.Text = System.IO.File.ReadAllText(MIFFilePath);
            highlightText();
            init();
            updateBoxes();
            updateLEDs();
        }

        private void highlightText()
        {
            int index = filetextbox.GetFirstCharIndexFromLine(filelines[PC]);
            filetextbox.Select(index, MIFFile[filelines[PC]].Length);

            //Set the selected text fore and background color
            filetextbox.SelectionColor = System.Drawing.Color.White;
            filetextbox.SelectionBackColor = System.Drawing.Color.Blue;
        }

        private void unHighlightText()
        {
            int index = filetextbox.GetFirstCharIndexFromLine(filelines[PC]);
            filetextbox.Select(index, MIFFile[filelines[PC]].Length);

            //Set the selected text fore and background color
            filetextbox.SelectionColor = System.Drawing.Color.Black;
            filetextbox.SelectionBackColor = System.Drawing.Color.White;
        }

        private void resetbutton_Click_1(object sender, EventArgs e)
        {
            PC = codelines.Keys.ElementAt(0);
            filetextbox.Text = System.IO.File.ReadAllText(MIFFilePath);
            highlightText();
            init();
            updateBoxes();
            updateLEDs();
        }
    }
}
