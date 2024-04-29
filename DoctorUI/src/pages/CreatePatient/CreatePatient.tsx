import {Patient} from "../../Interfaces/Patient.tsx";

const CreatePatient = () => {
    const handleSubmit = async (event: any) => {
        event.preventDefault();
        
        const formData: Patient = {
            ssn: event.target.ssn.value,
            name: event.target.name.value,
            email: event.target.email.value,
            measurements: []
        };
        
        try {
            const response = await fetch("http://patient-service:8080/Patient", {
                method: 'Post',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
            })
            if(response.status == 404){
                alert("Currently Service unavailable")
            }
            else if(response.status == 200){
                alert("Successful")
            }
        }
        catch (error) {
            console.error('Error submitting form:', error);
        }

        event.target.reset();
    };


    return (
        <form className="space-y-2 px-4 py-3 border border-gray-200 rounded-md" onSubmit={handleSubmit}>
            <div className="flex flex-col">
                <label htmlFor="ssn" className="text-gray-700 font-medium mb-1">Social Security Number (SSN)</label>
                <input type="text" id="ssn" name="ssn"
                       className="border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-1 focus:ring-blue-500"
                       required/>
            </div>
            <div className="flex flex-col">
                <label htmlFor="name" className="text-gray-700 font-medium mb-1">Patient Name</label>
                <input type="text" id="name" name="name"
                       className="border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-1 focus:ring-blue-500"
                       required/>
            </div>
            <div className="flex flex-col">
                <label htmlFor="email" className="text-gray-700 font-medium mb-1">Email Address</label>
                <input type="email" id="email" name="email"
                       className="border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-1 focus:ring-blue-500"
                       required/>
            </div>
            <button type="submit"
                    className="bg-blue-500 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-md shadow focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-400">
                Add Patient
            </button>
        </form>
    );
};

export default CreatePatient;