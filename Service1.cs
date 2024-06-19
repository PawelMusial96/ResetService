using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace ResetService
{
    public partial class ResetService : ServiceBase
    {
        private Timer timer;
        private string closeTime;
        private string openTime;
        private EventLog eventLog;
        private string logFilePath;

        public ResetService()
        {
            InitializeComponent();
            eventLog = new EventLog();

            if (!EventLog.SourceExists("ResetServiceSource"))
            {
                EventLog.CreateEventSource("ResetServiceSource", "ResetServiceLog");
            }

            eventLog.Source = "ResetServiceSource";
            eventLog.Log = "ResetServiceLog";

            logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResetServiceLog.txt");
        }

        private void InitializeComponent()
        {
            this.ServiceName = "ResetService";
        }

        protected override void OnStart(string[] args)
        {
            Log("Service is starting.");
            closeTime = "19:05"; // Example close time
            openTime = "19:10"; // Example open time
            SetupTimer();
        }

        protected override void OnStop()
        {
            Log("Service is stopping.");
            StopTimer();
        }

        private void SetupTimer()
        {
            try
            {
                var now = DateTime.Now;
                var closeDateTime = DateTime.ParseExact(closeTime, "HH:mm", null);
                var openDateTime = DateTime.ParseExact(openTime, "HH:mm", null);

                if (openDateTime < closeDateTime)
                {
                    openDateTime = openDateTime.AddDays(1);
                }

                TimeSpan startDelay = closeDateTime - now;
                if (startDelay < TimeSpan.Zero)
                {
                    startDelay = TimeSpan.Zero;
                }

                Log($"Timer setup: closeTime={closeDateTime}, openTime={openDateTime}, startDelay={startDelay}");

                StopTimer();
                timer = new Timer(TimerCallback, null, startDelay, TimeSpan.FromMinutes(1));
            }
            catch (FormatException ex)
            {
                Log($"Time format error: {ex.Message}");
            }
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }

        private void TimerCallback(object state)
        {
            CheckAndManageProcesses();
        }

        private string GetUserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        }

        private void CheckAndManageProcesses()
        {
            string userProfile = GetUserProfilePath();

            ManageProcess("BaselinekrSync", Path.Combine(userProfile, "BaselinekrSync", "BaselinekrSync.appref-ms"));
            ManageProcess("BaselinekrSync2", Path.Combine(userProfile, "BaselinekrSync2", "BaselinekrSync2.appref-ms"));
            ManageProcess("Synchronizator Optima-WMS PRODUKCJA", Path.Combine(userProfile, "Synchronizator Optima-WMS PRODUKCJA", "Synchronizator Optima-WMS PRODUKCJA.appref-ms"));
            ManageProcess("Synchronizator Optima-WMS IG PRODUKCJA", Path.Combine(userProfile, "Synchronizator Optima-WMS IG PRODUKCJA", "Synchronizator Optima-WMS IG PRODUKCJA.appref-ms"));
        }

        private void ManageProcess(string processName, string processPath)
        {
            var now = DateTime.Now;
            var closeDateTime = DateTime.ParseExact(closeTime, "HH:mm", null);
            var openDateTime = DateTime.ParseExact(openTime, "HH:mm", null);

            if (openDateTime < closeDateTime)
            {
                openDateTime = openDateTime.AddDays(1);
            }

            if (now.TimeOfDay >= closeDateTime.TimeOfDay && now.TimeOfDay < openDateTime.TimeOfDay)
            {
                RunCommand($"taskkill /IM \"{processName}.exe\" /F");
                Log($"Closed {processName} at {now}");
            }
            else if (now >= openDateTime)
            {
                if (!IsProcessRunning(processName))
                {
                    RunCommand($"start \"{processName}.appref-ms\" \"{processPath}\"");
                    Log($"Started {processName} at {now}");
                }
            }
        }
        private bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        private void RunCommand(string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
                Log($"Command: {command}\nOutput: {output}\nErrors: {errors}");
            }
        }

        private void Log(string message)
        {
            // Log to Event Log
            eventLog.WriteEntry(message);

            // Append message to log file
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry($"Failed to write to log file: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
