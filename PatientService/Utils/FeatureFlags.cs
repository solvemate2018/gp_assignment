namespace PatientService.Utils;

public static class FeatureFlags
{
    public static bool IsPatientDeleteFeatureOn = Environment.GetEnvironmentVariable("PATIENT_DELETE") == "true";
    public static bool IsPatientPostFeatureOn = Environment.GetEnvironmentVariable("PATIENT_POST") == "true";
}