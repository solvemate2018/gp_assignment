export interface Measurement {
    id: number,
    date: Date,
    systolic: number,
    diastolic: number,
    patientSSN: string,
    seen: boolean,
}