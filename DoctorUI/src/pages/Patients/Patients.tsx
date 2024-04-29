import {useEffect, useState} from 'react';
import PatientsList from "../../components/PatientsList/PatientsList.tsx";
import {Link} from "react-router-dom";

const Patients = () => {
    const [patients, setPatients] = useState(null);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await fetch("http://patient-service:8080/Patient");
                if (!response.ok) {
                    throw new Error('Failed to fetch user');
                }
                const userData = await response.json();
                setPatients(userData);
            }
            catch (ex){
                console.error(ex);
            }
        }

        fetchUsers();
    }, []);
    
    return (
        <div>
            <button><Link to={'/patient/create'}>Register new Patient</Link></button>
            {patients !== null && <PatientsList patients={patients}></PatientsList>}
        </div>
    );
};

export default Patients;