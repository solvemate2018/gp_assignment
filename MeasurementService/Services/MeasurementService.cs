using System.Diagnostics;
using MeasurementService.Data;
using MeasurementService.External;
using Models;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace MeasurementService.Services;

public class MeasurementService(MeasurementsDbContext measurementsDb, PatientServiceCommunicator communicator, ActivitySource _activitySource)
{
    public async Task<Measurement> CreateMeasurement(Measurement measurement)
    {
        if (await communicator.PatientExists(measurement.PatientSSN))
        {
            measurement.Date = DateTime.Now;
            var addedEntity = await measurementsDb.Measurements.AddAsync(measurement);
            await measurementsDb.SaveChangesAsync();
            return addedEntity.Entity;
        }
        else
        {
            return null;
        }
    }

    public async Task<Measurement> UpdateMeasurement(int measurementId, Measurement measurement)
    {
        var dbMeasurement = await measurementsDb.Measurements.FindAsync(measurementId);

        if (dbMeasurement != null)
        {
            dbMeasurement.Diastolic = measurement.Diastolic;
            dbMeasurement.Systolic = measurement.Systolic;
            dbMeasurement.Seen = measurement.Seen;

            await measurementsDb.SaveChangesAsync();
            return measurement;
        }

        throw new ArgumentException("No measurement with such Id");
    }

    public async Task<Measurement[]> GetMeasurementsForPatient(string patientSsn)
    {
         var result = measurementsDb.Measurements.Where(m => m.PatientSSN == patientSsn).ToArray();

         return result;
    }
}