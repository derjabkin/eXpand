﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Loggers;
using DevExpress.ExpressApp.EasyTest.WebAdapter;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
using Fasterflect;
using Xpand.EasyTest;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WebAdapter;
using HideScrollBarCommand = Xpand.ExpressApp.EasyTest.WebAdapter.Commands.HideScrollBarCommand;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

[assembly: Adapter(typeof (XpandTestWebAdapter))]

namespace Xpand.ExpressApp.EasyTest.WebAdapter{
    public class XpandTestWebAdapter : DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter, IXpandTestAdapter{
        private Process _process;

        public override void RunApplication(TestApplication testApplication){
            testApplication.CreateParametersFile();
            testApplication.DeleteUserModel();
            testApplication.CopyModel();
            ConfigApplicationModel(testApplication);
            RunApplicationCore(testApplication);
        }

        private void ConfigApplicationModel(TestApplication testApplication){
            var physicalPath = testApplication.ParameterValue<string>(ApplicationParams.PhysicalPath);
            var useModel = testApplication.ParameterValue<bool>(ApplicationParams.UseModel);
            if (useModel){
                var logPath = Logger.Instance.GetLogger<FileLogger>().LogPath;
                var model = Directory.GetFiles(logPath, "*.xafml").Single();
                File.Copy(Path.Combine(physicalPath, "Model.xafml"), Path.Combine(physicalPath, "restore.xafml"), true);
                File.Copy(model, Path.Combine(physicalPath, "Model.xafml"), true);
            }
            else{
                if (File.Exists(Path.Combine(physicalPath, "restore.xafml"))){
                    File.Copy(Path.Combine(physicalPath, "restore.xafml"), Path.Combine(physicalPath, "Model.xafml"), true);
                    File.Delete(Path.Combine(physicalPath, "restore.xafml"));
                }
            }
        }

        private void RunApplicationCore(TestApplication testApplication){
            var defaultWindowSize = testApplication.ParameterValue<string>(ApplicationParams.DefaultWindowSize);
            if (defaultWindowSize != null){
                WebBrowserCollection.DefaultFormSize = GetWindowSize(defaultWindowSize);
            }
            if (!testApplication.ParameterValue<bool>(ApplicationParams.UseIISExpress)){
                RunApplicationBase(testApplication);
            }
            else{
                var url = testApplication.ParameterValue<string>(ApplicationParams.Url);
                var uri = new Uri(url);
                webBrowsers = CreateWebBrowsers(testApplication);
                if (!WebDevWebServerHelper.IsWebDevServerStarted(uri)){
                    _process = IISExpressServerHelper.Run(testApplication, uri);
                }
                this.CreateBrowser(url);
            }
        }

        public override ICommandAdapter CreateCommandAdapter(){
            FileDownloadDialogHelper.SaveDialogOpened = false;
            var webCommandAdapter = new XpandWebCommandAdapter(this);
            webCommandAdapter.WaitForBrowserResponse(false);
            Win32Helper.MoveMousePointTo(new Point(0, 0));
            return webCommandAdapter;
        }

