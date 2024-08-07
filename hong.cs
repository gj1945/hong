using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.Json.Nodes;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

using System.Net;

using System.Drawing.Imaging;
using System.IO;
using System.Collections.Immutable;
using System.Text;
using Microsoft.VisualBasic;

public class MouseMacroForm : Form
{
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private MenuStrip menuStrip;
    private ToolStripMenuItem portMenuItem;
    private string selectedPort;

    Action listendo = null;

    public void SetListendo(Action newAction)
    {
        listendo = newAction;
    }

    public void ClearListendo()
    {
        listendo = null;
    }

    public void log(object obj)
    {
        Console.WriteLine(obj);
    }

    private async Task InitializeAsync()
    {
        await Task.Run(() =>
        {
            while (true)
            {
                if (listendo != null)
                {
                    listendo();
                }
                Thread.Sleep(1);
            }
        });
    }

    bool nohongrun = true;

    private async Task loadimage()
    {
        // await Task.Run(() =>
        // {
        //     while (true)
        //     {
        //         if (nohongrun)
        //         {
        //             netdll.kmNet_lcd_picture(GenerateImage());
        //             Thread.Sleep(60000);
        //         }
        //         Thread.Sleep(1);
        //     }
        // });
        // await Task.Run(() =>
        // {
        //     while (true)
        //     {
        //         netdll.kmNet_lcd_picture(CaptureWindow());
        //         Thread.Sleep(1000);
        //     }
        // });
    }


    string configPath = "";
    string bindPath = "config/bind.json";

    ToolStripMenuItem fileMenuItem = null;
    public MouseMacroForm()
    {
        // 判断是否有config目录 没有则创建
        if (!Directory.Exists("config"))
        {
            Directory.CreateDirectory("config");
        }

        netdll.kmNet_init("192.168.2.188", "12994", "73EFE04E");
        netdll.kmNet_unmask_all();
        netdll.kmNet_monitor(10086);

        _delegate = new WinEventDelegate(WinEventProc);
        _winEventHook = SetWinEventHook(0x0003, 0x0003, IntPtr.Zero, _delegate, 0, 0, 0);

        this.Load += async (sender, args) => await InitializeAsync();
        // this.Load += async (sender, args) =>
        // {
        //     // 启动 loadimage
        //     await loadimage();
        //     netdll.kmNet_lcd_picture(GenerateImage());
        //     // 计算到下一个整数秒的时间差
        //     var delay = 60000 - DateTime.Now.Second * 1000 - DateTime.Now.Millisecond;
        //     // 等待到下一个整数秒
        //     await Task.Delay(delay);
        // };

        // Create a MenuStrip for port selection.
        menuStrip = new MenuStrip();
        menuStrip.BackColor = Color.White;
        this.Controls.Add(menuStrip);


        // 添加一个下拉菜单 名字是文件 下拉选中正在操作的文件 ，文件目录从 config/目录下去读 .json文件
        fileMenuItem = new ToolStripMenuItem("文件");
        menuStrip.Items.Add(fileMenuItem);

        // 读取config目录下的所有json文件
        string[] files = Directory.GetFiles("config", "*.json");
        // 排除bind.json文件
        files = files.Where(file => Path.GetFileName(file) != "bind.json").ToArray();

        var newfile = new ToolStripMenuItem("新文件");
        newfile.Click += (object sender, EventArgs e) =>
        {
            saveconfig();
            netdll.kmNet_mouse_all(0, 0, 0, 0);
            netdll.kmNet_unmask_all();
            // 弹窗输入文件名 
            string filename = Interaction.InputBox("请输入文件名", "新建文件");
            if (filename == "")
            {
                MessageBox.Show("请输入文件名");
                return;
            }

            // 检测重复
            if (File.Exists("config/" + filename + ".json"))
            {
                MessageBox.Show("文件已存在");
                return;
            }

            configPath = "config/" + filename + ".json";

            // 创建新文件
            System.IO.File.WriteAllText(configPath, "[]");
            loadconfig();
            addhong(null, null);
            saveconfig();

            var menuItem = new ToolStripMenuItem(filename);
            menuItem.Click += (object sender, EventArgs e) =>
            {
                netdll.kmNet_mouse_all(0, 0, 0, 0);
                netdll.kmNet_unmask_all();
                configPath = "config/" + menuItem.Text + ".json"; ;
                loadconfig();
                // 其他的文件取消选中
                foreach (ToolStripMenuItem item in fileMenuItem.DropDownItems)
                {
                    item.Checked = false;
                }

                menuItem.Checked = true;
            };
            fileMenuItem.DropDownItems.Add(menuItem);

            foreach (ToolStripMenuItem item in fileMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            menuItem.Checked = true;
        };
        fileMenuItem.DropDownItems.Add(newfile);

        foreach (string file in files)
        {
            var menuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file));
            menuItem.Click += (object sender, EventArgs e) =>
            {
                netdll.kmNet_mouse_all(0, 0, 0, 0);
                netdll.kmNet_unmask_all();
                configPath = "config/" + menuItem.Text + ".json"; ;
                loadconfig();
                // 其他的文件取消选中
                foreach (ToolStripMenuItem item in fileMenuItem.DropDownItems)
                {
                    item.Checked = false;
                }

                menuItem.Checked = true;
            };
            fileMenuItem.DropDownItems.Add(menuItem);

            // 选中时间最新的文件
            if (configPath == "" || File.GetLastWriteTime(file) > File.GetLastWriteTime(configPath))
            {
                configPath = file;
            }
        }

        // 显示当前选中的文件
        foreach (ToolStripMenuItem item in fileMenuItem.DropDownItems)
        {
            if (item.Text == Path.GetFileNameWithoutExtension(configPath))
            {
                item.Checked = true;
            }
        }

        // 添加新增按钮
        var addMenuItem = new ToolStripMenuItem("新增");
        addMenuItem.Click += addhong;

        menuStrip.Items.Add(addMenuItem);

