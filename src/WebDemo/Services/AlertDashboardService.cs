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
            var currentAlerts = await _apiService.GetCurrentAlertsAsync(filters?.MinutesLookback);

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

            if (filters != null && !string.IsNullOrWhiteSpace(filters.CountryCode))
            {
                events = events.Where(x => x.Country == filters.CountryCode);
                ranges = ranges.Where(x => x.Country == filters.CountryCode);
            }

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
                            Event_Type = "Event",
                            Alert_Type = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id)?.Name,
                            Computer_Name = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id)?.Name,
                            Recording_Name = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id)?.Name,
                            Event_Date = alert.Timestamps.ElementAt(i).ToShortDateString(),
                            Event_Time = alert.Timestamps.ElementAt(i).ToShortTimeString(),
                            Time_Between = alert.Average_Time_Between_Events.ToString(),
                            Country = countries.FirstOrDefault(x => x.CountryCode == capturedPCs.FirstOrDefault(y => y.Id == alert.Capture_PC_Id).CountryCode).CountryName
                        });
                    }

                    continue;
                }


                events.Add(new TriggeredEvent
                {
                    Event_Type = "Event",
                    Alert_Type = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id)?.Name,
                    Computer_Name = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id)?.Name,
                    Recording_Name = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id)?.Name,
                    Event_Date = alert.Timestamps.ElementAt(0).ToShortDateString(),
                    Event_Time = alert.Timestamps.ElementAt(0).ToShortTimeString(),
                    Time_Between = alert.Average_Time_Between_Events.ToString(),
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
                            Event_Type = "Range",
                            Alert_Type = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id)?.Name,
                            Computer_Name = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id)?.Name,
                            Recording_Name = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id)?.Name,
                            Event_Date = alert.Time_Ranges.ElementAt(i).Start.ToShortDateString(),
                            Event_Time = alert.Time_Ranges.ElementAt(i).Start.ToShortTimeString(),
                            Is_Active = alert.Currently_In_Alert_State.ToString(),
                            Duration = alert.Time_Spent_In_Alert_State.ToString(),
                            Country = countries.FirstOrDefault(x => x.CountryCode == capturedPCs.FirstOrDefault(y => y.Id == alert.Capture_PC_Id).CountryCode).CountryName
                        });
                    }

                    continue;
                }


                events.Add(new TriggeredEvent
                {
                    Event_Type = "Range",
                    Alert_Type = alertTypes.FirstOrDefault(x => x.Id == alert.Alert_Type_Id)?.Name,
                    Computer_Name = capturedPCs.FirstOrDefault(x => x.Id == alert.Capture_PC_Id)?.Name,
                    Recording_Name = recordings.FirstOrDefault(x => x.Id == alert.Recording_Id)?.Name,
                    Event_Date = alert.Time_Ranges.ElementAt(0).Start.ToShortDateString(),
                    Event_Time = alert.Time_Ranges.ElementAt(0).Start.ToShortTimeString(),
                    Is_Active = alert.Currently_In_Alert_State.ToString(),
                    Duration = alert.Time_Spent_In_Alert_State.ToString(),
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