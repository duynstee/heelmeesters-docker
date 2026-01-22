import api from "./axiosInstance";

// Referrals (patient)
export async function getMyActiveReferrals() {
  const res = await api.get("patient/referrals/active");
  return res.data;
}

// Appointments (patient)
export async function getMyAppointments() {
  const res = await api.get("patient/appointments");
  return res.data;
}

export async function createAppointmentFromReferral(payload) {
  const res = await api.post("patient/appointments/from-referral", payload);
  return res.data;
}

export async function getPatientDoctors() {
  const res = await api.get("patient/appointments/doctors");
  return res.data;
}

export async function getPatientRooms() {
  const res = await api.get("patient/appointments/rooms");
  return res.data;
}

export async function getDoctorWeekSchedule({ employeeNumber, weekStartIso }) {
  const res = await api.get(
    `patient/appointments/doctors/${employeeNumber}/schedule/week`,
    { params: { weekStart: weekStartIso } }
  );
  return res.data;
}