        // 保存按钮
        var saveMenuItem = new ToolStripMenuItem("保存");
        saveMenuItem.Click += (object sender, EventArgs e) =>
        {
            saveconfig();

            // 按钮名称变为已保存 1s后恢复
            saveMenuItem.Text = "已保存";
            // 颜色red
            saveMenuItem.ForeColor = Color.Red;
            Task.Delay(700).ContinueWith(t =>
            {
                saveMenuItem.Text = "保存";
                saveMenuItem.ForeColor = Color.Black;
            });
        };
        menuStrip.Items.Add(saveMenuItem);

        // 重命名
        var renameMenuItem = new ToolStripMenuItem("重命名");
        renameMenuItem.Click += (object sender, EventArgs e) =>
        {
            string oldname = Path.GetFileNameWithoutExtension(configPath);
            if (configPath == "")
            {
                MessageBox.Show("请选择文件");
                return;
            }

            string filename = Interaction.InputBox("请输入文件名", "重命名", Path.GetFileNameWithoutExtension(configPath));
            if (filename == "")
            {
                MessageBox.Show("请输入文件名");
                return;
            }

            // 检测重复
            if (File.Exists("config/" + filename + ".json"))
            {
                MessageBox.Show("文件已存在");
                return;
            }

            File.Move(configPath, "config/" + filename + ".json");
            configPath = "config/" + filename + ".json";
            loadconfig();

            // 更新文件名
            foreach (ToolStripMenuItem item in fileMenuItem.DropDownItems)
            {
                if (item.Text == oldname)
                {
                    item.Text = filename;
                }
            }

            // 更新绑定对应的文件名
            string bindJson = System.IO.File.ReadAllText(bindPath);
            JsonObject bindJsonObject = (JsonObject)JsonObject.Parse(bindJson);
            foreach (var item in bindJsonObject)
            {
                string hong = item.Key;
                string needprocess = (string)item.Value;
                if (hong == oldname)
                {
                    bindJsonObject.Remove(hong);
                    bindJsonObject[filename] = needprocess;
                    break;
                }
            }
            System.IO.File.WriteAllText(bindPath, bindJsonObject.ToString());
        };
        menuStrip.Items.Add(renameMenuItem);

        // 绑定exe 当指定exe运行时，执行切换到指定文件
        var bindMenuItem = new ToolStripMenuItem("绑定");
        bindMenuItem.Click += (object sender, EventArgs e) =>
        {
            // 读取bind.json文件
            string bindJson = System.IO.File.ReadAllText(bindPath);
            JsonObject bindJsonObject = (JsonObject)JsonObject.Parse(bindJson);

            string oldname = "";
            foreach (var item in bindJsonObject)
            {
                string hong = item.Key;
                string needprocess = (string)item.Value;
                if (hong == Path.GetFileNameWithoutExtension(configPath))
                {
                    oldname = needprocess;
                    break;
                }
            }
            // 弹出下拉框选择exe
            string jincheng = Interaction.InputBox("请输入进程名，不带.exe后缀", "绑定进程名", oldname);

            if (jincheng == "")
            {
                return;
            }

            // 读取congig/bind.json文件 没有则创建

            if (!File.Exists(bindPath))
            {
                System.IO.File.WriteAllText(bindPath, "{}");
            }

            // 读取bind.json文件
            bindJsonObject[Path.GetFileNameWithoutExtension(configPath)] = jincheng;
            // 写入bind.json文件
            System.IO.File.WriteAllText(bindPath, bindJsonObject.ToString());
        };
        menuStrip.Items.Add(bindMenuItem);

        this.Text = "鼠标宏";
        this.Size = new Size(300, 160);
        this.Icon = new Icon("icon.ico");
        this.BackColor = Color.White;

        // 窗口最小尺寸不能小于300x160
        this.MinimumSize = new Size(300, 160);

        // 窗口居中
        this.StartPosition = FormStartPosition.CenterScreen;

        // 窗口关闭时触发事件 最小化
        this.FormClosing += OnExit;

        this.loadconfig();

        // Create a simple tray menu with only one item.
        trayMenu = new ContextMenuStrip();
        var closeMenuItem = new ToolStripMenuItem("退出");
        closeMenuItem.Click += OnExitmenu;
        trayMenu.Items.Add(closeMenuItem);

        // Create a tray icon. 
        trayIcon = new NotifyIcon();
        trayIcon.Text = "鼠标宏";
        trayIcon.Icon = new Icon("icon.ico");

        // Add menu to tray icon and show it.
        trayIcon.ContextMenuStrip = trayMenu;
        trayIcon.Visible = true; // Show tray icon at start.

