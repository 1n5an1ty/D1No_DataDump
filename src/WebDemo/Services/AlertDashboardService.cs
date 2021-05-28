using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebDemo.Models;

namespace WebDemo.Services
{
    public class AlertDashboardService : IDisposable
    {
        private readonly DatabaseService _databaseService;
        private readonly AlertsApiService _apiService;

        public AlertDashboardService()
        {
            _databaseService = new DatabaseService();
            _apiService = new AlertsApiService();
        }

        public async Task<IEnumerable<TriggeredEvent>> LoadAllAlerts()
        {
            var currentAlerts = await _apiService.GetCurrentAlertsAsync();

            var alertTypes = await _databaseService.GetAlertTypesByIdsAsync(currentAlerts.Events.Select(x => x.Alert_Type_Id).Concat(currentAlerts.Ranges.Select(x => x.Alert_Type_Id)).Distinct());
            var capturedPCs = await _databaseService.GetCapturedPCsByIdsAsync(currentAlerts.Events.Select(x => x.Capture_PC_Id).Concat(currentAlerts.Ranges.Select(x => x.Capture_PC_Id)).Distinct());
            var recordings = await _databaseService.GetRecordingsByIdsAsync(currentAlerts.Events.Select(x => x.Recording_Id).Concat(currentAlerts.Ranges.Select(x => x.Recording_Id)).Distinct());
            var countries = await _databaseService.GetCountriesByCodesAsync(capturedPCs.Select(x => x.CountryCode));

            var events = ProcessEvents(currentAlerts, alertTypes, capturedPCs, recordings, countries);
            var ranges = ProcessRanges(currentAlerts, alertTypes, capturedPCs, recordings, countries);

            return events.Concat(ranges);
        }

        public async Task<IEnumerable<TriggeredEvent>> SearcAlerts(RequestFilters filters)
        {
            var currentAlerts = await _apiService.GetCurrentAlertsAsync();
            if (filters != null)
            {
                currentAlerts = ProcessAlertsWithFilters(currentAlerts, filters);
            }

            var alertTypes = await _databaseService.GetAlertTypesByIdsAsync(currentAlerts.Events.Select(x => x.Alert_Type_Id).Concat(currentAlerts.Ranges.Select(x => x.Alert_Type_Id)).Distinct());
            var capturedPCs = await _databaseService.GetCapturedPCsByIdsAsync(currentAlerts.Events.Select(x => x.Capture_PC_Id).Concat(currentAlerts.Ranges.Select(x => x.Capture_PC_Id)).Distinct());
            var recordings = await _databaseService.GetRecordingsByIdsAsync(currentAlerts.Events.Select(x => x.Recording_Id).Concat(currentAlerts.Ranges.Select(x => x.Recording_Id)).Distinct());
            var countries = await _databaseService.GetCountriesByCodesAsync(capturedPCs.Select(x => x.CountryCode));

            var events = ProcessEvents(currentAlerts, alertTypes, capturedPCs, recordings, countries);
            var ranges = ProcessRanges(currentAlerts, alertTypes, capturedPCs, recordings, countries);

            return events.Concat(ranges);
        }

        private static AlertResponse ProcessAlertsWithFilters(AlertResponse currentAlerts, 
            RequestFilters filters)
        {
            if (filters.EventType.HasValue)
            {
                if (filters.EventType == EventType.Event)
                {
                    currentAlerts.Ranges = new List<AlertRange>();
                }

                if (filters.EventType == EventType.Range)
                {
                    currentAlerts.Events = new List<AlertEvent>();
                }
            }

            if (filters.AlertTypeId.HasValue)
            {
                currentAlerts.Events = currentAlerts.Events.Where(x => x.Alert_Type_Id == filters.AlertTypeId);
                currentAlerts.Ranges = currentAlerts.Ranges.Where(x => x.Alert_Type_Id == filters.AlertTypeId);
            }

            if (filters.CapturedPCId.HasValue)
            {
                currentAlerts.Events = currentAlerts.Events.Where(x => x.Capture_PC_Id == filters.CapturedPCId);
                currentAlerts.Ranges = currentAlerts.Ranges.Where(x => x.Capture_PC_Id == filters.CapturedPCId);
            }

            if (filters.RecordingId.HasValue)
            {
                currentAlerts.Events = currentAlerts.Events.Where(x => x.Recording_Id == filters.RecordingId);
                currentAlerts.Ranges = currentAlerts.Ranges.Where(x => x.Recording_Id == filters.RecordingId);
            }

            return currentAlerts;
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

        public void Dispose()
        {
            _databaseService.Dispose();
            _apiService.Dispose();
        }
    }
}