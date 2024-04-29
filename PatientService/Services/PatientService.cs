using Microsoft.EntityFrameworkCore;
using PatientService.Data;
using Models;
using PatientService.External;

namespace PatientService.Services;

public class PatientService
{
    private readonly PatientDbContext _patientDbContext;

    private readonly MeasurementServiceCommunicator _communicator;
    
    public PatientService(PatientDbContext patientDbContext, MeasurementServiceCommunicator communicator)
    {
        _patientDbContext = patientDbContext;
        _communicator = communicator;
    }

    public async Task<Patient[]> GetAllPatients()
    {
        return await _patientDbContext.Patients.OrderBy(p => p.Ssn).ToArrayAsync();
    }
    
    public async Task<Patient> GetPatientBySsn(string ssn)
    {
        var dbPatient = await _patientDbContext.Patients.FindAsync(ssn);

        if (dbPatient != null)
        {
            var patientMeasurements = await _communicator.GetPatientMeasurements(dbPatient.Ssn);
            if (patientMeasurements != null)
            {
                dbPatient.Measurements = patientMeasurements;
            }
            return dbPatient;
        }
        else
        {
            throw new ArgumentException("No patient with such SSN");
        }
    }

    public async Task<Patient> CreateNewPatient(Patient patient)
    {
        if (await _patientDbContext.Patients.FindAsync(patient.Ssn) == null)
        {
            var dbPatient = await _patientDbContext.Patients.AddAsync(patient);
            await _patientDbContext.SaveChangesAsync();
            return dbPatient.Entity;
        }
        else
        {
            throw new ArgumentException("Patient with such SSN already exists");
        }
    }
    
    public async Task DeletePatient(string ssn)
    {
        var dbPatient = await _patientDbContext.Patients.FindAsync(ssn);

        if (dbPatient != null)
        { 
            _patientDbContext.Patients.Remove(dbPatient);
            await _patientDbContext.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException("No patient with such SSN");
        }
    }
}