        // Add double click event.
        trayIcon.DoubleClick += TrayIcon_MouseClick;
    }

    DateTime lastQueryTime = DateTime.Now.AddMinutes(-15);
    string weatherinfo = "";
    string weatherinfo2 = "";
    public byte[] GenerateImage()
    {
        // 创建一个新的 Bitmap 对象 16位 565
        Bitmap bitmap = new Bitmap(160, 128, PixelFormat.Format16bppRgb565);

        // 创建一个 Graphics 对象
        Graphics graphics = Graphics.FromImage(bitmap);

        // 设置用于绘制背景的颜色
        graphics.Clear(Color.Black);

        // 背景图片1.jpg
        // Image image = Image.FromFile("1.jpg");
        // graphics.DrawImage(image, 0, 0, 160, 128);

        // 设置用于绘制文字的字体和颜色
        Font font = new Font("Arial", 40);
        SolidBrush brush = new SolidBrush(Color.Gainsboro);

        // 获取当前的时间并转换为字符串
        string time = DateTime.Now.ToString("HH:mm");

        // 计算字符串的大小
        SizeF stringSize = graphics.MeasureString(time, font);

        // 计算居中的位置
        float x = (bitmap.Width - stringSize.Width) / 2;
        float y = (bitmap.Height - stringSize.Height) / 2 - 20;

        // 在计算出的位置上绘制字符串
        graphics.DrawString(time, font, brush, x, y);


        Font font2 = new Font("Arial", 8);
        SolidBrush brush2 = new SolidBrush(Color.Yellow);


        // 查询天气温度 西安
        DateTime currentTime = DateTime.Now;
        TimeSpan timeSpan = currentTime - lastQueryTime;

        if (timeSpan.TotalMinutes >= 15)
        {
            string url = "http://t.weather.itboy.net/api/weather/city/101110101";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            JsonObject jsonObject = (JsonObject)JsonObject.Parse(retString);
            string temperature = (string)jsonObject["data"]["wendu"];
            string weather = (string)jsonObject["data"]["forecast"][0]["type"];
            string high = (string)jsonObject["data"]["forecast"][0]["high"];
            string low = (string)jsonObject["data"]["forecast"][0]["low"];
            string fengli = (string)jsonObject["data"]["forecast"][0]["fengli"];
            string fengxiang = (string)jsonObject["data"]["forecast"][0]["fengxiang"];
            string city = (string)jsonObject["cityInfo"]["city"];
            weatherinfo = city + " " + temperature + "°C " + weather;
            weatherinfo2 = high + " " + low + " " + fengli + " " + fengxiang;

            // 更新上次查询时间
            lastQueryTime = currentTime;
        }

        // 计算字符串的大小
        SizeF stringSize1 = graphics.MeasureString(weatherinfo, font2);

        // 计算居中的位置
        float x1 = (bitmap.Width - stringSize1.Width) / 2;
        float y1 = (bitmap.Height - stringSize1.Height) / 2 + 20;

        // 在计算出的位置上绘制字符串
        graphics.DrawString(weatherinfo, font2, brush2, x1, y1);
        graphics.DrawString(weatherinfo2, font2, brush2, x1, y1 + 15);



        // 锁定 Bitmap 的像素数据
        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

        // 创建一个 byte[] 数组来存储像素数据
        byte[] bmpbits = new byte[bitmapData.Stride * bitmapData.Height];

        // 将 Bitmap 的像素数据复制到 byte[] 数组
        System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, bmpbits, 0, bmpbits.Length);

        // 解锁 Bitmap 的像素数据
        bitmap.UnlockBits(bitmapData);

        // 逆时针旋转90度
        byte[] newbmpbits = new byte[128 * 160 * 2];
        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 160; j++)
            {
                newbmpbits[(160 - j - 1) * 128 * 2 + i * 2] = bmpbits[i * 160 * 2 + j * 2];
                newbmpbits[(160 - j - 1) * 128 * 2 + i * 2 + 1] = bmpbits[i * 160 * 2 + j * 2 + 1];
            }
        }

        // 顺时针旋转90度
        // byte[] newbmpbits = new byte[128 * 160 * 2];
        // for (int i = 0; i < 128; i++)
        // {
        //     for (int j = 0; j < 160; j++)
        //     {
        //         newbmpbits[j * 128 * 2 + (128 - i - 1) * 2] = bmpbits[i * 160 * 2 + j * 2];
        //         newbmpbits[j * 128 * 2 + (128 - i - 1) * 2 + 1] = bmpbits[i * 160 * 2 + j * 2 + 1];
        //     }
        // }

        return newbmpbits;
    }

    // 定义RECT结构体
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner

        public int Width
        {
            get { return Right - Left; }
        }

        public int Height
        {
            get { return Bottom - Top; }
        }
    }

    // 确保你的类中包含了对GetWindowRect的P/Invoke声明
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
    public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    // 获取指定窗口的截图

    public byte[] CaptureWindow()
    {
        // 根据指定进程获取窗口句柄
        Process process = Process.GetProcessesByName("tdxw")[0];
        IntPtr hWnd = process.MainWindowHandle;
        // 获取窗口的大小和位置
        RECT rect;
        GetWindowRect(hWnd, out rect);

        // 创建一个与窗口大小相同的 Bitmap 对象
        Bitmap bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format16bppRgb565);

        // 创建一个 Graphics 对象
        Graphics graphics = Graphics.FromImage(bitmap);

        // 获取窗口的设备上下文
        IntPtr hdc = GetWindowDC(hWnd);

        // 将窗口的图像复制到 Graphics 对象中
        BitBlt(graphics.GetHdc(), 0, 0, rect.Width, rect.Height, hdc, 0, 0, 0x00CC0020);

        // 释放设备上下文
        ReleaseDC(hWnd, hdc);

        // 释放 Graphics 对象
        graphics.Dispose();


        // 缩放到 160x128 的byte数组
        Bitmap newbitmap = new Bitmap(160, 128, PixelFormat.Format16bppRgb565);

        // 创建一个 Graphics 对象
        Graphics newgraphics = Graphics.FromImage(newbitmap);

        // 将窗口的图像复制到 Graphics 对象中
        newgraphics.DrawImage(bitmap, 0, 0, 160, 128);

        // 创建一个 byte[] 数组来存储像素数据
        byte[] bmpbits = new byte[160 * 128 * 2];

        // 锁定 Bitmap 的像素数据
        BitmapData bitmapData = newbitmap.LockBits(new Rectangle(0, 0, newbitmap.Width, newbitmap.Height), ImageLockMode.ReadOnly, newbitmap.PixelFormat);

        // 将 Bitmap 的像素数据复制到 byte[] 数组
        System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, bmpbits, 0, bmpbits.Length);

        // 解锁 Bitmap 的像素数据
        newbitmap.UnlockBits(bitmapData);
        newbitmap.Save("1.jpg", ImageFormat.Jpeg);

        // 逆时针旋转90度
        byte[] newbmpbits = new byte[128 * 160 * 2];
        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 160; j++)
            {
                newbmpbits[(160 - j - 1) * 128 * 2 + i * 2] = bmpbits[i * 160 * 2 + j * 2];
                newbmpbits[(160 - j - 1) * 128 * 2 + i * 2 + 1] = bmpbits[i * 160 * 2 + j * 2 + 1];
            }
        }

        return newbmpbits;

    }


    int menuHeight = 0;
    private void addhong(object sender, EventArgs e)
    {
        // 创建一个TableLayoutPanel
        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top; // 设置Anchor属性
        tableLayoutPanel.ColumnCount = 4;
        // tableLayoutPanel.RowCount = 2;
        // tableLayoutPanel.BackColor = Color.Blue;
        tableLayoutPanel.Height = 90;
        tableLayoutPanel.Width = this.Width - 15;


        // 设置TableLayoutPanel的初始位置，考虑到菜单栏的高度
        tableLayoutPanel.Location = new Point(0, menuHeight + this.menuStrip.Height);

        // 设置列的宽度比例
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50)); // 触发键label的宽度
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // 只读输入框的宽度
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // 监听按钮的宽度
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50)); // 监听按钮的宽度

        // 在窗口第一行中创建 触发键label 只读输入框 和 监听按钮
        Label triggerKeyLabel = new Label();
        triggerKeyLabel.Text = "触发键";
        triggerKeyLabel.AutoSize = true;
        triggerKeyLabel.Anchor = AnchorStyles.None;
        triggerKeyLabel.TextAlign = ContentAlignment.MiddleCenter;


        TextBox triggerKeyTextBox = new TextBox();
        // 禁用
        triggerKeyTextBox.Enabled = false;
        triggerKeyTextBox.Dock = DockStyle.Fill;
        triggerKeyTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        // 如果sender是JsonObject，说明是从配置文件中加载的，需要设置触发键
        if (sender is JsonObject)
        {
            JsonObject jsonObject = (JsonObject)sender;
            triggerKeyTextBox.Text = (string)jsonObject["triggerKey"];
        }

        Button startButton = new Button();
        Button delButton = new Button();
        TextBox hongTextBox = new TextBox();
        Button listenButton = new Button();
        listenButton.Text = "监听";
        listenButton.Anchor = AnchorStyles.None;

        // 添加一个勾选框 按住执行
        CheckBox holdCheckBox = new CheckBox();
        // 点击后监听鼠标和键盘的输入写入triggerKeyTextBox
        listenButton.Click += (object sender, EventArgs e) =>
        {
            if (listenButton.Enabled)
            {
                listenButton.Enabled = false;
                hongTextBox.Enabled = false;
                delButton.Enabled = false;
                startButton.Enabled = false;
                tableLayoutPanel.Focus();

                SetListendo(() =>
                {
                    bool isdo = false;
                    if (netdll.kmNet_monitor_mouse_right() == 1)
                    {
                        triggerKeyTextBox.Text = GetKeyName(netdll.kmNet_monitor_mouse_right_code());
                        isdo = true;
                    }

                    if (netdll.kmNet_monitor_mouse_middle() == 1)
                    {
                        triggerKeyTextBox.Text = GetKeyName(netdll.kmNet_monitor_mouse_middle_code());
                        isdo = true;
                    }

                    if (netdll.kmNet_monitor_mouse_side1() == 1)
                    {
                        triggerKeyTextBox.Text = GetKeyName(netdll.kmNet_monitor_mouse_side1_code());
                        isdo = true;
                    }

                    if (netdll.kmNet_monitor_mouse_side2() == 1)
                    {
                        triggerKeyTextBox.Text = GetKeyName(netdll.kmNet_monitor_mouse_side2_code());
                        isdo = true;
                    }

                    if (netdll.kmNet_monitor_keyboard_code() != -1)
                    {
                        triggerKeyTextBox.Text = GetKeyName((KeyboardButton)netdll.kmNet_monitor_keyboard_code());
                        isdo = true;
                    }

                    if (isdo)
                    {
                        ClearListendo();
                        listenButton.Enabled = true;
                        hongTextBox.Enabled = true;
                        delButton.Enabled = true;
                        startButton.Enabled = true;
                        // 移除焦点
                        tableLayoutPanel.Focus();
                        saveconfig();

                        starthong(holdCheckBox.Checked);
                    }
                });
            }
        };

        // 添加到TableLayoutPanel
        tableLayoutPanel.Controls.Add(triggerKeyLabel, 0, 0);
        tableLayoutPanel.Controls.Add(triggerKeyTextBox, 1, 0);
        tableLayoutPanel.SetColumnSpan(triggerKeyTextBox, 2);
        tableLayoutPanel.Controls.Add(listenButton, 3, 0);

        Label hongLabel = new Label();
        hongLabel.Text = "宏";
        hongLabel.AutoSize = true;
        hongLabel.Anchor = AnchorStyles.None;
        hongLabel.TextAlign = ContentAlignment.MiddleCenter;


        hongTextBox.Dock = DockStyle.Fill;
        hongTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        // 如果sender是JsonObject，说明是从配置文件中加载的，需要设置宏
        if (sender is JsonObject)
        {
            JsonObject jsonObject = (JsonObject)sender;
            hongTextBox.Text = (string)jsonObject["hong"];
        }


        delButton.Text = "删除";
        delButton.Anchor = AnchorStyles.None;

        // 实现删除功能
        delButton.Click += (object sender, EventArgs e) =>
        {
            tableLayoutPanel.Dispose();
            // 重新排列TableLayoutPanel位置
            int y = 0;
            int TableLayoutPanelnum = 0;
            foreach (Control control in this.Controls)
            {
                if (control is TableLayoutPanel)
                {
                    control.Location = new Point(0, y + this.menuStrip.Height);
                    control.Focus();
                    y += control.Height;
                    TableLayoutPanelnum++;
                }
            }
            // 重新设置窗口大小
            this.Size = new Size(this.Width, 160 + (TableLayoutPanelnum - 1) * 90);
            menuHeight = y;

            saveconfig();
        };

        // 添加到TableLayoutPanel
        tableLayoutPanel.Controls.Add(hongLabel, 0, 1);
        tableLayoutPanel.Controls.Add(hongTextBox, 1, 1);
        tableLayoutPanel.SetColumnSpan(hongTextBox, 2);
        tableLayoutPanel.Controls.Add(delButton, 3, 1);



        Label statustitle = new Label();
        statustitle.Text = "状态";
        statustitle.AutoSize = true;
        statustitle.Anchor = AnchorStyles.None;
        statustitle.TextAlign = ContentAlignment.MiddleCenter;

        Label statusLabel = new Label();
        statusLabel.Text = "未运行";
        statusLabel.AutoSize = true;
        statusLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        // statusLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 初始绿色
        statusLabel.ForeColor = Color.Green;



        holdCheckBox.Text = "松开即停";
        holdCheckBox.AutoSize = true;
        holdCheckBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        holdCheckBox.TextAlign = ContentAlignment.MiddleCenter;

        if (sender is JsonObject)
        {
            JsonObject jsonObject = (JsonObject)sender;
            holdCheckBox.Checked = (bool)jsonObject["hold"];
        }

        startButton.Text = "启动";
        startButton.Anchor = AnchorStyles.None;

        // 当监听有值时，启动按钮可用
        if (triggerKeyTextBox.Text != "")
        {
            startButton.Enabled = true;
        }
        else
        {
            startButton.Enabled = false;
        }

        // 实现启动功能
        startButton.Click += async (object sender, EventArgs e) =>
        {
            if (statusLabel.Text == "未运行")
            {
                statusLabel.Text = "运行中";
                startButton.Text = "停止";
                delButton.Enabled = false;
                hongTextBox.Enabled = false;
                listenButton.Enabled = false;
                holdCheckBox.Enabled = false;
                statusLabel.ForeColor = Color.Red;
                // startButton.hook = starthong(triggerKeyTextBox.Text, hongTextBox.Text);
                saveconfig();
                starthong(holdCheckBox.Checked);
            }
            else
            {
                statusLabel.Text = "未运行";
                startButton.Text = "启动";
                delButton.Enabled = true;
                hongTextBox.Enabled = true;
                listenButton.Enabled = true;
                holdCheckBox.Enabled = true;
                statusLabel.ForeColor = Color.Green;
                // startButton.hook.Dispose();
                // startButton.Listener.Stop();
                updateThreadDict();
            }
        };


        // 添加到TableLayoutPanel
        tableLayoutPanel.Controls.Add(statustitle, 0, 2);
        tableLayoutPanel.Controls.Add(statusLabel, 1, 2);
        tableLayoutPanel.Controls.Add(holdCheckBox, 2, 2);
        tableLayoutPanel.Controls.Add(startButton, 3, 2);

        // 添加TableLayoutPanel到窗口
        this.Controls.Add(tableLayoutPanel);

        this.Size = new Size(this.Width, 160 + menuHeight);
        menuHeight += tableLayoutPanel.Height;


        if (sender is JsonObject)
        {
            JsonObject jsonObject = (JsonObject)sender;
            statusLabel.Text = (string)jsonObject["status"];
            if (statusLabel.Text == "运行中")
            {
                statusLabel.Text = "运行中";
                startButton.Text = "停止";
                delButton.Enabled = false;
                hongTextBox.Enabled = false;
                listenButton.Enabled = false;
                holdCheckBox.Enabled = false;
                statusLabel.ForeColor = Color.Red;
                starthong(holdCheckBox.Checked);
            }
            if (statusLabel.Text == "未运行")
            {
                statusLabel.Text = "未运行";
                startButton.Text = "启动";
                delButton.Enabled = true;
                hongTextBox.Enabled = true;
                listenButton.Enabled = true;
                holdCheckBox.Enabled = true;
                statusLabel.ForeColor = Color.Green;
                updateThreadDict();
            }
        }
        if (!(sender is JsonObject))
        {
            saveconfig();
        }

    }

    Dictionary<string, Thread> threadDict = new Dictionary<string, Thread>();

    Dictionary<string, int> repeatDict = new Dictionary<string, int>();

    private void updateThreadDict()
    {
        threadDict = new Dictionary<string, Thread>();

        foreach (Control control in this.Controls)
        {

            if (control is TableLayoutPanel)
            {
                TableLayoutPanel tableLayoutPanel = (TableLayoutPanel)control;
                string triggerKey = tableLayoutPanel.Controls[1].Text;
                string hong = tableLayoutPanel.Controls[4].Text;
                string status = tableLayoutPanel.Controls[7].Text;
                bool hold = ((CheckBox)tableLayoutPanel.Controls[8]).Checked;

                if (status == "未运行")
                {
                    // 禁用监听鼠标
                    if (triggerKey == "右键")
                    {
                        netdll.kmNet_mask_mouse_right(0);
                    }
                    else
                    if (triggerKey == "左键")
                    {
                        netdll.kmNet_mask_mouse_left(0);
                    }
                    else
                    if (triggerKey == "中键")
                    {
                        netdll.kmNet_mask_mouse_middle(0);
                    }
                    else
                    if (triggerKey == "侧键1")
                    {
                        netdll.kmNet_mask_mouse_side1(0);
                    }
                    else
                    if (triggerKey == "侧键2")
                    {
                        netdll.kmNet_mask_mouse_side2(0);
                    }
                    else if (triggerKey != "")
                    {
                        netdll.kmNet_unmask_keyboard((short)GetKeyValue(triggerKey));
                    }

                    if (threadDict.ContainsKey(triggerKey))
                    {
                        threadDict[triggerKey].Interrupt();
                        threadDict.Remove(triggerKey);
                    }

                    if (repeatDict.ContainsKey(triggerKey))
                    {
                        repeatDict.Remove(triggerKey);
                    }
                }

                if (status == "运行中")
                {

                    // 禁用监听鼠标
                    if (triggerKey == "右键")
                    {
                        netdll.kmNet_mask_mouse_right(1);
                    }
                    else
                    if (triggerKey == "左键")
                    {
                        netdll.kmNet_mask_mouse_left(1);
                    }
                    else
                    if (triggerKey == "中键")
                    {
                        netdll.kmNet_mask_mouse_middle(1);
                    }
                    else
                    if (triggerKey == "侧键1")
                    {
                        netdll.kmNet_mask_mouse_side1(1);
                    }
                    else
                    if (triggerKey == "侧键2")
                    {
                        netdll.kmNet_mask_mouse_side2(1);
                    }
                    else
                    {
                        netdll.kmNet_mask_keyboard((short)GetKeyValue(triggerKey));
                    }

                    threadDict[triggerKey] = dohong(hong, hold);
                    if (hong.StartsWith("!"))
                    {
                        repeatDict[triggerKey] = 0;
                    }
                }
            }
        }
    }

    private void starthong(bool hold)
    {
        updateThreadDict();
        SetListendo(() =>
        {

            // 循环字典
            foreach (var item in threadDict)
            {
                if (
                   (netdll.kmNet_monitor_mouse_right_code() == GetKey(item.Key) && netdll.kmNet_monitor_mouse_right() == 1)
                || (netdll.kmNet_monitor_mouse_middle_code() == GetKey(item.Key) && netdll.kmNet_monitor_mouse_middle() == 1)
                || (netdll.kmNet_monitor_mouse_side1_code() == GetKey(item.Key) && netdll.kmNet_monitor_mouse_side1() == 1)
                || (netdll.kmNet_monitor_mouse_side2_code() == GetKey(item.Key) && netdll.kmNet_monitor_mouse_side2() == 1)
                )
                {
                    if (!threadDict[item.Key].IsAlive)
                    {
                        if (!hold)
                        {
                            netdll.kmNet_mouse_all(0, 0, 0, 0);
                            updateThreadDict();
                        }
                        threadDict[item.Key].Start();
                    }

                    if (repeatDict.ContainsKey(item.Key) && repeatDict[item.Key] == 1)
                    {
                        repeatDict[item.Key] = 2;
                    }
                }

                // 如果鼠标弹起，终止线程 根据threadDict IsAlive判断
                if (threadDict[item.Key].IsAlive)
                {
                    if (
                        (item.Key == "右键" && netdll.kmNet_monitor_mouse_right() == 0)
                    || (item.Key == "中键" && netdll.kmNet_monitor_mouse_middle() == 0)
                    || (item.Key == "侧键1" && netdll.kmNet_monitor_mouse_side1() == 0)
                    || (item.Key == "侧键2" && netdll.kmNet_monitor_mouse_side2() == 0)
                    )
                    {
                        if (hold)
                        {
                            netdll.kmNet_mouse_all(0, 0, 0, 0);
                            threadDict[item.Key].Interrupt();
                            updateThreadDict();
                        }

                        nohongrun = true;

                        if (repeatDict.ContainsKey(item.Key) && repeatDict[item.Key] == 0)
                        {
                            repeatDict[item.Key] = 1;
                        }

                        if (repeatDict.ContainsKey(item.Key) && repeatDict[item.Key] == 2)
                        {
                            repeatDict[item.Key] = 3;
                        }
                    }
                }

                if (GetKeyName((KeyboardButton)netdll.kmNet_monitor_keyboard_code()) == item.Key)
                {
                    if (!threadDict[item.Key].IsAlive)
                    {
                        if (!hold)
                        {
                            updateThreadDict();
                        }
                        threadDict[item.Key].Start();
                    }

                    if (repeatDict.ContainsKey(item.Key) && repeatDict[item.Key] == 1)
                    {
                        repeatDict[item.Key] = 2;
                    }
                }

                // 键盘弹起
                if (threadDict[item.Key].IsAlive)
                {

                    if (netdll.kmNet_monitor_keyboard(GetKey(item.Key)) == 0)
                    {
                        if (hold)
                        {
                            threadDict[item.Key].Interrupt();
                            updateThreadDict();
                        }

                        nohongrun = true;

                        if (repeatDict.ContainsKey(item.Key) && repeatDict[item.Key] == 0)
                        {
                            repeatDict[item.Key] = 1;
                        }

                        if (repeatDict.ContainsKey(item.Key) && repeatDict[item.Key] == 2)
                        {
                            repeatDict[item.Key] = 3;
                        }
                    }
                }
            }

            if (netdll.kmNet_monitor_mouse_left() == 1 && isdown.ContainsKey("leftisdown") && isdown["leftisdown"])
            {
                netdll.kmNet_mouse_left(0);
                isdown["leftisdown"] = false;
            }

            // repeatDict
            foreach (var item in repeatDict)
            {
                if (item.Value == 3)
                {
                    repeatDict[item.Key] = 0;
                    netdll.kmNet_mouse_all(0, 0, 0, 0);
                    threadDict[item.Key].Interrupt();
                }
            }
        });
    }


    public static string GetKeyName(KeyboardButton key)
    {
        return Enum.GetName(typeof(KeyboardButton), key);
    }

    public static byte GetKeyValue(string key)
    {
        if (Enum.TryParse(key, out KeyboardButton button))
        {
            return (byte)button;
        }
        else
        {
            throw new ArgumentException($"Invalid key: {key}");
        }
    }

    public static int GetKey(string key)
    {
        if (key == "左键")
        {
            return 1;
        }

        if (key == "右键")
        {
            return 2;
        }

        if (key == "中键")
        {
            return 4;
        }

        if (key == "侧键1")
        {
            return 8;
        }

        if (key == "侧键2")
        {
            return 16;
        }

        return (int)GetKeyValue(key);
    }

    public static string GetKeyName(int key)
    {
        if (key == 1)
        {
            return "左键";
        }

        if (key == 2)
        {
            return "右键";
        }

        if (key == 4)
        {
            return "中键";
        }

        if (key == 8)
        {
            return "侧键1";
        }

        if (key == 16)
        {
            return "侧键2";
        }

        return key + "";
    }


    Dictionary<string, bool> isdown = new Dictionary<string, bool>();
    private Thread dohong(string hongtxt, bool hold)
    {
        // 正则提取[]和{}中的内容
        string[] hongArray = System.Text.RegularExpressions.Regex.Split(hongtxt, @"(\[.*?\])|(\{.*?\})");
        bool isRepeat = false;
        // 如果是!开头的宏，循环执行
        if (hongArray[0].StartsWith("!"))
        {
            isRepeat = true;
        }
        Thread thread = new Thread(() =>
        {
            try
            {
                while (true)
                {
                    nohongrun = false;
                    foreach (string hong in hongArray)
                    {
                        if (hong.StartsWith("[-xy"))
                        {
                            // -xy100*100 移动到指定位置
                            // -xy后*前是x坐标
                            int start = hong.IndexOf("y") + 1;
                            int end = hong.IndexOf("*");
                            string xStr = hong.Substring(start, end - start);
                            int x = int.Parse(xStr);

                            start = hong.IndexOf("*") + 1;
                            end = hong.Length - 1;
                            string yStr = hong.Substring(start, end - start);
                            int y = int.Parse(yStr);

                            // 获取当前鼠标位置
                            System.Drawing.Point currentMousePosition = System.Windows.Forms.Cursor.Position;

                            // 计算相对移动距离
                            int deltaX = x - currentMousePosition.X;
                            int deltaY = y - currentMousePosition.Y;

                            // 移动鼠标
                            netdll.kmNet_enc_mouse_move_auto(deltaX, deltaY);
                        }
                        else
                         if (hong.StartsWith("[-x"))
                        {
                            // -x100 向左移动100
                            int start = hong.IndexOf("x") + 1;
                            int end = hong.IndexOf("]");
                            string xStr = hong.Substring(start, end - start);
                            int x = int.Parse(xStr);
                            netdll.kmNet_enc_mouse_move_auto(-x, 0);
                        }
                        else if (hong.StartsWith("[+x"))
                        {
                            // +x100 向右移动100
                            int start = hong.IndexOf("x") + 1;
                            int end = hong.IndexOf("]");
                            string xStr = hong.Substring(start, end - start);
                            int x = int.Parse(xStr);
                            netdll.kmNet_enc_mouse_move_auto(x, 0);
                        }
                        else if (hong.StartsWith("[-y"))
                        {
                            // -y100 向上移动100
                            int start = hong.IndexOf("y") + 1;
                            int end = hong.IndexOf("]");
                            string yStr = hong.Substring(start, end - start);
                            int y = int.Parse(yStr);
                            netdll.kmNet_enc_mouse_move_auto(0, -y);
                        }
                        else if (hong.StartsWith("[+y"))
                        {
                            // +y100 向下移动100
                            int start = hong.IndexOf("y") + 1;
                            int end = hong.IndexOf("]");
                            string yStr = hong.Substring(start, end - start);
                            int y = int.Parse(yStr);
                            // 显示每次执行完的时间
                            netdll.kmNet_enc_mouse_move_auto(0, y);
                        }
                        else
                        if (hong.StartsWith("[-"))
                        {
                            // [-l][-l1][-l0] 左键单击 左键按下 左键弹起 [-r][-r1][-r0] 右键单击 右键按下 右键弹起 [-m][-m1][-m0] 中键单击 中键按下 中键弹起 [-1][-11][-10] 左侧键1单击 左侧键1按下 左侧键1弹起 [-2][-21][-20] 左侧键2单击 左侧键2按下 左侧键2弹起  [-w1] 滚轮上滚 [-w0] 滚轮下滚
                            switch (hong)
                            {
                                case "[-l]":
                                    netdll.kmNet_mouse_left(1);
                                    Thread.Sleep(20);
                                    netdll.kmNet_mouse_left(0);
                                    break;
                                case "[-l1]":
                                    netdll.kmNet_mouse_left(1);
                                    isdown["leftisdown"] = true;
                                    break;
                                case "[-l0]":
                                    netdll.kmNet_mouse_left(0);
                                    isdown["leftisdown"] = false;
                                    break;
                                case "[-r]":
                                    netdll.kmNet_mouse_right(1);
                                    Thread.Sleep(20);
                                    netdll.kmNet_mouse_right(0);
                                    break;
                                case "[-r1]":
                                    netdll.kmNet_mouse_right(1);
                                    break;
                                case "[-r0]":
                                    netdll.kmNet_mouse_right(0);
                                    break;
                                case "[-m]":
                                    netdll.kmNet_mouse_middle(1);
                                    netdll.kmNet_mouse_middle(0);
                                    break;
                                case "[-m1]":
                                    netdll.kmNet_mouse_middle(1);
                                    break;
                                case "[-m0]":
                                    netdll.kmNet_mouse_middle(0);
                                    break;
                                case "[-1]":
                                    netdll.kmNet_mouse_side1(1);
                                    Thread.Sleep(20);
                                    netdll.kmNet_mouse_side1(0);
                                    break;
                                case "[-11]":
                                    netdll.kmNet_mouse_side1(1);
                                    break;
                                case "[-10]":
                                    netdll.kmNet_mouse_side1(0);
                                    break;
                                case "[-2]":
                                    netdll.kmNet_mouse_side2(1);
                                    Thread.Sleep(20);
                                    netdll.kmNet_mouse_side2(0);
                                    break;
                                case "[-21]":
                                    netdll.kmNet_mouse_side2(1);
                                    break;
                                case "[-20]":
                                    netdll.kmNet_mouse_side2(0);
                                    break;
                                case "[-w1]":
                                    netdll.kmNet_enc_mouse_wheel(1);
                                    break;
                                case "[-w0]":
                                    netdll.kmNet_enc_mouse_wheel(-1);
                                    break;
                            }
                        }
                        else if (hong.StartsWith("{"))
                        {
                            int delay = int.Parse(hong.Substring(1, hong.Length - 2));
                            Thread.Sleep(delay);
                        }
                        else
                        if (hong.StartsWith("["))
                        {
                            string Key = hong.Substring(1, hong.Length - 2);

                            if (KeyboardButtonMap.CharToKeyboardButton.TryGetValue(Key, out var result))
                            {
                                var (button, modifiers) = result;
                                if (modifiers == KeyboardModifiers.LeftShift)
                                {
                                    netdll.kmNet_keydown((int)KeyboardButton.KeyLeftshift);
                                    netdll.kmNet_keypress((int)button);
                                    netdll.kmNet_keyup((int)KeyboardButton.KeyLeftshift);
                                }
                                else
                                {
                                    // log(button);
                                    netdll.kmNet_keypress((int)button);
                                }
                            }
                            else
                            // 转换为小写
                            if (KeyboardButtonMap.CharToKeyboardButton.TryGetValue(Key.ToLower(), out var result2))
                            {
                                var (button, modifiers) = result2;
                                // log(button);
                                netdll.kmNet_keypress((int)button);
                            }
                            else
                            {
                                // 如果没有找到键值，直接发送字符串
                                foreach (char c in Key)
                                {
                                    if (KeyboardButtonMap.CharToKeyboardButton.TryGetValue("" + c, out var result3))
                                    {
                                        var (button, modifiers) = result3;
                                        // log(button);
                                        netdll.kmNet_keypress((int)button);
                                    }
                                }
                            }

                        }
                    }

                    Sleep(1);

                    if (isRepeat)
                    {
                        continue;
                    }

                    if (!hold)
                    {
                        break;
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                // Console.WriteLine("stop");
            }
        });

        return thread;
    }

    // 提高精度
    static void Sleep(int ms)
    {
        var sw = Stopwatch.StartNew();
        var sleepMs = ms - 16;
        if (sleepMs > 0)
        {
            Thread.Sleep(sleepMs);
        }
        while (sw.ElapsedMilliseconds < ms)
        {
            Thread.Sleep(0);
        }
    }

    private void TrayIcon_MouseClick(object? sender, EventArgs e)
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        // 刷新窗口 重新载入配置
        this.loadconfig();
    }

    protected override void OnResize(EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            this.Hide();
        }

        base.OnResize(e);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        trayIcon.Visible = false; // Hide tray icon before exit.
        base.OnFormClosing(e);
    }

    private bool isExiting = false;
    private void OnExit(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            // 取消关闭窗口
            e.Cancel = true;
            // 隐藏窗口
            this.Hide();
            trayIcon.Visible = true; 
        }
    }

    private void OnExitmenu(object sender, EventArgs e)
    {
        if (isExiting) return;

        isExiting = true;

        netdll.kmNet_unmask_all();

        trayIcon.Visible = false; // Hide tray icon before exit.

        saveconfig();
        Application.Exit();
    }

    private void saveconfig()
    {
        // 退出前保存当前配置 将每组宏的配置保存到json文件
        JsonArray jsonArray = new JsonArray();
        foreach (Control control in this.Controls)
        {
            if (control is TableLayoutPanel)
            {
                // 保存配置
                TableLayoutPanel tableLayoutPanel = (TableLayoutPanel)control;
                string triggerKey = tableLayoutPanel.Controls[1].Text;
                string hong = tableLayoutPanel.Controls[4].Text;
                string status = tableLayoutPanel.Controls[7].Text;
                bool hold = ((CheckBox)tableLayoutPanel.Controls[8]).Checked;
                JsonObject jsonObject = new JsonObject();
                jsonObject["triggerKey"] = triggerKey;
                jsonObject["hong"] = hong;
                jsonObject["status"] = status;
                jsonObject["hold"] = hold;
                jsonArray.Add(jsonObject);
            }
        }
        string jsonString = jsonArray.ToString();
        System.IO.File.WriteAllText(configPath, jsonString);
    }

    private void Clear()
    {
        List<Control> controlsToRemove = new List<Control>();

        foreach (Control control in this.Controls)
        {
            if (control is TableLayoutPanel)
            {
                controlsToRemove.Add(control);
            }
        }

        foreach (Control control in controlsToRemove)
        {
            this.Controls.Remove(control);
            control.Dispose();
        }
    }

    private void loadconfig()
    {
        // 解除鼠标屏蔽
        netdll.kmNet_unmask_all();

        menuHeight = 0;
        // 清空列表
        Clear();
        // 停止所有线程
        foreach (var item in threadDict)
        {
            threadDict[item.Key].Interrupt();

            // 解除键盘屏蔽
            if (item.Key != "右键" && item.Key != "左键" && item.Key != "中键" && item.Key != "侧键1" && item.Key != "侧键2")
            {
                netdll.kmNet_unmask_keyboard((short)GetKeyValue(item.Key));
            }
        }
        threadDict = new Dictionary<string, Thread>();

        if (System.IO.File.Exists(configPath))
        {
            string jsonString = System.IO.File.ReadAllText(configPath);
            JsonArray jsonArray = (JsonArray)JsonArray.Parse(jsonString);
            foreach (JsonObject jsonObject in jsonArray)
            {
                addhong(jsonObject, null);
            }
        }
    }

    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    private static IntPtr _winEventHook;
    private static WinEventDelegate _delegate;

    private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        // Check if the foreground window has changed
        string newpath = cheackProcess();
        if (newpath != "")
        {
            configPath = newpath;
            loadconfig();

            foreach (ToolStripMenuItem item in fileMenuItem.DropDownItems)
            {
                if (item.Text == Path.GetFileNameWithoutExtension(configPath))
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
            }
        }
    }

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]

    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    private string cheackProcess()
    {
        // 去读config/bind.json
        string bindPath = "config/bind.json";
        if (File.Exists(bindPath))
        {
            // 获取当前前台窗口的进程名
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process process = Process.GetProcessById((int)pid);
            string processName = process.ProcessName;
            string bindJsonString = File.ReadAllText(bindPath);
            JsonObject bindJsonObject = (JsonObject)JsonObject.Parse(bindJsonString);

            foreach (var item in bindJsonObject)
            {
                string hong = item.Key;
                string needprocess = (string)item.Value;
                if (needprocess == processName)
                {
                    return "config/" + hong + ".json";
                    break;
                }
            }
        }

        return "";
    }

    static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

    [STAThread]
    static void Main()
    {
        if (mutex.WaitOne(TimeSpan.Zero, true))
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MouseMacroForm());
            mutex.ReleaseMutex();
        }
        else
        {
            // 发送消息给已经运行的应用程序实例，然后退出
            MessageBox.Show("程序已经在运行了");
        }
    }
}