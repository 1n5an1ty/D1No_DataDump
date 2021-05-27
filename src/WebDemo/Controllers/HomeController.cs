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

            var dataSvc = new DataService();            
            var alertTypes = await dataSvc.GetAlertTypesByIdsAsync(currentAlerts.Events.Select(x => x.Alert_Type_Id).Concat(currentAlerts.Ranges.Select(x => x.Alert_Type_Id)).Distinct());
            var capturedPCs = await dataSvc.GetCapturedPCsByIdsAsync(currentAlerts.Events.Select(x => x.Capture_PC_Id).Concat(currentAlerts.Ranges.Select(x => x.Capture_PC_Id)).Distinct());
            var recordings = await dataSvc.GetRecordingsByIdsAsync(currentAlerts.Events.Select(x => x.Recording_Id).Concat(currentAlerts.Ranges.Select(x => x.Recording_Id)).Distinct());
            var countries = await dataSvc.GetCountriesByCodesAsync(capturedPCs.Select(x => x.CountryCode));
            dataSvc.Dispose();            

            var events = ProcessEvents(currentAlerts, alertTypes, capturedPCs, recordings, countries);
            var ranges = ProcessRanges(currentAlerts, alertTypes, capturedPCs, recordings, countries);

            var allAlerts = events.Concat(ranges);

            return View(allAlerts);
        }

        private static IEnumerable<TriggeredEvent> ProcessEvents(AlertResponse currentAlerts, 
            IEnumerable<Data.Models.AlertType> alertTypes, 
            IEnumerable<Data.Models.CapturedPC> capturedPCs,
            IEnumerable<Data.Models.Recording> recordings,
            IEnumerable<Data.Models.Country> countries)
        {
            var events = new List<TriggeredEvent>();

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
                            Triggered = alert.Timestamps.ElementAt(i),
                            AvgTimeBetweenEvents = alert.Average_Time_Between_Events,
                            Country = countries.FirstOrDefault(x => x.CountryCode == capturedPCs.FirstOrDefault(y => y.Id == alert.Capture_PC_Id).CountryCode).CountryName
                        });
                    }

                    continue;
                }


                events.Add(new TriggeredEvent
                {
                    Alert = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id),
                    Computer = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id),
                    CapturedRecording = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id),
                    Triggered = alert.Timestamps.ElementAt(0),
                    AvgTimeBetweenEvents = alert.Average_Time_Between_Events,
                    Country = countries.FirstOrDefault(x => x.CountryCode == capturedPCs.FirstOrDefault(y => y.Id == alert.Capture_PC_Id).CountryCode).CountryName
                });
            }

            return events;
        }

        private static IEnumerable<TriggeredEvent> ProcessRanges(AlertResponse currentAlerts,
            IEnumerable<Data.Models.AlertType> alertTypes,
            IEnumerable<Data.Models.CapturedPC> capturedPCs,
            IEnumerable<Data.Models.Recording> recordings,
            IEnumerable<Data.Models.Country> countries)
        {
            var events = new List<TriggeredEvent>();

            foreach (var alert in currentAlerts.Ranges)
            {
                if (alert.Time_Ranges.Count() > 1)
                {
                    for (int i = 0; i < alert.Time_Ranges.Count(); i++)
                    {
                        events.Add(new TriggeredEvent
                        {
                            Alert = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id),
                            Computer = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id),
                            CapturedRecording = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id),
                            Triggered = alert.Time_Ranges.ElementAt(i).Start,
                            IsActive = alert.Currently_In_Alert_State,
                            Duration = alert.Time_Spent_In_Alert_State,
                            Country = countries.FirstOrDefault(x => x.CountryCode == capturedPCs.FirstOrDefault(y => y.Id == alert.Capture_PC_Id).CountryCode).CountryName
                        });
                    }

                    continue;
                }


                events.Add(new TriggeredEvent
                {
                    Alert = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id),
                    Computer = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id),
                    CapturedRecording = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id),
                    Triggered = alert.Time_Ranges.ElementAt(0).Start,
                    IsActive = alert.Currently_In_Alert_State,
                    Duration = alert.Time_Spent_In_Alert_State,
                    Country = countries.FirstOrDefault(x => x.CountryCode == capturedPCs.FirstOrDefault(y => y.Id == alert.Capture_PC_Id).CountryCode).CountryName
                });
            }

            return events;
        }
    }
}