import {Measurement} from "./Measurement.tsx";

export interface Patient {
    ssn: string,
    email: string,
    name: string,
    measurements: Measurement[] | null
}