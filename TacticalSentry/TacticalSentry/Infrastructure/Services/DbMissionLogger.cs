using System;
using TacticalSentry.Core.Entities;
using TacticalSentry.Core.Enums;
using TacticalSentry.Core.Interfaces;
using TacticalSentry.Infrastructure.Data;

namespace TacticalSentry.Infrastructure.Services
{
    public class DbMissionLogger : IMissionLogger
    {
        public void Log(string message, LogSeverity severity)
        {
            try
            {
                using (var context = new TacticalDbContext())
                {
                    context.Database.EnsureCreated();

                    
                    context.Logs.Add(new SecurityLog
                    {
                        ThreatType = message, 
                        DetectedTime = DateTime.Now.ToString("HH:mm:ss"), 
                        Status = severity == LogSeverity.CriticalAlert ? "Engellendi" : "Tespit",
                        Confidence = 90 
                    });

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Loglama Hatası: " + ex.Message);
            }
        }
    }
}