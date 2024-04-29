import './App.css'
import {BrowserRouter as Router, Route, Routes} from 'react-router-dom';
import Patients from "./pages/Patients/Patients.tsx";
import PatientDetails from "./pages/PatientDetails/PatientDetails.tsx";
import CreatePatient from "./pages/CreatePatient/CreatePatient.tsx";


function App() {

  return (
    <Router>
      <div>
          <Routes>
          <Route path="/" Component={Patients}></Route>
          <Route path="/patient/:ssn" Component={PatientDetails}></Route>
          <Route path="/patient/create" Component={CreatePatient}></Route>
          </Routes>
      </div>
    </Router>
  )
}

export default App
