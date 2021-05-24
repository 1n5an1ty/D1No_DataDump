using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDemo.Models;
using WebDemo.Services;

namespace WebDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AlertsApiService _alertService;

        public HomeController()
        {
            _alertService = new AlertsApiService();
        }
        public async Task<ActionResult> Index()
        {
            var currentAlerts = await _alertService.GetCurrentAlertsAsync();
            var events = await ProcessEvents(currentAlerts);

            return View(events);
        }

        private static async Task<IEnumerable<TriggeredEvent>> ProcessEvents(AlertResponse currentAlerts)
        {
            var events = new List<TriggeredEvent>();

            using (var svc = new DataService())
            {
                var alertTypes = await svc.GetAlertTypesByIdsAsync(currentAlerts.Events.Select(x => x.Alert_Type_Id));
                var capturedPCs = await svc.GetCapturedPCsByIdsAsync(currentAlerts.Events.Select(x => x.Capture_PC_Id));
                var recordings = await svc.GetRecordingsByIdsAsync(currentAlerts.Events.Select(x => x.Recording_Id));

                foreach (var alert in currentAlerts.Events)
                {
                    if (alert.Timestamps.Count > 1)
                    {
                        for (int i = 0; i < alert.Timestamps.Count; i++)
                        {
                            events.Add(new TriggeredEvent
                            {
                                Alert = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id),
                                Computer = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id),
                                CapturedRecording = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id),
                                Triggered = new DateTime(alert.Timestamps[i])
                            });
                        }

                        continue;
                    }


                    events.Add(new TriggeredEvent
                    {
                        Alert = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id),
                        Computer = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id),
                        CapturedRecording = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id),
                        Triggered = new DateTime(alert.Timestamps[0])
                    });
                }
            }

            return events;
        }
    }
}