import api from "./axiosInstance";

// Hospital (doctor/huisarts)
export async function getHospitalMedicalRecords() {
  const res = await api.get("hospital/medicalrecords");
  return res.data;
}

export async function createHospitalMedicalRecord(payload) {
  // payload: { patientNumber, description, file }
  const res = await api.post("hospital/medicalrecords", payload);
  return res.data;
}

export async function getHospitalMedicalRecordsByPatient(patientNumber) {
  const res = await api.get(`hospital/medicalrecords/patient/${patientNumber}`);
  return res.data;
}

export async function getHospitalMedicalRecordByLineNumber(lineNumber) {
  const res = await api.get(`hospital/medicalrecords/${lineNumber}`);
  return res.data;
}

// Patient
export async function getPatientMedicalRecords(patientNumber) {
  const res = await api.get(`patient/medicalrecords`);
  return res.data;
}

export async function getMedicalRecordPdf(recordId) {
  const res = await api.get(`hospital/medicalrecords/${recordId}/pdf`, {
    responseType: "blob"
  });
  return res.data;
}