        private void RunApplicationBase(TestApplication testApplication){
            var url = testApplication.ParameterValue<string>(ApplicationParams.Url);
            var uri = new Uri(url);
            
            webBrowsers=CreateWebBrowsers(testApplication);
            var physicalPath = testApplication.ParameterValue<string>(ApplicationParams.PhysicalPath);
            if (string.IsNullOrEmpty(physicalPath) && !testApplication.ParameterValue<bool>(ApplicationParams.DontRestartIIS)) {
                RestartIIS();
            }
            else {
                if (!testApplication.ParameterValue<bool>(ApplicationParams.DontRunWebDev) && !string.IsNullOrEmpty(physicalPath)) {
                    typeof(DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter).CallMethod("LoadFileInfo",Path.GetFullPath(physicalPath));
                    if (testApplication.ParameterValue<string>(ApplicationParams.SingleWebDev) == null) {
                        if (WebDevWebServerHelper.IsWebDevServerStarted(uri)) {
                            WebDevWebServerHelper.KillWebDevWebServer();
                        }
                        WebDevWebServerHelper.RunWebDevWebServer(Path.GetFullPath(physicalPath), uri.Port.ToString(CultureInfo.InvariantCulture));
                    }
                    else {
                        if (!WebDevWebServerHelper.IsWebDevServerStarted(uri)) {
                            WebDevWebServerHelper.RunWebDevWebServer(Path.GetFullPath(physicalPath), uri.Port.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
            var defaultWindowSize = testApplication.ParameterValue<string>(ApplicationParams.DefaultWindowSize);
            if (defaultWindowSize != null) {
                WebBrowserCollection.DefaultFormSize = GetWindowSize(defaultWindowSize);
            }
            var waitDebuggerAttached = testApplication.ParameterValue<bool>(ApplicationParams.WaitDebuggerAttached);
            if (waitDebuggerAttached) {
                Thread.Sleep(8000);
                if (Debugger.IsAttached) {
                    MessageBox.Show("Start web application?", "Warning", MessageBoxButtons.OK);
                }
            }
            DateTime current = DateTime.Now;
            while (!WebDevWebServerHelper.IsWebDevServerStarted(uri) && DateTime.Now.Subtract(current).TotalSeconds < 60) {
                Thread.Sleep(200);
            }
            this.CreateBrowser(url);
        }

        private IWebBrowserCollection CreateWebBrowsers(TestApplication testApplication){
            var webBrowserType = testApplication.ParameterValue<string>(ApplicationParams.WebBrowserType);
            if (string.IsNullOrEmpty(webBrowserType)) {
                webBrowserType = "Default";
            }
            return webBrowserType == "Default"
                ? (IWebBrowserCollection) new WebBrowserCollection()
                : new StandaloneWebBrowserCollection();
        }

        public override void KillApplication(TestApplication testApplication, KillApplicationConext context){
            testApplication.ClearModel();
            testApplication.DeleteParametersFile();
            ScreenCaptureCommand.Stop();
            webBrowsers.KillAllWebBrowsers();
            var isSingleWebDev = testApplication.ParameterValue<bool>(ApplicationParams.SingleWebDev);
            if (!testApplication.ParameterValue<bool>(ApplicationParams.DontKillWebDev)&&_process!=null) {
                if (isSingleWebDev) {
                    if (context != KillApplicationConext.TestNormalEnded) {
                        IISExpressServerHelper.Stop(_process);
                    }
                }
                else {
                    IISExpressServerHelper.Stop(_process);
                }
            }
        }

        public override void RegisterCommands(IRegisterCommand registrator){
            base.RegisterCommands(registrator);
            registrator.RegisterCommands(this);
            registrator.RegisterCommand(Xpand.EasyTest.Commands.HideScrollBarCommand.Name, typeof (HideScrollBarCommand));
        }
    }

    public class StandaloneWebBrowserCollection : DevExpress.ExpressApp.EasyTest.WebAdapter.StandaloneWebBrowserCollection,IWebBrowserCollection {
        public const string EasyTestBrowser = "EasyTest Browser";
        IEasyTestWebBrowser IWebBrowserCollection.CreateWebBrowser() {
            var webBrowser = CreateWebBrowser();
            var standaloneWebBrowser = ((XAFStandaloneWebBrowser)webBrowser);
            var browserControl = ((StandaloneWebBrowserControl) standaloneWebBrowser.WebBrowser);
            browserControl.ScrollBarsEnabled = false;
            var form = browserControl.Parent;
            form.Invoke(new MethodInvoker(delegate{
                form.Location = new Point(0, 0);
                form.Text = EasyTestBrowser;
                if (WebBrowserCollection.DefaultFormSize!=new Size()){
                    form.Width = WebBrowserCollection.DefaultFormSize.Width;
                    form.Height = WebBrowserCollection.DefaultFormSize.Height;
                }
            }));
            return webBrowser;
        }
    }

    public class XpandWebCommandAdapter : WebCommandAdapter {
        public XpandWebCommandAdapter(DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter adapter) : base(adapter){
        }
    }

    public class IISExpressServerHelper{
        public static void Stop(Process process) {
            process.Kill();
        }

        public static Process Run(TestApplication testApplication, Uri uri) {
            string physicalPath = Path.GetFullPath(testApplication.ParameterValue<string>(ApplicationParams.PhysicalPath));
            string arguments = String.Format(@"/path:""{0}"" /port:{1}", physicalPath, uri.Port);
            EasyTestTracer.Tracer.InProcedure(String.Format("RunIISExpressServer({0})", arguments));
            try{
                string serverPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"IIS Express\iisexpress.exe");
                EasyTestTracer.Tracer.InProcedure(String.Format("IISExpressServerPath= {0}", serverPath));
                var serverProcess = new Process{
                    StartInfo ={
                        FileName = serverPath,
                        Arguments = arguments,
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                serverProcess.Start();
                return serverProcess;
            }
            finally{
                EasyTestTracer.Tracer.OutProcedure(String.Format("RunWebDevWebServer({0})", arguments));
            }
        }
    }
}
