using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using PatientService.Utils;
using Serilog.Core;

namespace PatientService.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientController : ControllerBase
{
    private Services.PatientService _patientService;
    
    private Logger _logger;
    
    private ActivitySource _activitySource;
    
    public PatientController(Services.PatientService patientService, Logger logger, ActivitySource activitySource)
    {
        _patientService = patientService;
        _logger = logger;
        _activitySource = activitySource;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        using (var activity = _activitySource.StartActivity())
        {
            try
            {
                HandleParentContext();
                _logger.Information("Getting all patients.");
                return Ok(await _patientService.GetAllPatients());
            }
            catch (Exception e)
            {
                _logger.Error("An error happened while getting all patients.");
                return Problem();
            }
        }
    }
    
    [HttpGet]
    [Route("{ssn}")]
    public async Task<IActionResult> GetPatientBySsn([FromRoute] string ssn)
    {
        using (var activity = _activitySource.StartActivity())
        {
            try
            {
                HandleParentContext();
                _logger.Information("Getting patient with SSN: {ssn}.", ssn);
                return Ok(await _patientService.GetPatientBySsn(ssn));
            }
            catch (Exception e)
            {
                _logger.Error("An error happened while getting patient by SSN.");
                return Problem();
            }
        }
    }
    
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreatePatient([FromBody] Patient patient)
    {
        using (var activity = _activitySource.StartActivity())
        {
            try
            {
                if (FeatureFlags.IsPatientPostFeatureOn)
                {
                    HandleParentContext();
                    _logger.Information("Creating patient with SSN {ssn}.", patient.Ssn);
                    return Ok(await _patientService.CreateNewPatient(patient));
                }
                return NotFound("This feature is currently unavailable.");
            }
            catch (Exception e)
            {
                _logger.Error("An error happened while creating new patient.");
                return Problem();
            }
        }
    }
    
    [HttpDelete]
    [Route("{ssn}")]
    public async Task<IActionResult> RemovePatient([FromRoute] string ssn)
    {
        using (var activity = _activitySource.StartActivity())
        {
            try
            {
                if (FeatureFlags.IsPatientDeleteFeatureOn)
                {
                    HandleParentContext();
                    _logger.Information("Deleting patient with SSN {ssn}.", ssn);
                    await _patientService.DeletePatient(ssn);
                    return Ok();
                }
                return NotFound("This feature is currently unavailable.");
            }
            catch (Exception e)
            {
                _logger.Error("An error happened while Deleting patient with SSN {ssn}.", ssn);
                return Problem();
            }
        }
    }
    
    [HttpPost]
    [Route("/feature/{featureName}")]
    public async Task<IActionResult> ToggleFeature([FromRoute] string featureName)
    {
        using (var activity = _activitySource.StartActivity())
        {
            try
            {
                _logger.Information("Toggling feature {featureName}.", featureName);
                switch (featureName)
                {
                    case "DeletePatient":
                        FeatureFlags.IsPatientDeleteFeatureOn = !FeatureFlags.IsPatientDeleteFeatureOn;
                        break;
                    case "PostPatient":
                        FeatureFlags.IsPatientPostFeatureOn = !FeatureFlags.IsPatientPostFeatureOn;
                        break;
                    default:
                        return BadRequest("No such feature.");
                }
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error("An error happened while toggling feature {featureName}.", featureName);
                return Problem();
            }
        }
    }


    private void HandleParentContext()
    {
        var propagator = new TraceContextPropagator();
        var parentContext = propagator.Extract(default, Request, (r, key) =>
        {
            return new List<string>(new[] { r.Headers[key].ToString() });
        });
        Baggage.Current = parentContext.Baggage;
    }
}