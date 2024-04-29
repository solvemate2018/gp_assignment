import {Patient} from "../../Interfaces/Patient.tsx";
import {Link} from "react-router-dom";

interface Props {
    patients: Patient[]
}

const PatientsList = (props: Props) => {
    
    const deletePatient = async (patient: Patient) => {
            const response = await fetch("http://patient-service:8080/Patient/" + patient.ssn, {
                method: 'DELETE',
            })

        if(response.status == 404){
            alert("Currently Service unavailable")
        }
        else if(response.status == 200){
            alert("Deleted Patient")
        }
    }
    return (
        <div className="flex flex-col space-y-2 overflow-auto border border-gray-200 rounded-md p-2">
            <ul>
                {props.patients.map((p) => (
                    <li key={p.ssn} className="flex items-center py-1 hover:bg-gray-100">
                        <p className="text-gray-700 mr-4">{p.ssn}</p>
                        <p className="text-gray-700 font-medium">{p.name}</p>
                        <p className="text-gray-500 text-sm">{p.email}</p>
                        <Link to={"/patient/" + p.ssn} className="text-gray-500 text-sm">Details</Link>
                        <button className="text-gray-500 text-sm" onClick={() => deletePatient(p)}>DELETE PATIENT</button>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default PatientsList;