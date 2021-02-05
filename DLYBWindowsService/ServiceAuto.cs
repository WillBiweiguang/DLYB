using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DLYBWindowsService
{
    public partial class ServiceAuto : ServiceBase
    {
        public ServiceAuto()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer(90000);
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(DoEvent);
            timer.Start();
        }

        protected override void OnStop()
        {
        }

        public void DoEvent(object sender, ElapsedEventArgs e)
        {
            AutomationService.Dotask();
        }

    }
}
