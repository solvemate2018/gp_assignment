import {useState} from "react";

const MeasurementForm = () => {
    const [measurement, setMeasurement] = useState({
        systolic: 0,
        diastolic: 0,
        patientSSN: '',
    });

    const handleSubmit = async (event: any) => {
        event.preventDefault();

        try {
            const response = await fetch("http://measurement-service:8080/Measurement", {
                method: 'Post',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(measurement),
            })
            if(response.status == 500){
                alert("No patient with such SSN")
            }
            else if(response.status == 200){
                alert("Successful new recording")
            }
        }
        catch (error) {
            console.error('Error submitting form:', error);
        }


        setMeasurement({
            systolic: 0,
            diastolic: 0,
            patientSSN: '',
        });
    };

    const handleChange = (event: any) => {
        setMeasurement({ ...measurement, [event.target.name]: event.target.value });
    };

    return (
        <form className="space-y-2 px-4 py-3 border border-gray-200 rounded-md" onSubmit={handleSubmit}>
            <div className="grid grid-cols-2 gap-4">
                <label htmlFor="systolic" className="text-sm font-medium">Systolic:</label>
                <input
                    type="number"
                    id="systolic"
                    name="systolic"
                    value={measurement.systolic}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                    required
                />
                <label htmlFor="diastolic" className="text-sm font-medium">Diastolic:</label>
                <input
                    type="number"
                    id="diastolic"
                    name="diastolic"
                    value={measurement.diastolic}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                    required
                />
                <label htmlFor="patientSSN" className="text-sm font-medium">Patient SSN:</label>
                <input
                    type="text"
                    id="patientSSN"
                    name="patientSSN"
                    value={measurement.patientSSN}
                    onChange={handleChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                    required
                />
            </div>
            <button type="submit" className="inline-flex items-center px-4 py-2 bg-indigo-500 text-white font-bold rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                Submit Measurement
            </button>
        </form>
    );
};

export default MeasurementForm;