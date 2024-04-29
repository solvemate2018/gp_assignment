using System.Diagnostics;
using Models;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace PatientService.External;

public class MeasurementServiceCommunicator
{
    private HttpClient _client = new HttpClient();
    
    public async Task<Measurement[]> GetPatientMeasurements(string patientSsn)
    {
        var request = PrepareGetRequestWithPropagator($"http://measurement-service:8080/Measurement/patient/{patientSsn}");
        var response = await _client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var Measurements = await response.Content.ReadFromJsonAsync<Measurement[]>();

            if (Measurements != null)
            {
                return Measurements;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    
    private HttpRequestMessage PrepareGetRequestWithPropagator(string requestUrl)
    {
        var activityContext = Activity.Current?.Context ?? default;
        var propagationContext = new PropagationContext(activityContext, Baggage.Current);
        var propagator = new TraceContextPropagator();
        var headers = new Dictionary<string, string>();

        propagator.Inject(propagationContext, headers, (dictionary, key, value) => { dictionary[key] = value; });

        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        return request;
    }
}