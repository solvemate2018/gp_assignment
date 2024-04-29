using System.Diagnostics;
using Models;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;
using Serilog.Core;

namespace MeasurementService.Controllers;

[ApiController]
[Route("[controller]")]
public class MeasurementController : ControllerBase
{
    private Services.MeasurementService _measurementService;

    private Logger _logger;

    private ActivitySource _activitySource;

    public MeasurementController(Services.MeasurementService measurementService, Logger logger,
        ActivitySource activitySource)
    {
        _measurementService = measurementService;
        _logger = logger;
        _activitySource = activitySource;
    }

    [HttpGet]
    [Route("patient/{patientSsn}")]
    public async Task<IActionResult> GetMeasurementsForPatient([FromRoute] string patientSsn)
    {
        using (var activity = _activitySource.StartActivity())
        {
            try
            {
                HandleParentContext();
                _logger.Information("Getting measurements for the patient.");
                return Ok(await _measurementService.GetMeasurementsForPatient(patientSsn));
            }
            catch (Exception e)
            {
                _logger.Error("An error happened while getting measurements for the patient.");
                return Problem();
            }
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> SavePatientMeasurement([FromBody] Measurement measurement)
    {
        using (var activity = _activitySource.StartActivity())
        {
            try
            {
                HandleParentContext();
                _logger.Information("Saving new measurement.");
                var createdMeasurement = await _measurementService.CreateMeasurement(measurement);
                if (createdMeasurement == null)
                {
                    return Problem("No patient with such SSN");
                }
                return Ok(createdMeasurement);
            }
            catch (Exception e)
            {
                _logger.Error("An error happened while saving measurement.");
                return Problem();
            }
        }
    }

    [HttpPut]
    [Route("{measurementId}")]
    public async Task<IActionResult> UpdatePatientMeasurement([FromRoute] int measurementId, [FromBody] Measurement measurement)
    {
        using (var activity = _activitySource.StartActivity())
        {
            try
            {
                HandleParentContext();
                _logger.Information("Updating measurements for the patient.");
                var updateEntity = await _measurementService.UpdateMeasurement(measurementId, measurement);
                return Ok(updateEntity);
            }
            catch (Exception e)
            {
                _logger.Error("An error happened while updating measurement.");
                return Problem();
            }
        }
    }

    private void HandleParentContext()
    {
        var propagator = new TraceContextPropagator();
        var parentContext = propagator.Extract(default, Request,
            (r, key) => { return new List<string>(new[] { r.Headers[key].ToString() }); });
        Baggage.Current = parentContext.Baggage;
    }
}