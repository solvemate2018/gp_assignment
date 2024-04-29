using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace MeasurementService.External;

public class PatientServiceCommunicator
{
    private HttpClient _client = new HttpClient();

    public async Task<bool> PatientExists(string patientSsn)
    {
        var request = PrepareGetRequestWithPropagator($"http://patient-service:8080/Patient/{patientSsn}");
        var response = await _client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
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