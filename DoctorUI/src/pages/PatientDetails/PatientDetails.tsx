import {useEffect, useState} from "react";
import {useParams} from "react-router-dom";
import {Patient} from "../../Interfaces/Patient.tsx";
import {Measurement} from "../../Interfaces/Measurement.tsx";

const PatientDetails = () => {
    const [currentPatient, setCurrentPatient] = useState<Patient | null>(null);
    const { ssn } = useParams();
    
    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await fetch("http://patient-service:8080/Patient/" + ssn);
                if (!response.ok) {
                    throw new Error('Failed to fetch user');
                }
                const userData = await response.json();
                setCurrentPatient(userData);
            }
            catch (ex){
                console.error(ex);
            }
        }

        fetchUsers();
    }, []);
    
    const markAsSeen = async (measurement: Measurement) => {
        measurement.seen = true;
        const response = await fetch("http://measurement-service:8080/Measurement/" + measurement.id, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(measurement),
        })
        
        if(response.ok){
            alert("Measurement seen")
        }
    }
    
    return <div>
        <p>{currentPatient?.ssn}</p>
        <p>{currentPatient?.name}</p>
        <p>{currentPatient?.email}</p>
        <ul>
            {currentPatient?.measurements?.map((m) => (
                <li key={m.id} className="flex items-center py-1 hover:bg-gray-100">
                    <p className="text-gray-700 mr-4">{m.date.toString()}</p>
                    <p className="text-gray-700 font-medium">{m.diastolic}</p>
                    <p className="text-gray-500 text-sm">{m.systolic}</p>
                    {!m.seen && <button className="text-gray-500 text-sm" onClick={() => markAsSeen(m)}>Mark as seen</button>}
                </li>
            ))}
        </ul>
    </div>
};

export default PatientDetails;